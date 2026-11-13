using global::OpenPCR.DbStubs;

namespace Elements.Data
{
	public class MasterInventoryDatabase : AbstractMasterDatabase
	{
		private PreparedQuery _selectQuery_masterExEquipmentData;
		public PreparedQuery GetSelectQuery_ExEquipmentData()
		{
			if (_selectQuery_masterExEquipmentData != null)
				return _selectQuery_masterExEquipmentData;
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterInventoryDatabase::GetSelectQuery_ExEquipmentData():OpenPCR.DbStubs.PreparedQuery: runtime capability is unavailable.");
		}

		private global::OpenPCR.DbStubs.PreparedQuery _selectQuery_masterItemData;
		public global::OpenPCR.DbStubs.PreparedQuery GetSelectQuery_ItemData()
		{
			if (_selectQuery_masterItemData != null)
				return _selectQuery_masterItemData;
			throw new OpenPCR.OutOfBoundaryException("Elements.Data.MasterInventoryDatabase::GetSelectQuery_ItemData():OpenPCR.DbStubs.PreparedQuery: runtime capability is unavailable.");
		}

		public global::OpenPCR.DbStubs.Query GetSelectAllQuery_ItemData()
		{
			throw new OpenPCR.OutOfBoundaryException("Elements.Data.MasterInventoryDatabase::GetSelectAllQuery_ItemData():OpenPCR.DbStubs.Query: runtime capability is unavailable.");
		}

		public MasterInventoryDatabase(global::OpenPCR.DbStubs.DBProxy dbProxy)
		{
			throw new OpenPCR.OutOfBoundaryException("Elements.Data.MasterInventoryDatabase::.ctor(OpenPCR.DbStubs.DBProxy): runtime capability is unavailable.");
		}

		public override void Unload()
		{
			throw new OpenPCR.OutOfBoundaryException("Elements.Data.MasterInventoryDatabase::Unload():System.Void: persistent SQLite database lifecycle");
		}

		public override global::OpenPCR.DbStubs.Query Query(string sql)
		{
			throw new OpenPCR.OutOfBoundaryException("Elements.Data.MasterInventoryDatabase::Query(System.String):OpenPCR.DbStubs.Query: runtime capability is unavailable.");
		}
	}
}
