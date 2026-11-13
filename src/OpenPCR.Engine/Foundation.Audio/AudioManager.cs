using global::OpenPCR.Foundation;
using global::OpenPCR.Foundation.Audio;
using global::OpenPCR.Runtime.Api;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using global::OpenPCR.AudioStubs;


namespace OpenPCR.Foundation.Audio
{
	public class AudioManager : MonoBehaviour, IManager
	{

		public AudioManager()
		{
			throw new global::OpenPCR.MigrationPendingException("OpenPCR.Foundation.Audio.AudioManager::.ctor()", "body migration pending");
		}

		public void Stop(SoundGroup group, int index, float fadeOutTime = 0f, Action stoppedCallback = null)
		{
			throw new global::OpenPCR.OutOfBoundaryException("OpenPCR.Foundation.Audio.AudioManager::Stop(OpenPCR.Foundation.Audio.SoundGroup,System.Int32,System.Single,System.Action):System.Void: runtime capability is unavailable.");
		}

		public AudioPlayback Play(SoundGroup group, string cueSheetName, string cueName, bool isLoop = false, float startTime = 0f, float volume = 1f, Action stoppedCallback = null, bool isSameBgmPlay = false, int additionalVoicePriority = 0)
		{
			throw new global::OpenPCR.MigrationPendingException("OpenPCR.Foundation.Audio.AudioManager::Play(OpenPCR.Foundation.Audio.SoundGroup,System.String,System.String,System.Boolean,System.Single,System.Single,System.Action,System.Boolean,System.Int32):OpenPCR.Foundation.Audio.AudioPlayback", "body migration pending");
		}

		public AudioPlayback PlayCrossFade(SoundGroup group, string cueSheetName, string cueName, bool isLoop = false, float fadeOutTime = 0f, float fadeInTime = 0f, float offsetTime = 0f)
		{
			throw new global::OpenPCR.OutOfBoundaryException("OpenPCR.Foundation.Audio.AudioManager::PlayCrossFade(OpenPCR.Foundation.Audio.SoundGroup,System.String,System.String,System.Boolean,System.Single,System.Single,System.Single):OpenPCR.Foundation.Audio.AudioPlayback: runtime capability is unavailable.");
		}

		public void Enable3dListener(bool isEnable)
		{
			throw new global::OpenPCR.OutOfBoundaryException("OpenPCR.Foundation.Audio.AudioManager::Enable3dListener(System.Boolean):System.Void: runtime capability is unavailable.");
		}

		public void Set3dListenerPos(Vector3 listenerPos)
		{
			throw new global::OpenPCR.OutOfBoundaryException("OpenPCR.Foundation.Audio.AudioManager::Set3dListenerPos(OpenPCR.Runtime.Api.Vector3):System.Void: runtime capability is unavailable.");
		}
	}
}
