using global::OpenPCR.Runtime.Api;
using System;
using System.Runtime.CompilerServices;
using Elements.Battle.Core;

namespace Elements
{
	[Serializable]
	public class ShakeEffect
	{
		public ShakeType ShakeType;
		public float StartTime;
		public float Duration;
		public int TargetMotion;
		public float StartAmplitude;
		public float EndAmplitude;
		public float StartFrequency;
		public float EndFrequency;
		public float Rotation;
		public CustomEasing EasingFrequency;
		public CustomEasing EasingAmplitude;
		[NonSerialized]
		private Skill skill;
		private UnitCtrl owner;
		private float timer { get; set; }
		private int numberOfShake { get; set; }
		public float CurrentAmplutude { get; set; }
		public Vector3 CurrentShakePos { get; set; }
		public bool ShakeLoopEnd { get; set; }

		public ShakeEffect()
		{
			throw new global::OpenPCR.MigrationPendingException("Elements.ShakeEffect::.ctor()", "body migration pending");
		}

		[global::OpenPCR.Migration("3cbd7a7c-c8aa-41d4-93a3-1cf0c5e0235e")]
		public void ResetStart(Skill skill, UnitCtrl owner)
		{
			this.skill = skill;
			this.owner = owner;
			timer = 0f;
			numberOfShake = 0;
			CurrentAmplutude = StartAmplitude;
			CurrentShakePos = Vector3.zero;
			EasingFrequency = new CustomEasing(CustomEasing.eType.outQuad, StartFrequency, EndFrequency, Duration);
			EasingAmplitude = new CustomEasing(CustomEasing.eType.outQuad, StartAmplitude, EndAmplitude, Duration);
		}

		[global::OpenPCR.Migration("14ea85d8-f561-4765-9b13-36e0eb29e12a")]
		public bool GetOwnerPause()
		{
			if (owner != null)
			{
				return owner.Pause;
			}

			return false;
		}

		[global::OpenPCR.Migration("3cc289b8-ece1-4e00-bf21-826585dbf789")]
		public bool Simulate(float deltaTime)
		{
			if (ShakeLoopEnd)
			{
				return true;
			}

			if (skill != null && skill.Cancel)
			{
				ResetStart(null, null);
				return true;
			}

			if (owner != null && owner.Pause)
			{
				return false;
			}

			timer += deltaTime;
			int num = numberOfShake;
			float curVal = EasingFrequency.GetCurVal(deltaTime);
			CurrentAmplutude = EasingAmplitude.GetCurVal(deltaTime);
			numberOfShake = global::OpenPCR.NativeNumeric.ToInt32(curVal * 2f * (float)Math.PI * timer / (float)Math.PI / 2f);
			if (num != numberOfShake && ShakeType == ShakeType.RANDOM)
			{
				Rotation = global::OpenPCR.Runtime.Api.Random.Range(0f, 360f);
			}

			CurrentShakePos = new Vector3(CurrentAmplutude * Mathf.Sin(curVal * 2f * (float)Math.PI * timer) * Mathf.Cos(Rotation / 180f * (float)Math.PI), CurrentAmplutude * Mathf.Sin(curVal * 2f * (float)Math.PI * timer) * Mathf.Sin(Rotation / 180f * (float)Math.PI), 0f);
			if (timer > Duration)
			{
				ResetStart(null, null);
				return true;
			}

			return false;
		}
	}
}
