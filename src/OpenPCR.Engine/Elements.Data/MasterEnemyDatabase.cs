using global::OpenPCR.DbStubs;
using System.Runtime.CompilerServices;

namespace Elements.Data
{
	public class MasterEnemyDatabase : AbstractMasterDatabase
	{
		private PreparedQuery _selectQuery_masterEnemyEnableVoice;
		public PreparedQuery GetSelectQuery_EnemyEnableVoice()
		{
			if (_selectQuery_masterEnemyEnableVoice != null)
				return _selectQuery_masterEnemyEnableVoice;
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterEnemyDatabase::GetSelectQuery_EnemyEnableVoice():OpenPCR.DbStubs.PreparedQuery: runtime capability is unavailable.");
		}

		private global::OpenPCR.DbStubs.PreparedQuery _selectQuery_masterEnemyIgnoreSkillRf;
		public global::OpenPCR.DbStubs.PreparedQuery GetSelectQuery_EnemyIgnoreSkillRf()
		{
			if (_selectQuery_masterEnemyIgnoreSkillRf != null)
				return _selectQuery_masterEnemyIgnoreSkillRf;
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterEnemyDatabase::GetSelectQuery_EnemyIgnoreSkillRf():OpenPCR.DbStubs.PreparedQuery: runtime capability is unavailable.");
		}

		public MasterEnemyDatabase(DBProxy dbProxy)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterEnemyDatabase::.ctor(OpenPCR.DbStubs.DBProxy): runtime capability is unavailable.");
		}

		public override void Unload()
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterEnemyDatabase::Unload():System.Void: Behavior implementation is not available.");
		}

		public override Query Query(string sql)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterEnemyDatabase::Query(System.String):OpenPCR.DbStubs.Query: runtime capability is unavailable.");
		}
	}
}
