using System.Runtime.CompilerServices;

namespace Elements
{
	public class UnitDamageInfo
	{

		[global::OpenPCR.Migration("1e5f1c56-0c1e-4180-85e4-977e6ea58f3c")]
		public void SetDamage(long _damage)
		{
			damage = _damage;
		}

		public long viewer_id { get; private set; }
		public int unit_id { get; private set; }
		public long damage { get; private set; }
		public int rarity { get; private set; }
		public SkinData skin_data { get; private set; }

		[global::OpenPCR.Migration("a2a06cc1-a909-400b-afde-34e72065b60e")]
		public UnitDamageInfo()
		{
			initializeUnitDamageInfo();
		}

		[global::OpenPCR.Migration("d08a46ce-4b63-44c9-af6a-ebb3a27845a9")]
		public void SetUnitId(int _unitId)
		{
			unit_id = _unitId;
		}

		[global::OpenPCR.Migration("e46b1cb1-dd54-4f4b-a9bb-30d413169b89")]
		public void SetViewerId(long _viewerId)
		{
			viewer_id = _viewerId;
		}

		[global::OpenPCR.Migration("04968e42-e9b1-473f-aa77-f8b1744592bc")]
		public void SetRarity(int _rarity)
		{
			rarity = _rarity;
		}

		[global::OpenPCR.Migration("49c8b579-0120-4306-aeb4-b237a59ae86f")]
		public void SetSkinData(SkinData _skinData)
		{
			skin_data = _skinData;
		}

		[global::OpenPCR.Migration("192e2518-f68b-43e8-86d7-636a0f14af97")]
		private void initializeUnitDamageInfo()
		{
			viewer_id = 0;
			unit_id = 0;
			damage = 0;
			rarity = 0;
			skin_data = null;
		}
	}
}
