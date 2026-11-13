using System;

namespace Elements
{
	[Serializable]
	public class AutoSpeedSetting
	{
		public SystemSettingDefine.eTypeButtonState ButtonState;
		public bool IsSpeedUp;
		public bool IsAuto;
		public bool IsAllSet;
		public AutoSpeedSetting()
		{
			ButtonState = (SystemSettingDefine.eTypeButtonState)0;
		}
	}
}
