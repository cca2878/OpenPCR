namespace Elements.Utility
{
	public class SkillActionValue
	{
		public float Value1 { get; private set; }

		public float Value2 { get; private set; }

		public float Value3 { get; private set; }

		public float Value4 { get; private set; }

		public float Value5 { get; private set; }

		public float Value6 { get; private set; }

		public float Value7 { get; private set; }

		[global::OpenPCR.Migration("8a919049-4ff6-4a1f-843f-4aaaad731bbc")]
		public void SetValue1(float _value1)
		{
			Value1 = _value1;
		}

		[global::OpenPCR.Migration("ed472fde-4691-462c-b8b0-532878fb308c")]
		public void SetValue2(float _value2)
		{
			Value2 = _value2;
		}

		[global::OpenPCR.Migration("5c907316-5f62-413a-8345-8e21263925a8")]
		public void SetValue3(float _value3)
		{
			Value3 = _value3;
		}

		[global::OpenPCR.Migration("321280db-2d8a-4e5e-9d49-da99ae827bd1")]
		public void SetValue4(float _value4)
		{
			Value4 = _value4;
		}

		[global::OpenPCR.Migration("134d04de-6294-4b55-a295-46e4ee4297fe")]
		public void SetValue5(float _value5)
		{
			Value5 = _value5;
		}

		[global::OpenPCR.Migration("4e1103bb-d003-4829-b820-8bd4a5ac0ed2")]
		public void SetValue6(float _value6)
		{
			Value6 = _value6;
		}

		[global::OpenPCR.Migration("affd7183-2413-4b6a-b217-65b42d887cdb")]
		public void SetValue7(float _value7)
		{
			Value7 = _value7;
		}

		[global::OpenPCR.Migration("5fd23906-086e-4367-9d78-acb7620b87a1")]
		private void initializeSkillActionValue()
		{
			Value1 = 0f;
			Value2 = 0f;
			Value3 = 0f;
			Value4 = 0f;
			Value5 = 0f;
			Value6 = 0f;
			Value7 = 0f;
		}

		[global::OpenPCR.Migration("6fd056f1-ff79-47c5-b912-c3bd6cfc0138")]
		public SkillActionValue()
		{
			initializeSkillActionValue();
		}
	}
}
