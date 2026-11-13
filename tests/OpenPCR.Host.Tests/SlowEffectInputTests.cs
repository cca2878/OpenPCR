using System;
using System.IO;
using System.Reflection;
using Elements;
using Google.Protobuf;
using Microsoft.Data.Sqlite;
using OpenPCR.Host;
using Pcr.Prefab;

internal static class SlowEffectInputTests
{
	internal static void Run(Action<bool, string> check)
	{
		using var database = new SqliteConnection("Data Source=:memory:");
		database.Open();
		using (var command = database.CreateCommand())
		{
			command.CommandText = "CREATE TABLE prefab_tree(prefab_id INTEGER PRIMARY KEY,data BLOB,source_sha256 TEXT,unity_version TEXT,extract_status TEXT);";
			command.ExecuteNonQuery();
		}
		var tree = new UnitPrefab { PrefabId = 1, SourceSha256 = new string('a', 64), UnityVersion = "6000.0.58f2", FormatVersion = 1 };
		void Write()
		{
			using var command = database.CreateCommand();
			command.CommandText = "INSERT OR REPLACE INTO prefab_tree VALUES(1,$data,$source,$unity,'ok');";
			command.Parameters.AddWithValue("$data", tree.ToByteArray());
			command.Parameters.AddWithValue("$source", tree.SourceSha256);
			command.Parameters.AddWithValue("$unity", tree.UnityVersion);
			command.ExecuteNonQuery();
		}
		void Reject(Action action, string expected, string message)
		{
			Exception? error = null;
			try { action(); } catch (Exception caught) { error = caught is TargetInvocationException ? caught.InnerException : caught; }
			check(error is InvalidDataException && error.Message.Contains(expected, StringComparison.Ordinal), message);
		}
		foreach (int version in new[] { 0, -1, 2 })
		{
			tree.FormatVersion = version; Write();
			Reject(() => new PrefabInputs(database).ReadTree(1), "format_version", "Unmarked and unsupported payload versions must require re-extraction.");
		}
		tree.FormatVersion = 1; Write();
		check(new PrefabInputs(database).ReadTree(1).FormatVersion == 1, "The current payload version must load without altering authored values.");
		var slot = new SkillSlot
		{
			BlurEffectsExtracted = true,
			ChangeSortDatasExtracted = true,
			ScaleChangersExtracted = true,
			SlowEffect = new Pcr.Prefab.SlowEffect(),
			ZoomEffect = new Pcr.Prefab.ZoomEffect()
		};
		var binding = new UnitPrefabBindings((_, _) => throw new InvalidOperationException("Unexpected prefab request."), new PrefabInputs(database));
		Skill Read() => (Skill)typeof(UnitPrefabBindings).GetMethod("ReadSkill", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(binding, new object[] { 1, slot })!;
		Reject(() => Read(), "SlowEffect.To", "A current-format skill cannot silently use the engine's constructor slow target.");
		foreach (float target in new[] { 0f, -0.0f, 0.14f, 0.9f, 1f })
		{
			slot.SlowEffect.To = target;
			check(BitConverter.SingleToInt32Bits(Read().SlowEffect.To) == BitConverter.SingleToInt32Bits(target), "An authored slow target must bind its exact Single bits, including explicit zero.");
		}
		slot.SlowEffect.ClearTo();
		Reject(() => Read(), "SlowEffect.To", "Clearing presence must reject even after another skill supplied its slow target.");
	}
}
