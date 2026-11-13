using global::Elements;
using global::OpenPCR.Runtime.Api;
using System;

namespace Elements
{
	public class AnimKitNormalEffectStar3Shadow : AnimKitNormalEffect
	{
		public int Star1NormalIndex = 1;

		public int Star3NormalIndex = 2;

		public int Star6NormalIndex = 3;

		public int Star1ShadowIndex = 4;

		public int Star3ShadowIndex = 5;

		public int Star6ShadowIndex = 6;

		public int Star1SkinId = 100111;

		public int Star3SkinId = 100131;

		public int Star6SkinId = 100161;

		private int currentSkinId = 100111;

		private bool currentIsShadow;

		private BattleAnimKitController star1NormalSpine;

		private BattleAnimKitController star3NormalSpine;

		private BattleAnimKitController star6NormalSpine;

		private BattleAnimKitController star1ShadowSpine;

		private BattleAnimKitController star3ShadowSpine;

		private BattleAnimKitController star6ShadowSpine;

		[global::OpenPCR.Migration("d98ca6d7-c1ff-4e4c-acba-35585e462b40")] public AnimKitNormalEffectStar3Shadow() { }

		[global::OpenPCR.Migration("51d10b0a-ac4b-4d85-8384-60eada235ba0")]
		protected override void loadSpine(Action endAction = null)
		{
			if (currentIsShadow)
			{
				if (currentSkinId == Star1SkinId)
				{
					if (star1ShadowSpine == null)
					{
						BattleAnimKitController.LoadCreateImmidiateBySkinId(eAnimKitType.SKILL_EFFECT, skillId, skillId, Star1ShadowIndex, base.transform, delegate (BattleAnimKitController unit)
						{
							unit.PlayAnime(eAnimKitCharacterAnimeId.SUMMON_SKILL_EFFECT, skillId, Roop);
							star1ShadowSpine = unit;
							// Rendered layer output omitted.
							if (endAction != null)
							{
								endAction();
							}
						});
					}
					unitSpineController = star1ShadowSpine;
				}
				else if (currentSkinId == Star3SkinId)
				{
					if (star3ShadowSpine == null)
					{
						BattleAnimKitController.LoadCreateImmidiateBySkinId(eAnimKitType.SKILL_EFFECT, skillId, skillId, Star3ShadowIndex, base.transform, delegate (BattleAnimKitController unit)
						{
							unit.PlayAnime(eAnimKitCharacterAnimeId.SUMMON_SKILL_EFFECT, skillId, Roop);
							star3ShadowSpine = unit;
							// Rendered layer output omitted.
							if (endAction != null)
							{
								endAction();
							}
						});
					}
					unitSpineController = star3ShadowSpine;
				}
				else
				{
					if (star6ShadowSpine == null)
					{
						BattleAnimKitController.LoadCreateImmidiateBySkinId(eAnimKitType.SKILL_EFFECT, skillId, skillId, Star6ShadowIndex, base.transform, delegate (BattleAnimKitController unit)
						{
							unit.PlayAnime(eAnimKitCharacterAnimeId.SUMMON_SKILL_EFFECT, skillId, Roop);
							star6ShadowSpine = unit;
							// Rendered layer output omitted.
							if (endAction != null)
							{
								endAction();
							}
						});
					}
					unitSpineController = star6ShadowSpine;
				}
			}
			else if (currentSkinId == Star1SkinId)
			{
				if (star1NormalSpine == null)
				{
					BattleAnimKitController.LoadCreateImmidiateBySkinId(eAnimKitType.SKILL_EFFECT, skillId, skillId, Star1NormalIndex, base.transform, delegate (BattleAnimKitController unit)
					{
						unit.PlayAnime(eAnimKitCharacterAnimeId.SUMMON_SKILL_EFFECT, skillId, Roop);
						star1NormalSpine = unit;
						// Rendered layer output omitted.
						if (endAction != null)
						{
							endAction();
						}
					});
				}
				unitSpineController = star1NormalSpine;
			}
			else if (currentSkinId == Star3SkinId)
			{
				if (star3NormalSpine == null)
				{
					BattleAnimKitController.LoadCreateImmidiateBySkinId(eAnimKitType.SKILL_EFFECT, skillId, skillId, Star3NormalIndex, base.transform, delegate (BattleAnimKitController unit)
					{
						unit.PlayAnime(eAnimKitCharacterAnimeId.SUMMON_SKILL_EFFECT, skillId, Roop);
						star3NormalSpine = unit;
						// Rendered layer output omitted.
						if (endAction != null)
						{
							endAction();
						}
					});
				}
				unitSpineController = star3NormalSpine;
			}
			else
			{
				if (star6NormalSpine == null)
				{
					BattleAnimKitController.LoadCreateImmidiateBySkinId(eAnimKitType.SKILL_EFFECT, skillId, skillId, Star6NormalIndex, base.transform, delegate (BattleAnimKitController unit)
					{
						unit.PlayAnime(eAnimKitCharacterAnimeId.SUMMON_SKILL_EFFECT, skillId, Roop);
						star6NormalSpine = unit;
						// Rendered layer output omitted.
						if (endAction != null)
						{
							endAction();
						}
					});
				}
				unitSpineController = star6NormalSpine;
			}
			unitSpineController.SetActive(_isActive: true);
		}

		[global::OpenPCR.Migration("472fc47b-5105-4e29-bf53-67173a939f54")]
		public override void ResetParameter(GameObject _prefab, int _skinId = 0, bool _isShadow = false)
		{
			base.ResetParameter(_prefab, _skinId, _isShadow);
			currentSkinId = _skinId;
			currentIsShadow = _isShadow;
			if (unitSpineController != null)
			{
				unitSpineController.SetActive(_isActive: false);
			}
			loadSpine();
		}
	}
}
