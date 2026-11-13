using global::OpenPCR.DbStubs;

namespace Elements.Data
{
	public class MasterTalentDatabase : AbstractMasterDatabase
	{
		private global::OpenPCR.DbStubs.PreparedQuery _indexedSelectQuery_talentFormationOverwrite_contentType_targetValue1;
		public global::OpenPCR.DbStubs.PreparedQuery GetSelectQueryWithIndex_TalentFormationOverwrite_ContentType_TargetValue1()
		{
			if (_indexedSelectQuery_talentFormationOverwrite_contentType_targetValue1 != null)
				return _indexedSelectQuery_talentFormationOverwrite_contentType_targetValue1;
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterTalentDatabase::GetSelectQueryWithIndex_TalentFormationOverwrite_ContentType_TargetValue1(): Uncached Native SQLite prepared-query production requires the initialized master database connection.");
		}

		private PreparedQuery _indexedSelectQuery_unitTalent_unitId;
		public PreparedQuery GetSelectQueryWithIndex_UnitTalent_UnitId()
		{
			if (_indexedSelectQuery_unitTalent_unitId != null)
				return _indexedSelectQuery_unitTalent_unitId;
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterTalentDatabase::GetSelectQueryWithIndex_UnitTalent_UnitId():OpenPCR.DbStubs.PreparedQuery: runtime capability is unavailable.");
		}

		private PreparedQuery _selectQuery_masterUnitTalent;
		public PreparedQuery GetSelectQuery_UnitTalent()
		{
			if (_selectQuery_masterUnitTalent != null)
				return _selectQuery_masterUnitTalent;
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterTalentDatabase::GetSelectQuery_UnitTalent():OpenPCR.DbStubs.PreparedQuery: runtime capability is unavailable.");
		}

		private global::OpenPCR.DbStubs.PreparedQuery _selectQuery_masterTalentFormationBonus;
		public global::OpenPCR.DbStubs.PreparedQuery GetSelectQuery_TalentFormationBonus()
		{
			if (_selectQuery_masterTalentFormationBonus != null)
				return _selectQuery_masterTalentFormationBonus;
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterTalentDatabase::GetSelectQuery_TalentFormationBonus():OpenPCR.DbStubs.PreparedQuery: runtime capability is unavailable.");
		}

		public MasterTalentDatabase(DBProxy dbProxy)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterTalentDatabase::.ctor(OpenPCR.DbStubs.DBProxy): runtime capability is unavailable.");
		}

		public override void Unload()
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterTalentDatabase::Unload():System.Void: database producer boundary");
		}

		public override Query Query(string sql)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterTalentDatabase::Query(System.String):OpenPCR.DbStubs.Query: runtime capability is unavailable.");
		}
	}
}
