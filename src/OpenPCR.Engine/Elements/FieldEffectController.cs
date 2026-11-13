using global::OpenPCR.Runtime.Api;
using System.Collections.Generic;
using Elements.Battle;

namespace Elements
{
	[global::OpenPCR.ImplicitMigration("dce1b215-f824-4f4f-8422-86c1903b0f27")]
	public class FieldEffectController : SkillEffectCtrl
	{
		private bool isBack;

		public bool IsBehindSortBackBoss;

		public List<PrefabWithTime> EndEffectPrefab;

		public bool DisplayCenter;

		public float EndDuration;

		[global::OpenPCR.Migration("e285703a-4d7b-49de-89d9-cdb36a0e1da2")]
		public override void InitializeSort()
		{
			base.InitializeSort();
			isBack = false;
		}

		[global::OpenPCR.Migration("6b3b3124-a50b-43d2-9b2c-ffab89504a90")]
		public override void ResetParameter(GameObject _prefab, int _skinID = 0, bool _isShadow = false)
		{
			base.ResetParameter(_prefab, _skinID);
			if (base.particles != null)
			{
				for (int i = 0; i < base.particles.Length; i++)
				{
					ParticleSystem.EmissionModule emission = base.particles[i].emission;
					emission.enabled = true;
				}
			}
		}

		[global::OpenPCR.Migration("89296769-b4c7-4213-8085-10308deac1a8")]
		public override void SetSortOrderFront()
		{
			foreach (KeyValuePair<Renderer, SortOrderAndOffset> item in particleRendererDictionary)
			{
				Renderer key = item.Key;
				if (item.Value.SortOrder < 4500)
				{
					if (!base.battleManager.IsSortBackBossBattle || IsBehindSortBackBoss)
					{
						key.sortingOrder = 12190;
					}
					else
					{
						key.sortingOrder = BattleDefine.GetUnitSortOrder(base.battleManager.GetBossUnit()) + 300 + 11500;
					}
				}
				else
				{
					key.sortingOrder = 22500;
				}
			}
		}

		[global::OpenPCR.Migration("40974587-72c7-44a5-9b60-56b67e86e7e8")]
		public override void SetSortOrderBack()
		{
			if (isBack)
			{
				return;
			}
			isBack = true;
			foreach (KeyValuePair<Renderer, SortOrderAndOffset> item in particleRendererDictionary)
			{
				Renderer key = item.Key;
				if (item.Value.SortOrder < 4500)
				{
					if (!base.battleManager.IsSortBackBossBattle || IsBehindSortBackBoss)
					{
						key.sortingOrder = 690;
					}
					else
					{
						key.sortingOrder = BattleDefine.GetUnitSortOrder(base.battleManager.GetBossUnit()) + 300;
					}
				}
				else
				{
					key.sortingOrder = 11000;
				}
			}
		}

		[global::OpenPCR.Migration("7c84a586-7c73-4752-889c-bcdbcb1fb80a")]
		public void StopEmitter()
		{
			global::OpenPCR.Presentation.Noop();
		}
	}
}
