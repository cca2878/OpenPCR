using global::OpenPCR.Runtime.Api;
using global::OpenPCR.Runtime.Impl;
namespace OpenPCR.Runtime.Api
{
	[global::System.Runtime.InteropServices.StructLayout(global::System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1, Size = 16)]
	public class AnimationCurve : global::System.IEquatable<global::OpenPCR.Runtime.Api.AnimationCurve>
	{
		internal global::System.IntPtr m_Ptr;
		private bool m_RequiresNativeCleanup;

		[global::OpenPCR.Migration("fda5aac8-64ed-4176-a72e-8b4fee738a62")]
		public AnimationCurve(params global::OpenPCR.Runtime.Api.Keyframe[] keys)
		{
			m_Ptr = Internal_Create(keys);
			m_RequiresNativeCleanup = true;
		}
		[global::OpenPCR.Migration("8dc48c05-00e3-4f1d-97c6-8c33e3698f30")]
		public AnimationCurve()
		{
			m_Ptr = Internal_Create(null);
			m_RequiresNativeCleanup = true;
		}
		[global::OpenPCR.Migration("57f6558a-0486-43fa-9e5f-4f2ba6c7f7ba")]
		public float Evaluate(float time)
		{
			return Evaluate_Injected(m_Ptr, time);
		}
		[global::OpenPCR.Migration("91111076-e42c-43e6-98f5-8136bdb350c2")]
		public bool Equals(global::OpenPCR.Runtime.Api.AnimationCurve other)
		{
			throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.AnimationCurve::Equals(OpenPCR.Runtime.Api.AnimationCurve):System.Boolean", "Unity infrastructure not implemented");
		}
		[global::OpenPCR.Migration("3f73500f-8abd-4ef5-a3dc-7f181fbef79c")]
		private static global::System.IntPtr Internal_Create(global::OpenPCR.Runtime.Api.Keyframe[] keys)
		{
			return global::OpenPCR.Runtime.Impl.CurveScripting.Create(
				keys == null ? global::System.ReadOnlySpan<byte>.Empty : global::System.Runtime.InteropServices.MemoryMarshal.AsBytes(global::System.MemoryExtensions.AsSpan(keys)), 2, 2);
		}

		[global::OpenPCR.Migration("3a47b7d1-99b7-4f7f-8f4a-b2e37dd15e47")]
		private static void Internal_Destroy(global::System.IntPtr ptr)
		{
			global::OpenPCR.Runtime.Impl.CurveScripting.Destroy(ptr);
		}

		[global::OpenPCR.Migration("7d44eefb-d2c5-4c40-bf68-12b0b6bb1450")]
		private static float Evaluate_Injected(global::System.IntPtr _unity_self, float time)
		{
			return global::OpenPCR.Runtime.Impl.CurveScripting.Evaluate(_unity_self, time);
		}

		[global::OpenPCR.Migration("926bf387-e622-4218-9784-b4e58679465d")]
		~AnimationCurve()
		{
			if (m_RequiresNativeCleanup) Internal_Destroy(m_Ptr);
		}

	}
}
