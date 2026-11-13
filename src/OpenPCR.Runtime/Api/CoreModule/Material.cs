using global::OpenPCR.Runtime.Api;
namespace OpenPCR.Runtime.Api
{
	public class Material : global::OpenPCR.Runtime.Api.Object
	{
		[global::OpenPCR.Migration("da74a81c-881e-40d4-82c3-6ced38509364")]
		public Material(global::OpenPCR.Runtime.Api.Shader shader)
		{
			throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Material::.ctor(OpenPCR.Runtime.Api.Shader)", "Unity infrastructure not implemented");
		}
		[global::OpenPCR.Migration("c57fed7e-e24c-4142-96c2-1f9acf0a154f")]
		public void SetTexture(string name, global::OpenPCR.Runtime.Api.Texture value)
		{
			global::OpenPCR.Presentation.Noop();
		}
		public global::OpenPCR.Runtime.Api.Shader shader
		{
			[global::OpenPCR.Migration("6382699f-8eca-45de-b598-49fd96688f6a")]
			get
			{
				throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Material::get_shader():OpenPCR.Runtime.Api.Shader", "Unity infrastructure not implemented");
			}
			[global::OpenPCR.Migration("12d19a3f-d0bf-44bd-9b8f-5a96168be325")]
			set
			{
				global::OpenPCR.Presentation.Noop();
			}
		}
		public global::OpenPCR.Runtime.Api.Color color
		{
			[global::OpenPCR.Migration("941f1ab9-8ae8-4454-9b23-035e5bbdbddc")]
			get
			{
				throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Material::get_color():OpenPCR.Runtime.Api.Color", "Unity infrastructure not implemented");
			}
			[global::OpenPCR.Migration("518ad7e8-155a-4a56-abe0-9558f5431056")]
			set
			{
				throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Material::set_color(OpenPCR.Runtime.Api.Color):System.Void", "Unity infrastructure not implemented");
			}
		}
		public global::OpenPCR.Runtime.Api.Texture mainTexture
		{
			[global::OpenPCR.Migration("ea960415-d3e2-4090-8e0f-69cf481d5594")]
			get
			{
				throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Material::get_mainTexture():OpenPCR.Runtime.Api.Texture", "Unity infrastructure not implemented");
			}
			[global::OpenPCR.Migration("22d095fe-de69-4dd1-bdf2-8c67f4853d07")]
			set
			{
				global::OpenPCR.Presentation.Noop();
			}
		}
	}
}
