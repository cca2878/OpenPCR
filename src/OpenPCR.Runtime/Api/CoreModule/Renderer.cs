using global::OpenPCR.Runtime.Api;
namespace OpenPCR.Runtime.Api
{
	public class Renderer : global::OpenPCR.Runtime.Api.Component
	{
		[global::OpenPCR.Migration("3ce5da27-c331-4057-9863-28cc08c2574d")]
		public Renderer()
		{
			throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Renderer::.ctor()", "Unity infrastructure not implemented");
		}
		[global::OpenPCR.Migration("7d3ac649-f4b6-4eb3-9308-4dcb291f1517")]
		public void SetPropertyBlock(global::OpenPCR.Runtime.Api.MaterialPropertyBlock properties)
		{
			global::OpenPCR.Presentation.Noop();
		}
		public string sortingLayerName
		{
			[global::OpenPCR.Migration("c12e99e0-4bb3-4da0-94c3-ff0f48b1e696")]
			get
			{
				throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Renderer::get_sortingLayerName():System.String", "Unity infrastructure not implemented");
			}
			[global::OpenPCR.Migration("17a8a541-5c8e-467f-84e5-d5b769739544")]
			set
			{
				throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Renderer::set_sortingLayerName(System.String):System.Void", "Unity infrastructure not implemented");
			}
		}
		public int sortingOrder
		{
			[global::OpenPCR.Migration("419d8286-38e1-42f5-97b3-2259fa7da447")]
			get
			{
				throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Renderer::get_sortingOrder():System.Int32", "Unity infrastructure not implemented");
			}
			[global::OpenPCR.Migration("3e2eeb1f-8015-4be7-88a8-09260ed1d9f5")]
			set
			{
				throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Renderer::set_sortingOrder(System.Int32):System.Void", "Unity infrastructure not implemented");
			}
		}
		public global::OpenPCR.Runtime.Api.Material[] materials
		{
			[global::OpenPCR.Migration("a9f7ed41-aef4-4dbd-91f4-53cf95a96104")]
			get
			{
				throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Renderer::get_materials():OpenPCR.Runtime.Api.Material[]", "Unity infrastructure not implemented");
			}
			[global::OpenPCR.Migration("7e91dabb-2003-4127-b295-5df826927d9b")]
			set
			{
				throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Renderer::set_materials(OpenPCR.Runtime.Api.Material[]):System.Void", "Unity infrastructure not implemented");
			}
		}
		public global::OpenPCR.Runtime.Api.Material material
		{
			[global::OpenPCR.Migration("92e12b17-e639-44f2-8521-39c22c53f9fb")]
			get
			{
				throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Renderer::get_material():OpenPCR.Runtime.Api.Material", "Unity infrastructure not implemented");
			}
			[global::OpenPCR.Migration("97456a0e-a9c3-4e80-b930-0fed0581122f")]
			set
			{
				throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Renderer::set_material(OpenPCR.Runtime.Api.Material):System.Void", "Unity infrastructure not implemented");
			}
		}
		public global::OpenPCR.Runtime.Api.Material sharedMaterial
		{
			[global::OpenPCR.Migration("71a17139-60a9-4392-936c-d503e1b024fa")]
			get
			{
				throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Renderer::get_sharedMaterial():OpenPCR.Runtime.Api.Material", "Unity infrastructure not implemented");
			}
			[global::OpenPCR.Migration("d3e524da-a177-420f-9db0-8be5c65c2098")]
			set
			{
				throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Renderer::set_sharedMaterial(OpenPCR.Runtime.Api.Material):System.Void", "Unity infrastructure not implemented");
			}
		}
	}
}
