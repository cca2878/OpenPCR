using global::Elements;
using global::OpenPCR.Foundation;
using System;

namespace Elements.Battle.Core
{
	public class UnitComponentAnimation
	{
		private Func<BattleAnimKitController> getSpineControllerFunc;
		private Func<bool> getIsSkippingFunc;
		private Func<bool> getCanPlaySeFunc;
		private UnitComponentSound componentSound;

		[global::OpenPCR.Migration("0bb88fac-f91f-42e7-b1af-7824cf7fb5e6")]
		public UnitComponentAnimation(Func<BattleAnimKitController> _getSpineControllerFunc, Func<bool> _getIsSkippingFunc, Func<bool> _getCanPlaySeFunc)
		{
			getSpineControllerFunc = _getSpineControllerFunc;
			getIsSkippingFunc = _getIsSkippingFunc;
			getCanPlaySeFunc = _getCanPlaySeFunc;
		}

		[global::OpenPCR.Migration("646a80be-1a87-49ef-b390-2c79f94816bf")]
		public void PlayAnime(eAnimKitCharacterAnimeId _animeId, int _index1 = -1, int _index2 = -1, int _index3 = -1, bool _isLoop = true, BattleAnimKitController _targetCtr = null, bool _quiet = false, float _startTime = 0f, bool _ignoreBlackout = false)
		{
			if (_targetCtr == null) _targetCtr = global::OpenPCR.Foundation.EventExtension.Call(getSpineControllerFunc);
			_targetCtr.PlayAnime(_animeId, _index1, _index2, _index3, _isLoop, _startTime, _ignoreBlackout);
			_targetCtr.state.GetCurrent(0).lastTime = _startTime;
			_targetCtr.state.GetCurrent(0).time = _startTime;
			if (!getIsSkippingFunc()) _targetCtr.state.Apply(_targetCtr.skeleton);
			if (!_quiet) componentSound.PlaySeWithMotion(_targetCtr, getCanPlaySeFunc(), _animeId, _index1, _index2, _index3, _isLoop, _startTime);
		}

		[global::OpenPCR.Migration("3c711abc-d511-4380-ae9b-d9cedf71f0d9")]
		public void PlayAnimeNoOverlap(eAnimKitCharacterAnimeId _animeId, int _index1 = -1, int _index2 = -1, int _index3 = -1, bool _isLoop = false, BattleAnimKitController _targetCtr = null)
		{
			if (_targetCtr == null) _targetCtr = getSpineControllerFunc();
			_targetCtr.PlayAnimeNoOverlap(_animeId, _index1, _index2, _index3, _isLoop);
			componentSound.PlaySeWithMotion(_targetCtr, getCanPlaySeFunc(), _animeId, _index1, _index2, _index3, _isLoop, 0f);
		}

		[global::OpenPCR.Migration("64b485cf-8948-4a5c-a53b-e0cd94f3ecdb")]
		public void RegisterComponentSet(IUnitComponentContainer _container)
		{
			componentSound = _container.ComponentSound;
		}
	}
}
