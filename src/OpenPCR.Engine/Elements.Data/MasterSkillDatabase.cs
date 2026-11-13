using global::OpenPCR.DbStubs;
using System.Runtime.CompilerServices;

namespace Elements.Data
{
	public class MasterSkillDatabase : AbstractMasterDatabase
	{
		private PreparedQuery _selectQuery_masterSpskillLvInitializeData;
		public PreparedQuery GetSelectQuery_SpskillLvInitializeData()
		{
			if (_selectQuery_masterSpskillLvInitializeData != null)
				return _selectQuery_masterSpskillLvInitializeData;
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterSkillDatabase::GetSelectQuery_SpskillLvInitializeData():OpenPCR.DbStubs.PreparedQuery: runtime capability is unavailable.");
		}

		private global::OpenPCR.DbStubs.PreparedQuery _indexedSelectQuery_unitSkillDataRf_rfSkillId;
		public global::OpenPCR.DbStubs.PreparedQuery GetSelectQueryWithIndex_UnitSkillDataRf_RfSkillId()
		{
			if (_indexedSelectQuery_unitSkillDataRf_rfSkillId != null)
				return _indexedSelectQuery_unitSkillDataRf_rfSkillId;
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterSkillDatabase::GetSelectQueryWithIndex_UnitSkillDataRf_RfSkillId():OpenPCR.DbStubs.PreparedQuery: runtime capability is unavailable.");
		}

		private global::OpenPCR.DbStubs.PreparedQuery _indexedSelectQuery_unitSkillDataRf_skillId;
		public global::OpenPCR.DbStubs.PreparedQuery GetSelectQueryWithIndex_UnitSkillDataRf_SkillId()
		{
			if (_indexedSelectQuery_unitSkillDataRf_skillId != null)
				return _indexedSelectQuery_unitSkillDataRf_skillId;
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterSkillDatabase::GetSelectQueryWithIndex_UnitSkillDataRf_SkillId():OpenPCR.DbStubs.PreparedQuery: runtime capability is unavailable.");
		}

		private global::OpenPCR.DbStubs.PreparedQuery _selectQuery_masterUnitSkillDataRf;
		public global::OpenPCR.DbStubs.PreparedQuery GetSelectQuery_UnitSkillDataRf()
		{
			if (_selectQuery_masterUnitSkillDataRf != null)
				return _selectQuery_masterUnitSkillDataRf;
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterSkillDatabase::GetSelectQuery_UnitSkillDataRf():OpenPCR.DbStubs.PreparedQuery: runtime capability is unavailable.");
		}

		public MasterSkillDatabase(DBProxy dbProxy)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterSkillDatabase::.ctor(OpenPCR.DbStubs.DBProxy): runtime capability is unavailable.");
		}

		public override void Unload()
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterSkillDatabase::Unload():System.Void");
		}

		public override Query Query(string sql)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterSkillDatabase::Query(System.String):OpenPCR.DbStubs.Query: runtime capability is unavailable.");
		}

		private PreparedQuery _selectQuery_masterMetamorphose;
		public PreparedQuery GetSelectQuery_Metamorphose()
		{
			if (_selectQuery_masterMetamorphose != null)
				return _selectQuery_masterMetamorphose;
			// Creating a live SQLite prepared query requires the external runtime database provider.
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterSkillDatabase::GetSelectQuery_Metamorphose():OpenPCR.DbStubs.PreparedQuery: runtime capability is unavailable.");
		}

		private PreparedQuery _indexedSelectQuery_metamorphose_typeId;
		public PreparedQuery GetSelectQueryWithIndex_Metamorphose_TypeId()
		{
			if (_indexedSelectQuery_metamorphose_typeId != null)
				return _indexedSelectQuery_metamorphose_typeId;
			// Creating a live SQLite prepared query requires the external runtime database provider.
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterSkillDatabase::GetSelectQueryWithIndex_Metamorphose_TypeId():OpenPCR.DbStubs.PreparedQuery: runtime capability is unavailable.");
		}
	}
}
