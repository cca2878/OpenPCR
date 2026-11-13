namespace OpenPCR.Runtime.Api
{
	[global::System.Runtime.InteropServices.StructLayout(global::System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1, Size = 28)]
	public struct Keyframe
	{
		private float m_Time;
		private float m_Value;
		private float m_InTangent;
		private float m_OutTangent;
		private int m_WeightedMode;
		private float m_InWeight;
		private float m_OutWeight;

		// Tangent mode is editor-only and has no runtime field.
		[global::OpenPCR.Migration("3f17d635-cd85-4115-a80d-a991654ed65f")]
		public Keyframe(float time, float value, float inTangent, float outTangent)
		{
			m_Time = time;
			m_Value = value;
			m_InTangent = inTangent;
			m_OutTangent = outTangent;
			m_WeightedMode = 0;
			m_InWeight = 0f;
			m_OutWeight = 0f;
		}
	}
}
