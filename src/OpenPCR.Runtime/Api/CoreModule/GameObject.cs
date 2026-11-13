using global::OpenPCR.Runtime.Api;
using global::OpenPCR.Runtime.Impl;
namespace OpenPCR.Runtime.Api
{
	public sealed class GameObject : global::OpenPCR.Runtime.Api.Object
	{
		[global::OpenPCR.Migration("0e01a0ba-872f-462c-a6f9-10840b1d6c04")]
		public GameObject()
		{
			Internal_CreateGameObject(this, null);
		}
		[global::OpenPCR.Migration("732e11dc-b784-4e12-b684-71699d82a8c0")]
		private static void Internal_CreateGameObject(GameObject self, string name)
		{
			global::OpenPCR.Runtime.Impl.PreparedSceneOperations.CreateEmpty(self, name);
		}
		[global::OpenPCR.Migration("f533a2ba-a72f-4997-bb2b-d7295ba8d52e")]
		public T GetComponent<T>()
		{
			return (T)global::OpenPCR.Runtime.Impl.SceneScripting.GetComponent(this, typeof(T));
		}
		[global::OpenPCR.Migration("7a952d2a-c7c1-490d-8f06-314d959ba90a")]
		public T GetComponentInChildren<T>()
		{
			return (T)global::OpenPCR.Runtime.Impl.SceneScripting.GetComponentInChildren(this, typeof(T));
		}
		[global::OpenPCR.Migration("a8c02a7d-e330-45e2-94e5-6441377d9ca5")]
		public T[] GetComponentsInChildren<T>(bool includeInactive)
		{
			throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.GameObject::GetComponentsInChildren`1(System.Boolean):!!0[]", "Unity infrastructure not implemented");
		}
		[global::OpenPCR.Migration("0cf85f7d-bc2e-4fce-a1a7-7e26deb3b3df")]
		public T[] GetComponentsInChildren<T>()
		{
			throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.GameObject::GetComponentsInChildren`1():!!0[]", "Unity infrastructure not implemented");
		}
		[global::OpenPCR.Migration("c214860a-9eba-4a94-97b6-a323687fad33")]
		public T AddComponent<T>() where T : global::OpenPCR.Runtime.Api.Component
		{
			return (T)global::OpenPCR.Runtime.Impl.SceneScripting.AddComponent(this, typeof(T));
		}
		[global::OpenPCR.Migration("826742ab-9908-4304-a8df-e0e39cf6df37")]
		public void SetActive(bool value)
		{
			global::OpenPCR.Runtime.Impl.PreparedSceneOperations.SetActive(this, value);
		}
		[global::OpenPCR.Migration("7089b5cd-b3e5-494b-8c7f-319fe2f4847a")]
		public void SendMessage(string methodName, object value, global::OpenPCR.Runtime.Api.SendMessageOptions options)
		{
			throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.GameObject::SendMessage(System.String,System.Object,OpenPCR.Runtime.Api.SendMessageOptions):System.Void", "Unity infrastructure not implemented");
		}
		public global::OpenPCR.Runtime.Api.Transform transform
		{
			[global::OpenPCR.Migration("d1784825-3582-4219-a350-e9f5db63954e")]
			get
			{
				return (global::OpenPCR.Runtime.Api.Transform)global::OpenPCR.Runtime.Impl.SceneScripting.Transform(this);
			}
		}
		public int layer
		{
			[global::OpenPCR.Migration("59001472-fdcf-4e82-b6f8-6ccd5a612df1")]
			get
			{
				throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.GameObject::get_layer():System.Int32", "Unity infrastructure not implemented");
			}
			[global::OpenPCR.Migration("032a7ddb-87ab-4e6f-aba3-fc6a2193d3c6")]
			set
			{
				throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.GameObject::set_layer(System.Int32):System.Void", "Unity infrastructure not implemented");
			}
		}
		public bool activeSelf
		{
			[global::OpenPCR.Migration("353c4847-0d89-4913-9667-b1ae7a08e29a")]
			get
			{
				return global::OpenPCR.Runtime.Impl.SceneScripting.ActiveSelf(this);
			}
		}
		public bool activeInHierarchy
		{
			[global::OpenPCR.Migration("ebc49e41-2353-4d53-aaab-a3dc85fa878b")]
			get
			{
				return global::OpenPCR.Runtime.Impl.SceneScripting.ActiveInHierarchy(this);
			}
		}
		public global::OpenPCR.Runtime.Api.GameObject gameObject
		{
			[global::OpenPCR.Migration("a44b1698-dea8-4948-aeb0-8eb543e9cd16")]
			get
			{
				return this;
			}
		}
	}
}
