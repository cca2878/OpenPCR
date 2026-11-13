using global::OpenPCR.UICompat;
using global::OpenPCR.Runtime.Api;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Elements
{
	public partial class BlockLayerManager : ManagerSingleton<BlockLayerManager>
	{

		public class eColliderType_DictComparer : IEqualityComparer<eColliderType>
		{


			public bool Equals(eColliderType _x, eColliderType _y) => _x == _y;
			public int GetHashCode(eColliderType _obj) => (int)_obj;
		}

		public enum eColliderType
		{
			COMMON_MAIN,
			COMMON_SUB,
			TUTORIAL,
			CONTENT_RELEASE,
			LOAD,
			CONNECT,
			DIALOG,
			DOWNLOAD,
			BUTTON,
			NON_LOGIC,
			CLAN_BATTLE,
			GAME_EXIT,
			PERMISSION,
			MAX
		}

		public Dictionary<eColliderType, bool> depthDictionary = new Dictionary<eColliderType, bool>(new eColliderType_DictComparer())
		{
			{
				(eColliderType)0,
				false
			},
			{
				(eColliderType)1,
				false
			},
			{
				(eColliderType)2,
				false
			},
			{
				(eColliderType)3,
				false
			},
			{
				(eColliderType)4,
				false
			},
			{
				(eColliderType)5,
				false
			},
			{
				(eColliderType)6,
				true
			},
			{
				(eColliderType)7,
				false
			},
			{
				(eColliderType)8,
				true
			},
			{
				(eColliderType)9,
				true
			},
			{
				(eColliderType)10,
				true
			},
			{
				(eColliderType)11,
				true
			},
			{
				(eColliderType)12,
				true
			},
		};
		[SerializeField]
		private GameObject backObj;
		[global::OpenPCR.Runtime.Api.SerializeField]
		private UITexture backTexture;
		[global::OpenPCR.Runtime.Api.SerializeField]
		private UITexture frontTexture;
		private static readonly Vector2 BLOCK_COLLIDER_SIZE = new Vector2(4000f, 4000f);
		[SerializeField]
		private GameObject frontObj;
		private BoxCollider[] colliderArray;
		public BlockLayerManager()
		{
		}


		public void SetEnabled(eColliderType _colliderType, bool _enable)
		{
			colliderArray[(int)_colliderType].enabled = _enable;
			bool flag = depthDictionary[_colliderType];
			bool active = false;
			if (_enable)
			{
				active = true;
			}
			else
			{
				for (int i = 0; i < 13; i++)
				{
					if (colliderArray[i].enabled && depthDictionary[(eColliderType)i] == flag)
					{
						active = true;
						break;
					}
				}
			}

			if (flag)
			{
				frontObj.SetActive(active);
			}
			else
			{
				backObj.SetActive(active);
			}
		}

		[global::OpenPCR.Migration("7cb3b836-3e38-4877-af2f-438ffee985b3")]
		public void SetBlockButton(bool _isEnabled)
		{
			SetEnabled(eColliderType.BUTTON, _isEnabled);
		}

		protected override void onInit()
		{
			Func<UITexture, BoxCollider> createCollider = uiTexture =>
			{
				BoxCollider collider = uiTexture.gameObject.AddComponent<BoxCollider>();
				collider.size = new Vector3(BLOCK_COLLIDER_SIZE.x, BLOCK_COLLIDER_SIZE.y, 0f);
				collider.enabled = false;
				return collider;
			};
			colliderArray = new BoxCollider[13];
			for (int i = 0; i < 13; i++)
				colliderArray[i] = createCollider(depthDictionary[(eColliderType)i] ? frontTexture : backTexture);
		}

		[global::OpenPCR.Migration("a0a9f7f1-5af3-463c-b642-5f7efea1e391")]
		public void SetBlockCommon(bool _isBlockEnable)
		{
			global::OpenPCR.Presentation.Noop();
		}
	}
}
