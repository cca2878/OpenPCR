using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Elements;
using Elements.Data;
using Microsoft.Data.Sqlite;

namespace OpenPCR.Host;

/// <summary>Read-only deployment rows feed the mirror's real ORM instances and caches.</summary>
internal sealed class RuntimeMasterData : IDisposable
{
	private readonly SqliteConnection database;
	// Manager's product database initialization is an InputBoundary. The host supplies every used table below.
	public MasterDataManager Manager { get; } = (MasterDataManager)RuntimeHelpers.GetUninitializedObject(typeof(MasterDataManager));
	public RuntimeMasterData(ResourceDatabases resources)
	{
		database = resources.Master;
		if (ManagerSingleton<MasterDataManager>.Instance is not null) throw new InvalidOperationException("A masterdata runtime is already active.");
		RuntimeBinding.Instance(typeof(ManagerSingleton<>), typeof(MasterDataManager), Manager);
	}
	public void LoadBattleTables()
	{
		Load("masterTalent", "talent");
		Load("masterUnitPosAdjustment", "unit_pos_adjustment");
		LoadAttenuations<MasterTpRecoveryAt.TpRecoveryAt>("masterTpRecoveryAt", "tp_recovery_at", value => value.limit_value);
		LoadAttenuations<MasterHpDrainAt.HpDrainAt>("masterHpDrainAt", "hp_drain_at", value => value.limit_value);
		Load("masterContentReleaseData", "content_release_data");
		Load("masterLabyrinthSetting", "labyrinth_setting");
		Load("masterSkillAction", "skill_action");
		Load("masterLabyrinthSkillActionReplace", "labyrinth_skill_action_replace");
		Load("masterSkillData", "skill_data");
		Load("masterUnitSkillData", "unit_skill_data");
		Load("masterUnitAttackPattern", "unit_attack_pattern");
		Load("masterUnitEnemyData", "unit_enemy_data");
		Load("masterEnemyParameter", "enemy_parameter");
		Load("masterEnemyMParts", "enemy_m_parts");
		var rf = Load("masterUnitSkillDataRf", "unit_skill_data_rf");
		Load("masterEnemyEnableVoice", "enemy_enable_voice");
		Load("masterEnemyIgnoreSkillRf", "enemy_ignore_skill_rf");
		Load("masterEnemyTalentWeakness", "enemy_talent_weakness");
		Load("masterTalentWeakness", "talent_weakness");
		Load("masterResistData", "resist_data");
		Load("masterAilmentData", "ailment_data");
		Load("masterResistVariationData", "resist_variation_data");
		Load("masterExceptEr", "except_er");
		Load("masterPositionSetting", "position_setting");
		Load("masterDodgeTpRecovery", "dodge_tp_recovery");
		Load("masterTalentFormationBonus", "talent_formation_bonus");
		var formation = Load("masterTalentFormationOverwrite", "talent_formation_overwrite");
		GroupContentTarget(formation, "_dictionaryOfListWithContentTypeAndTargetValue1");
		var extra = Load("masterExtraEffectData", "extra_effect_data");
		GroupContentTarget(extra, "_dictionaryOfListWithContentTypeAndTargetValue1");
		var clanMap = Load("masterClanBattle2MapData", "clan_battle_2_map_data", "id");
		Group(clanMap, "_dictionaryOfListWithClanBattleId", "_clan_battle_id", Keys("SELECT DISTINCT clan_battle_id FROM clan_battle_2_map_data"));
		Load("masterExEquipmentData", "ex_equipment_data");
		Load("masterEReduction", "e_reduction");
		var voice = Load("masterSpBattleVoice", "sp_battle_voice");
		Load("masterUnitRoleData", "unit_role_data");
		Load("masterSpskillLvInitializeData", "spskill_lv_initialize_data");
		var talent = Load("masterUnitTalent", "unit_talent", "talent_id");
		var conversion = Load("masterUnitConversion", "unit_conversion");
		Index(conversion, "_dictionaryOfOrmWithUnitId", "_unit_id");
		LoadUnitMetadata();
		Group(voice, "_dictionaryOfListWithUnitId", "_unit_id", Keys("SELECT unit_id FROM unit_data UNION SELECT unit_id FROM unit_enemy_data UNION SELECT prefab_id FROM unit_enemy_data"));
		Group(rf, "_dictionaryOfListWithSkillId", "_skill_id", Keys("SELECT skill_id FROM skill_data"));
		Group(rf, "_dictionaryOfListWithRfSkillId", "_rf_skill_id", Keys("SELECT skill_id FROM skill_data"));
		Group(talent, "_dictionaryOfListWithUnitId", "_unit_id", Keys("SELECT unit_id FROM unit_data UNION SELECT unit_id FROM unit_enemy_data UNION SELECT enemy_id FROM enemy_parameter"));
	}
	public void LoadLabyrinthRelicTables()
	{
		Load("masterLabyrinthRelicEffectGroup", "labyrinth_relic_effect_group");
		Load("masterLabyrinthRelicEffect", "labyrinth_relic_effect");
		var skill = Load("masterRelicSkillExecTime", "relic_skill_exec_time", "id");
		var action = Load("masterRelicActionExecTime", "relic_action_exec_time", "id");
		Group(skill, "_dictionaryOfListWithSkillId", "_skill_id", Keys("SELECT skill_id FROM skill_data"));
		Group(action, "_dictionaryOfListWithActionId", "_action_id", Keys("SELECT action_id FROM skill_action"));
	}
	public void PrepareLabyrinthQuestLookups(int category, int questId)
	{
		if (category is not (59 or 60) || questId <= 0) throw new ArgumentException("Explicit Labyrinth category and quest required.");
		foreach (object master in new object[] { Manager.masterTalentFormationOverwrite, Manager.masterExtraEffectData })
		{
			var groups = (IDictionary)RuntimeBinding.Get(master, "_dictionaryOfListWithContentTypeAndTargetValue1")!;
			var list = (IList)Activator.CreateInstance(groups.GetType().GetGenericArguments()[1])!;
			// Both complete tables were loaded from this deployment; absence is now an explicit query result.
			foreach (object row in ((IDictionary)RuntimeBinding.Get(master, "_lazyPrimaryKeyDictionary")!).Values)
				if ((int)RuntimeBinding.Get(row, "_content_type")! == category && (int)RuntimeBinding.Get(row, "_target_value_1")! == questId) list.Add(row);
			groups[unchecked((uint)category | ((ulong)(uint)questId << 32))] = list;
		}
	}
	private void LoadAttenuations<T>(string property, string table, Func<T, int> limit)
	{
		var master = Load(property, table);
		var cache = (IDictionary)RuntimeBinding.Get(master, "_lazyPrimaryKeyDictionary")!;
		var values = cache.Values.Cast<T>().ToList();
		values.Sort((left, right) => limit(left).CompareTo(limit(right)));
		if (values.Count == 0) throw new InvalidDataException("Empty attenuation table: " + table);
		RuntimeBinding.Set(master, "Attenuations", values);
	}
	private static readonly string[] ClanMapColumns = { "clan_battle_id", "lap_num_from", "lap_num_to", "aura_effect", "phase" };
	private static readonly string[] ClanMapFields = ClanMapColumns.Select(name => "_" + name).ToArray();
	private static readonly System.Collections.Concurrent.ConcurrentDictionary<(string, long, long, string?, ConstructorInfo), (object?[] Arguments, int[] Values, int[] BossIds)[]> clanMapRows = new();
	private static MasterClanBattle2MapData.ClanBattle2MapData PrepareClanMap(int[] values, int[] bossIds)
	{
		// Named deployed input projection; full product row constructor remains an explicit boundary.
		var row = (MasterClanBattle2MapData.ClanBattle2MapData)RuntimeHelpers.GetUninitializedObject(typeof(MasterClanBattle2MapData.ClanBattle2MapData));
		for (int index = 0; index < ClanMapColumns.Length; index++)
			RuntimeBinding.Set(row, ClanMapFields[index], (global::System.Int32)values[index]);
		RuntimeBinding.Set(row, "BossIds", (int[])bossIds.Clone());
		return row;
	}

	internal static string Column(string name) => string.Concat(name.Select(character => char.IsUpper(character) ? "_" + char.ToLowerInvariant(character) : character.ToString())).TrimStart('_');
	internal int[] EnemyPartLookupIds(IEnumerable<int> parentIds)
	{
		ArgumentNullException.ThrowIfNull(parentIds);
		var table = Manager.masterEnemyMParts ?? throw new InvalidOperationException("Enemy part table must be loaded before preparing part lookups.");
		return parentIds.Distinct().Select(table.Get).OfType<MasterEnemyMParts.EnemyMParts>()
			.SelectMany(parts => Enumerable.Range(1, 5).Select(parts.GetChildUnitId)).Where(id => id != 0).Distinct().ToArray();
	}
	public void PrepareEnemyLookupMisses(IEnumerable<int> ids, bool allowLabyrinth = false)
	{
		// Exact Native fallback order, including Sekai's different key. Dedicated Labyrinth rows
		// are projected only when the caller explicitly enables that input scope.
		var resolved = new Dictionary<int, Func<object?>>();
		var requested = ids.Distinct().ToArray();
		var identity = DatabaseIdentity();
		foreach (int id in requested)
		{
			if (RuntimeBinding.Call(Manager.masterEnemyParameter, "Get", id) is not null) continue;
			foreach (string name in new[] { "event", "tower", "sekai", "shiori", "sre", "talent_quest", "abyss", "acn", "mirage", "seven", "labyrinth", "mre" })
			{
				if (!enemyRowPresence.GetOrAdd((identity, name, id), _ =>
				{
					using var command = database.CreateCommand();
					command.CommandText = "SELECT COUNT(*) FROM " + name + "_enemy_parameter WHERE " + (name == "sekai" ? "sekai_enemy_id" : "enemy_id") + "=$id";
					command.Parameters.AddWithValue("$id", id);
					return Convert.ToInt64(command.ExecuteScalar(), CultureInfo.InvariantCulture) != 0;
				})) continue;
				if (name != "labyrinth" || !allowLabyrinth) throw new NotSupportedException("Enemy lookup requires mode-specific row conversion: " + name + "/" + id);
				using var select = database.CreateCommand();
				select.CommandText = "SELECT * FROM labyrinth_enemy_parameter WHERE enemy_id=$id";
				select.Parameters.AddWithValue("$id", id);
				using var reader = select.ExecuteReader();
				var constructor = typeof(MasterEnemyParameter.EnemyParameter).GetConstructors().Single();
				var arguments = ReadArguments("labyrinth_enemy_parameter", constructor, reader);
				if (!reader.Read()) throw new InvalidDataException("Labyrinth enemy row disappeared: " + id);
				var rowArguments = arguments();
				resolved.Add(id, () => constructor.Invoke(rowArguments));
				break;
			}
			if (!resolved.ContainsKey(id)) resolved.Add(id, () => null);
		}
		PreparedEnemyLookupInputs.BindFactories(Manager.masterEnemyParameter, resolved);
		if (allowLabyrinth && Manager.masterUnitTalent is { } talent)
		{
			var groups = (Dictionary<int, List<MasterUnitTalent.UnitTalent>>)RuntimeBinding.Get(talent, "_dictionaryOfListWithUnitId")!;
			var rows = ((IDictionary)RuntimeBinding.Get(talent, "_lazyPrimaryKeyDictionary")!).Values.Cast<MasterUnitTalent.UnitTalent>().ToArray();
			foreach (int id in requested)
				if (!groups.ContainsKey(id)) groups.Add(id, rows.Where(row => row.unit_id == id).OrderBy(row => row.talent_id).ToList());
		}
	}
	internal object Load(string property, string table, string? order = null)
	{
		var type = typeof(MasterDataManager).GetProperty(property)!.PropertyType;
		var constructor = type.GetConstructors().Single();
		var master = constructor.Invoke(new object?[] { null });
		RuntimeBinding.Set(Manager, property, master);
		var cacheField = type.GetField("_lazyPrimaryKeyDictionary", RuntimeBinding.Members) ?? throw new MissingFieldException(type.FullName, "_lazyPrimaryKeyDictionary");
		var cache = (IDictionary)cacheField.GetValue(master)!;
		var rowType = cacheField.FieldType.GetGenericArguments()[1];
		var rowConstructor = rowType.GetConstructors().Where(item => item.GetParameters().All(parameter => parameter.ParameterType.IsPrimitive || parameter.ParameterType == typeof(string)))
			.OrderByDescending(item => item.GetParameters().Length).FirstOrDefault();
		if (rowConstructor is null) throw new InvalidDataException("No row input constructor: " + rowType.FullName);
		bool compositeKey = cacheField.FieldType.GetGenericArguments()[0] == typeof(ulong);
		if (table == "clan_battle_2_map_data")
		{
			// The projected columns are read once per deployed file; every battle still gets fresh rows and BossIds arrays.
			var (path, length, written) = DatabaseIdentity();
			var inputs = clanMapRows.GetOrAdd((path, length, written, order, rowConstructor), _ =>
			{
				using var command = Select(table, order);
				using var reader = command.ExecuteReader();
				var arguments = ReadArguments(table, rowConstructor, reader);
				var list = new List<(object?[], int[], int[])>();
				while (reader.Read())
					list.Add((arguments(), ClanMapColumns.Select(name => reader.GetInt32(reader.GetOrdinal(name))).ToArray(),
						Enumerable.Range(1, 5).Select(index => reader.GetInt32(reader.GetOrdinal("boss_id_" + index))).ToArray()));
				return list.ToArray();
			});
			foreach (var (arguments, values, bossIds) in inputs) AddRow(cache, compositeKey, arguments, PrepareClanMap(values, bossIds));
		}
		else
		{
			// Constructor inputs are immutable strings/primitives, shared per process; every battle still gets fresh rows.
			var (path, length, written) = DatabaseIdentity();
			var rows = tableRows.GetOrAdd((path, length, written, table, order, rowConstructor), _ =>
			{
				using var command = Select(table, order);
				using var reader = command.ExecuteReader();
				var arguments = ReadArguments(table, rowConstructor, reader);
				var list = new List<object?[]>();
				while (reader.Read()) list.Add(arguments());
				return list.ToArray();
			});
			var create = factories.GetOrAdd(rowConstructor, CompileFactory);
			foreach (var arguments in rows) AddRow(cache, compositeKey, arguments, create(arguments));
		}
		type.GetField("_strictPrimaryKeyDictionary", RuntimeBinding.Members)?.SetValue(master, cache);
		return master;
	}

	private static readonly System.Collections.Concurrent.ConcurrentDictionary<(string, long, long, string, string?, ConstructorInfo), object?[][]> tableRows = new();
	private static readonly System.Collections.Concurrent.ConcurrentDictionary<ConstructorInfo, Func<object?[], object>> factories = new();

	private SqliteCommand Select(string table, string? order)
	{
		var command = database.CreateCommand();
		command.CommandText = "SELECT * FROM \"" + table + "\"" + (order is null ? "" : " ORDER BY \"" + order + "\"");
		return command;
	}

	/// <summary>Maps constructor parameters to columns once; the returned reader converts the current row.</summary>
	private static Func<object?[]> ReadArguments(string table, ConstructorInfo rowConstructor, SqliteDataReader reader)
	{
		var columns = Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, ordinal => ordinal, StringComparer.Ordinal);
		var parameters = rowConstructor.GetParameters();
		var names = parameters.Select(parameter => ConstructorColumn(table, parameter.Name!, columns)).ToArray();
		foreach (string name in names) if (!columns.ContainsKey(name)) throw new InvalidDataException(table + " missing required constructor column " + name);
		var ordinals = names.Select(name => columns[name]).ToArray();
		return () =>
		{
			var arguments = new object?[parameters.Length];
			for (int index = 0; index < parameters.Length; index++)
			{
				var parameter = parameters[index]; int ordinal = ordinals[index];
				// Arguments are cached per process (tableRows); equal values share one box.
				arguments[index] = SharedBoxes.Share(parameter.Name == "nameIsNull" ? reader.IsDBNull(ordinal) :
					Convert.ChangeType(reader.IsDBNull(ordinal) ? parameter.ParameterType == typeof(string) ? "" : 0 : reader.GetValue(ordinal), parameter.ParameterType, CultureInfo.InvariantCulture));
			}
			return arguments;
		};
	}

	private static void AddRow(IDictionary cache, bool compositeKey, object?[] arguments, object row)
	{
		object key = arguments[0]!;
		if (compositeKey)
			key = unchecked(Convert.ToUInt32(arguments[0], CultureInfo.InvariantCulture) | ((ulong)Convert.ToUInt32(arguments[1], CultureInfo.InvariantCulture) << 32));
		cache.Add(key, row);
	}

	private static Func<object?[], object> CompileFactory(ConstructorInfo constructor)
	{
		var arguments = System.Linq.Expressions.Expression.Parameter(typeof(object?[]));
		var values = constructor.GetParameters().Select((parameter, index) => (System.Linq.Expressions.Expression)System.Linq.Expressions.Expression.Convert(
			System.Linq.Expressions.Expression.ArrayIndex(arguments, System.Linq.Expressions.Expression.Constant(index)), parameter.ParameterType));
		var body = System.Linq.Expressions.Expression.Convert(System.Linq.Expressions.Expression.New(constructor, values), typeof(object));
		return System.Linq.Expressions.Expression.Lambda<Func<object?[], object>>(body, arguments).Compile();
	}

	private static string ConstructorColumn(string table, string name, IReadOnlyDictionary<string, int> columns)
	{
		if (name == "nameIsNull") return "name";
		if (columns.ContainsKey(name)) return name;
		if (table == "e_reduction" && (name.StartsWith("Threshold", StringComparison.Ordinal) || name.StartsWith("Value", StringComparison.Ordinal))) return Column(name[..^1]) + "_" + name[^1];
		if (table == "ex_equipment_data")
		{
			string? special = name switch
			{
				"DefaultCritical" => "default_physical_critical",
				"MaxCritical" => "max_physical_critical",
				"DefaultPenetrate" => "default_physical_penetrate",
				"MaxPenetrate" => "max_physical_penetrate",
				"Skill1" => "passive_skill_id_1",
				"Skill2" => "passive_skill_id_2",
				"SkillPower" => "passive_skill_power",
				_ => null
			};
			if (special is not null) return special;
		}
		return Column(name);
	}

	private void LoadUnitMetadata()
	{
		var master = new MasterUnitData(null);
		RuntimeBinding.Set(Manager, "masterUnitData", master);
		var rows = (IDictionary)RuntimeBinding.Get(master, "_lazyPrimaryKeyDictionary")!;
		var fields = typeof(MasterUnitData.UnitData).GetFields(RuntimeBinding.Members).Where(field => !field.IsStatic).ToArray();
		using var command = database.CreateCommand(); command.CommandText = "SELECT * FROM unit_data";
		using var reader = command.ExecuteReader();
		while (reader.Read())
		{
			var row = (MasterUnitData.UnitData)RuntimeHelpers.GetUninitializedObject(typeof(MasterUnitData.UnitData));
			foreach (var field in fields)
			{
				// This slot is assigned by the enemy-data constructor, not by the unit_data ORM.
				if (field.Name == "<VisualChangeFlag>k__BackingField") continue;
				string column = field.Name switch
				{
					"_AtkCastTime" => "normal_atk_cast_time",
					"_CutIn1" => "cutin_1",
					"_CutIn2" => "cutin_2",
					"<VisualChangeFlag>k__BackingField" => "visual_change_flag",
					_ => Column(field.Name)
				};
				field.SetValue(row, Wrap(reader[column], field.FieldType));
			}
			rows.Add(reader.GetInt32(reader.GetOrdinal("unit_id")), row);
		}
		// Native MasterUnitEnemyData.WholeWarmUp publishes the enemy-side
		// UnitData projection into this same cache before resource consumers run.
		foreach (var enemy in ((IDictionary)RuntimeBinding.Get(Manager.masterUnitEnemyData, "_strictPrimaryKeyDictionary")!).Values.Cast<MasterUnitEnemyData.UnitEnemyData>())
		{
			var row = new MasterUnitData.UnitData(enemy);
			rows.Add((int)row.UnitId, row);
		}
		RuntimeBinding.Set(master, "_strictPrimaryKeyDictionary", rows);
	}
	internal static object Wrap(object value, Type type)
	{
		if (type.IsEnum) return Enum.ToObject(type, value);
		if (type.IsPrimitive || type == typeof(string)) return Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
		var conversion = type.GetMethods(BindingFlags.Public | BindingFlags.Static).Single(method => method.Name == "op_Implicit" && method.ReturnType == type);
		return conversion.Invoke(null, new[] { Convert.ChangeType(value, conversion.GetParameters()[0].ParameterType, CultureInfo.InvariantCulture) })!;
	}
	// Grouping keys and enemy-row presence come from the same read-only deployment as tableRows and are shared per process.
	private static readonly System.Collections.Concurrent.ConcurrentDictionary<((string, long, long), string), int[]> keyRows = new();
	private static readonly System.Collections.Concurrent.ConcurrentDictionary<((string, long, long), string, int), bool> enemyRowPresence = new();
	private (string Path, long Length, long Written) DatabaseIdentity()
	{
		var file = new FileInfo(database.DataSource);
		return (file.FullName, file.Length, file.LastWriteTimeUtc.Ticks);
	}
	private int[] Keys(string sql) => keyRows.GetOrAdd((DatabaseIdentity(), sql), _ =>
	{
		using var command = database.CreateCommand(); command.CommandText = sql;
		using var reader = command.ExecuteReader(); var keys = new List<int>();
		while (reader.Read()) keys.Add(reader.GetInt32(0));
		return keys.ToArray();
	});
	private static readonly System.Collections.Concurrent.ConcurrentDictionary<Type, Func<object>> listFactories = new();
	private static Func<object> ListFactory(Type listType) => listFactories.GetOrAdd(listType, type =>
		System.Linq.Expressions.Expression.Lambda<Func<object>>(System.Linq.Expressions.Expression.New(type)).Compile());
	private void GroupContentTarget(object master, string field)
	{
		var groups = (IDictionary)RuntimeBinding.Get(master, field)!;
		var newList = ListFactory(groups.GetType().GetGenericArguments()[1]);
		foreach (int id in Keys("SELECT unit_id FROM unit_data UNION SELECT enemy_id FROM enemy_parameter"))
			foreach (int category in new[] { 9, 32 })
			{ ulong key = unchecked((uint)category | ((ulong)(uint)id << 32)); if (!groups.Contains(key)) groups.Add(key, newList()); }
		foreach (object row in ((IDictionary)RuntimeBinding.Get(master, "_lazyPrimaryKeyDictionary")!).Values)
		{
			int category = (int)(global::System.Int32)RuntimeBinding.Get(row, "_content_type")!;
			int target = (int)(global::System.Int32)RuntimeBinding.Get(row, "_target_value_1")!;
			ulong key = unchecked((uint)category | ((ulong)(uint)target << 32));
			if (!groups.Contains(key)) groups.Add(key, newList());
			((IList)groups[key]!).Add(row);
		}
	}
	// Unique secondary-key cache (the Native _SelectWith* single-row query); absent keys stay misses.
	private static void Index(object master, string field, string column)
	{
		var index = (IDictionary)RuntimeBinding.Get(master, field)!;
		foreach (object row in ((IDictionary)RuntimeBinding.Get(master, "_lazyPrimaryKeyDictionary")!).Values)
			index.Add((int)(global::System.Int32)RuntimeBinding.Get(row, column)!, row);
	}
	private static void Group(object master, string field, string column, IEnumerable<int> keys)
	{
		var groups = (IDictionary)RuntimeBinding.Get(master, field)!;
		var newList = ListFactory(groups.GetType().GetGenericArguments()[1]);
		foreach (int key in keys.Append(0)) if (!groups.Contains(key)) groups.Add(key, newList());
		foreach (object row in ((IDictionary)RuntimeBinding.Get(master, "_lazyPrimaryKeyDictionary")!).Values)
		{
			var value = RuntimeBinding.Get(row, column)!;
			int key = (int)value;
			if (!groups.Contains(key)) groups.Add(key, newList());
			((IList)groups[key]!).Add(row);
		}
	}
	public void Dispose()
	{
		if (Manager.masterEnemyParameter is { } enemyTable) PreparedEnemyLookupInputs.Release(enemyTable);
		if (ReferenceEquals(ManagerSingleton<MasterDataManager>.Instance, Manager)) Manager.DestroyInstance();
	}
}
