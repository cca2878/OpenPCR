using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Elements;
using Elements.Data;

namespace OpenPCR.Growth;

public sealed partial class GrowthCalculator
{
	public TeamEnhanceParameter? CalculateTeam(bool enemy = false)
	{
		var team = enemy ? ActiveInput.EnemyTeam : ActiveInput.Team;
		if (team is null) return null;
		var pki = team.PrincessKnight;
		var levels = new Dictionary<int, int>();
		var buffs = new Dictionary<int, Dictionary<eParamType, int>>();
		var roles = new Dictionary<int, Dictionary<UnitRoleDefine.eRoleParamType, int>>();
		if (pki is not null)
		{
			foreach (var info in pki.TalentLevels)
			{
				long point = info.TotalPoint;
				// MasterExperienceTalentLevel.CalcTalentLevel: last eligible row by level.
				var row = master.Rows("experience_talent_level").OrderBy(r => r.Int("talent_level")).LastOrDefault(r => r.Long("total_point") <= point) ?? master.Find("experience_talent_level", ("talent_level", 0));
				levels.Add(info.TalentId, row?.Int("total_enhance_value") ?? 0);
			}
			var nodes = master.Rows("talent_skill_node");
			int minPage = nodes.Min(node => node.Int("page_num"));
			var latestInfos = pki.LastEnhancedPageNodes.ToArray();
			int latestPage = GetTalentSkillLatestPageNum(latestInfos, minPage);
			foreach (var talent in master.Rows("talent"))
			{
				int id = talent.Int("talent_id"); var result = new Dictionary<eParamType, int>();
				foreach (var node in nodes) if (node.Int("page_num") < latestPage) AddNode(result, node, MaxLevel(node), id, "talent");
				foreach (var info in latestInfos)
				{
					var node = master.Find("talent_skill_node", ("node_id", info.NodeId));
					if (node is not null) AddNode(result, node, info.EnhanceLevel, id, "talent");
				}
				var latestInfo = pki.TeamSkillLatestNode;
				if (latestInfo is not null)
				{
					var latest = master.Find("team_skill_node", ("node_id", latestInfo.NodeId));
					if (latest is not null)
					{
						foreach (var node in master.Rows("team_skill_node")) if (node.Int("pos_y") < latest.Int("pos_y")) AddNode(result, node, MaxLevel(node), id, "team");
						AddNode(result, latest, latestInfo.EnhanceLevel, id, "team");
					}
				}
				buffs.Add(id, result);
			}
		}
		foreach (var info in team.Roles)
		{
			int role = info.UnitRoleId; var result = new Dictionary<UnitRoleDefine.eRoleParamType, int>();
			if (info.SlotLevels.Count != 4 || info.EnhanceLevels.Count != 4) throw new InvalidDataException("Unit role requires four slot and enhance levels: " + role);
			for (int slot = 1; slot <= 4; slot++)
			{
				int masteryId = master.Require("unit_role_mastery_id", ("unit_role_id", role), ("slot_id", slot)).Int("mastery_id");
				var row = master.Require("unit_role_mastery_enhance_data", ("mastery_id", masteryId), ("slot_level", info.SlotLevels[slot - 1]), ("enhance_level", info.EnhanceLevels[slot - 1]));
				for (int i = 1; i <= 2; i++)
				{
					int type = row.Int("role_param_type_" + i);
					if (type != 0) Add(result, (UnitRoleDefine.eRoleParamType)type, row.Int("enhance_value_" + i));
				}
			}
			roles.Add(role, result);
		}
		var output = new TeamEnhanceParameter(null, null, true);
		RuntimeBinding.Set(output, "talentLevelParameter", levels);
		RuntimeBinding.Set(output, "knightEnhanceBuffParameter", buffs);
		RuntimeBinding.Set(output, "roleBuffParameter", roles);
		return output;
	}
	private int GetTalentSkillLatestPageNum(RawSkillNode[] infos, int minPage)
	{
		// Native FirstOrDefault reads the first node, even for a synthetic mixed-page list.
		return infos.Length == 0 ? minPage : master.Find("talent_skill_node", ("node_id", infos[0].NodeId))?.Int("page_num") ?? minPage;
	}
	private static int MaxLevel(GrowthMasterData.Row node) => Enumerable.Range(1, 5).LastOrDefault(level => node.Int("enhance_level_id_" + level) != 0);
	private void AddNode(Dictionary<eParamType, int> result, GrowthMasterData.Row node, int level, int talent, string prefix)
	{
		if (level == -1) level = MaxLevel(node);
		if (level is < 1 or > 5) return;
		var row = master.Find(prefix + "_skill_enhance_level", ("enhance_level_id", node.Int("enhance_level_id_" + level)));
		if (row is null) return;
		for (int index = 1; index <= 5; index++)
		{
			int enhance = row.Int("enhance_id_" + index);
			var data = master.Find(prefix + "_skill_enhance_data", ("enhance_id", enhance));
			if (data is not null && Enumerable.Range(1, 5).Any(i => data.Int("talent_id_" + i) == talent)) Add(result, (eParamType)data.Int("parameter_type"), data.Int("enhance_value"));
		}
	}
	private static void Add<T>(Dictionary<T, int> dictionary, T key, int value) where T : notnull => dictionary[key] = unchecked(dictionary.GetValueOrDefault(key) + value);
}
