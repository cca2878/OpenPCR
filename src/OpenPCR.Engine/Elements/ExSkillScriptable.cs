using global::OpenPCR.Runtime.Api;
using Elements.Battle.Core;

namespace Elements
{
	public class ExSkillScriptable : ScriptableObject
	{
		public AnimationCurve KnockAnimationCurve;
		public AnimationCurve KnockDownAnimationCurve;
		public eEffectType EffectType;
		public ExSkillScriptable()
		{
			throw new global::OpenPCR.MigrationPendingException("Elements.ExSkillScriptable::.ctor()", "body migration pending");
		}
	}
}
