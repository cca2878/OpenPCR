using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class OverwriteAbnormalStateData
	{
		public BasePartsData TargetParts { get; private set; }
		public UnitCtrl Source { get; private set; }
		public Skill Skill { get; private set; }
		public ActionParameter ActionParameter { get; private set; }
		public eOverwriteAbnormalState AbnormalState { get; private set; }
		public double EffectValue { get; private set; }

		[global::OpenPCR.Migration("243a7777-fcc8-48f6-8436-530246575cee")]
		public OverwriteAbnormalStateData(BasePartsData _parts, UnitCtrl _source, Skill _skill, ActionParameter _actionParameter, eOverwriteAbnormalState _abnormalState, double _effectValue)
		{
			TargetParts = _parts;
			Source = _source;
			Skill = _skill;
			ActionParameter = _actionParameter;
			AbnormalState = _abnormalState;
			EffectValue = _effectValue;
		}
	}
}
