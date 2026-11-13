namespace Elements
{
	public class SupportUnitDefine : object
	{
		public class QuestStartParam
		{

			public void Init() { UnitStatus = null; ViewerId = 0L; UnitId = 0; BattleRarity = 0; IsFriend = 0; }


			public int UnitId { get; set; }

			public int BattleRarity { get; set; }

			public int IsFriend { get; set; }

			public global::Elements.SupportUnitStatus UnitStatus { get; set; }


			public long ViewerId { get; set; }

			public QuestStartParam()
			{
				Init();
			}
		}
		public SupportUnitDefine()
		{
			throw new global::OpenPCR.MigrationPendingException("Elements.SupportUnitDefine::.ctor():System.Void", "Native consumer declaration; behavior evidence pending");
		}
	}
}
