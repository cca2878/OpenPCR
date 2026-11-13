using global::OpenPCR.Runtime.Api;
using global::OpenPCR.Runtime.Impl;
namespace OpenPCR.Runtime.Api
{
	[global::System.Runtime.InteropServices.StructLayout(global::System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1, Size = 8)]
	public class Object
	{
		// The engine scene owner writes this target field; managed construction leaves it zero.
#pragma warning disable CS0649
		private global::System.IntPtr m_CachedPtr;
#pragma warning restore CS0649

		[global::OpenPCR.Migration("9fa65130-6651-4bf9-b31c-222b11226086")]
		public Object()
		{

		}
		[global::OpenPCR.Migration("6cfa82eb-6e32-4bb1-81fa-c3609bf90ddf")]
		public static implicit operator bool(global::OpenPCR.Runtime.Api.Object exists)
		{
			return !CompareBaseObjects(exists, null);
		}
		[global::OpenPCR.Migration("5c67211a-624f-4a08-8c8e-7336807fb33b")]
		public static T Instantiate<T>(T original) where T : global::OpenPCR.Runtime.Api.Object
		{
			return (T)global::OpenPCR.Runtime.Impl.PreparedSceneOperations.Clone(original);
		}
		[global::OpenPCR.Migration("00984474-03bb-47e8-870f-400f7aee1769")]
		public static T Instantiate<T>(T original, global::OpenPCR.Runtime.Api.Transform parent) where T : global::OpenPCR.Runtime.Api.Object
		{
			throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Object::Instantiate`1(!!0,OpenPCR.Runtime.Api.Transform):!!0", "Unity infrastructure not implemented");
		}
		[global::OpenPCR.Migration("064a9229-d5e3-4412-827e-89f917269f56")]
		public static void Destroy(global::OpenPCR.Runtime.Api.Object obj)
		{
			global::OpenPCR.Runtime.Impl.SceneScripting.RequestDestroy(obj);
		}
		[global::OpenPCR.Migration("a360555d-0b8e-4f65-bf0d-956189c86963")]
		public static void DestroyImmediate(global::OpenPCR.Runtime.Api.Object obj, bool allowDestroyingAssets)
		{
			throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Object::DestroyImmediate(OpenPCR.Runtime.Api.Object,System.Boolean):System.Void", "Unity infrastructure not implemented");
		}
		[global::OpenPCR.Migration("800635ce-fc28-42d3-a0ca-0c967f995c42")]
		public static void DestroyImmediate(global::OpenPCR.Runtime.Api.Object obj)
		{
			throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Object::DestroyImmediate(OpenPCR.Runtime.Api.Object):System.Void", "Unity infrastructure not implemented");
		}
		[global::OpenPCR.Migration("5b090082-e686-4f29-be59-df553ca9fa84")]
		public static bool operator ==(global::OpenPCR.Runtime.Api.Object x, global::OpenPCR.Runtime.Api.Object y)
		{
			return CompareBaseObjects(x, y);
		}
		[global::OpenPCR.Migration("180a1129-3f00-4b1b-8fab-1929b8e793f6")]
		public static bool operator !=(global::OpenPCR.Runtime.Api.Object x, global::OpenPCR.Runtime.Api.Object y)
		{
			return !CompareBaseObjects(x, y);
		}
		public string name
		{
			[global::OpenPCR.Migration("5ad75cb4-6941-4c21-9c14-81076e086085")]
			get
			{
				return global::OpenPCR.Runtime.Impl.PreparedSceneOperations.Name(this);
			}
			[global::OpenPCR.Migration("a45b4efa-8d47-4d0e-85c6-ae2ebb323f38")]
			set
			{
				global::OpenPCR.Presentation.Noop();
			}
		}

		[global::OpenPCR.Migration("a38e4636-a629-4895-8cb0-dff13f35f118")]
		public override bool Equals(object other)
		{
			Object otherAsObject = other as Object;
			if (otherAsObject == null && other != null && !(other is Object))
				return false;
			return CompareBaseObjects(this, otherAsObject);
		}

		[global::OpenPCR.Migration("a142f11f-2299-43d3-9cab-2d30b0b72939")]
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		[global::OpenPCR.Migration("87008994-ad73-4daa-8561-563ce0e2f5dc")]
		private static bool CompareBaseObjects(Object lhs, Object rhs)
		{
			bool lhsNull = (object)lhs == null;
			bool rhsNull = (object)rhs == null;
			if (lhsNull && rhsNull) return true;
			if (rhsNull) return !IsNativeObjectAlive(lhs);
			if (lhsNull) return !IsNativeObjectAlive(rhs);
			return (object)lhs == (object)rhs;
		}

		[global::OpenPCR.Migration("ddb6c6d7-3e83-4dcd-8574-8290d8896c69")]
		private static bool IsNativeObjectAlive(Object o)
		{
			return o.GetCachedPtr() != global::System.IntPtr.Zero;
		}

		[global::OpenPCR.Migration("810cf012-b85b-4921-8a1d-1b466fe7b802")]
		private global::System.IntPtr GetCachedPtr()
		{
			return m_CachedPtr;
		}

	}
}
