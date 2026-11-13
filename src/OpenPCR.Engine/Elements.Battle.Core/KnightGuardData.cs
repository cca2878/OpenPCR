namespace Elements.Battle.Core
{
	public class KnightGuardData
	{
		public long Value;
		public float LifeTime;
		public KnightGuardAction.eKnightGuardType KnightGuardType = KnightGuardAction.eKnightGuardType.HEAL;
		public SkillEffectCtrl Effect;
		public NormalSkillEffect ExecEffectData;
		public eStateIconType StateIconType = eStateIconType.INVALID_VALUE;
		public eInhibitHealType InhibitHealType = global::Elements.Battle.Core.eInhibitHealType.NO_EFFECT;
		public Skill Skill;
		public UnitCtrl Source;
		[global::OpenPCR.Migration("4b1dcb7a-5707-408a-bfeb-0975a5923261")]
		public KnightGuardData()
		{

		}
	}
}
