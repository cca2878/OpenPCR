using global::OpenPCR.Runtime.Api;
namespace OpenPCR.Runtime.Api
{
	public sealed class Mesh : global::OpenPCR.Runtime.Api.Object
	{
		[global::OpenPCR.Migration("9a744977-d238-4f32-9a4f-936c44b55999")]
		public Mesh()
		{
			throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Mesh::.ctor()", "Unity infrastructure not implemented");
		}
		[global::OpenPCR.Migration("17a9cbe1-75c4-4de9-ba32-58384c6c7d8a")]
		public void SetTriangles(int[] triangles, int submesh, bool calculateBounds)
		{
			throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Mesh::SetTriangles(System.Int32[],System.Int32,System.Boolean):System.Void", "Unity infrastructure not implemented");
		}
		public int subMeshCount
		{
			[global::OpenPCR.Migration("dcd24e67-db0f-448e-aceb-14fcbcb3ac61")]
			get
			{
				throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Mesh::get_subMeshCount():System.Int32", "Unity infrastructure not implemented");
			}
			[global::OpenPCR.Migration("e113fb49-aa65-48ed-b2cb-2053b0db9271")]
			set
			{
				throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Mesh::set_subMeshCount(System.Int32):System.Void", "Unity infrastructure not implemented");
			}
		}
		public global::OpenPCR.Runtime.Api.Bounds bounds
		{
			[global::OpenPCR.Migration("a9d43112-ae52-4c77-80ea-587e2267589e")]
			get
			{
				throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Mesh::get_bounds():OpenPCR.Runtime.Api.Bounds", "Unity infrastructure not implemented");
			}
			[global::OpenPCR.Migration("7209bcef-731f-4eeb-ac27-ba9c12ac50af")]
			set
			{
				throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Mesh::set_bounds(OpenPCR.Runtime.Api.Bounds):System.Void", "Unity infrastructure not implemented");
			}
		}
		public global::OpenPCR.Runtime.Api.Vector3[] vertices
		{
			[global::OpenPCR.Migration("a8e3a958-ab73-481a-8101-38884b8f0b97")]
			get
			{
				throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Mesh::get_vertices():OpenPCR.Runtime.Api.Vector3[]", "Unity infrastructure not implemented");
			}
			[global::OpenPCR.Migration("a15302b1-9d94-4be8-b5fd-013ca40b20b0")]
			set
			{
				throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Mesh::set_vertices(OpenPCR.Runtime.Api.Vector3[]):System.Void", "Unity infrastructure not implemented");
			}
		}
		public global::OpenPCR.Runtime.Api.Vector3[] normals
		{
			[global::OpenPCR.Migration("81288d4f-a97a-44b3-a70d-83328874d8ff")]
			get
			{
				throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Mesh::get_normals():OpenPCR.Runtime.Api.Vector3[]", "Unity infrastructure not implemented");
			}
			[global::OpenPCR.Migration("5562f98e-f847-4a2f-8d67-f4f83722cb9e")]
			set
			{
				throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Mesh::set_normals(OpenPCR.Runtime.Api.Vector3[]):System.Void", "Unity infrastructure not implemented");
			}
		}
		public global::OpenPCR.Runtime.Api.Vector4[] tangents
		{
			[global::OpenPCR.Migration("ec0fa55f-62d3-4e6e-868a-a585e88afb91")]
			get
			{
				throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Mesh::get_tangents():OpenPCR.Runtime.Api.Vector4[]", "Unity infrastructure not implemented");
			}
			[global::OpenPCR.Migration("c2392331-be1c-4f81-ae57-a0b4dee43706")]
			set
			{
				throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Mesh::set_tangents(OpenPCR.Runtime.Api.Vector4[]):System.Void", "Unity infrastructure not implemented");
			}
		}
		public global::OpenPCR.Runtime.Api.Vector2[] uv
		{
			[global::OpenPCR.Migration("f0720554-1ad4-445f-a336-7861f9a34430")]
			get
			{
				throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Mesh::get_uv():OpenPCR.Runtime.Api.Vector2[]", "Unity infrastructure not implemented");
			}
			[global::OpenPCR.Migration("f05d1c05-a910-4709-83b3-6de5d75a7c4d")]
			set
			{
				throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Mesh::set_uv(OpenPCR.Runtime.Api.Vector2[]):System.Void", "Unity infrastructure not implemented");
			}
		}
		public global::OpenPCR.Runtime.Api.Vector2[] uv2
		{
			[global::OpenPCR.Migration("228c0628-ab89-4053-b4bc-c94a92ddeb72")]
			get
			{
				throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Mesh::get_uv2():OpenPCR.Runtime.Api.Vector2[]", "Unity infrastructure not implemented");
			}
			[global::OpenPCR.Migration("9cfedb75-ecdd-495c-afa2-0270b1865033")]
			set
			{
				throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Mesh::set_uv2(OpenPCR.Runtime.Api.Vector2[]):System.Void", "Unity infrastructure not implemented");
			}
		}
		public global::OpenPCR.Runtime.Api.Vector2[] uv3
		{
			[global::OpenPCR.Migration("598c1425-7399-4934-93ab-deac493648e3")]
			get
			{
				throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Mesh::get_uv3():OpenPCR.Runtime.Api.Vector2[]", "Unity infrastructure not implemented");
			}
			[global::OpenPCR.Migration("8e8c0fc4-3ee8-429e-ade7-9adf43142d72")]
			set
			{
				throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Mesh::set_uv3(OpenPCR.Runtime.Api.Vector2[]):System.Void", "Unity infrastructure not implemented");
			}
		}
		public global::OpenPCR.Runtime.Api.Color32[] colors32
		{
			[global::OpenPCR.Migration("75a58af0-92fc-403c-8950-d474bbac0d03")]
			get
			{
				throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Mesh::get_colors32():OpenPCR.Runtime.Api.Color32[]", "Unity infrastructure not implemented");
			}
			[global::OpenPCR.Migration("d1bbd975-0811-48b3-b9bf-ea5449006e93")]
			set
			{
				throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Mesh::set_colors32(OpenPCR.Runtime.Api.Color32[]):System.Void", "Unity infrastructure not implemented");
			}
		}
	}
}
