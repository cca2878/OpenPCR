using global::OpenPCR.UICompat;
using global::OpenPCR.Runtime.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace Elements.Battle
{
	public class TalentBonusAppearEffect : MonoBehaviour
	{
		[SerializeField] private UISprite backgroundSprite;
		[SerializeField] private UISprite backgroundRightSprite;
		[SerializeField] private UISprite singleSymbolSprite;
		[SerializeField] private UISprite doubleSymbolLeftSprite;
		[SerializeField] private UISprite doubleSymbolRightSprite;
		[SerializeField] private Transform singleEffectParentTransform;
		[SerializeField] private Transform doubleLeftEffectParentTransform;
		[SerializeField] private Transform doubleRightEffectParentTransform;
		[SerializeField] private AnimatorFunction animatorFunction;
		private const string SINGLE_TALENT_BONUS_ANIMATION_STATE = "single";
		private const string DOUBLE_TALENT_BONUS_ANIMATION_STATE = "double";
		private List<ParticleSystem> effectList = new List<ParticleSystem>();

		public TalentBonusAppearEffect()
		{
		}



		[global::OpenPCR.Migration("cc1df8ec-bf41-4bf2-ae53-db5993a15e55")]
		public void Play(List<int> _talentBonusIds, Action _showUiCallback, Action _endCallback)
		{
			List<int> talentBonusIds = new List<int>(_talentBonusIds);
			talentBonusIds.Sort();
			setSprite(talentBonusIds);
			setEffect(talentBonusIds);
			string state = talentBonusIds.Count == 1
				? SINGLE_TALENT_BONUS_ANIMATION_STATE
				: DOUBLE_TALENT_BONUS_ANIMATION_STATE;
			animatorFunction.Init(_showUiCallback, 0, _endCallback);
			animatorFunction.Animator.Play(state);
		}

		private void setSprite(List<int> _talentBonusIds)
		{
			bool isSingle = _talentBonusIds.Count == 1;
			backgroundSprite.spriteName = "battle_talent_bonus_bg_{0}".Format(_talentBonusIds.ElementAt(0));
			backgroundRightSprite.SetActive(!isSingle);
			if (!isSingle)
			{
				backgroundRightSprite.spriteName = "battle_talent_bonus_bg_r_{0}".Format(_talentBonusIds.ElementAt(1));
			}
			singleSymbolSprite.SetActive(isSingle);
			doubleSymbolLeftSprite.SetActive(!isSingle);
			doubleSymbolRightSprite.SetActive(!isSingle);
			if (isSingle)
			{
				singleSymbolSprite.spriteName = "battle_talent_bonus_{0}".Format(_talentBonusIds.First());
			}
			else
			{
				doubleSymbolLeftSprite.spriteName = "battle_talent_bonus_{0}".Format(_talentBonusIds.ElementAt(0));
				doubleSymbolRightSprite.spriteName = "battle_talent_bonus_{0}".Format(_talentBonusIds.ElementAt(1));
			}
		}

		private void setEffect(List<int> _talentBonusIds)
		{
			if (_talentBonusIds.Count == 1)
			{
				GameObject effect = ManagerSingleton<ResourceManager>.Instance.LoadImmediately(
					eResourceId.ALL_EF_KNIGHT_ENHANCE_TALENT_LEVEL_ENHANCE,
					singleEffectParentTransform, _talentBonusIds.First());
				effectList.Add(effect.gameObject.GetComponentInChildren<ParticleSystem>());
			}
			else
			{
				GameObject leftEffect = ManagerSingleton<ResourceManager>.Instance.LoadImmediately(
					eResourceId.ALL_EF_KNIGHT_ENHANCE_TALENT_LEVEL_ENHANCE,
					doubleLeftEffectParentTransform, _talentBonusIds.ElementAt(0));
				GameObject rightEffect = ManagerSingleton<ResourceManager>.Instance.LoadImmediately(
					eResourceId.ALL_EF_KNIGHT_ENHANCE_TALENT_LEVEL_ENHANCE,
					doubleRightEffectParentTransform, _talentBonusIds.ElementAt(1));
				ParticleSystem leftParticle = leftEffect.gameObject.GetComponentInChildren<ParticleSystem>();
				ParticleSystem rightParticle = rightEffect.gameObject.GetComponentInChildren<ParticleSystem>();
				effectList.Add(leftParticle);
				effectList.Add(rightParticle);
			}
			effectList.ForEach(effect =>
			{
				ParticleSystem.MainModule main = effect.main;
				main.playOnAwake = true;
			});
		}
	}
}
