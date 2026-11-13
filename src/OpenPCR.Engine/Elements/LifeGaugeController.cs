using global::OpenPCR.Runtime.Api;
using System;
using System.Collections;
using global::OpenPCR.Foundation;
using Elements.Battle;

namespace Elements
{
	public class LifeGaugeController : MonoBehaviour, ISingletonField
	{

		private static Yggdrasil<LifeGaugeController> staticSingletonTree = null;

		private static IBattleManagerForLifeGaugeController staticBattleManager = null;

		private static readonly string RED_SPRITE_NAME = "battle_gauge_bar_enemy_2";

		private static readonly Vector3 SKILL_NAME_BALLOON_SCALE = new Vector3(1.25f, 1.2f, 1f);

		private static readonly Vector3 SKILL_NAME_BALLOON_POSITION = new Vector3(0f, 105f);

		private static readonly Vector3 STATE_ICON_SCALE = new Vector3(0.85f, 0.85f, 1f);

		private static readonly Vector3 STATE_ICON_POSITION = new Vector3(0f, 180f);

		private IBattleManagerForLifeGaugeController battleManager => staticBattleManager;

		private UnitCtrl owner { get; set; }



		[global::OpenPCR.Migration("87b504e0-8af7-4aaf-95f8-c21467f2d188")]
		public static void StaticRelease()
		{
			staticSingletonTree = null;
			staticBattleManager = null;
		}

		[global::OpenPCR.Migration("53f7d453-2b34-4950-bf33-62e651bbd3e5")]
		public void Initialize(bool _isOther, float _height, UnitCtrl _unit, bool _isGaugeVisibleAlways = false)
		{
			global::OpenPCR.Presentation.Noop();
		}

		[global::OpenPCR.Migration("8062c53e-fc15-4b59-8d1e-7ca6ffe136e6")]
		public void SetSortOrderFront()
		{
			global::OpenPCR.Presentation.Noop();
		}

		[global::OpenPCR.Migration("1eaa8d00-aa10-4021-a9b9-c659a81260b4")]
		public void SetSortOrderBack()
		{
			global::OpenPCR.Presentation.Noop();
		}

		[global::OpenPCR.Migration("69ab61d7-c32e-4337-be0a-42885a566b8c")]
		public void SetNormalizedLifeAmount(float _value, float _maxValue, bool _isInitialize = false, bool _isTowerTimeUp = false)
		{
			global::OpenPCR.Presentation.Noop();
		}

		[global::OpenPCR.Migration("9155de84-f370-4e64-b841-2c18e91b10bf")]
		public void IndicateSkillName(string _skillName)
		{
			global::OpenPCR.Presentation.Noop();
		}

		[global::OpenPCR.Migration("c166a6d1-946d-42b0-b877-957e690bc52c")]
		public void AddIcon(eStateIconType _state)
		{
			global::OpenPCR.Presentation.Noop();
		}

		[global::OpenPCR.Migration("d9aa965d-4dc3-4109-a116-6b0e66da7f7b")]
		public void SetIconNum(eStateIconType _state, int _num)
		{
			global::OpenPCR.Presentation.Noop();
		}

		[global::OpenPCR.Migration("99f22e34-ce49-4adf-b769-53d31bcc0dbe")]
		public void SetIconPassiveEnable(eStateIconType _state, bool _enable)
		{
			global::OpenPCR.Presentation.Noop();
		}

		[global::OpenPCR.Migration("cd9710de-9244-4aeb-8128-532b4889c3b4")]
		public void DeleteIcon(eStateIconType _state)
		{
			global::OpenPCR.Presentation.Noop();
		}

	}
}
