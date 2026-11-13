namespace Elements
{
	public class RestrictionExtraEquip
	{
		public global::System.Int32 SerialId { get; private set; }
		public global::System.Int32 UnitId { get; private set; }
		[global::OpenPCR.Migration("9dc50d05-ab9c-4bef-a3a7-41935e59c53b")]
		public RestrictionExtraEquip(int _serialId, int _unitId)
		{
			SerialId = _serialId;
			UnitId = _unitId;
		}
	}
}
