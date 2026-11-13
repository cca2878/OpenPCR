using global::OpenPCR.Runtime.Api;
namespace OpenPCR.Runtime.Api
{

	[global::System.Reflection.DefaultMemberAttribute("Item")]

	public unsafe sealed class @Animation : global::OpenPCR.Runtime.Api.Behaviour, global::System.Collections.IEnumerable
	{


		[global::OpenPCR.Migration("f07757db-b198-4a8a-be6e-dd6f0302a18f")]
		public @Animation() { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }



		[global::OpenPCR.Migration("39c136e8-a80b-4fbf-821c-65d383dd2f55")]
		public bool @Play() { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }


		[global::OpenPCR.Migration("2cfeaab6-b8c2-4f90-9fc9-0ab21df9cbd2")]
		public global::System.Collections.IEnumerator @GetEnumerator() { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }

		[global::OpenPCR.Migration("693e9626-60b5-44a7-b305-b2a58c3dcf1d")]
		public bool @isPlaying
		{



			[global::OpenPCR.Migration("3e8b6da2-bc67-4225-9c83-697c0f7be7e5")]
			get { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }
		}
	}
}
namespace OpenPCR.Runtime.Api
{



	public unsafe sealed class @AnimationState : global::OpenPCR.Runtime.Api.TrackedReference
	{


		[global::OpenPCR.Migration("12ace3fa-cffc-4902-ae9e-77867d9ea976")]
		public @AnimationState() { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }

		[global::OpenPCR.Migration("6fe38995-30a6-42be-8d11-e922c759781c")]
		public float @speed
		{


			[global::OpenPCR.Migration("fbef20f7-d8a7-4d0a-82a8-9952f3dab44c")]
			get { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }


			[global::OpenPCR.Migration("5fedfb6a-fdae-497b-b821-0a1dbf6bef50")]
			set { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }
		}
	}
}
namespace OpenPCR.Runtime.Api
{





	public unsafe class @Animator : global::OpenPCR.Runtime.Api.Behaviour
	{


		[global::OpenPCR.Migration("6e591abf-d14b-4571-9da3-acd1394bb4a1")]
		public @Animator() { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }


		[global::OpenPCR.Migration("bd5174b7-16ff-4534-b21e-ee1ddd7fe4d9")]
		public global::OpenPCR.Runtime.Api.AnimatorStateInfo @GetCurrentAnimatorStateInfo(int @layerIndex) { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }


		[global::OpenPCR.Migration("43d42a0b-4105-4a61-b647-aed184fdd260")]
		public void @Play(string @stateName) { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }


		[global::OpenPCR.Migration("b074df2a-32aa-48ff-a947-49819b9d994e")]
		public void @Play(string @stateName, int @layer, float @normalizedTime) { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }
	}
}
namespace OpenPCR.Runtime.Api
{



	[global::System.Runtime.InteropServices.StructLayout(global::System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1, Size = 36)]
	public unsafe struct @AnimatorStateInfo
	{

		[global::OpenPCR.Migration("f8f40b60-917a-464d-b1a9-d21f699eb64e")]
		public float @normalizedTime
		{


			[global::OpenPCR.Migration("e6b4cebd-7e88-4ffd-b60d-e89e9fc875db")]
			get { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }
		}
	}
}
