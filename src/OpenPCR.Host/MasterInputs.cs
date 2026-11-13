using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;

namespace OpenPCR.Host;

/// <summary>Raw masterdata dependencies for resource preparation; no growth or game-side SetUp calculations.</summary>
internal sealed record MasterSkillInput(int SkillId, double CastTime, IReadOnlyList<int> ActionIds, IReadOnlyList<int> DependedIds);
internal sealed record MasterActionInput(int ActionId, int ActionType);

internal sealed class MasterInputs
{
	private readonly SqliteConnection database;
	internal MasterInputs(SqliteConnection database) { this.database = database; }
	public MasterSkillInput Skill(int skillId)
	{
		using var command = database.CreateCommand();
		command.CommandText = "SELECT * FROM skill_data WHERE skill_id=$id";
		command.Parameters.AddWithValue("$id", skillId);
		using var reader = command.ExecuteReader();
		if (!reader.Read()) throw new InvalidDataException("Missing skill_data.skill_id=" + skillId);
		var actions = new int[10]; var depended = new int[10];
		for (int i = 0; i < 10; i++)
		{
			actions[i] = reader.GetInt32(reader.GetOrdinal("action_" + (i + 1)));
			depended[i] = reader.GetInt32(reader.GetOrdinal("depend_action_" + (i + 1)));
		}
		return new MasterSkillInput(reader.GetInt32(reader.GetOrdinal("skill_id")),
			reader.GetDouble(reader.GetOrdinal("skill_cast_time")), actions, depended);
	}
	public MasterActionInput Action(int actionId)
	{
		using var command = database.CreateCommand();
		command.CommandText = "SELECT action_id,action_type FROM skill_action WHERE action_id=$id";
		command.Parameters.AddWithValue("$id", actionId);
		using var reader = command.ExecuteReader();
		if (!reader.Read()) throw new InvalidDataException("Missing skill_action.action_id=" + actionId);
		return new MasterActionInput(reader.GetInt32(0), checked((ushort)reader.GetInt32(1)));
	}
}
