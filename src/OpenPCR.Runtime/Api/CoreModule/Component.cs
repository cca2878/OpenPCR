using global::OpenPCR.Runtime.Api;
using global::OpenPCR.Runtime.Impl;
namespace OpenPCR.Runtime.Api
{
	public class Component : global::OpenPCR.Runtime.Api.Object
	{
		[global::OpenPCR.Migration("cfa869af-ebf4-450d-be7e-b5dcb321f9ac")]
		public Component()
		{

		}
		[global::OpenPCR.Migration("9d2f8290-7bd6-4aba-9409-282c17922bb3")]
		public T GetComponent<T>()
		{
			return gameObject.GetComponent<T>();
		}
		[global::OpenPCR.Migration("ca94e01a-d5c0-498a-870a-b4973813048b")]
		public T[] GetComponentsInChildren<T>()
		{
			throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Component::GetComponentsInChildren`1():!!0[]", "Unity infrastructure not implemented");
		}
		public global::OpenPCR.Runtime.Api.Transform transform
		{
			[global::OpenPCR.Migration("865950b7-4387-4c33-8971-e6fb6dfd9e8a")]
			get
			{
				return (global::OpenPCR.Runtime.Api.Transform)global::OpenPCR.Runtime.Impl.SceneScripting.Transform(this);
			}
		}
		public global::OpenPCR.Runtime.Api.GameObject gameObject
		{
			[global::OpenPCR.Migration("7edd9d0a-5682-4670-a327-652d1930b010")]
			get
			{
				return (global::OpenPCR.Runtime.Api.GameObject)global::OpenPCR.Runtime.Impl.SceneScripting.GameObject(this);
			}
		}
	}
}
