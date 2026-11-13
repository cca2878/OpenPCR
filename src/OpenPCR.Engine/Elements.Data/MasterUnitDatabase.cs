using global::OpenPCR.DbStubs;
using System.Runtime.CompilerServices;

namespace Elements.Data
{
	public class MasterUnitDatabase : AbstractMasterDatabase
	{

		public MasterUnitDatabase(DBProxy dbProxy)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterUnitDatabase::.ctor(OpenPCR.DbStubs.DBProxy): runtime capability is unavailable.");
		}

		public override void Unload()
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterUnitDatabase::Unload():System.Void");
		}

		public override Query Query(string sql)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterUnitDatabase::Query(System.String):OpenPCR.DbStubs.Query: runtime capability is unavailable.");
		}

		private PreparedQuery _selectQuery_masterCharaIdentity;
		public PreparedQuery GetSelectQuery_CharaIdentity()
		{
			if (_selectQuery_masterCharaIdentity != null)
				return _selectQuery_masterCharaIdentity;
			// Creating a live SQLite prepared query requires the external runtime database provider.
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterUnitDatabase::GetSelectQuery_CharaIdentity():OpenPCR.DbStubs.PreparedQuery: runtime capability is unavailable.");
		}

		private PreparedQuery _selectQuery_masterExtraEffectUnitGroup;
		public PreparedQuery GetSelectQuery_ExtraEffectUnitGroup()
		{
			if (_selectQuery_masterExtraEffectUnitGroup != null)
				return _selectQuery_masterExtraEffectUnitGroup;
			// Creating a live SQLite prepared query requires the external runtime database provider.
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterUnitDatabase::GetSelectQuery_ExtraEffectUnitGroup():OpenPCR.DbStubs.PreparedQuery: runtime capability is unavailable.");
		}

		private PreparedQuery _indexedSelectQuery_extraEffectUnitGroup_groupId;
		public PreparedQuery GetSelectQueryWithIndex_ExtraEffectUnitGroup_GroupId()
		{
			if (_indexedSelectQuery_extraEffectUnitGroup_groupId != null)
				return _indexedSelectQuery_extraEffectUnitGroup_groupId;
			// Creating a live SQLite prepared query requires the external runtime database provider.
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterUnitDatabase::GetSelectQueryWithIndex_ExtraEffectUnitGroup_GroupId():OpenPCR.DbStubs.PreparedQuery: runtime capability is unavailable.");
		}

		private PreparedQuery _selectQuery_masterReferenceCharaTypeGroup;
		public PreparedQuery GetSelectQuery_ReferenceCharaTypeGroup()
		{
			if (_selectQuery_masterReferenceCharaTypeGroup != null)
				return _selectQuery_masterReferenceCharaTypeGroup;
			// Creating a live SQLite prepared query requires the external runtime database provider.
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterUnitDatabase::GetSelectQuery_ReferenceCharaTypeGroup():OpenPCR.DbStubs.PreparedQuery: runtime capability is unavailable.");
		}

		private PreparedQuery _indexedSelectQuery_referenceCharaTypeGroup_groupId;
		public PreparedQuery GetSelectQueryWithIndex_ReferenceCharaTypeGroup_GroupId()
		{
			if (_indexedSelectQuery_referenceCharaTypeGroup_groupId != null)
				return _indexedSelectQuery_referenceCharaTypeGroup_groupId;
			// Creating a live SQLite prepared query requires the external runtime database provider.
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterUnitDatabase::GetSelectQueryWithIndex_ReferenceCharaTypeGroup_GroupId():OpenPCR.DbStubs.PreparedQuery: runtime capability is unavailable.");
		}

		private PreparedQuery _indexedSelectQuery_spBattleVoice_unitId;
		public PreparedQuery GetSelectQueryWithIndex_SpBattleVoice_UnitId()
		{
			if (_indexedSelectQuery_spBattleVoice_unitId != null)
				return _indexedSelectQuery_spBattleVoice_unitId;
			// The native uncached path creates a prepared SQL query through the external runtime provider.
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterUnitDatabase::GetSelectQueryWithIndex_SpBattleVoice_UnitId():OpenPCR.DbStubs.PreparedQuery: runtime capability is unavailable.");
		}
	}
}
