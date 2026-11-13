using global::OpenPCR.UICompat;
using global::OpenPCR.Runtime.Api;
using System.Collections.Generic;

namespace Elements.Battle
{
	public class BattleEffectManager : MonoBehaviour
	{
		private BattleEffectData battleEffectData;
		private BattleEffectData abnormalEffectData;
		[global::OpenPCR.Migration("6670fc2c-b888-4910-9bc2-8ec1879d9a9e")]
		public BattleEffectManager()
		{
		}

		[global::OpenPCR.Migration("ffbb2b99-7806-4397-b44c-f4aedfa9ab42")]
		public void Init(List<UnitCtrl> _blackOutUnitList, List<UnitCtrl> _unitList, List<UnitCtrl> _otherList)
		{
			battleEffectData = new BattleEffectData(_blackOutUnitList, _unitList, _otherList);
			abnormalEffectData = new BattleEffectData(_blackOutUnitList, _unitList, _otherList);
		}

		[global::OpenPCR.Migration("ccc183f3-d96b-4ae1-a524-5d96e7f80c9c")]
		public void SetSkillEffectSortOrderBack()
		{
			battleEffectData.SetSkillEffectSortBack();
		}

		[global::OpenPCR.Migration("eef2eaa7-6eb2-49f6-abfc-8de105f0b96a")]
		public void Pause()
		{
			battleEffectData.Pause();
		}

		[global::OpenPCR.Migration("7ad40ab7-fa36-48fd-a8f1-b5e121e905b8")]
		public void Resume()
		{
			battleEffectData.Resume();
		}

		[global::OpenPCR.Migration("198afa15-e9fc-4bba-91b0-9fdc1fee60ac")]
		public void SetSortOrderBack()
		{
			battleEffectData.SetSortOrderBack();
			foreach (SkillEffectCtrl skillEffect in abnormalEffectData.SkillEffectList)
			{
				if (skillEffect is AwakeUbNoneStopEffect)
				{
					skillEffect.SetSortOrderBack();
				}
			}
		}

		[global::OpenPCR.Migration("b615fec1-29b5-4b20-adac-17d1b2a613d3")]
		public void AppendEffect(SkillEffectCtrl _effect, UnitCtrl _unit, bool _isAbnormal)
		{
			if (_unit == null)
			{
				_effect.SetSortOrderBack();
			}
			else if (_unit.ComponentSortOrder.IsFront)
			{
				_effect.SetSortOrderFront();
			}
			else
			{
				_effect.SetSortOrderBack();
			}

			if (_isAbnormal)
			{
				abnormalEffectData.SkillEffectList.Add(_effect);
				return;
			}

			if (_unit == null)
			{
				battleEffectData.SkillEffectList.Add(_effect);
				return;
			}

			if (!battleEffectData.BattleEffectDataDictionary.ContainsKey(_unit))
			{
				battleEffectData.BattleEffectDataDictionary.Add(_unit, new BattleEffectDataBase());
			}

			battleEffectData.BattleEffectDataDictionary[_unit].SkillEffectList.Add(_effect);
		}

		[global::OpenPCR.Migration("ad323a4c-6ac7-4233-9cc6-7a6284b080ef")]
		public void AppendTweener(UITweener effect, UnitCtrl unit = null, bool ignoreBlackout = false)
		{
			if (ignoreBlackout)
			{
				battleEffectData.UiTweenerListIgnoreBlackout.Add(effect);
				return;
			}

			if (unit == null)
			{
				battleEffectData.UiTweenerList.Add(effect);
				return;
			}

			if (!battleEffectData.BattleEffectDataDictionary.ContainsKey(unit))
			{
				battleEffectData.BattleEffectDataDictionary.Add(unit, new BattleEffectDataBase());
			}

			battleEffectData.BattleEffectDataDictionary[unit].UiTweenerList.Add(effect);
		}
	}
}
