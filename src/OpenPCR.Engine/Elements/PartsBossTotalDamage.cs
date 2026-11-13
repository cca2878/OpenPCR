using global::OpenPCR.Runtime.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using Elements.Battle;
using Object = global::OpenPCR.Runtime.Api.Object;

namespace Elements
{

	public partial class PartsBossTotalDamage : MonoBehaviour, ISingletonField
	{
		private enum eParticleType
		{
			COMBO,
			PATH,
			LARGE
		}

		private class eParticleType_DictComparer : IEqualityComparer<eParticleType>
		{


			public bool Equals(eParticleType _x, eParticleType _y)
			{
				return _x == _y;
			}

			public int GetHashCode(eParticleType _obj)
			{
				return (int)_obj;
			}
		}

		private enum eNumSpriteType
		{
			NORMAL,
			EFFECT1,
			EFFECT2
		}

		private class eNumSpriteType_DictComparer : IEqualityComparer<eNumSpriteType>
		{


			public bool Equals(eNumSpriteType _x, eNumSpriteType _y)
			{
				return _x == _y;
			}

			public int GetHashCode(eNumSpriteType _obj)
			{
				return (int)_obj;
			}
		}

		private class ParticleInfo
		{
			public GameObject ParticleObj;

			public ParticleSystem ParticleSystem;

			public bool InUse;

			public ParticleInfo(GameObject _particleObj)
			{
				ParticleObj = _particleObj;
				ParticleSystem = _particleObj.GetComponentInChildren<ParticleSystem>();
				InUse = false;
				ParticleSystem.Stop(true);
			}
		}

		private const int TOTAL_DAMAGE_ICON_MIN_SIZE = 100;

		private const long TOTAL_DAMAGE_MAX = 99999999999L;

		private readonly Vector3 TRAINING_TEXT_SCALE;

		private long currentDamage;

		private Dictionary<eParticleType, List<ParticleInfo>> particlePool;

		private static readonly Dictionary<eNumSpriteType, List<string>> numSpriteNames;

		private static readonly WaitForSeconds COMBO_WAIT_TIME;

		private static readonly WaitForSeconds PATH_WAIT_TIME;

		private static readonly WaitForSeconds LARGE_NUM_SHOW_TIME;

		private static readonly WaitForSeconds LARGE_SHOW_TIME;

		private static readonly WaitForSeconds LARGE_WAIT_TIME;

		private static readonly Vector3 PATH_MOVE_GOAL_POS;

		private const float PATH_MOVE_TIME = 0.3f;

		private const int POOL_COUNT_COMBO_PARTICLE = 5;

		private const int POOL_COUNT_PATH_PARTICLE = 2;

		private const int POOL_COUNT_LARGE_PARTICLE = 2;

		private const float NUM_LABEL_DEFAULT_POS_X = -468.5f;

		private const float NUM_LABEL_INTERVAL_X = -9.625f;

		private static Yggdrasil<PartsBossTotalDamage> staticSingletonTree;

		private static IBattleCameraEffectForPartsBossTotalDamage staticBattleCameraEffect;

		protected IBattleCameraEffectForPartsBossTotalDamage battleCameraEffect => staticBattleCameraEffect;

		[global::OpenPCR.Migration("f4f9c391-d9a3-4745-83ad-9a857781ff2e")]
		public static void StaticRelease()
		{
			staticSingletonTree = null;
			staticBattleCameraEffect = null;
		}

		[global::OpenPCR.Migration("3d4616ac-1b9a-4f99-9052-f20e559304d5")]
		public void Init(long _currentDamage, bool _isRehearsal, bool _isTraining = false, bool _isInfinityHpMode = false)
		{
			global::OpenPCR.Presentation.Noop();
		}

		[global::OpenPCR.Migration("d1c213fb-464a-43a8-aa5d-c8c029b8783f")]
		public void SetDamageNormal(long _damage)
		{
			global::OpenPCR.Presentation.Noop();
		}

		[global::OpenPCR.Migration("64f174af-2f19-4b2c-b9d6-14ccd60081f9")]


		public void SetDamageUB(long _damage, eDamageEffectType _effectType, UnitCtrl _unit = null, Vector3 _pos = default(Vector3))
		{
			if (gameObject.activeInHierarchy && _effectType == eDamageEffectType.LARGE)
				StartCoroutine(playLargeUBEffect(_damage, _unit, _pos));
		}

		[global::OpenPCR.Migration("cc000b7b-5dc5-4cbd-a5ea-fa4d6488e513")]

		private IEnumerator playLargeUBEffect(long _damage, UnitCtrl _unit = null, Vector3 _pos = default(Vector3))
		{
			yield return LARGE_NUM_SHOW_TIME;
			currentDamage = Math.Max(currentDamage, _damage);
			StartCoroutine(moveParticle(null, 0.3f, _pos, PATH_MOVE_GOAL_POS));
			yield return LARGE_SHOW_TIME;
		}

		[global::OpenPCR.Migration("56157435-f414-4147-8b73-041aca5af55d")]

		private IEnumerator moveParticle(ParticleInfo particle, float time, Vector3 fromPos, Vector3 toPos)
		{
			float now = time;
			_ = (float)BattleManager.HeldRandom(-2000, 2000) / 1000f;
			while (now > 0f)
			{
				yield return null;
				now -= Time.deltaTime;
			}
		}

		public PartsBossTotalDamage()
		{
			TRAINING_TEXT_SCALE = new Vector3(0.9f, 1f, 1f);
			particlePool = new Dictionary<eParticleType, List<ParticleInfo>>(new eParticleType_DictComparer())
		{
			{
				eParticleType.COMBO,
				new List<ParticleInfo>()
			},
			{
				eParticleType.PATH,
				new List<ParticleInfo>()
			},
			{
				eParticleType.LARGE,
				new List<ParticleInfo>()
			}
		};
		}

		static PartsBossTotalDamage()
		{
			numSpriteNames = new Dictionary<eNumSpriteType, List<string>>(new eNumSpriteType_DictComparer())
		{
			{
				eNumSpriteType.NORMAL,
				new List<string> { "battle_num_yellow_s_0", "battle_num_yellow_s_1", "battle_num_yellow_s_2", "battle_num_yellow_s_3", "battle_num_yellow_s_4", "battle_num_yellow_s_5", "battle_num_yellow_s_6", "battle_num_yellow_s_7", "battle_num_yellow_s_8", "battle_num_yellow_s_9" }
			},
			{
				eNumSpriteType.EFFECT1,
				new List<string> { "battle_ef_num_yellow_s_0", "battle_ef_num_yellow_s_1", "battle_ef_num_yellow_s_2", "battle_ef_num_yellow_s_3", "battle_ef_num_yellow_s_4", "battle_ef_num_yellow_s_5", "battle_ef_num_yellow_s_6", "battle_ef_num_yellow_s_7", "battle_ef_num_yellow_s_8", "battle_ef_num_yellow_s_9" }
			},
			{
				eNumSpriteType.EFFECT2,
				new List<string> { "battle_ef_num_white_s_0", "battle_ef_num_white_s_1", "battle_ef_num_white_s_2", "battle_ef_num_white_s_3", "battle_ef_num_white_s_4", "battle_ef_num_white_s_5", "battle_ef_num_white_s_6", "battle_ef_num_white_s_7", "battle_ef_num_white_s_8", "battle_ef_num_white_s_9" }
			}
		};
			COMBO_WAIT_TIME = new WaitForSeconds(0.5f);
			PATH_WAIT_TIME = new WaitForSeconds(0.3f);
			LARGE_NUM_SHOW_TIME = new WaitForSeconds(0.2f);
			LARGE_SHOW_TIME = new WaitForSeconds(0.3f);
			LARGE_WAIT_TIME = new WaitForSeconds(0.5f);
			PATH_MOVE_GOAL_POS = new Vector3(-480f, 296f, 0f);
			staticSingletonTree = null;
			staticBattleCameraEffect = null;
		}
	}

}
