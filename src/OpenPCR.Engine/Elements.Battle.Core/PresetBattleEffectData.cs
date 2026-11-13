using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class PresetBattleEffectData
	{
		public global::System.Collections.Generic.List<Elements.ShakeEffect> GameStartShakes;
		public global::System.Collections.Generic.List<Elements.PrefabWithTime> GameStartEffects;
		public global::System.Collections.Generic.List<Elements.PrefabWithTime> DieEffects;
		public global::System.Collections.Generic.List<Elements.ShakeEffect> DieShakes;
		public global::System.Collections.Generic.List<Elements.PrefabWithTime> DamageEffects;
		public global::System.Collections.Generic.List<Elements.ShakeEffect> DamageShakes;
		public global::System.Collections.Generic.List<Elements.PrefabWithTime> SummonEffects;
		public global::System.Collections.Generic.List<Elements.PrefabWithTime> IdleEffects;
		public global::System.Collections.Generic.List<Elements.PrefabWithTime> AuraEffects;
		[global::OpenPCR.Migration("fe810684-f86d-4897-b7e1-84491e3fc397")]
		public PresetBattleEffectData(List<ShakeEffect> _gameStartShakes, List<PrefabWithTime> _gameStartEffects, List<PrefabWithTime> _dieEffects, List<ShakeEffect> _dieShakes, List<PrefabWithTime> _damageEffects, List<ShakeEffect> _damageShakes, List<PrefabWithTime> _summonEffects, List<PrefabWithTime> _idleEffects, List<PrefabWithTime> _auraEffects)
		{
			GameStartShakes = _gameStartShakes;
			GameStartEffects = _gameStartEffects;
			DieEffects = _dieEffects;
			DieShakes = _dieShakes;
			DamageEffects = _damageEffects;
			DamageShakes = _damageShakes;
			SummonEffects = _summonEffects;
			IdleEffects = _idleEffects;
			AuraEffects = _auraEffects;
		}
	}
}
