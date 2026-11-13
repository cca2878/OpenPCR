using System;

namespace Elements
{
	public class CustomEasing
	{
		public enum eType
		{
			linear,
			inQuad,
			outQuad,
			inOutQuad,
			inCubic,
			outCubic,
			inOutCubic,
			inQuart,
			outQuart,
			inOutQuart,
			inSine,
			outSine,
			inOutSine,
			inExpo,
			outExpo,
			inOutExpo,
			inCirc,
			outCirc,
			inOutCirc,
			inElastic,
			outElastic,
			inOutElastic,
			inBack,
			outBack,
			inOutBack,
			inBounce,
			outBounce,
			inOutBounce
		}

		private delegate float easingFunc(float curTime);

		private float beginVal;

		private float endVal;

		private float changeVal;

		private float duration;

		private float curTime;

		private easingFunc func;

		public bool IsMoving { get; private set; }

		[global::OpenPCR.Migration("8cba0731-babe-480a-8c2f-821e91ddfdf7")]
		public CustomEasing(eType type, float beginValue, float endValue, float durationTime)
		{
			switch (type)
			{
				case eType.linear:
					func = linear;
					break;
				case eType.inQuad:
					func = inQuad;
					break;
				case eType.outQuad:
					func = outQuad;
					break;
				case eType.inOutQuad:
					func = inOutQuad;
					break;
				case eType.inCubic:
					func = inCubic;
					break;
				case eType.outCubic:
					func = outCubic;
					break;
				case eType.inOutCubic:
					func = inOutCubic;
					break;
				case eType.inQuart:
					func = inQuart;
					break;
				case eType.outQuart:
					func = outQuart;
					break;
				case eType.inOutQuart:
					func = inOutQuart;
					break;
				case eType.inSine:
					func = inSine;
					break;
				case eType.outSine:
					func = outSine;
					break;
				case eType.inOutSine:
					func = inOutSine;
					break;
				case eType.inExpo:
					func = inExpo;
					break;
				case eType.outExpo:
					func = outExpo;
					break;
				case eType.inOutExpo:
					func = inOutExpo;
					break;
				case eType.inCirc:
					func = inCirc;
					break;
				case eType.outCirc:
					func = outCirc;
					break;
				case eType.inOutCirc:
					func = inOutCirc;
					break;
				case eType.inElastic:
					func = inElastic;
					break;
				case eType.outElastic:
					func = outElastic;
					break;
				case eType.inOutElastic:
					func = inOutElastic;
					break;
				case eType.inBack:
					func = inBack;
					break;
				case eType.outBack:
					func = outBack;
					break;
				case eType.inOutBack:
					func = inOutBack;
					break;
				case eType.inBounce:
					func = inBounce;
					break;
				case eType.outBounce:
					func = outBounce;
					break;
				case eType.inOutBounce:
					func = inOutBounce;
					break;
			}
			beginVal = beginValue;
			endVal = endValue;
			duration = durationTime;
			curTime = 0f;
			changeVal = endValue - beginValue;
			IsMoving = true;
		}

		[global::OpenPCR.Migration("a8a26c2a-d101-4088-b9ab-4ce08d567cf9")]
		public float GetCurVal(float deltaTime, bool canOver = false)
		{
			curTime += deltaTime;
			if (curTime >= duration && !canOver)
			{
				IsMoving = false;
				return endVal;
			}
			return func(curTime) + beginVal;
		}

		[global::OpenPCR.Migration("94fe86b2-37c8-4e9e-8236-086309434137")]
		private float linear(float t)
		{
			return changeVal * t / duration;
		}

		[global::OpenPCR.Migration("97022314-516f-47fa-badc-7748f7773303")]
		private float inQuad(float t)
		{
			float num = t / duration;
			return changeVal * num * num;
		}

		[global::OpenPCR.Migration("d7c443c9-b296-4ed1-b107-7765b75598f3")]
		private float outQuad(float t)
		{
			float num = t / duration;
			return (-changeVal) * num * (num - 2f);
		}

		[global::OpenPCR.Migration("76631750-56ed-4c93-a2e9-dec87f31c67f")]
		private float inOutQuad(float t)
		{
			float num = t * 2f / duration;
			if (num < 1f)
			{
				return changeVal / 2f * num * num;
			}
			return (-changeVal) / 2f * ((num - 1f) * (num - 3f) - 1f);
		}

		[global::OpenPCR.Migration("799b582b-6dcd-4d70-ac09-f172c43c8f33")]
		private float inCubic(float t)
		{
			float num = t / duration;
			return changeVal * num * num * num;
		}

		[global::OpenPCR.Migration("6d7e1f0e-ce3c-41b1-a2eb-5f39f683d266")]
		private float outCubic(float t)
		{
			float num = t / duration - 1f;
			return changeVal * (num * num * num + 1f);
		}

		[global::OpenPCR.Migration("95c20a0a-03c0-4d83-aad1-8d691c6f0994")]
		private float inOutCubic(float t)
		{
			float num = t * 2f / duration;
			if (num < 1f)
			{
				return changeVal / 2f * num * num * num;
			}
			num -= 2f;
			return changeVal / 2f * (num * num * num + 2f);
		}

		[global::OpenPCR.Migration("736e5522-d5b2-47c2-9310-590a4fed0b57")]
		private float inQuart(float t)
		{
			float num = t / duration;
			return changeVal * num * num * num * num;
		}

		[global::OpenPCR.Migration("6be36548-11fe-4908-96a9-b38c6f08ea0a")]
		private float outQuart(float t)
		{
			float num = t / duration - 1f;
			return (-changeVal) * (num * num * num * num - 1f);
		}

		[global::OpenPCR.Migration("dea6b0f4-fde5-4b4d-8764-09988f811841")]
		private float inOutQuart(float t)
		{
			float num = t * 2f / duration;
			if (num < 1f)
			{
				return changeVal / 2f * num * num * num * num;
			}
			num -= 2f;
			return (-changeVal) / 2f * (num * num * num * num - 2f);
		}

		[global::OpenPCR.Migration("9b64df3e-a604-4a59-abf6-05e72d572b98")]
		private float inSine(float t)
		{
			return (-changeVal) * (float)Math.Cos((double)(t / duration) * (Math.PI / 2.0)) + changeVal;
		}

		[global::OpenPCR.Migration("929c7850-9c24-4646-bc7c-065b0fd7e774")]
		private float outSine(float t)
		{
			return changeVal * (float)Math.Sin((double)(t / duration) * (Math.PI / 2.0));
		}

		[global::OpenPCR.Migration("81bbaebf-e24d-4dd8-897b-edb1d24db142")]
		private float inOutSine(float t)
		{
			return (-changeVal) / 2f * ((float)Math.Cos((double)(t / duration) * Math.PI) - 1f);
		}

		[global::OpenPCR.Migration("1cedd933-fcdf-48c7-82e6-38cc048f76be")]
		private float inExpo(float t)
		{
			if (t == 0f)
			{
				return 0f;
			}
			return changeVal * (float)Math.Pow(2.0, 10f * (t / duration - 1f));
		}

		[global::OpenPCR.Migration("b384eda9-32aa-4aba-9afd-6376101e7f7a")]
		private float outExpo(float t)
		{
			return changeVal * (-(float)Math.Pow(2.0, -10f * t / duration) + 1f);
		}

		[global::OpenPCR.Migration("170aa8d9-7b71-4912-a355-49943e56ba0b")]
		private float inOutExpo(float t)
		{
			if (t == 0f)
			{
				return 0f;
			}
			float num = t * 2f / duration;
			if (num < 1f)
			{
				return changeVal / 2f * (float)Math.Pow(2.0, 10f * (num - 1f));
			}
			return changeVal / 2f * (-(float)Math.Pow(2.0, -10f * (num - 1f)) + 2f);
		}

		[global::OpenPCR.Migration("dd58460b-cbbd-4163-83d2-ef2aa7a95c17")]
		private float inCirc(float t)
		{
			float num = t / duration;
			return (-changeVal) * ((float)Math.Sqrt(1f - num * num) - 1f);
		}

		[global::OpenPCR.Migration("a723b358-bd77-4adb-86ed-caa535b4761f")]
		private float outCirc(float t)
		{
			float num = t / duration - 1f;
			return changeVal * (float)Math.Sqrt(1f - num * num);
		}

		[global::OpenPCR.Migration("109dd96e-2802-45c3-b60d-b71c7887b259")]
		private float inOutCirc(float t)
		{
			float num = t * 2f / duration;
			if (num < 1f)
			{
				return (-changeVal) / 2f * ((float)Math.Sqrt(1f - num * num) - 1f);
			}
			num -= 2f;
			return changeVal / 2f * ((float)Math.Sqrt(1f - num * num) + 1f);
		}

		[global::OpenPCR.Migration("df692c4c-f01b-4694-be36-a32906f2d3c3")]
		private float inElastic(float t)
		{
			float num = t / duration;
			float num2 = 1.70158f;
			float num3 = changeVal;
			if (num == 0f)
			{
				return 0f;
			}
			float num4 = duration * 0.3f;
			num2 = ((!(num3 < Math.Abs(changeVal))) ? (num4 / ((float)Math.PI * 2f) * (float)Math.Asin(changeVal / num3)) : (num4 / 4f));
			num--;
			return -num3 * (float)Math.Pow(2.0, 10f * num) * (float)Math.Sin((num * duration - num2) * ((float)Math.PI * 2f) / num4);
		}

		[global::OpenPCR.Migration("8bcfa2f5-2ad4-4723-800c-9b50b31dbaec")]
		private float outElastic(float t)
		{
			float num = t / duration;
			float num2 = 1.70158f;
			float num3 = changeVal;
			if (num == 0f)
			{
				return 0f;
			}
			float num4 = duration * 0.3f;
			num2 = ((!(num3 < Math.Abs(changeVal))) ? (num4 / ((float)Math.PI * 2f) * (float)Math.Asin(changeVal / num3)) : (num4 / 4f));
			return num3 * (float)Math.Pow(2.0, -10f * num) * (float)Math.Sin((num * duration - num2) * ((float)Math.PI * 2f) / num4) + changeVal;
		}

		[global::OpenPCR.Migration("0a532b68-1ccf-4b3e-9010-8a648f6cb8c5")]
		private float inOutElastic(float t)
		{
			float num = t * 2f / duration;
			float num2 = 1.70158f;
			float num3 = changeVal;
			if (num == 0f)
			{
				return 0f;
			}
			float num4 = duration * 0.45f;
			num2 = ((!(num3 < Math.Abs(changeVal))) ? (num4 / ((float)Math.PI * 2f) * (float)Math.Asin(changeVal / num3)) : (num4 / 4f));
			if (num < 1f)
			{
				num--;
				return -0.5f * (num3 * (float)Math.Pow(2.0, 10f * num) * (float)Math.Sin((num * duration - num2) * ((float)Math.PI * 2f) / num4));
			}
			num--;
			return num3 * (float)Math.Pow(2.0, -10f * num) * (float)Math.Sin((num * duration - num2) * ((float)Math.PI * 2f / num4)) * 0.5f + changeVal;
		}

		[global::OpenPCR.Migration("b1c4d2e8-8d8f-4e77-8504-313e440975c4")]
		private float inBack(float t)
		{
			float num = t / duration;
			float num2 = 1.70158f;
			return changeVal * num * num * ((num2 + 1f) * num - num2);
		}

		[global::OpenPCR.Migration("6b790f65-ee05-45a0-a9f9-682bda32af91")]
		private float outBack(float t)
		{
			float num = t / duration - 1f;
			float num2 = 1.70158f;
			return changeVal * (num * num * ((num2 + 1f) * num + num2) + 1f);
		}

		[global::OpenPCR.Migration("46bcc031-b142-458f-9b21-a935b2802f90")]
		private float inOutBack(float t)
		{
			float num = t * 2f / duration;
			float num2 = 2.5949094f;
			if (num < 1f)
			{
				return changeVal / 2f * num * num * ((num2 + 1f) * num - num2);
			}
			num -= 2f;
			return changeVal / 2f * (num * num * ((num2 + 1f) * num + num2) + 2f);
		}

		[global::OpenPCR.Migration("dc3bb3f7-1b70-47c4-996a-c1b728cdcc24")]
		private float inBounce(float t)
		{
			return changeVal - outBounce(duration - t);
		}

		[global::OpenPCR.Migration("9fc38ee9-5480-4376-a5cf-6bf750edbcbe")]
		private float outBounce(float t)
		{
			float num = t / duration;
			if (num < 0.36363637f)
			{
				return changeVal * (7.5625f * num * num);
			}
			if (num < 0.72727275f)
			{
				num -= 0.54545456f;
				return changeVal * (7.5625f * num * num + 0.75f);
			}
			if (num < 0.90909094f)
			{
				num -= 0.8181818f;
				return changeVal * (7.5625f * num * num + 0.9375f);
			}
			num -= 21f / 22f;
			return changeVal * (7.5625f * num * num + 63f / 64f);
		}

		[global::OpenPCR.Migration("05e1b4a5-7e80-4509-a43e-e7c64873d67e")]
		private float inOutBounce(float t)
		{
			if (t * 2f < duration)
			{
				return inBounce(t * 2f) * 0.5f;
			}
			return 0.5f * outBounce(t * 2f - duration) + changeVal * 0.5f;
		}
	}
}
