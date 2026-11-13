using global::OpenPCR.UICompat;
using global::OpenPCR.Runtime.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Elements.Battle;

namespace Elements
{
	public class AbromalStateIconController : MonoBehaviour, ISingletonField
	{
		private class SpriteAndLabel
		{
			public UISprite Sprite { get; set; }
			public eStateIconType StateIconType { get; set; }
			public CustomUILabel Label { get; set; }



			public void SetIconType(eStateIconType _type, List<EnableAndCount> _statusList)
			{
				StateIconType = _type;
				Sprite.spriteName = string.Format(ResourceDefine.STATUS_ICON, (int)_type);
				if (_type == eStateIconType.NONE)
				{
					setCount(0);
					return;
				}

				setCount(_statusList[(int)_type].Count);
				if (!_statusList[(int)_type].PassiveEnable)
				{
					Sprite.spriteName = string.Format(ResourceDefine.STATUS_ICON_DISABLE, (int)_type);
				}
			}

			private void setCount(int _count)
			{
				if (_count == 0)
				{
					Label.SetText(eTextId.NONE);
				}
				else
				{
					Label.SetTextNum(_count);
				}
			}
		}

		[StructLayout((LayoutKind)0, Pack = 1, Size = 12)]
		private struct EnableAndCount
		{
			public bool Enable;
			public int Count;
			public bool PassiveEnable;
		}
		private readonly Vector3 REVERSE_X = new Vector3(-1f, 1f, 1f);
		[SerializeField]
		private GameObject sprite;
		[SerializeField]
		private GameObject sprite2;
		private IEnumerator updateCoroutine;
		private static Yggdrasil<AbromalStateIconController> staticSingletonTree;
		private static IBattleManagerForAbromalStateIconController staticBattleManager;
		private int elementNum { get; set; }
		private SpriteAndLabel[] sprites1 { get; set; }
		private SpriteAndLabel[] sprites2 { get; set; }
		private List<EnableAndCount> statusList { get; set; }
		private int count { get; set; }
		private bool changePulse { get; set; }
		private Dictionary<eStateIconType, int> statusCountDictionary { get; set; }
		protected IBattleManagerForAbromalStateIconController battleManager => staticBattleManager;

		public AbromalStateIconController()
		{
		}



		[global::OpenPCR.Migration("47f4c57e-bf59-4005-9630-3c93d1c67eb0")]
		public static void StaticRelease()
		{
			staticSingletonTree = null;
			staticBattleManager = null;
		}

		[global::OpenPCR.Migration("64149767-e5c5-4b45-9260-cbd0aeccfcbb")]
		public void AddIcon(eStateIconType _state)
		{
			if (_state != eStateIconType.NONE)
			{
				statusCountDictionary[_state]++;
				if (statusCountDictionary[_state] == 1)
				{
					addStatus((int)_state);
				}
			}
		}

		[global::OpenPCR.Migration("9f3b46aa-2502-4eb0-875b-7b5cd79bd86b")]
		public void SetStateNum(eStateIconType _state, int _num)
		{
			EnableAndCount value = statusList[(int)_state];
			value.Count = _num;
			statusList[(int)_state] = value;
			Array.Find(sprites1, (SpriteAndLabel e) => e.StateIconType == _state)?.SetIconType(_state, statusList);
			Array.Find(sprites2, (SpriteAndLabel e) => e.StateIconType == _state)?.SetIconType(_state, statusList);
		}

		[global::OpenPCR.Migration("63f8d3c7-4242-4920-b380-c0e7f148e087")]
		public void DeleteIcon(eStateIconType _state)
		{
			statusCountDictionary[_state]--;
			statusCountDictionary[_state] = Math.Max(statusCountDictionary[_state], 0);
			if (statusCountDictionary[_state] <= 0)
			{
				deleteStatus((int)_state);
			}
		}

		[global::OpenPCR.Migration("7eec567b-c538-42ad-8c2e-e6c7f5d167a6")]
		public void SetStatePassiveEnable(eStateIconType _state, bool _enable)
		{
			EnableAndCount value = statusList[(int)_state];
			value.PassiveEnable = _enable;
			statusList[(int)_state] = value;
			Array.Find(sprites1, (SpriteAndLabel e) => e.StateIconType == _state)?.SetIconType(_state, statusList);
			Array.Find(sprites2, (SpriteAndLabel e) => e.StateIconType == _state)?.SetIconType(_state, statusList);
		}

		[global::OpenPCR.Migration("2b3de907-764a-49b4-b626-0759840a024b")]
		public void Initialize(int _colNum, bool _isCentering, bool _isReverse = false, int _rowNum = 1, bool _narrowMode = false)
		{
			global::OpenPCR.Presentation.Noop();
		}

		private void addStatus(int num)
		{
			if (!statusList[num].Enable)
			{
				EnableAndCount value = statusList[num];
				value.Enable = true;
				value.PassiveEnable = true;
				statusList[num] = value;
				count++;
				changePulse = true;
				SetActiveAllIconGameObject();
			}
		}

		private void deleteStatus(int num)
		{
			if (statusList[num].Enable)
			{
				count--;
				EnableAndCount value = statusList[num];
				value.Enable = false;
				statusList[num] = value;
				changePulse = true;
			}
		}

		public void SetActiveAllIconGameObject(bool _isActive = true)
		{
			for (int i = 0; i < elementNum; i++)
			{
				((MonoBehaviour)(object)sprites1[i].Sprite).SetActiveWithCheck(_isActive);
				((MonoBehaviour)(object)sprites2[i].Sprite).SetActiveWithCheck(_isActive);
			}
		}
	}
}
