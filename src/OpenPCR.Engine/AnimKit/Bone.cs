using global::OpenPCR.AnimKit;
using global::OpenPCR.Runtime.Api;
using System;
namespace OpenPCR.AnimKit
{
	public class Bone : IUpdatable
	{
		public static bool yDown;
		internal BoneData data;
		internal Skeleton skeleton;
		internal Bone parent;
		internal ExposedList<Bone> children = new ExposedList<Bone>();
		internal float x;
		internal float y;
		internal float rotation;
		internal float scaleX;
		internal float scaleY;
		internal float shearX;
		internal float shearY;
		internal float ax;
		internal float ay;
		internal float arotation;
		internal float ascaleX;
		internal float ascaleY;
		internal float ashearX;
		internal float ashearY;
		internal bool appliedValid;
		internal float a;
		internal float b;
		internal float worldX;
		internal float c;
		internal float d;
		internal float worldY;
		internal bool sorted;
		[global::OpenPCR.ImplicitMigration("2b9b0ac3-2799-4042-bfc8-29e265116187")]
		public float WorldRotationX => MathUtils.Atan2(c, a) * (180f / (float)Math.PI);

		[global::OpenPCR.ImplicitMigration("75f1319f-f035-48f8-917c-269f779f632a")]
		public float WorldY => worldY;

		[global::OpenPCR.Migration("b0f2f97b-68e0-431d-9772-84e686c88e9f")]
		public Bone(BoneData data, Skeleton skeleton, Bone parent)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data", "data cannot be null.");
			}

			if (skeleton == null)
			{
				throw new ArgumentNullException("skeleton", "skeleton cannot be null.");
			}

			this.data = data;
			this.skeleton = skeleton;
			this.parent = parent;
			SetToSetupPose();
		}

		[global::OpenPCR.Migration("a1e78fcb-5755-4036-bf35-13b7b731dfd3")]
		public void Update()
		{
			UpdateWorldTransform(x, y, rotation, scaleX, scaleY, shearX, shearY);
		}

		public void UpdateWorldTransform()
		{
			UpdateWorldTransform(x, y, rotation, scaleX, scaleY, shearX, shearY);
		}

		[global::OpenPCR.Migration("f984dc4e-2029-4488-a42c-df4124e13821")]
		public void UpdateWorldTransform(float x, float y, float rotation, float scaleX, float scaleY, float shearX, float shearY)
		{
			ax = x;
			ay = y;
			arotation = rotation;
			ascaleX = scaleX;
			ascaleY = scaleY;
			ashearX = shearX;
			ashearY = shearY;
			appliedValid = true;
			Skeleton skeleton = this.skeleton;
			Bone bone = parent;
			if (bone == null)
			{
				float degrees = rotation + 90f + shearY;
				float num = MathUtils.CosDeg(rotation + shearX) * scaleX;
				float num2 = MathUtils.CosDeg(degrees) * scaleY;
				float num3 = MathUtils.SinDeg(rotation + shearX) * scaleX;
				float num4 = MathUtils.SinDeg(degrees) * scaleY;
				if (skeleton.flipX)
				{
					x = -x;
					num = -num;
					num2 = -num2;
				}

				if (skeleton.flipY != yDown)
				{
					y = -y;
					num3 = -num3;
					num4 = -num4;
				}

				a = num;
				b = num2;
				c = num3;
				d = num4;
				worldX = x + skeleton.x;
				worldY = y + skeleton.y;
				return;
			}

			float num5 = bone.a;
			float num6 = bone.b;
			float num7 = bone.c;
			float num8 = bone.d;
			worldX = num5 * x + num6 * y + bone.worldX;
			worldY = num7 * x + num8 * y + bone.worldY;
			switch (data.transformMode)
			{
				case TransformMode.Normal:
					{
						float degrees2 = rotation + 90f + shearY;
						float num20 = MathUtils.CosDeg(rotation + shearX) * scaleX;
						float num21 = MathUtils.CosDeg(degrees2) * scaleY;
						float num22 = MathUtils.SinDeg(rotation + shearX) * scaleX;
						float num23 = MathUtils.SinDeg(degrees2) * scaleY;
						a = num5 * num20 + num6 * num22;
						b = num5 * num21 + num6 * num23;
						c = num7 * num20 + num8 * num22;
						d = num7 * num21 + num8 * num23;
						return;
					}

				case TransformMode.OnlyTranslation:
					{
						float degrees5 = rotation + 90f + shearY;
						a = MathUtils.CosDeg(rotation + shearX) * scaleX;
						b = MathUtils.CosDeg(degrees5) * scaleY;
						c = MathUtils.SinDeg(rotation + shearX) * scaleX;
						d = MathUtils.SinDeg(degrees5) * scaleY;
						break;
					}

				case TransformMode.NoRotationOrReflection:
					{
						float num24 = num5 * num5 + num7 * num7;
						float num25;
						if (num24 > 0.0001f)
						{
							num24 = Math.Abs(num5 * num8 - num6 * num7) / num24;
							num6 = num7 * num24;
							num8 = num5 * num24;
							num25 = MathUtils.Atan2(num7, num5) * (180f / (float)Math.PI);
						}
						else
						{
							num5 = 0f;
							num7 = 0f;
							num25 = 90f - MathUtils.Atan2(num8, num6) * (180f / (float)Math.PI);
						}

						float degrees3 = rotation + shearX - num25;
						float degrees4 = rotation + shearY - num25 + 90f;
						float num26 = MathUtils.CosDeg(degrees3) * scaleX;
						float num27 = MathUtils.CosDeg(degrees4) * scaleY;
						float num28 = MathUtils.SinDeg(degrees3) * scaleX;
						float num29 = MathUtils.SinDeg(degrees4) * scaleY;
						a = num5 * num26 - num6 * num28;
						b = num5 * num27 - num6 * num29;
						c = num7 * num26 + num8 * num28;
						d = num7 * num27 + num8 * num29;
						break;
					}

				case TransformMode.NoScale:
				case TransformMode.NoScaleOrReflection:
					{
						float num9 = MathUtils.CosDeg(rotation);
						float num10 = MathUtils.SinDeg(rotation);
						float num11 = num5 * num9 + num6 * num10;
						float num12 = num7 * num9 + num8 * num10;
						float num13 = Mathf.Sqrt(num11 * num11 + num12 * num12);
						if (num13 > 1E-05f)
						{
							num13 = 1f / num13;
						}

						num11 *= num13;
						num12 *= num13;
						num13 = Mathf.Sqrt(num11 * num11 + num12 * num12);
						float radians = (float)Math.PI / 2f + MathUtils.Atan2(num12, num11);
						float num14 = MathUtils.Cos(radians) * num13;
						float num15 = MathUtils.Sin(radians) * num13;
						float num16 = MathUtils.CosDeg(shearX) * scaleX;
						float num17 = MathUtils.CosDeg(90f + shearY) * scaleY;
						float num18 = MathUtils.SinDeg(shearX) * scaleX;
						float num19 = MathUtils.SinDeg(90f + shearY) * scaleY;
						if ((data.transformMode != TransformMode.NoScaleOrReflection) ? (num5 * num8 - num6 * num7 < 0f) : (skeleton.flipX != skeleton.flipY))
						{
							num14 = -num14;
							num15 = -num15;
						}

						a = num11 * num16 + num14 * num18;
						b = num11 * num17 + num14 * num19;
						c = num12 * num16 + num15 * num18;
						d = num12 * num17 + num15 * num19;
						return;
					}
			}

			if (skeleton.flipX)
			{
				a = -a;
				b = -b;
			}

			if (skeleton.flipY != yDown)
			{
				c = -c;
				d = -d;
			}
		}

		[global::OpenPCR.Migration("59df30be-f065-4bd8-b26f-b24228058922")]
		public void SetToSetupPose()
		{
			BoneData boneData = data;
			x = boneData.x;
			y = boneData.y;
			rotation = boneData.rotation;
			scaleX = boneData.scaleX;
			scaleY = boneData.scaleY;
			shearX = boneData.shearX;
			shearY = boneData.shearY;
		}

		[global::OpenPCR.Migration("f4096f0e-f761-42d4-ac8c-01b888acfdff")]
		internal void UpdateAppliedTransform()
		{
			appliedValid = true;
			Bone bone = parent;
			if (bone == null)
			{
				ax = worldX;
				ay = worldY;
				arotation = MathUtils.Atan2(c, a) * (180f / (float)Math.PI);
				ascaleX = Mathf.Sqrt(a * a + c * c);
				ascaleY = Mathf.Sqrt(b * b + d * d);
				ashearX = 0f;
				ashearY = MathUtils.Atan2(a * b + c * d, a * d - b * c) * (180f / (float)Math.PI);
				return;
			}

			float num = bone.a;
			float num2 = bone.b;
			float num3 = bone.c;
			float num4 = bone.d;
			float num5 = 1f / (num * num4 - num2 * num3);
			float num6 = worldX - bone.worldX;
			float num7 = worldY - bone.worldY;
			ax = num6 * num4 * num5 - num7 * num2 * num5;
			ay = num7 * num * num5 - num6 * num3 * num5;
			float num8 = num5 * num4;
			float num9 = num5 * num;
			float num10 = num5 * num2;
			float num11 = num5 * num3;
			float num12 = num8 * a - num10 * c;
			float num13 = num8 * b - num10 * d;
			float num14 = num9 * c - num11 * a;
			float num15 = num9 * d - num11 * b;
			ashearX = 0f;
			ascaleX = Mathf.Sqrt(num12 * num12 + num14 * num14);
			if (ascaleX > 0.0001f)
			{
				float num16 = num12 * num15 - num13 * num14;
				ascaleY = num16 / ascaleX;
				ashearY = MathUtils.Atan2(num12 * num13 + num14 * num15, num16) * (180f / (float)Math.PI);
				arotation = MathUtils.Atan2(num14, num12) * (180f / (float)Math.PI);
			}
			else
			{
				ascaleX = 0f;
				ascaleY = Mathf.Sqrt(num13 * num13 + num15 * num15);
				ashearY = 0f;
				arotation = 90f - MathUtils.Atan2(num15, num13) * (180f / (float)Math.PI);
			}
		}

		[global::OpenPCR.Migration("0acd6dc0-27b3-4409-b7b1-9d439431404c")]
		public void LocalToWorld(float localX, float localY, out float worldX, out float worldY)
		{
			worldX = localX * a + localY * b + this.worldX;
			worldY = localX * c + localY * d + this.worldY;
		}
	}
}
