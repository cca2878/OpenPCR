using global::OpenPCR.Runtime.Api;
namespace OpenPCR.Runtime.Api
{










	public unsafe sealed class @ParticleSystem : global::OpenPCR.Runtime.Api.Component
	{


		[global::OpenPCR.Migration("8c125e66-ef5c-46f5-b5dc-698301a735d3")]
		public @ParticleSystem() { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }



		[global::OpenPCR.Migration("828824e7-9569-4c8e-8cdb-f8419b8c79ed")]
		public void @Play(bool @withChildren) { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }


		[global::OpenPCR.Migration("63b86367-ec3e-4038-92dc-f32d0e30c7e3")]
		public void @Play() { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }



		[global::OpenPCR.Migration("dff298f0-0f97-493f-ba3f-84c9f8b61bf6")]
		public void @Pause(bool @withChildren) { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }


		[global::OpenPCR.Migration("a66bcf6f-8a5d-454d-a0ae-2e6b5acb6f3c")]
		public void @Stop(bool @withChildren) { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }



		[global::OpenPCR.Migration("97953553-ebf2-4847-96a1-a1c2c4fc8f9e")]
		public void @Clear(bool @withChildren) { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }

		[global::OpenPCR.Migration("2066b365-a708-4883-81c1-b414e6e93cd5")]
		public bool @isPaused
		{



			[global::OpenPCR.Migration("3ab7d18f-3b32-493c-a0ea-781b9fa584f9")]
			get { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }
		}

		[global::OpenPCR.Migration("5df07482-358a-45b6-b6a4-f068d914d949")]
		public float @time
		{



			[global::OpenPCR.Migration("a4408686-4b1f-4c6b-913d-6a9eee048caa")]
			get { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }



			[global::OpenPCR.Migration("d95213fa-e4dc-453c-8733-f5e5f364fc48")]
			set { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }
		}

		[global::OpenPCR.Migration("c56b6119-a270-4dba-b37e-54ca01408803")]
		public global::OpenPCR.Runtime.Api.ParticleSystem.MainModule @main
		{


			[global::OpenPCR.Migration("d37009c5-24d1-4ce6-9cb9-b804bcf42a32")]
			get { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }
		}

		[global::OpenPCR.Migration("c522bdb3-d008-4240-a3eb-16cb54be3213")]
		public global::OpenPCR.Runtime.Api.ParticleSystem.EmissionModule @emission
		{


			[global::OpenPCR.Migration("d5ba44f7-a7e9-4091-9cce-4d0e00c5ca3e")]
			get { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }
		}

		[global::System.Runtime.InteropServices.StructLayout(global::System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1)]
		public unsafe struct @MainModule
		{


			[global::OpenPCR.Migration("c4c52070-6997-47c3-a9f0-31f2c1a44933")]
			internal global::OpenPCR.Runtime.Api.ParticleSystem @m_ParticleSystem;


			[global::OpenPCR.Migration("2d4d29cf-8aa6-48c1-be4c-82f6a7e7af09")]
			internal @MainModule(global::OpenPCR.Runtime.Api.ParticleSystem @particleSystem) { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }

			[global::OpenPCR.Migration("844bb607-a560-40ac-91f8-6ac01f8035be")]
			public float @startDelayMultiplier
			{


				[global::OpenPCR.Migration("7070ada7-c2f2-4200-9d41-56fa78b797af")]
				get { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }



				[global::OpenPCR.Migration("a7030efc-7772-4f3b-9e62-4ec426dd1a5c")]
				set { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }
			}

			[global::OpenPCR.Migration("5c287e0b-5279-4bc0-b542-c93f47c008d5")]
			public bool @playOnAwake
			{



				[global::OpenPCR.Migration("3c28203e-5187-4287-a6a5-8d7c8200aafa")]
				set { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }
			}
		}

		[global::System.Runtime.InteropServices.StructLayout(global::System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1)]
		public unsafe struct @EmissionModule
		{


			[global::OpenPCR.Migration("cb550549-a7d8-4f45-92df-d2591dd13ca4")]
			internal global::OpenPCR.Runtime.Api.ParticleSystem @m_ParticleSystem;


			[global::OpenPCR.Migration("5b1aeb09-4938-4cde-8ee9-08fc53d690ae")]
			internal @EmissionModule(global::OpenPCR.Runtime.Api.ParticleSystem @particleSystem) { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }

			[global::OpenPCR.Migration("5eea225c-aee6-46e9-93a3-866b58f064ff")]
			public bool @enabled
			{



				[global::OpenPCR.Migration("ac91b1cd-2835-4ca6-8f5b-abecfed6b8aa")]
				set { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }
			}
		}
	}
}
