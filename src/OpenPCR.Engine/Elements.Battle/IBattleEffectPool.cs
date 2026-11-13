using global::OpenPCR.Runtime.Api;

namespace Elements.Battle
{
	public interface IBattleEffectPool
	{
		bool ForceNormalSD { get; }

		SkillEffectCtrl GetEffect(GameObject _prefab, UnitCtrl _owner = null);

		DamageEffectCtrlBase LoadNumberEffect(GameObject _prefab, UnitCtrl _owner);

		void DisappearUnitNumberEffect(UnitCtrl _owner);
	}
}
