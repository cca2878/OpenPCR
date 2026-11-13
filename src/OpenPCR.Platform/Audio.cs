using global::OpenPCR.AudioStubs;
using global::OpenPCR.Runtime.Api;

namespace OpenPCR.AudioStubs
{

	public unsafe class @AudioAtomSource : global::OpenPCR.AudioStubs.AudioAtomSourceBase
	{


		public @AudioAtomSource() { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }


		public override global::OpenPCR.AudioStubs.AudioAtomExPlayback @Play() { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }


		protected override global::OpenPCR.AudioStubs.AudioAtomExAcb @GetAcb() { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }


		protected override void @PlayOnStart() { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }
	}

}
namespace OpenPCR.AudioStubs
{

	public unsafe class @AudioAtomExAcb : global::OpenPCR.AudioStubs.AudioDisposable
	{


		internal @AudioAtomExAcb(nint @handle, global::System.Runtime.InteropServices.GCHandle? @dataHandle) { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }


		public override void @Dispose() { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }
	}
}
namespace OpenPCR.AudioStubs
{

	public unsafe struct @AudioAtomExPlayback
	{

		public const uint @invalidId = 4294967295u;


		public @AudioAtomExPlayback(uint @id) { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }
	}
}
namespace OpenPCR.AudioStubs
{

	public unsafe abstract class @AudioAtomSourceBase : global::OpenPCR.AudioStubs.AudioMonoBehaviour
	{


		protected @AudioAtomSourceBase() { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }


		public override void @CriInternalUpdate() { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }


		public override void @CriInternalLateUpdate() { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }

		public abstract global::OpenPCR.AudioStubs.AudioAtomExPlayback @Play();

		protected abstract global::OpenPCR.AudioStubs.AudioAtomExAcb @GetAcb();

		protected abstract void @PlayOnStart();


		public void @Stop() { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }

		public float @volume
		{


			get { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }


			set { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }
		}
	}
}
namespace OpenPCR.AudioStubs
{

	public unsafe abstract class @AudioDisposable : global::System.IDisposable
	{


		public @AudioDisposable() { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }

		public abstract void @Dispose();
	}
}
namespace OpenPCR.AudioStubs
{

	public unsafe abstract class @AudioMonoBehaviour : global::OpenPCR.Runtime.Api.MonoBehaviour
	{


		public @AudioMonoBehaviour() { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }

		public abstract void @CriInternalUpdate();

		public abstract void @CriInternalLateUpdate();
	}
}
