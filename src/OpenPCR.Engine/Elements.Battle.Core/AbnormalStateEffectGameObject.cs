using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class AbnormalStateEffectGameObject
	{
		public global::Elements.SkillEffectCtrl RightEffect { get; set; }
		public global::Elements.SkillEffectCtrl LeftEffect { get; set; }

		[global::OpenPCR.Migration("707bcb47-19db-4c50-8e77-d8d81bbf64ba")]
		public AbnormalStateEffectGameObject()
		{
		}
	}
}
