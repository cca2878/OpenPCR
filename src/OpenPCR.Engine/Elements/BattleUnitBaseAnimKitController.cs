using global::Elements;
using global::OpenPCR;
using global::OpenPCR.AnimKit;
using global::OpenPCR.Runtime.Api;
using System.Runtime.CompilerServices;
using Elements.Data;

namespace Elements
{
	public class BattleUnitBaseAnimKitController : AnimKitController
	{
		protected int skinId;
		protected eAnimKitType spineType;
		public int UnitId { get; private set; }
		public int MotionType { get; private set; }
		protected bool isPause { get; set; }
		private float timeScaleForEffect { get; set; } = 1f;
		private float timeScaleMain { get; set; } = 1f;

		[global::OpenPCR.Migration("243b09b5-ec6b-45b5-b610-ea505352da2c")]
		public BattleUnitBaseAnimKitController()
		{
		}

		[global::OpenPCR.Migration("7b429d57-13bc-49b2-858b-481044c979c8")]
		public string ConvertAnimeIdToAnimeName(eAnimKitCharacterAnimeId _animeId, int _index1 = -1, int _index2 = -1, int _index3 = -1)
		{
			return AnimKitDefine.GetAnimeName(_animeId, UnitId, MotionType, _index1, _index2, _index3);
		}

		[global::OpenPCR.Migration("9b45bfd8-7a64-4ecb-bf7c-35d2a2aa1c9a")]
		public bool IsAnimation(eAnimKitCharacterAnimeId _animeId, int _index1 = -1, int _index2 = -1, int _index3 = -1)
		{
			return IsAnimation(ConvertAnimeIdToAnimeName(_animeId, _index1, _index2, _index3));
		}

		[global::OpenPCR.Migration("52a98cc9-e5c2-4bb6-ad08-2a972cc93a31")]
		public void Pause()
		{
			timeScaleMain = 0f;
			isPause = true;
			applyTimeScale();
		}

		[global::OpenPCR.Migration("f61938cb-b5e4-4d50-88a8-663422101df0")]
		public void Resume()
		{
			isPause = false;
			timeScaleMain = 1f;
			applyTimeScale();
		}

		[global::OpenPCR.Migration("a6cdb081-f1b9-4cc4-9e26-9df65f899016")]
		public void SetTimeScale(float _timeScale)
		{
			timeScaleMain = _timeScale;
			applyTimeScale();
		}

		[global::OpenPCR.Migration("88911fad-58a9-4945-a1cf-ee7e71c534b5")]
		private void applyTimeScale()
		{
			state.TimeScale = timeScaleForEffect * timeScaleMain;
		}

		[global::OpenPCR.Migration("a8f6dbce-2ac7-4e27-8df1-3cff1b0dda6a")]
		public void LoadAnimationIDImmediately(eAnimKitBinaryAnimationId _binaryAnimationId)
		{
			int resourceIndexFromUnitIdWeaponId = getResourceIndexFromUnitIdWeaponId(_binaryAnimationId);
			global::OpenPCR.PreparedAnimKitLoads.LoadAnimation(this, (int)_binaryAnimationId, resourceIndexFromUnitIdWeaponId);
		}

		[global::OpenPCR.Migration("eccc0e2e-ae1a-459a-891a-4a659a6ae9dc")]
		public void LoadAnimationIDImmediately(eAnimKitBinaryAnimationId _binaryAnimationId, int _index)
		{
			global::OpenPCR.PreparedAnimKitLoads.LoadAnimation(this, (int)_binaryAnimationId, _index);
		}

		private int getResourceIndexFromUnitIdWeaponId(eAnimKitBinaryAnimationId _binaryAnimationId)
		{
			return AnimKitManager.SelectResourceIndexFromUnitIdWeaponId(_binaryAnimationId, UnitId, MotionType);
		}

		[global::OpenPCR.Migration("91853ded-8a1c-4cf4-9eac-3e6ef5a65e71")]
		public static Vector3 BoneWorldToGlobalPosConsiderRotate(Bone _bone, Transform _position, Vector3 _lossyScale, bool _callFromBossDialog = false)
		{
			if (_callFromBossDialog)
			{
				return BoneWorldToGlobalPos(_bone, _position.position, _lossyScale);
			}

			return _position.position + Quaternion.Euler(_position.eulerAngles) * (Vector3.Scale(new Vector3(_bone.worldX, _bone.worldY, 0f), _lossyScale) - Vector3.Scale(_position.localPosition, _position.lossyScale));
		}


		[global::OpenPCR.Migration("301d5ada-158c-42c4-b3b2-525cd179d2eb")]
		public static Vector3 BoneWorldToGlobalPos(Bone _bone, Vector3 _position, Vector3 _lossyScale)
		{
			return _position + Vector3.Scale(new Vector3(_bone.worldX, _bone.worldY, 0f), _lossyScale);
		}
		[global::OpenPCR.Migration("4e2b31e3-e78d-4bdf-bcca-c94558ce04ff")] public void SetTimeScaleForEffect(float _timeScale) { timeScaleForEffect = _timeScale; applyTimeScale(); }
		[global::OpenPCR.ImplicitMigration("40b34cd4-86d1-446e-9241-64832e35716f")]
		public bool IsPause => isPause;
	}
}
