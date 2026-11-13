namespace Elements.Battle.Core
{
	public abstract class UnitComponentBase
	{
		protected readonly UnitCtrl unitController;
		[global::OpenPCR.Migration("237e379c-75a9-44ac-887a-1e5f96567ab5")]
		public UnitComponentBase(UnitCtrl _unitController)
		{
			unitController = _unitController;
		}
	}
}
