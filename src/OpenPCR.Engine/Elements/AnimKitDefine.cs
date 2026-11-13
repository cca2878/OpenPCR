using global::Elements;
using System;
namespace Elements
{
	public class AnimKitDefine
	{

		[global::OpenPCR.Migration("f3a47ba3-7343-4f9d-b1f3-69b9ec2c4e5b")]
		public static string GetAnimeName(eAnimKitCharacterAnimeId animeId, int unitId = -1, int weaponId = -1, int index1 = -1, int index2 = -1, int index3 = -1)
		{
			if (UnitUtility.JudgeIsGuest(unitId))
			{
				weaponId = 0;
			}
			if (animeId == eAnimKitCharacterAnimeId.RUN && UnitUtility.IsEnemyUnit(unitId))
			{
				animeId = eAnimKitCharacterAnimeId.WALK;
			}
			string animeName = AnimKitConverterDefine.DataDic[animeId].AnimeName;
			switch (animeId)
			{
				case eAnimKitCharacterAnimeId.AWAKE_MAP:
				case eAnimKitCharacterAnimeId.AWAKE_MAP_RELEASE:
				case eAnimKitCharacterAnimeId.AWAKE:
				case eAnimKitCharacterAnimeId.ATTACK_SKIPQUEST:
				case eAnimKitCharacterAnimeId.AWAKE_RESULT:
					if (weaponId == 0)
					{
						return string.Format("{0:D6}_" + animeName, unitId);
					}
					return string.Format("{0:D2}_" + animeName, weaponId);
				case eAnimKitCharacterAnimeId.IDLE:
				case eAnimKitCharacterAnimeId.IDLE_NO_WEAPON:
				case eAnimKitCharacterAnimeId.IDLE_STAND_BY:
				case eAnimKitCharacterAnimeId.RUN_GAME_START:
				case eAnimKitCharacterAnimeId.WALK:
				case eAnimKitCharacterAnimeId.JOY_SHORT:
				case eAnimKitCharacterAnimeId.JOY_LONG:
				case eAnimKitCharacterAnimeId.COOP_STAND_BY:
				case eAnimKitCharacterAnimeId.STAND_BY:
				case eAnimKitCharacterAnimeId.RUN:
				case eAnimKitCharacterAnimeId.LANDING:
				case eAnimKitCharacterAnimeId.LOADING:
				case eAnimKitCharacterAnimeId.IDLE_SKIP:
				case eAnimKitCharacterAnimeId.DAMAGE_SKIP:
				case eAnimKitCharacterAnimeId.ANNIHILATION:
				case eAnimKitCharacterAnimeId.IDLE_MAP:
				case eAnimKitCharacterAnimeId.IDLE_MAP_S:
				case eAnimKitCharacterAnimeId.DAMEGE:
				case eAnimKitCharacterAnimeId.GAME_START:
				case eAnimKitCharacterAnimeId.IDLE_RESULT:
				case eAnimKitCharacterAnimeId.IDLE_RESULT_LATER:
				case eAnimKitCharacterAnimeId.RACE_ENEMY:
					if (index1 == -1)
					{
						if (weaponId == 0)
						{
							return string.Format("{0:D6}_" + animeName, unitId);
						}
						return string.Format("{0:D2}_" + animeName, weaponId);
					}
					if (weaponId == 0)
					{
						return string.Format("{0:D6}_{1:D1}_" + animeName, unitId, index1);
					}
					return string.Format("{0:D2}_{1:D1}_" + animeName, unitId, index1);
				case eAnimKitCharacterAnimeId.JOY_RESULT:
					if (index1 == -1)
					{
						return string.Format("{0:D6}_" + animeName, unitId);
					}
					return string.Format("{0:D6}_{1:D1}_" + animeName, unitId, index1);
				case eAnimKitCharacterAnimeId.DIE_MAP:
				case eAnimKitCharacterAnimeId.DIE:
				case eAnimKitCharacterAnimeId.LOSE:
				case eAnimKitCharacterAnimeId.DEFEAT:
				case eAnimKitCharacterAnimeId.DIE_LOOP:
					if (index1 != -1 && index2 != -1)
					{
						return string.Format("{0:D6}_{1:D1}_" + animeName + "_{2:D1}", unitId, index1, index2);
					}
					if (index1 != -1)
					{
						return string.Format("{0:D6}_" + animeName + "_{1:D1}", unitId, index1);
					}
					if (index2 != -1)
					{
						return string.Format("{0:D6}_{1:D1}_" + animeName, unitId, index2);
					}
					if (weaponId == 0)
					{
						return string.Format("{0:D6}_" + animeName, unitId);
					}
					return string.Format("{0:D2}_" + animeName, weaponId);
				case eAnimKitCharacterAnimeId.SKILL:
				case eAnimKitCharacterAnimeId.SPECIAL_SKILL:
				case eAnimKitCharacterAnimeId.SPECIAL_SKILL_EVOLUTION:
				case eAnimKitCharacterAnimeId.SKILL_EVOLUTION:
				case eAnimKitCharacterAnimeId.SUB_UNION_BURST:
				case eAnimKitCharacterAnimeId.SUMMON:
				case eAnimKitCharacterAnimeId.SPECIAL_IDLE:
					{
						if (index3 == -1)
						{
							string text = string.Format("{0:D6}_" + animeName + "{1:D1}", unitId, index1);
							if (index2 == -1)
							{
								return text;
							}
							return $"{text}_{index2:D1}";
						}
						string text2 = string.Format("{0:D6}_{2:D1}_" + animeName + "{1:D1}", unitId, index1, index3);
						if (index2 == -1)
						{
							return text2;
						}
						return $"{text2}_{index2:D1}";
					}
				case eAnimKitCharacterAnimeId.PRINCESS_SKILL:
					if (index2 == -1)
					{
						if (index3 == -1)
						{
							return $"{unitId}_p_{index1}_skill";
						}
						return $"{unitId}_{index3}_p_{index1}_skill";
					}
					if (index3 == -1)
					{
						return $"{unitId}_p_{index1}_skill_{index2}";
					}
					return $"{unitId}_{index3}_p_{index1}_skill_{index2}";
				case eAnimKitCharacterAnimeId.PRINCESS_SKILL_EVOLUTION:
					if (index2 == -1)
					{
						if (index3 == -1)
						{
							return $"{unitId}_p_{index1}_skill_evolution";
						}
						return $"{unitId}_{index3}_p_{index1}_skill_evolution";
					}
					if (index3 == -1)
					{
						return $"{unitId}_p_{index1}_skill_evolution_{index2}";
					}
					return $"{unitId}_{index3}_p_{index1}_skill_evolution_{index2}";
				case eAnimKitCharacterAnimeId.IDLE_MULTI_TARGET:
				case eAnimKitCharacterAnimeId.DAMAGE_MULTI_TARGET:
					return string.Format("{0:D6}_{2}{1:D1}", unitId, index2, animeName);
				case eAnimKitCharacterAnimeId.ATTACK:
					if (index3 == -1)
					{
						if (weaponId == 0)
						{
							return string.Format("{0:D6}_" + animeName, unitId);
						}
						return string.Format("{0:D2}_" + animeName, weaponId);
					}
					return string.Format("{0:D6}_{1:D1}_" + animeName, unitId, index3);
				case eAnimKitCharacterAnimeId.SLEEP:
					return $"{unitId:D6}_{animeName}_{index2:D1}";
				case eAnimKitCharacterAnimeId.DUNGEON_IDLE_SKIP:
				case eAnimKitCharacterAnimeId.STORY_EYE_IDLE:
				case eAnimKitCharacterAnimeId.STORY_EYE_BLINK:
				case eAnimKitCharacterAnimeId.STORY_EYE_OPEN:
				case eAnimKitCharacterAnimeId.STORY_EYE_CLOSE:
				case eAnimKitCharacterAnimeId.STORY_MOUTH_IDLE:
				case eAnimKitCharacterAnimeId.STORY_MOUTH_TALK:
				case eAnimKitCharacterAnimeId.STORY_MOUTH_OPEN:
				case eAnimKitCharacterAnimeId.STORY_MOUTH_CLOSE:
					return animeName;
				case eAnimKitCharacterAnimeId.SUMMON_SKILL_EFFECT:
				case eAnimKitCharacterAnimeId.SUMMON_SKILL_EFFECT_REVERSE:
				case eAnimKitCharacterAnimeId.SUMMON_SKILL_EFFECT_WALK:
				case eAnimKitCharacterAnimeId.SUMMON_SKILL_EFFECT_ATTACK:
				case eAnimKitCharacterAnimeId.SUMMON_SKILL_EFFECT_IDLE:
				case eAnimKitCharacterAnimeId.SUMMON_SKILL_EFFECT_SUMMON:
				case eAnimKitCharacterAnimeId.SUMMON_SKILL_EFFECT_DIE:
				case eAnimKitCharacterAnimeId.TREASURE_EFFECT01:
				case eAnimKitCharacterAnimeId.TREASURE_EFFECT02:
				case eAnimKitCharacterAnimeId.TREASURE_EFFECT03:
				case eAnimKitCharacterAnimeId.TREASURE_EFFECT04:
				case eAnimKitCharacterAnimeId.TREASURE_EFFECT05:
				case eAnimKitCharacterAnimeId.TREASURE_EFFECT06:
				case eAnimKitCharacterAnimeId.TREASURE_OUT_EFFECT01:
				case eAnimKitCharacterAnimeId.TREASURE_OUT_EFFECT02:
				case eAnimKitCharacterAnimeId.TREASURE_OUT_EFFECT03:
				case eAnimKitCharacterAnimeId.TREASURE_OUT_EFFECT04:
				case eAnimKitCharacterAnimeId.TREASURE_OUT_EFFECT05:
				case eAnimKitCharacterAnimeId.TREASURE_OUT_EFFECT06:
					return $"{index1:D7}_{animeName}";
				case eAnimKitCharacterAnimeId.COMBINE_JOY_RESULT:
					return string.Format("{0:D6}" + animeName + "{1:D6}", unitId, index1);
				case eAnimKitCharacterAnimeId.SMILE:
				case eAnimKitCharacterAnimeId.RARITYUP_POSING:
				case eAnimKitCharacterAnimeId.MANA_IDLE:
				case eAnimKitCharacterAnimeId.MANA_JUMP:
				case eAnimKitCharacterAnimeId.STAMINA_BOW:
				case eAnimKitCharacterAnimeId.FRIEND_HIGHTOUCH:
				case eAnimKitCharacterAnimeId.RUN_JUMP:
				case eAnimKitCharacterAnimeId.RUN_HIGH_JUMP:
				case eAnimKitCharacterAnimeId.NO_WEAPON_IDLE:
				case eAnimKitCharacterAnimeId.NO_WEAPON_JOY_SHORT:
				case eAnimKitCharacterAnimeId.NO_WEAPON_RUN:
				case eAnimKitCharacterAnimeId.NO_WEAPON_RUN_SUPER:
				case eAnimKitCharacterAnimeId.RACE_EAT_JUN:
				case eAnimKitCharacterAnimeId.RACE_EAT_NORMAL:
				case eAnimKitCharacterAnimeId.RACE_EAT_SHAVEICE:
				case eAnimKitCharacterAnimeId.RACE_RUN_SPRING_JUMP:
				case eAnimKitCharacterAnimeId.RACE_BALLOON_FLYING_DOWN:
				case eAnimKitCharacterAnimeId.RACE_BALLOON_FLYING_LOOP:
				case eAnimKitCharacterAnimeId.RACE_BALLOON_FLYING_UP:
				case eAnimKitCharacterAnimeId.RACE_BALLOON_IN:
				case eAnimKitCharacterAnimeId.RACE_BALLOON_OUT:
				case eAnimKitCharacterAnimeId.RACE_RUN:
				case eAnimKitCharacterAnimeId.UNIT_RACE:
				case eAnimKitCharacterAnimeId.DEAR_IDOL:
				case eAnimKitCharacterAnimeId.DEAR_JUMP:
				case eAnimKitCharacterAnimeId.DEAR_SMILE:
					return string.Format("{0:D6}_" + animeName, (weaponId == 0) ? unitId : 0);
				case eAnimKitCharacterAnimeId.EVENT_BANNER_MOTION:
				case eAnimKitCharacterAnimeId.EVENT_BANNER_MOTION_CIRCLE:
				case eAnimKitCharacterAnimeId.FKE_IDLE:
				case eAnimKitCharacterAnimeId.EKD_GLD_01:
				case eAnimKitCharacterAnimeId.EKD_GLD_02:
				case eAnimKitCharacterAnimeId.EKD_GLD_03:
				case eAnimKitCharacterAnimeId.EKD_GLD_04:
				case eAnimKitCharacterAnimeId.EKD_GLD_05:
				case eAnimKitCharacterAnimeId.EKD_GLD_06:
				case eAnimKitCharacterAnimeId.EKD_IDLE_ANGER:
				case eAnimKitCharacterAnimeId.EKD_IDLE_JOY:
				case eAnimKitCharacterAnimeId.EKD_IDLE_JOY2:
				case eAnimKitCharacterAnimeId.EKD_IDLE_NORMAL:
				case eAnimKitCharacterAnimeId.EKD_IDLE_SAD:
				case eAnimKitCharacterAnimeId.EKD_IDLE_SURPRISE1:
				case eAnimKitCharacterAnimeId.EKD_IDLE_SURPRISE2:
				case eAnimKitCharacterAnimeId.EKD_OTHER_01:
				case eAnimKitCharacterAnimeId.EKD_OTHER_02:
				case eAnimKitCharacterAnimeId.EKD_OTHER_03:
				case eAnimKitCharacterAnimeId.EKD_OTHER_04:
				case eAnimKitCharacterAnimeId.EKD_OTHER_05:
				case eAnimKitCharacterAnimeId.EKD_OTHER_06:
				case eAnimKitCharacterAnimeId.EKD_OTHER_07:
				case eAnimKitCharacterAnimeId.EKD_OTHER_08:
				case eAnimKitCharacterAnimeId.EKD_OTHER_09:
				case eAnimKitCharacterAnimeId.EKD_OTHER_10:
				case eAnimKitCharacterAnimeId.EKD_PARTY_01:
				case eAnimKitCharacterAnimeId.EKD_PARTY_02:
				case eAnimKitCharacterAnimeId.EKD_PARTY_03:
				case eAnimKitCharacterAnimeId.EKD_PARTY_04:
				case eAnimKitCharacterAnimeId.EKD_PARTY_05:
				case eAnimKitCharacterAnimeId.EKD_PARTY_06:
				case eAnimKitCharacterAnimeId.EKD_PARTY_07:
				case eAnimKitCharacterAnimeId.EKD_PARTY_08:
				case eAnimKitCharacterAnimeId.EKD_PARTY_09:
				case eAnimKitCharacterAnimeId.EKD_PARTY_10:
				case eAnimKitCharacterAnimeId.EKD_PSN_01:
				case eAnimKitCharacterAnimeId.EKD_PSN_02:
				case eAnimKitCharacterAnimeId.EKD_PSN_03:
				case eAnimKitCharacterAnimeId.EKD_PSN_04:
				case eAnimKitCharacterAnimeId.EKD_PSN_05:
				case eAnimKitCharacterAnimeId.EKD_PSN_06:
				case eAnimKitCharacterAnimeId.EKD_PSN_07:
				case eAnimKitCharacterAnimeId.EKD_PSN_08:
				case eAnimKitCharacterAnimeId.EKD_PSN_09:
				case eAnimKitCharacterAnimeId.EKD_PSN_10:
				case eAnimKitCharacterAnimeId.EKD_RUN:
				case eAnimKitCharacterAnimeId.EKD_SMILE:
				case eAnimKitCharacterAnimeId.EKD_SURPRISE:
				case eAnimKitCharacterAnimeId.EKD_TALK_ANGER:
				case eAnimKitCharacterAnimeId.EKD_TALK_JOY:
				case eAnimKitCharacterAnimeId.EKD_TALK_JOY2:
				case eAnimKitCharacterAnimeId.EKD_TALK_NORMAL:
				case eAnimKitCharacterAnimeId.EKD_TALK_SAD:
				case eAnimKitCharacterAnimeId.EKD_TALK_SURPRISE1:
				case eAnimKitCharacterAnimeId.EKD_TALK_SURPRISE2:
					return animeName;
				default:
					return string.Empty;
			}
		}
	}
}
