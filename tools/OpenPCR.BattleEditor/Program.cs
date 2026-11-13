using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using OpenPCR.BattleEditor;

if (args.Length == 0 || args is ["--help"] or ["help"])
{
	Console.WriteLine("""
OpenPCR.BattleEditor — create/edit openpcr-battle/v2 requests (no in-battle operations).
  catalog   --resources ROOT --master-version VERSION --kind units|enemies|backgrounds|clan-maps|equipment|stories|labyrinth-enemies|labyrinth-quests|labyrinth-relics
            [--query TEXT] [--id ID] [--limit 30]
  unit-info --resources ROOT --master-version VERSION --id UNIT
  create    --resources ROOT --master-version VERSION --preset trial-basic|clan-rehearsal-basic|labyrinth-basic
            --units ID[,ID...] --output FILE [unit growth options] [scene options]
            labyrinth-basic takes --quest-id --difficulty; the clan presets take --battle-id --phase --order
  inspect   --input FILE
  validate  --input FILE --resources ROOT
  labyrinth --input FILE --resources ROOT --output FILE [Labyrinth options]
  scene     --input FILE --resources ROOT --output FILE [scene options]
  party     --input FILE --resources ROOT --output FILE --units ID[,0,ID...] [--battle N]
  unit-add  --input FILE --resources ROOT --output FILE --id UNIT [--slot 1..5] [unit growth options]
  unit-edit --input FILE --resources ROOT --output FILE --id UNIT [--slot 1..5] [unit growth options]
  unit-remove --input FILE --resources ROOT --output FILE --id UNIT
  stories   --input FILE --resources ROOT --output FILE --ids ID[,ID...]|none
  set       --input FILE --resources ROOT --output FILE --path /JSON/Pointer --value JSON
            (or --patch FILE: array of {"Path":"/pointer","Value":...}, applied as one edit)
Unit growth: --level N --rarity N --battle-rarity 0|3|4|5 --rank N --skills N
             --equipment none|all --enhance N
Scene: [--battle N] --seed N --auto true|false --speed 1|2|4 --set-units IDS|none
       --team-level N --rarity-change --skill-sync --slip-damage-barrier --knight-enhance true|false
       --fps 30|60 --battle-id N --phase N --order N --lap N
       --boss-hp INT64 --boss-current-hp INT64 --quad-speed true|false (clan) --infinite-hp true|false (trial)
Labyrinth: --quest-id N --difficulty N --alpha-rupee N --reinforcement-nodes IDS|none
           --unit-sources JSON_FILE --relics JSON_FILE --ex-seeds A[,B[,C]] --ex-state JSON_FILE
           [--ex-party 0|1|2] --party-units IDS --party-auto true|false --party-speed 1|2|4 --party-set IDS|none
Scenes come from the clan battle or quest IDs; enemies, time limit and background are not inputs.
set paths begin at the request root, e.g. /Battles/0/Seed, /Player/unit_list/0/unit_level, /ClanBattle/phase.

Mutation commands support --dry-run true instead of --output. Every write validates and prepares
products before atomically replacing the output. Existing input may also be the output.
Presets start at level/team-level 100, rarity 3, rank 1, skills at unit level,
no equipment/stories/team enhancement/support, seed 12345. labyrinth-basic is category 59 with
the created units as owned USER sources and no relics, reinforcement nodes or AlphaRupee.
Other context/settings come from the named example preset. Use inspect to review them.
stdout: JSON results; stderr: JSON errors. Exit 0 = success, 1 = rejected/error.
""");
	return 0;
}

try
{
	string command = args[0]; var options = new Arguments(args[1..]);
	object result;
	if (command is "catalog" or "unit-info")
	{
		string resources = options.Require("resources"), version = options.Require("master-version");
		using var catalog = new Catalog(resources, version);
		if (command == "catalog")
		{
			string kind = options.Require("kind"); string? query = options.Take("query");
			int? id = options.Integer("id"); int limit = options.Integer("limit") ?? 30; options.Complete();
			result = new { MasterVersion = version, Kind = kind, Items = catalog.Search(kind, query, id, limit) };
		}
		else
		{
			int id = options.Integer("id") ?? throw new ArgumentException("Required: --id"); options.Complete();
			result = catalog.DescribeUnit(id);
		}
	}
	else if (command == "inspect")
	{
		var document = SceneDocument.Read(options.Require("input")); options.Complete(); result = document.Summary();
	}
	else if (command == "validate")
	{
		string input = options.Require("input"); string resources = options.Require("resources"); options.Complete();
		var document = SceneDocument.Read(input);
		result = new { Valid = true, Prepared = document.Validate(resources), Summary = document.Summary() };
	}
	else if (command is "create" or "scene" or "party" or "unit-add" or "unit-edit" or "unit-remove" or "stories" or "set" or "labyrinth")
	{
		string resources = options.Require("resources"); string? output = options.Take("output"); bool dryRun = options.Boolean("dry-run") ?? false;
		if (output is null && !dryRun) throw new ArgumentException("Required: --output FILE or --dry-run true.");
		SceneDocument document;
		if (command == "create")
		{
			document = SceneDocument.Preset(options.Require("preset")); document.Root["MasterVersion"] = options.Require("master-version");
			int[] units = SceneDocument.Integers(options.Require("units"));
			if (units.Length is < 1 or > 5 || units.Any(id => id <= 1) || units.Distinct().Count() != units.Length) throw new ArgumentException("Create requires 1..5 distinct owned units.");
			using var catalog = new Catalog(resources, document.Version);
			document.InitializeUnits(catalog, units, SceneDocument.ReadUnitOptions(options));
			// The quest and difficulty are caller inputs; enemies, time limit and background come from the quest.
			if (document.IsLabyrinth)
			{
				options.RequirePresent("quest-id", "difficulty");
				document.DeclareUserUnits(units);
			}
		}
		else document = SceneDocument.Read(options.Require("input"));
		switch (command)
		{
			case "create":
				if (document.IsLabyrinth) document.EditLabyrinth(options);
				document.EditScene(options);
				break;
			case "scene": document.EditScene(options); break;
			case "party": document.Party(options.Require("units"), options.Integer("battle")); break;
			case "labyrinth": document.EditLabyrinth(options); break;
			case "unit-add":
			case "unit-edit":
				using (var catalog = new Catalog(resources, document.Version)) document.EditUnit(catalog, options, command == "unit-add");
				break;
			case "unit-remove":
				int id = int.Parse(options.Require("id"), System.Globalization.CultureInfo.InvariantCulture);
				if (!document.RemoveUnit(id)) throw new ArgumentException("Unit is not owned: " + id);
				break;
			case "stories": document.Stories(options.Require("ids")); break;
			case "set":
				if (options.Take("patch") is { } patch) document.Patch(patch);
				else document.Set(options.Require("path"), options.Require("value"));
				break;
		}
		options.Complete(); result = document.Save(resources, dryRun ? null : output);
	}
	else throw new ArgumentException("Unknown command: " + command + ". Use --help.");
	Console.WriteLine(JsonSerializer.Serialize(new { Ok = true, Result = result }, new JsonSerializerOptions { WriteIndented = true }));
	return 0;
}
catch (Exception exception)
{
	Console.Error.WriteLine(JsonSerializer.Serialize(new { Ok = false, Error = exception.GetType().Name, Message = exception.Message }));
	return 1;
}
