using System;
using System.Collections.Generic;

namespace Elements
{

	public class SingletonNode : ISingletonNodeForInstanceEditing
	{
		private SingletonNode parent;

		private Dictionary<string, SingletonNode> children = new Dictionary<string, SingletonNode>();

		private Dictionary<string, object> dicInstance = new Dictionary<string, object>();

		[global::OpenPCR.Migration("dd322d73-64b1-4927-b78d-2e27f4590dd7")]
		public SingletonNode()
		{
		}

		[global::OpenPCR.Migration("24d21de2-8b42-4437-97df-f2a71293a32a")]
		public void SetParent(SingletonNode _parent)
		{
			parent = _parent;
		}

		[global::OpenPCR.Migration("3ffebc18-05b4-4b4a-a3f5-00c7e8ebcdf6")]
		public void AddChild(string _key, SingletonNode _child)
		{
			_child.SetParent(this);
			children.Add(_key, _child);
		}

		[global::OpenPCR.Migration("c01fbde8-d08d-4536-bb3c-80d2cd98740f")]
		public void RemoveChild(string _key)
		{
			children.Remove(_key);
		}

		[global::OpenPCR.Migration("746b6b64-cdbf-477c-9849-e5cd6694dfeb")]
		public SingletonNode FindChild(string _key)
		{
			if (children.Count <= 0)
			{
				return null;
			}
			if (children.ContainsKey(_key))
			{
				return children[_key];
			}
			Dictionary<string, SingletonNode>.Enumerator enumerator = children.GetEnumerator();
			while (enumerator.MoveNext())
			{
				SingletonNode singletonNode = enumerator.Current.Value.FindChild(_key);
				if (singletonNode != null)
				{
					return singletonNode;
				}
			}
			return null;
		}

		[global::OpenPCR.Migration("64d9bae6-b014-47aa-8643-46e82cf7911d")]
		public void AddInstance(object _instance)
		{
			AddInstance(_instance.GetType(), _instance);
		}

		[global::OpenPCR.Migration("b67ff870-c65b-4151-b4cd-0032bb049110")]
		public void AddInstance(Type _type, object _instance)
		{
			string className = _type.ToString();
			AddInstance(className, _instance);
		}

		[global::OpenPCR.Migration("2f858311-96cc-4ab8-9688-7a213dfeab67")]
		public void AddInstance(string _className, object _instance)
		{
			dicInstance.Add(_className, _instance);
		}

		[global::OpenPCR.Migration("b810b040-97f7-4d18-beb4-5d260d3a30a3")]
		public void RemoveInstance(object _instance)
		{
			RemoveInstance(_instance.GetType());
		}

		[global::OpenPCR.Migration("bb028552-100b-4b0e-b80b-71874c59bec8")]
		public void RemoveInstance(Type _type)
		{
			string key = _type.ToString();
			RemoveInstance(key);
		}

		[global::OpenPCR.Migration("19a3ed14-2644-4853-b22e-435dbecbde57")]
		public void RemoveInstance(string _key)
		{
			dicInstance.Remove(_key);
		}

		public void AllRemoveInstance()
		{
			dicInstance.Clear();
		}

		[global::OpenPCR.Migration("23d3f0e8-95b3-46d3-b2a4-29ff20305520")]
		public T Get<T>()
		{
			string className = typeof(T).ToString();
			return getWithClassName<T>(className, this);
		}

		public T Get<T>(string _key)
		{
			return getWithClassName<T>(_key, this);
		}

		[global::OpenPCR.Migration("a9e5cadd-65b8-47d1-ad6a-a16b5201d58d")]
		private T getWithClassName<T>(string _className, SingletonNode _node)
		{
			if (dicInstance.ContainsKey(_className))
			{
				return (T)dicInstance[_className];
			}
			if (parent == null)
			{
				return default(T);
			}
			return parent.getWithClassName<T>(_className, _node);
		}
	}

}
