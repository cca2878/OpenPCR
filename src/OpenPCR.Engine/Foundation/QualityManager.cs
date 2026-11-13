using global::OpenPCR.Foundation;
using global::OpenPCR.Runtime.Api;
using System;
using Common;

namespace OpenPCR.Foundation
{
	public class QualityManager : MonoBehaviour, IManager
	{
		public enum GPUQualityLevel
		{
			Level_1,
			Level_2,
			Level_3,
			Level_4
		}

		public enum IOQualityLevel
		{
			None = 0,
			Level_1 = 1,
			Level_2 = 2
		}

		private static IOQualityLevel _ioQualityLevel;
		private static int _maxDownloadFrameRate;
		private static int _maxLoadFrameRate;
		private static bool _isVoiceDataAvailable;
		private static bool _isFirstInit;
		private static Vector2 _bufferDeviceResolution;
		static QualityManager()
		{
			_ioQualityLevel = IOQualityLevel.None;
			_maxDownloadFrameRate = 60;
			_maxLoadFrameRate = 60;
			_isVoiceDataAvailable = true;
			_isFirstInit = true;
			_bufferDeviceResolution = Vector2.zero;
		}

		[global::OpenPCR.Migration("e66d6e76-2078-427f-9080-c3df6d619c32")]
		public static bool GetVoiceDataAvailable() => _isVoiceDataAvailable;
		private GPUQualityLevel _gpuQualityLevel;
		private Vector2 _deviceResolution = Vector2.zero;
		private Vector2 _gameResolution = Vector2.zero;
		private int _defaultFrameRate = 60;
		public bool isFullScreen = true;
		private float _LowScreenRatio = 1f;
		private readonly float _lowMemoryAmount = 512f;



		[global::OpenPCR.Migration("6e0a6ebf-9e06-4801-8478-b271a3383279")]
		public static GPUQualityLevel GetGPUQualityLevel()
		{
			if (Toolbox.QualityManager == null)
				return GPUQualityLevel.Level_1;
			return Toolbox.QualityManager._gpuQualityLevel;
		}
	}
}
