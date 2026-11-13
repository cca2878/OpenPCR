using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class AddAbnormalEffectTimeData
	{
		public enum eValueType
		{
			FIXED = 0,
			PERCENTAGE = 1
		}

		public global::Elements.Battle.Core.AddAbnormalEffectTimeData.eValueType ValueType { get; set; }
		public float Value { get; set; }

		[global::OpenPCR.Migration("4db60d8a-6b13-4055-b3e5-1ad5c9aefa35")]
		public AddAbnormalEffectTimeData()
		{
		}

		[global::OpenPCR.Migration("2a521cb6-cea4-499e-a2cd-6f9bca1fd62e")]
		public float GetResultTime(float _value)
		{
			if ((int)ValueType == 1)
				return (Value + 100f) * _value / 100f;
			if ((int)ValueType == 0)
				return Value + _value;
			return _value;
		}
	}
}
