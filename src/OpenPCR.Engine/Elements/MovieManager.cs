using global::OpenPCR.Foundation.Audio.Movie;
using global::OpenPCR.Runtime.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using global::OpenPCR.Foundation.Audio;

namespace Elements
{
	public class MovieManager : ManagerSingleton<MovieManager>
	{

		[global::OpenPCR.Migration("96c003b2-5cdc-41ff-9176-4989868db6e1")]
		public void SetCollider(global::Elements.eMovieType _movieType, long _movieId, bool _isEnable, long _movieIndex = 0L)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.MovieManager::SetCollider(Elements.eMovieType,System.Int64,System.Boolean,System.Int64):System.Void: periphery timing member; behavior is supplied by the wiring phase");
		}

		[global::OpenPCR.Migration("4b23d5c6-2453-45f0-a567-0fc5ea0ee069")]
		public void SetSortOrder(global::Elements.eMovieType _movieType, long _movieId, int _sortOrder, long _movieIndex = 0L)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.MovieManager::SetSortOrder(Elements.eMovieType,System.Int64,System.Int32,System.Int64):System.Void: periphery timing member; behavior is supplied by the wiring phase");
		}


		[global::OpenPCR.Migration("ef9fe6d2-2182-4f30-aa50-2983df7329fb")]
		public void SetPlayEndTypeDestroy(global::Elements.eMovieType _movieType, long _movieId, long _movieIndex)
		{
			global::OpenPCR.PreparedPrincessMoviePlayback.SetEndTypeDestroy(this, (int)_movieType, _movieId, _movieIndex);
		}

		[global::OpenPCR.Migration("f93c2834-2b83-48cf-b7a6-953b85f25238")]
		public void SetMoviePlayerLocalPosition(global::Elements.eMovieType _movieType, long _movieId, global::OpenPCR.Runtime.Api.Vector3 _pos, long _movieIndex = 0L)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.MovieManager::SetMoviePlayerLocalPosition(Elements.eMovieType,System.Int64,OpenPCR.Runtime.Api.Vector3,System.Int64):System.Void: runtime capability is unavailable.");
		}


		[global::OpenPCR.Migration("1006f6ec-0be0-4ef7-be74-7988b0374874")]
		public bool GetStopIsNotComplete(global::Elements.eMovieType _movieType, long _movieId, long _movieIndex)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.MovieManager::GetStopIsNotComplete(Elements.eMovieType,System.Int64,System.Int64):System.Boolean: periphery timing member; behavior is supplied by the wiring phase");
		}
		public MovieManager()
		{
			throw new global::OpenPCR.MigrationPendingException("Elements.MovieManager::.ctor()", "body migration pending");
		}

		[global::OpenPCR.Migration("a4733b3d-7ef8-4661-9177-7f639613d894")]
		public void Load(eMovieType _movieType, long _movieId, bool _isShowSubtitle = false, Action _finishCallback = null, bool _isWaitPrepare = true, long _movieIndex = 0L, bool _isAndroidOnMemory = false, bool _fadeLoop = false, bool _isSoundOff = false, float _movieStartTime = 0f, float _bgmVolumeRate = 1f, int _playIndex = 0)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.MovieManager::Load(Elements.eMovieType, long, bool, System.Action, bool, long, bool, bool, bool, float, float, int)");
		}

		[global::OpenPCR.Migration("de7dd8f9-832e-4432-a980-f5157ddc5890")]
		public void Play(eMovieType _movieType, long _movieId, eMoviePlayType _play, Action _finishCallback = null, PlayEndType _playEndType = PlayEndType.Destroy, float _fadeStartTime = 0f, float _fadeDurationTime = 0f, long _movieIndex = 0L)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.MovieManager::Play(Elements.eMovieType,System.Int64,Elements.eMoviePlayType,System.Action,OpenPCR.Foundation.Audio.Movie.PlayEndType,System.Single,System.Single,System.Int64):System.Void: runtime capability is unavailable.");
		}

		[global::OpenPCR.Migration("673efe81-4b04-4efa-90d1-94bb5042bc92")]
		public void SetPlaySpeed(eMovieType _movieType, long _movieId, float _speed, long _movieIndex = 0L)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.MovieManager::SetPlaySpeed(Elements.eMovieType, long, float, long)");
		}

		[global::OpenPCR.Migration("842a25f7-9298-4be0-b8ca-20ae04e60074")]
		public void SetScreenScale(eMovieType _movieType, long _movieId, Vector2 _scale, bool _isPanScan = false, long _movieIndex = 0L)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.MovieManager::SetScreenScale(Elements.eMovieType,System.Int64,OpenPCR.Runtime.Api.Vector2,System.Boolean,System.Int64):System.Void: runtime capability is unavailable.");
		}

		[global::OpenPCR.Migration("c606cfc1-77ca-4be5-abf6-dde5164c90ed")]
		public void SetMirrorMode(bool _enable)
		{
			global::OpenPCR.Presentation.Noop();
		}

		[global::OpenPCR.Migration("1811dea8-1c92-4b2e-9511-f09579fe01dd")]
		public float GetSeekPositionTime(eMovieType _movieType, long _movieId, long _movieIndex = 0L, bool _nothingTypeIndexNoError = false)
		{
			if (_nothingTypeIndexNoError) throw new global::OpenPCR.OutOfBoundaryException("Offline princess queries require an actual started segment.");
			return global::OpenPCR.PreparedPrincessMoviePlayback.Seek(this, (int)_movieType, _movieId, _movieIndex);
		}

		[global::OpenPCR.Migration("b489b570-9aad-45ee-ad92-f42021613d75")]
		public float GetDurationTime(eMovieType _movieType, long _movieId, long _movieIndex = 0L)
		{
			return global::OpenPCR.PreparedPrincessMoviePlayback.Duration(this, (int)_movieType, _movieId, _movieIndex);
		}

		protected override void onInit() => global::OpenPCR.MigrationPending.Throw("Elements.MovieManager.onInit()", "behavior migration pending");
	}
}
