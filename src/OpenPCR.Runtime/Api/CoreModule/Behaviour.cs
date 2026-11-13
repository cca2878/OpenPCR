using global::OpenPCR.Runtime.Api;
using global::OpenPCR.Runtime.Impl;
namespace OpenPCR.Runtime.Api
{
	public class Behaviour : global::OpenPCR.Runtime.Api.Component
	{
		[global::OpenPCR.Migration("0d5c7110-50fd-4f8e-8fe5-d3507c4b19b0")]
		public Behaviour()
		{

		}
		public bool enabled
		{
			[global::OpenPCR.Migration("03797978-e518-4f55-bc1d-b44638d05bb6")]
			get
			{
				return global::OpenPCR.Runtime.Impl.PreparedSceneOperations.Enabled(this);
			}
			[global::OpenPCR.Migration("7b6063e0-dc48-48b4-a6ea-f62a3d7977c6")]
			set
			{
				global::OpenPCR.Runtime.Impl.PreparedSceneOperations.SetEnabled(this, value);
			}
		}
		public bool isActiveAndEnabled
		{
			[global::OpenPCR.Migration("1a1e83e6-7077-4918-8efd-6393d91e3704")]
			get
			{
				return global::OpenPCR.Runtime.Impl.SceneScripting.IsActiveAndEnabled(this);
			}
		}
	}
}
