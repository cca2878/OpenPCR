using System.Collections.Generic;
namespace Elements
{
	public class eDamageEffectType_DictComparer : IEqualityComparer<eDamageEffectType>
	{
		[global::OpenPCR.Migration("103359b8-b33b-4469-90c3-4d6d1c95143b")]
		public eDamageEffectType_DictComparer() { }
		[global::OpenPCR.Migration("9e2c9db2-4755-46e8-807e-54c75a804996")]
		public bool Equals(eDamageEffectType _x, eDamageEffectType _y) => _x == _y;
		[global::OpenPCR.Migration("af780811-32af-42af-a5e1-17ca43dfb1a6")]
		public int GetHashCode(eDamageEffectType _obj) => (int)_obj;
	}
}
