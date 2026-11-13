using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class ChangeResistIdAction : ActionParameter
	{
		[global::OpenPCR.Migration("e04fce47-7142-481c-b301-ea6015ddf092")]
		public ChangeResistIdAction() : base()
		{
		}

		[global::OpenPCR.Migration("0447ba66-62a5-458c-9c1e-bd0e8042eeb8")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			setResistId(_target, base.ActionDetail2);
			_sourceActionController.AppendCoroutine(updateResistId(_target, getResistSetCount(_target)), ePauseType.SYSTEM);
		}

		[global::OpenPCR.Migration("eb07145f-f6e8-4017-8850-e4a7ce7fd9ff")]
		private int getResistSetCount(BasePartsData _target)
		{
			int nativeValue1 = (this).ActionDetail1;
			if ((unchecked((uint)(nativeValue1)) == 2u))
			{
				if (global::System.Object.ReferenceEquals(@_target, null))
					throw new global::System.NullReferenceException();
				int nativeValue2 = (@_target).DebuffResistIdSetCount;
				return unchecked((int)(unchecked((uint)(nativeValue2))));
			}
			else
			{
				if (!((unchecked((uint)(nativeValue1)) == 1u)))
				{
					return unchecked((int)(0u));
				}
				else
				{
					if (global::System.Object.ReferenceEquals(@_target, null))
						throw new global::System.NullReferenceException();
					int nativeValue3 = (@_target).AbnormalResistIdSetCount;
					return unchecked((int)(unchecked((uint)(nativeValue3))));
				}
			}
		}

		[global::OpenPCR.Migration("a4920cbd-6f7e-4853-bb3a-34735e85bd16")]
		private IEnumerator updateResistId(BasePartsData _target, int _resistSetCount)
		{
			float passedTime = 0f;
			float effectTime = (float)Value[eValueNumber.VALUE_1];
			while (true)
			{
				yield return null;
				passedTime += _target.Owner.DeltaTimeForPause;
				if (_resistSetCount != getResistSetCount(_target))
					yield break;
				if (BattleUtil.LessThanOrApproximately(effectTime, passedTime))
				{
					setResistId(_target, getBattleStartResistId(_target));
					yield break;
				}
			}
		}

		[global::OpenPCR.Migration("3fd25633-0bce-4356-9225-0d9d099073cd")]
		private int getBattleStartResistId(BasePartsData _target)
		{
			int nativeValue1 = (this).ActionDetail1;
			if ((unchecked((uint)(nativeValue1)) == 2u))
			{
				if (global::System.Object.ReferenceEquals(@_target, null))
					throw new global::System.NullReferenceException();
				int nativeValue2 = (@_target).StartDebuffResistId;
				return unchecked((int)(unchecked((uint)(nativeValue2))));
			}
			else
			{
				if (!((unchecked((uint)(nativeValue1)) == 1u)))
				{
					return unchecked((int)(0u));
				}
				else
				{
					if (global::System.Object.ReferenceEquals(@_target, null))
						throw new global::System.NullReferenceException();
					int nativeValue3 = (@_target).StartAbnormalResistId;
					return unchecked((int)(unchecked((uint)(nativeValue3))));
				}
			}
		}

		[global::OpenPCR.Migration("1b34ac04-9ad2-4b18-8c03-772df5598fa8")]
		private void setResistId(BasePartsData _target, int _resistId)
		{
			int nativeValue1 = (this).ActionDetail1;
			if ((unchecked((uint)(nativeValue1)) == 2u))
			{
				if (global::System.Object.ReferenceEquals(@_target, null))
					throw new global::System.NullReferenceException();
				(@_target).SetDebuffResistId(unchecked((int)(unchecked((uint)(@_resistId)))), false);
				return;
			}
			else
			{
				if (!((unchecked((uint)(nativeValue1)) == 1u)))
				{
					return;
				}
				else
				{
					if (global::System.Object.ReferenceEquals(@_target, null))
						throw new global::System.NullReferenceException();
					(@_target).SetAbnormalResistId(unchecked((int)(unchecked((uint)(@_resistId)))), false);
					return;
				}
			}
		}
	}
}
