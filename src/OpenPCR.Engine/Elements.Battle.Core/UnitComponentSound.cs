using global::Elements;
using global::OpenPCR.AudioStubs;
using global::OpenPCR.Runtime.Api;
using Elements.Data;
using global::OpenPCR.Foundation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class UnitComponentSound : UnitComponentBase
	{
		private BattleManager battleManager;
		private SoundManager soundManager;
		private Dictionary<SoundManager.eVoiceType, int> voiceSuffixDic = new Dictionary<SoundManager.eVoiceType, int>(new SoundManager.eVoiceType_DictComparer());
		private Dictionary<SoundManager.eVoiceType, Dictionary<int, int>> voiceSuffixDicTenth = new Dictionary<SoundManager.eVoiceType, Dictionary<int, int>>(new SoundManager.eVoiceType_DictComparer());
		public AudioAtomSource VoiceSource { get; private set; }
		public AudioAtomSource SeSource { get; private set; }
		public int SoundUnitId { get; private set; }
		public Dictionary<SoundManager.eVoiceType, Dictionary<int, int>> VoiceMaxForMultiUnit { get; set; } = new Dictionary<SoundManager.eVoiceType, Dictionary<int, int>>();

		[global::OpenPCR.Migration("50a30fac-3eb9-403c-b3f7-cda713fbaf16")]
		public UnitComponentSound(UnitCtrl _unitController, BattleManager _battleManager, int _soundUnitId, GameObject _gameObject) : base(_unitController)
		{
			battleManager = _battleManager;
			soundManager = ManagerSingleton<SoundManager>.Instance;
			SoundUnitId = _soundUnitId;
			if (VoiceSource == null) VoiceSource = _gameObject.AddComponent<AudioAtomSource>();
			if (SeSource == null) SeSource = _gameObject.AddComponent<AudioAtomSource>();
		}

		[global::OpenPCR.Migration("bf80bc4e-41c8-4639-8c44-b25019bcd96a")]
		public void PlayVoice(SoundManager.eVoiceType _voiceType, int _index, bool _lastSuffixIsRandom)
		{
			if (unitController.GetIsMuteVoice() && _voiceType != (SoundManager.eVoiceType)46 && _voiceType != (SoundManager.eVoiceType)12)
				return;
			if (isVoicePlaying())
				unitController.StartCoroutine(fadeoutVoice(() => playVoiceImpl(_voiceType, _index, _lastSuffixIsRandom)));
			else
				playVoiceImpl(_voiceType, _index, _lastSuffixIsRandom);
		}

		[global::OpenPCR.Migration("a887185f-e5e7-416c-bb02-05d162662b75")]
		private int getNextVoiceSuffix(SoundManager.eVoiceType _voiceType, bool _tenthPlace = false, int _index = 0)
		{
			int num = 3;
			int key = 0;
			SoundManager.eVoiceType key2 = _voiceType;
			switch (_voiceType)
			{
				case SoundManager.eVoiceType.APPLAUD_FOR_MULTI:
					key2 = SoundManager.eVoiceType.APPLAUD;
					break;
				case SoundManager.eVoiceType.THANKS_FOR_MULTI:
					key2 = SoundManager.eVoiceType.THANKS;
					break;
				case SoundManager.eVoiceType.LAST_WAVE_FOR_MULTI:
					key2 = SoundManager.eVoiceType.LAST_WAVE;
					break;
				case SoundManager.eVoiceType.WAVE_START_FOR_MULTI:
					key2 = SoundManager.eVoiceType.WAVE_START;
					break;
				case SoundManager.eVoiceType.NEXT_WAVE_FOR_MULTI:
					key2 = SoundManager.eVoiceType.NEXT_WAVE;
					break;
			}

			if (VoiceMaxForMultiUnit.TryGetValue(key2, out var value))
			{
				switch (_voiceType)
				{
					case SoundManager.eVoiceType.ATTACK:
					case SoundManager.eVoiceType.DAMAGE:
					case SoundManager.eVoiceType.RETIRE:
					case SoundManager.eVoiceType.WAVE_START_FOR_MULTI:
					case SoundManager.eVoiceType.NEXT_WAVE_FOR_MULTI:
					case SoundManager.eVoiceType.LAST_WAVE_FOR_MULTI:
					case SoundManager.eVoiceType.APPLAUD_FOR_MULTI:
					case SoundManager.eVoiceType.THANKS_FOR_MULTI:
						num = value[0];
						break;
					case SoundManager.eVoiceType.CUT_IN:
						num = value[_index];
						key = _index;
						break;
					case SoundManager.eVoiceType.UNION_BURST:
					case SoundManager.eVoiceType.SKILL:
						key = _index - _index % 10;
						num = value[key];
						break;
				}
			}

			if (_tenthPlace)
			{
				if (voiceSuffixDicTenth.ContainsKey(_voiceType))
				{
					if (voiceSuffixDicTenth[_voiceType].ContainsKey(key))
					{
						voiceSuffixDicTenth[_voiceType][key]++;
						voiceSuffixDicTenth[_voiceType][key] %= num;
					}
					else
					{
						voiceSuffixDicTenth[_voiceType].Add(key, BattleManager.HeldRandom(0, num));
					}
				}
				else
				{
					voiceSuffixDicTenth.Add(_voiceType, new Dictionary<int, int> { { key, BattleManager.HeldRandom(0, num) } });
				}

				return (voiceSuffixDicTenth[_voiceType][key] + 1) * 10 + _index;
			}

			if (voiceSuffixDic.ContainsKey(_voiceType))
			{
				voiceSuffixDic[_voiceType]++;
				voiceSuffixDic[_voiceType] %= num;
			}
			else
			{
				voiceSuffixDic.Add(_voiceType, BattleManager.HeldRandom(0, num));
			}

			return voiceSuffixDic[_voiceType] + 1;
		}

		[global::OpenPCR.Migration("a2a2ef77-f35a-40b3-988a-4197dcf3654c")]
		public void PauseVoice(bool _isPause)
		{ global::OpenPCR.Presentation.Noop(); }

		[global::OpenPCR.Migration("b57a9c9c-fa55-441d-abce-5a01981f3a6c")]
		public void PauseSound(bool _isPause)
		{
			PauseSe(_isPause);
			PauseVoice(_isPause);
		}

		[global::OpenPCR.Migration("94cd3dc6-c42d-4470-97aa-534039982b68")]
		public void PlayUbChainVoice(eUbResponceVoiceType _voiceType, int _unitId, CutinVoiceStatus _cutinVoiceStatus, List<UnitCtrl> _aliveUnitList)
		{
			if (_voiceType == eUbResponceVoiceType.THANKS)
				_aliveUnitList = _aliveUnitList.FindAll(unit => unit.ComponentUnionBurst.ThanksTargetUnitId == _unitId);
			if (_aliveUnitList.Count > 0)
			{
				if (_voiceType == eUbResponceVoiceType.APPLOUD)
					BattleVoiceUtility.PlayDialogueVoice(SoundManager.eVoiceType.APPLAUD, unitController, _aliveUnitList, _cutinVoiceStatus.GroupIdSet.GroupId, _cutinVoiceStatus.GroupIdSet.GroupUnitId);
				else if (_voiceType == eUbResponceVoiceType.THANKS)
					BattleVoiceUtility.PlayDialogueVoice(SoundManager.eVoiceType.THANKS, unitController, _aliveUnitList, _cutinVoiceStatus.GroupIdSet.GroupId, _cutinVoiceStatus.GroupIdSet.GroupUnitId);
			}
		}

		[global::OpenPCR.Migration("899e1692-30a2-4c97-99d5-89e2c3c2ed25")]
		public void StopVoiceExceptUbVoice()
		{ if (!unitController.GetIsMuteVoice()) throw new global::OpenPCR.OutOfBoundaryException("Offline host requires explicitly muted voice output; CRI cue/fade selection is outside this boundary."); }

		[global::OpenPCR.Migration("f3dce590-27ee-41c0-b1f3-205a33045dd3")]
		public void StopVoice()
		{ global::OpenPCR.Presentation.Noop(); }

		public int LastVoiceFrame { get; set; }
		public int EnemyVoiceId { get; set; }
		public eUbResponceVoiceType UnionBurstResponceVoiceType { get; set; }
		private UnitComponentAbnormal componentAbnormal { get; set; }

		[global::OpenPCR.Migration("67401221-6ce7-44d9-a02b-4f6e108f1fe6")]
		public void RegisterComponentSet(IUnitComponentContainer _container)
		{
			componentAbnormal = _container.ComponentAbnormal;
		}

		[global::OpenPCR.Migration("035a1d73-4ceb-4845-99ed-df44875f0641")]
		public void InitializeVoiceMax()
		{
			foreach (MasterSpBattleVoice.SpBattleVoice item in ManagerSingleton<MasterDataManager>.Instance.masterSpBattleVoice.GetListWithUnitId(SoundUnitId))
			{
				switch ((int)item.voice_type)
				{
					case 1001:
						VoiceMaxForMultiUnit.Add(SoundManager.eVoiceType.ATTACK, new Dictionary<int, int> { { 0, item.value } });
						break;
					case 1002:
						VoiceMaxForMultiUnit.Add(SoundManager.eVoiceType.WIN, new Dictionary<int, int> { { 0, item.value } });
						break;
					case 1003:
						VoiceMaxForMultiUnit.Add(SoundManager.eVoiceType.LOSE, new Dictionary<int, int> { { 0, item.value } });
						break;
					case 1004:
						VoiceMaxForMultiUnit.Add(SoundManager.eVoiceType.RETIRE, new Dictionary<int, int> { { 0, item.value } });
						break;
					case 1005:
						VoiceMaxForMultiUnit.Add(SoundManager.eVoiceType.DAMAGE, new Dictionary<int, int> { { 0, item.value } });
						break;
					case 1006:
						if (!VoiceMaxForMultiUnit.ContainsKey(SoundManager.eVoiceType.CUT_IN))
						{
							VoiceMaxForMultiUnit.Add(SoundManager.eVoiceType.CUT_IN, new Dictionary<int, int> { { 1, item.value } });
						}
						else
						{
							VoiceMaxForMultiUnit[SoundManager.eVoiceType.CUT_IN].Add(1, item.value);
						}
						break;
					case 1007:
						if (!VoiceMaxForMultiUnit.ContainsKey(SoundManager.eVoiceType.CUT_IN))
						{
							VoiceMaxForMultiUnit.Add(SoundManager.eVoiceType.CUT_IN, new Dictionary<int, int> { { 2, item.value } });
						}
						else
						{
							VoiceMaxForMultiUnit[SoundManager.eVoiceType.CUT_IN].Add(2, item.value);
						}
						break;
					case 1008:
						if (!VoiceMaxForMultiUnit.ContainsKey(SoundManager.eVoiceType.UNION_BURST))
						{
							VoiceMaxForMultiUnit.Add(SoundManager.eVoiceType.UNION_BURST, new Dictionary<int, int> { { 100, item.value } });
						}
						else
						{
							VoiceMaxForMultiUnit[SoundManager.eVoiceType.UNION_BURST].Add(100, item.value);
						}
						break;
					case 1009:
						if (!VoiceMaxForMultiUnit.ContainsKey(SoundManager.eVoiceType.UNION_BURST))
						{
							VoiceMaxForMultiUnit.Add(SoundManager.eVoiceType.UNION_BURST, new Dictionary<int, int> { { 200, item.value } });
						}
						else
						{
							VoiceMaxForMultiUnit[SoundManager.eVoiceType.UNION_BURST].Add(200, item.value);
						}
						break;
					case 1010:
						if (!VoiceMaxForMultiUnit.ContainsKey(SoundManager.eVoiceType.SKILL))
						{
							VoiceMaxForMultiUnit.Add(SoundManager.eVoiceType.SKILL, new Dictionary<int, int> { { 100, item.value } });
						}
						else
						{
							VoiceMaxForMultiUnit[SoundManager.eVoiceType.SKILL].Add(100, item.value);
						}
						break;
					case 1011:
						if (!VoiceMaxForMultiUnit.ContainsKey(SoundManager.eVoiceType.SKILL))
						{
							VoiceMaxForMultiUnit.Add(SoundManager.eVoiceType.SKILL, new Dictionary<int, int> { { 200, item.value } });
						}
						else
						{
							VoiceMaxForMultiUnit[SoundManager.eVoiceType.SKILL].Add(200, item.value);
						}
						break;
					case 1012:
						VoiceMaxForMultiUnit.Add(SoundManager.eVoiceType.APPLAUD, new Dictionary<int, int> { { 0, item.value } });
						break;
					case 1013:
						VoiceMaxForMultiUnit.Add(SoundManager.eVoiceType.THANKS, new Dictionary<int, int> { { 0, item.value } });
						break;
					case 1014:
						VoiceMaxForMultiUnit.Add(SoundManager.eVoiceType.WAVE_START, new Dictionary<int, int> { { 0, item.value } });
						break;
					case 1015:
						VoiceMaxForMultiUnit.Add(SoundManager.eVoiceType.NEXT_WAVE, new Dictionary<int, int> { { 0, item.value } });
						break;
					case 1016:
						VoiceMaxForMultiUnit.Add(SoundManager.eVoiceType.LAST_WAVE, new Dictionary<int, int> { { 0, item.value } });
						break;
					case 1017:
						VoiceMaxForMultiUnit.Add(SoundManager.eVoiceType.BATTLE_START, new Dictionary<int, int> { { 0, item.value } });
						break;
				}
			}
		}

		[global::OpenPCR.Migration("2392a9e2-2f2d-4b47-8164-d10052c88280")]
		private IEnumerator fadeoutVoice(Action _callback, float _fadeSec = 0.3f)
		{
			var easing = new CustomEasing(CustomEasing.eType.outQuad, 1f, 0f, _fadeSec);
			while (easing.IsMoving)
			{
				_ = easing.GetCurVal(Time.deltaTime);
				yield return null;
			}

			_callback.Call();
		}

		[global::OpenPCR.Migration("4005f33c-86ee-4ad8-a7a7-dae32faa0816")]
		private void playVoiceImpl(SoundManager.eVoiceType _voiceType, int _index, bool _lastSuffixIsRandom)
		{
			if (!unitController.JudgeCanPlayVoice(_voiceType, EnemyVoiceId))
				return;
			// CRI playback stop is audio output only.
			// CRI volume output only.
			LastVoiceFrame = unitController.GetBattleFrameCount();
			int id = SoundUnitId;
			if (unitController.JudgeIsRarity6() && BattleDefine.VoiceJudgeDataDictionary[_voiceType].Star6Voice)
				id = UnitUtility.GetSkinId(SoundUnitId, 6);
			if (EnemyVoiceId != 0)
				id = EnemyVoiceId;
			bool tenth = false;
			switch (_voiceType)
			{
				case SoundManager.eVoiceType.UNION_BURST:
				case SoundManager.eVoiceType.SKILL:
				case SoundManager.eVoiceType.CUT_IN:
					tenth = VoiceMaxForMultiUnit.ContainsKey(_voiceType) && VoiceMaxForMultiUnit[_voiceType].ContainsKey(_index);
					break;
				case SoundManager.eVoiceType.APPLAUD:
					if (VoiceMaxForMultiUnit.ContainsKey(_voiceType))
					{
						_lastSuffixIsRandom = true;
						_voiceType = SoundManager.eVoiceType.APPLAUD_FOR_MULTI;
					}

					break;
				case SoundManager.eVoiceType.THANKS:
					if (VoiceMaxForMultiUnit.ContainsKey(_voiceType))
					{
						_lastSuffixIsRandom = true;
						_voiceType = SoundManager.eVoiceType.THANKS_FOR_MULTI;
					}

					break;
				case SoundManager.eVoiceType.WAVE_START:
					if (VoiceMaxForMultiUnit.ContainsKey(_voiceType))
					{
						_lastSuffixIsRandom = true;
						_voiceType = SoundManager.eVoiceType.WAVE_START_FOR_MULTI;
					}

					break;
				case SoundManager.eVoiceType.NEXT_WAVE:
					if (VoiceMaxForMultiUnit.ContainsKey(_voiceType))
					{
						_lastSuffixIsRandom = true;
						_voiceType = SoundManager.eVoiceType.NEXT_WAVE_FOR_MULTI;
					}

					break;
				case SoundManager.eVoiceType.LAST_WAVE:
					if (VoiceMaxForMultiUnit.ContainsKey(_voiceType))
					{
						_lastSuffixIsRandom = true;
						_voiceType = SoundManager.eVoiceType.LAST_WAVE_FOR_MULTI;
					}

					break;
			}

			AudioAtomSource source = VoiceSource;
			int index = tenth || _lastSuffixIsRandom ? getNextVoiceSuffix(_voiceType, tenth, _index) : _index;
			playVoiceForModeChange(source, id, _voiceType, index);
		}

		[global::OpenPCR.Migration("d3a8583c-30ca-4cae-b776-99ee2fc4ec67")]
		private void playVoiceForModeChange(AudioAtomSource _voiceSource, int _unitId, SoundManager.eVoiceType _type, int _index = 0)
		{ global::OpenPCR.Presentation.Noop(); }

		[global::OpenPCR.Migration("bf4b7c11-ca3b-48ba-831b-877985f031f2")]
		public void PlaySeWithMotion(BattleAnimKitController _controller, bool _canPlaySe, eAnimKitCharacterAnimeId _animeId, int _index1, int _index2, int _index3, bool _isLoop, float _startTime = 0f)
		{
			global::OpenPCR.Presentation.Noop();
		}

		[global::OpenPCR.Migration("789a40b7-2a86-44db-8aca-96b67962438c")]
		public void PauseSe(bool _isPause)
		{ global::OpenPCR.Presentation.Noop(); }

		[global::OpenPCR.Migration("cea4359d-7db4-4ced-9fe1-faa16782ebae")]
		public void PlayRetireVoice()
		{
			if (battleManager.IsSpecialBattle && !unitController.IsOther && !unitController.IsInitialMode())
			{
				var continuous = unitController.SearchContinuousUnit();
				if (continuous == null || continuous.Hp < 1L)
					return;
			}

			if (unitController.IsPlayerUnit || EnemyVoiceId != 0)
			{
				if (Time.time - battleManager.LastRetireVoiceTime > 0.45f || EnemyVoiceId != 0)
				{
					unitController.UpdateLastRetireVoiceTime();
					PlayVoice(SoundManager.eVoiceType.RETIRE, 0, true);
				}
			}
		}

		[global::OpenPCR.Migration("c1176e31-4453-4284-a417-2fec472c7ec1")]
		public void PlayDamageVoice()
		{
			if (unitController.IsPlayerUnit || EnemyVoiceId != 0)
			{
				if (Time.time - battleManager.LastDamageVoiceTime > 0.45f || EnemyVoiceId != 0)
				{
					unitController.UpdateLastDamageVoiceTime();
					PlayVoice(SoundManager.eVoiceType.DAMAGE, 0, true);
				}
			}
		}

		[global::OpenPCR.Migration("8cf5667b-78b9-4fbb-83d8-2404fc333894")]
		public bool TryGetVoiceDictionary(SoundManager.eVoiceType _voiceType, out Dictionary<int, int> _dictionary)
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.Core.UnitComponentSound::TryGetVoiceDictionary(Elements.SoundManager\u002BeVoiceType,out System.Collections.Generic.Dictionary\u00602\u003CSystem.Int32,System.Int32\u003E):System.Boolean (outside supported first-phase lifecycle/input scope)"); }

		[global::OpenPCR.Migration("41462feb-615c-4162-be45-ffa3f0ba77c3")]
		private bool isVoicePlaying()
		{ return global::OpenPCR.Presentation.Noop<bool>(); }

		[global::OpenPCR.Migration("652ddb52-1aed-40c0-b286-8f449a1db18c")]
		public bool IsVoicePlaying(string _cueName)
		{ return global::OpenPCR.Presentation.Noop<bool>(); }

		[global::OpenPCR.Migration("447e9632-7a49-4d25-b0fb-61c2f087c868")]
		public void SetUnionBurstResponceVoiceType(eUbResponceVoiceType _type)
		{
			UnionBurstResponceVoiceType = _type;
		}
	}
}
