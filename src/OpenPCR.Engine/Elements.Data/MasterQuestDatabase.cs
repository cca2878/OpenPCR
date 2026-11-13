using global::OpenPCR.DbStubs;
using System.Runtime.CompilerServices;

namespace Elements.Data
{
	public class MasterQuestDatabase : AbstractMasterDatabase
	{

		private global::OpenPCR.DbStubs.PreparedQuery _selectQuery_masterRarity6QuestData;

		public global::OpenPCR.DbStubs.PreparedQuery GetSelectQuery_Rarity6QuestData()
		{
			if (_selectQuery_masterRarity6QuestData != null) return _selectQuery_masterRarity6QuestData;
			throw new OpenPCR.OutOfBoundaryException("Elements.Data.MasterQuestDatabase::GetSelectQuery_Rarity6QuestData():OpenPCR.DbStubs.PreparedQuery: runtime capability is unavailable.");
		}

		private global::OpenPCR.DbStubs.PreparedQuery _indexedSelectQuery_rarity6QuestData_rarity6QuestId;

		public global::OpenPCR.DbStubs.PreparedQuery GetSelectQueryWithIndex_Rarity6QuestData_Rarity6QuestId()
		{
			if (_indexedSelectQuery_rarity6QuestData_rarity6QuestId != null) return _indexedSelectQuery_rarity6QuestData_rarity6QuestId;
			throw new OpenPCR.OutOfBoundaryException("Elements.Data.MasterQuestDatabase::GetSelectQueryWithIndex_Rarity6QuestData_Rarity6QuestId():OpenPCR.DbStubs.PreparedQuery: runtime capability is unavailable.");
		}

		public MasterQuestDatabase(DBProxy dbProxy)
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterQuestDatabase::.ctor(OpenPCR.DbStubs.DBProxy): runtime capability is unavailable."); }
		public override void Unload()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterQuestDatabase::Unload():System.Void" + ": persistent SQLite runtime backend"); }
		public override Query Query(string sql)
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterQuestDatabase::Query(System.String):OpenPCR.DbStubs.Query: runtime capability is unavailable."); }
	}

}
