using global::OpenPCR.Runtime.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Elements.Battle
{
	public class BattleTimeScale : IBattleTimeScaleForDamageEffectCtrlBase, IBattleTimeScaleForUnitUiCtrl, IBattleTimeScaleForUnitCtrl, IBattleTimeScaleForUnitActionController, IBattleTimeScaleForBattleProcessor
	{
		private IBattleManagerForBattleTimeScale battleManager;
		private float baseTimeScale = 1f;
		private Dictionary<UnitCtrl, bool> slowStopFlagDic = new Dictionary<UnitCtrl, bool>();
		private bool speedupFlag;
		private float speedUpRate = 2f;
		private bool isSpeedQuadruple;
		[global::OpenPCR.ImplicitMigration("c8299626-3caa-4181-9729-5f8e0629cebc")]
		public float BaseTimeScale => baseTimeScale;

		public bool SpeedupFlag
		{
			[global::OpenPCR.Migration("af2a3382-ff22-4bb4-86f6-5e028e837ca0")]
			get
			{
				return speedupFlag;
			}

			[global::OpenPCR.Migration("0fa49939-207b-4199-bf2c-9cc1b286594e")]
			set
			{
				speedupFlag = value;
				Time.timeScale = (speedupFlag ? speedUpRate : 1f) * BaseTimeScale;
			}
		}

		[global::OpenPCR.ImplicitMigration("3b86bdda-8c12-459d-8c6e-fa6bb4b51a87")]
		public float SpeedUpRate => speedUpRate;

		public bool IsSpeedQuadruple
		{
			[global::OpenPCR.Migration("c8958b45-b372-4cc7-8073-57a2d8610f91")]
			get
			{
				return isSpeedQuadruple;
			}

			set
			{
				isSpeedQuadruple = value;
				if (isSpeedQuadruple)
				{
					speedUpRate = 4f;
				}
				else
				{
					speedUpRate = 2f;
				}
			}
		}

		[global::OpenPCR.Migration("19e799b1-55da-43f2-8960-951c9dbed1f4")]
		public BattleTimeScale(IBattleManagerForBattleTimeScale _battleManager)
		{
			battleManager = _battleManager;
		}

		[global::OpenPCR.Migration("4b039205-f451-4498-830a-abc20eb15e89")]
		public void SetBaseTimeScale(float _timeScale)
		{
			baseTimeScale = _timeScale;
		}

		[global::OpenPCR.Migration("2e6b1d5b-5dd5-4b8e-a27f-ca72c001ad91")]
		public void StartSlowEffect(BattleDefine.SlowEffect _slowEffect, UnitCtrl _unit, float _startTime, bool _useStartCoroutine)
		{
			if (_useStartCoroutine)
			{
				battleManager.StartCoroutineIgnoreFps(updateSlowEffect(_slowEffect, _unit, _startTime));
			}
			else
			{
				battleManager.AppendCoroutine(updateSlowEffect(_slowEffect, _unit, _startTime), ePauseType.VISUAL, _unit);
			}
		}

		[global::OpenPCR.Migration("246cabf6-9e93-4c12-a2eb-a847eb7fcd70")]
		public void StopSlowEffect()
		{
			List<UnitCtrl> list = new List<UnitCtrl>(slowStopFlagDic.Keys);
			for (int i = 0; i < list.Count; i++)
			{
				slowStopFlagDic[list[i]] = true;
			}

			baseTimeScale = 1f;
			Time.timeScale = (speedupFlag ? speedUpRate : 1f) * BaseTimeScale;
		}

		[global::OpenPCR.Migration("c64ad39a-4f6b-4609-9baf-e43248716057")]
		private IEnumerator updateSlowEffect(BattleDefine.SlowEffect _slowEffect, UnitCtrl _unit, float _startTime)
		{
			float time = _startTime;
			while (true)
			{
				time += battleManager.DeltaTime;
				if (time > _slowEffect.StartDelay)
				{
					break;
				}

				yield return null;
			}

			List<UnitCtrl> list = new List<UnitCtrl>(slowStopFlagDic.Keys);
			for (int i = 0; i < list.Count; i++)
			{
				slowStopFlagDic[list[i]] = true;
			}

			if (slowStopFlagDic.ContainsKey(_unit))
			{
				slowStopFlagDic[_unit] = false;
			}
			else
			{
				slowStopFlagDic.Add(_unit, value: false);
			}

			_slowEffect.StartEasing = new CustomEasing(CustomEasing.eType.outQuad, BaseTimeScale, _slowEffect.To, _slowEffect.StartDuration);
			_slowEffect.EndEasing = new CustomEasing(CustomEasing.eType.outQuad, _slowEffect.To, 1f, _slowEffect.EndDuration);
			time = 0f;
			while (true)
			{
				if (slowStopFlagDic[_unit])
				{
					yield break;
				}

				time += battleManager.DeltaTime;
				baseTimeScale = _slowEffect.StartEasing.GetCurVal(battleManager.DeltaTime);
				Time.timeScale = (speedupFlag ? speedUpRate : 1f) * BaseTimeScale;
				if (time > _slowEffect.StartDuration)
				{
					break;
				}

				yield return null;
			}

			time = 0f;
			while (true)
			{
				if (slowStopFlagDic[_unit])
				{
					yield break;
				}

				time += battleManager.DeltaTime;
				if (time > _slowEffect.Duration)
				{
					break;
				}

				yield return null;
			}

			time = 0f;
			while (true)
			{
				if (slowStopFlagDic[_unit])
				{
					yield break;
				}

				time += battleManager.DeltaTime;
				baseTimeScale = _slowEffect.EndEasing.GetCurVal(battleManager.DeltaTime);
				Time.timeScale = (speedupFlag ? speedUpRate : 1f) * BaseTimeScale;
				if (time > _slowEffect.EndDuration)
				{
					break;
				}

				yield return null;
			}

			baseTimeScale = 1f;
		}

		[global::OpenPCR.Migration("5758d7eb-3fa1-46ce-8b5f-5bf929d6c51a")]
		public void Release()
		{
		}
	}
}
