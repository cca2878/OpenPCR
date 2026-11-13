namespace Elements
{
	public class RewardData
	{
		public int Id;
		public int Type;
		public int Count;
		public bool IsRare;
		[global::OpenPCR.Migration("d29a5999-cdb6-4ccd-a3de-4cf0eaf2827e")]
		public RewardData(int _id, int _type, int _count, bool _isRare = false)
		{
			Id = _id;
			Type = _type;
			Count = _count;
			IsRare = _isRare;
		}
	}
}
