using global::OpenPCR.DbStubs;
namespace OpenPCR.DbStubs
{

	public unsafe class @Connection : global::System.IDisposable
	{


		public @Connection() { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }


		public virtual void @Dispose() { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }
	}
}
namespace OpenPCR.DbStubs
{

	public unsafe class @DBProxy : global::OpenPCR.DbStubs.Connection
	{


		public @DBProxy() { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }
	}
}
namespace OpenPCR.DbStubs
{

	public unsafe class @PreparedQuery : global::OpenPCR.DbStubs.Query
	{


		public @PreparedQuery(global::OpenPCR.DbStubs.Connection @conn, string @sql) : base(default(global::OpenPCR.DbStubs.Connection), default(string)) { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }


		public bool @BindInt(int @idx, int @iValue) { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }


		public bool @Reset() { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }
	}
}
namespace OpenPCR.DbStubs
{

	public unsafe class @Query : global::System.IDisposable
	{


		public @Query(global::OpenPCR.DbStubs.Connection @conn, string @sql) { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }


		public virtual void @Dispose() { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }


		public bool @Step() { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }


		public int @GetInt(int @idx) { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }


		public double @GetDouble(int @idx) { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }


		public string @GetText(int @idx) { throw new global::System.InvalidOperationException("Unsupported runtime capability."); }
	}
}
