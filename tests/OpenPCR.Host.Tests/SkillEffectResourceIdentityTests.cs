using System;
using System.IO;
using Microsoft.Data.Sqlite;
using OpenPCR.Host;

internal static class SkillEffectResourceIdentityTests
{
	internal static void Run(Action<bool, string> check)
	{
		string root = Path.Combine(Path.GetTempPath(), "openpcr-effect-identities-" + Guid.NewGuid());
		const string version = "200001010000";
		Directory.CreateDirectory(Path.Combine(root, "masterdata"));
		Directory.CreateDirectory(Path.Combine(root, "prefab"));
		try
		{
			using (var master = Open(Path.Combine(root, "masterdata", version + "_unhashed.db")))
			using (var command = master.CreateCommand()) { command.CommandText = "CREATE TABLE synthetic_marker(id INTEGER);"; command.ExecuteNonQuery(); }
			using (var prefab = Open(Path.Combine(root, "prefab", "unit_prefab_native.sqlite3")))
			{
				using var command = prefab.CreateCommand();
				command.CommandText = """
					CREATE TABLE prefab_meta(key TEXT PRIMARY KEY,value TEXT);
					INSERT INTO prefab_meta VALUES('schema_version','10'),('unity_version','6000.0.58f2');
					CREATE TABLE skill_effect_component(bundle TEXT,prefab_path_id INTEGER,prefab_name TEXT,component_class TEXT,
					 is_repeat INTEGER,particle_path_id INTEGER,root_position_x BLOB,root_position_y BLOB,root_position_z BLOB,component_index INTEGER);
					CREATE TABLE skill_effect_component_data(bundle TEXT,prefab_path_id INTEGER,component_index INTEGER,data TEXT);
					INSERT INTO skill_effect_component VALUES
					 ('bundle-a',1,'alpha','SkillEffectCtrl',0,0,$zero,$zero,$zero,0),
					 ('bundle-b',1,'beta','SkillEffectCtrl',0,0,$zero,$zero,$zero,0),
					 ('bundle-a',2,'shared','SkillEffectCtrl',0,0,$zero,$zero,$zero,0),
					 ('bundle-b',2,'shared','SkillEffectCtrl',0,0,$zero,$zero,$zero,0),
					 ('bundle-a',3,'unique','SkillEffectCtrl',0,0,$zero,$zero,$zero,0);
					INSERT INTO skill_effect_component_data VALUES
					 ('bundle-a',1,0,'{"m_Enabled":1}'),('bundle-b',1,0,'{"m_Enabled":0}'),
					 ('bundle-a',2,0,'{"m_Enabled":1}'),('bundle-b',2,0,'{"m_Enabled":1}'),
					 ('bundle-a',3,0,'{"m_Enabled":1}');
					""";
				command.Parameters.AddWithValue("$zero", BitConverter.GetBytes(0d)); command.ExecuteNonQuery();
			}
			using var databases = new ResourceDatabases(root, version);
			void Ambiguous(Action action, string message)
			{
				bool rejected = false;
				try { action(); } catch (InvalidDataException error) when (error.Message == "Ambiguous authored effect identity: 1") { rejected = true; }
				check(rejected, message);
			}
			Ambiguous(() => new SkillEffectResources(databases).Read(1), "An uncached global path ID must reject conflicting bundles.");
			var named = new SkillEffectResources(databases);
			var alpha = named.Read("alpha"); var beta = named.Read("beta");
			check(alpha.Bundle == "bundle-a" && alpha.Name == "alpha" && named.Enabled(alpha), "An explicit name must retain bundle A's resource and fields.");
			check(beta.Bundle == "bundle-b" && beta.Name == "beta" && !named.Enabled(beta), "An explicit name must retain bundle B's resource and fields.");
			Ambiguous(() => named.Read(1), "Name lookups must not suppress global path-ID ambiguity.");
			var child = new SkillEffectResources(databases);
			var childA = child.ReadChild(alpha, 1); var childB = child.ReadChild(beta, 1);
			check(childA.Bundle == "bundle-a" && childA.Name == "alpha" && child.Enabled(childA), "Child lookup must use the parent's bundle A.");
			check(childB.Bundle == "bundle-b" && childB.Name == "beta" && !child.Enabled(childB), "Child lookup must use the parent's bundle B.");
			check(ReferenceEquals(childA, child.ReadChild(alpha, 1)), "Bundle-scoped cache must preserve immutable resource identity.");
			Ambiguous(() => child.Read(1), "Child lookups must not suppress global path-ID ambiguity.");
			var reversed = new SkillEffectResources(databases);
			_ = reversed.ReadChild(beta, 1); _ = reversed.Read("alpha");
			Ambiguous(() => reversed.Read(1), "The global ambiguity result must not depend on which conflicting bundle was read first.");
			check(named.Read(2).Name == "shared", "Equivalent cross-bundle resources must retain the accepted global lookup behavior.");
			var unique = named.Read("unique");
			check(ReferenceEquals(unique, named.Read(3)), "Name and unambiguous global lookup must share a complete resource identity.");
			check(ReferenceEquals(unique, named.ReadChild(alpha, 3)), "Parent lookup may reuse the same bundle and path ID.");
		}
		finally { Directory.Delete(root, true); }
	}
	private static SqliteConnection Open(string path)
	{
		var connection = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = path, Pooling = false }.ToString());
		connection.Open(); return connection;
	}
}
