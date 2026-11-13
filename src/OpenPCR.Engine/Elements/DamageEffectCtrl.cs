using System.Collections.Generic;
using global::OpenPCR.Runtime.Api;

namespace Elements
{
	public class DamageEffectCtrl : DamageEffectCtrlBase
	{
		protected float deltaX = 33.75f;
		[global::OpenPCR.Migration("4fd1c2c5-835e-4c37-bfac-921e4af710c7")]
		public DamageEffectCtrl()
		{

		}
		[global::OpenPCR.Migration("90af4f77-5813-4cdf-9a19-892f054b1d41")]
		public virtual void SetDamageText(string _damageStr, eDamageEffectColor _color, List<int> _talentIds)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.DamageEffectCtrl::SetDamageText: digit and talent-icon renderer writes only; number-effect lifetime is owned by DamageEffectCtrlBase.ResetParameter/updateAnimation");
		}
	}
}
