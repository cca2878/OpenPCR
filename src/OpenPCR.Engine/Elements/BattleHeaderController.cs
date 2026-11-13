using global::OpenPCR.Runtime.Api;
using System;
using Object = global::OpenPCR.Runtime.Api.Object;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using global::OpenPCR.Foundation;
using Elements.Battle;
using Elements.Data;

namespace Elements
{
	public class BattleHeaderController : SingletonMonoBehaviour<BattleHeaderController>, ISingletonField
	{
		[global::OpenPCR.Runtime.Api.SerializeField]
		private PartsHeaderRewardTween goldHeaderReward;
		[global::OpenPCR.Runtime.Api.SerializeField]
		private PartsBossGauge partsBossGauge;
		[global::OpenPCR.Runtime.Api.SerializeField]
		private PartsBossTotalDamage partsBossTotalDamage;
		[global::OpenPCR.Runtime.Api.SerializeField]
		private PartsSkillWindow partsSkillWindow;
		public List<PartsMultiBossGauge> DoubleBossGaugeList = new List<PartsMultiBossGauge>();
		public List<PartsMultiBossGauge> TripleBossGaugeList = new List<PartsMultiBossGauge>();
		private ViewManager viewManager = ManagerSingleton<ViewManager>.Instance;
		private float nextSeTiming = 10f;
		private int preSetMin = -1;
		private int preSetSec = -1;
		private IBattleManagerForBattleHeaderController battleManager;
		private TempData tempData;
		public bool IsPaused;
		public int Gold { get; private set; }
		public BonusIconController LeftBonusIcon { get; set; }
		public BonusIconController RightBonusIcon { get; set; }

		public PartsBossGauge BossGauge
		{
			[global::OpenPCR.Migration("dbd6a617-7696-48d7-af9b-05e91c2e7034")]
			get
			{
				return partsBossGauge;
			}
		}

		public PartsBossTotalDamage BossTotalDamage
		{
			[global::OpenPCR.Migration("78699aa9-8534-46ce-b40c-52e6ac0f414a")]
			get
			{
				return partsBossTotalDamage;
			}
		}

		public PartsSkillWindow SkillWindow
		{
			[global::OpenPCR.Migration("b376d414-46a0-4974-9d99-9efd962eb0b0")]
			get
			{
				return partsSkillWindow;
			}
		}



		[global::OpenPCR.Migration("ea61da67-3c0f-4642-aab0-d6d91e3a58b1")]
		public void SetGoldNum(int _num)
		{
			Gold = _num;
			goldHeaderReward.SetNum(_num);
		}

		[global::OpenPCR.Migration("c7c16fa4-bae8-4aca-bdab-abd66a19df57")]
		public void SetSkillReserveLabel(bool _useSkillReserve)
		{
			global::OpenPCR.Presentation.Noop();
		}

		[global::OpenPCR.Migration("8983256f-ff04-40ed-b9a5-43efc081c362")]
		public void PauseNoMoreTimeUp(bool _pause)
		{
			global::OpenPCR.Presentation.Noop();
		}

		[global::OpenPCR.Migration("22bfa21e-7982-4d55-b46b-13df0b807413")]
		public void SetRestTime(float _restTime)
		{
			global::OpenPCR.Presentation.Noop();
		}

		[global::OpenPCR.Migration("8f93c310-239a-4d6b-bdd4-9e02f9bcfe37")]
		public void AppearEnvironmentIcon(bool _isEnemy, int _iconId)
		{
			global::OpenPCR.Presentation.Noop();
		}

		[global::OpenPCR.Migration("6c779e36-5908-4ea8-9ea6-babffce07e17")]
		public void DisappearEnvironmentIcon(bool _isEnemy)
		{
			global::OpenPCR.Presentation.Noop();
		}
	}
}
