using global::OpenPCR.Runtime.Api;
using System;
using System.Collections.Generic;
using Elements.Battle;
using global::OpenPCR.Foundation;

namespace Elements.Battle.Core
{
	public class BattleComponentTalent
	{
		private enum eBonusEffectState
		{
			STANDBY = 0,
			PLAYING = 1,
			FINISHED = 2
		}

		private eBonusEffectState talentBonusState;

		[global::OpenPCR.Migration("52ddd528-0857-47b6-9021-71a75247c350")]
		public bool IsPlayTalentBonusEffect
		{
			[global::OpenPCR.Migration("96cf2115-07f3-4ea0-98be-074d3dec3522")]
			get { return talentBonusState == eBonusEffectState.STANDBY; }
			[global::OpenPCR.Migration("28ab3e82-264e-43ac-bd56-47079b28b564")]
			set { talentBonusState = value ? eBonusEffectState.STANDBY : eBonusEffectState.FINISHED; }
		}

		[global::OpenPCR.Migration("6d09c573-87d2-4e6d-8457-80ebb8b65f0c")]
		public bool IsTalentBonusEffectPlaying
		{
			[global::OpenPCR.Migration("2159dc52-85c4-4f4d-b152-df817411440d")]
			get { return talentBonusState == eBonusEffectState.PLAYING; }
		}

		[global::OpenPCR.Migration("6cf1fe1f-5b67-467b-bcc5-0589c97fbb87")]
		public BattleComponentTalent()
		{
			talentBonusState = eBonusEffectState.STANDBY;
		}

		[global::OpenPCR.Migration("603d6be7-76f7-4fa3-b8e4-4fda43f99f49")]
		public void PlayTalentBonusEffect(List<int> _bonusTalentIdList, Transform _effectParent, Transform _controllerParent, Action _showUiCallback, Action _finishCallback)
		{
			TalentBonusAppearEffect effect = ManagerSingleton<ResourceManager>.Instance.InstantiateAndGetComponent<TalentBonusAppearEffect>(eResourceId.ANIM_BATTLE_TALENT_BONUS_APPEAR, _effectParent, 0L);
			effect.transform.position = _controllerParent.position;
			talentBonusState = eBonusEffectState.PLAYING;
			ManagerSingleton<SaveDataManager>.Instance.TalentFormationBonusFirst = true;
			effect.Play(_bonusTalentIdList, _showUiCallback, delegate
			{
				effect.SetActive(false);
				talentBonusState = eBonusEffectState.FINISHED;
				_finishCallback.Call();
			});
		}
	}
}
