using System;
using System.Collections.Generic;

namespace Elements
{

	public abstract class SingletonTree<TOwnerClass> : BaseSingletonTree where TOwnerClass : ISingletonField
	{
		protected SingletonNode currentNode;

		[global::OpenPCR.Migration("e3a13fef-f268-4463-a243-73aa9cf263dc")]
		protected SingletonTree()
		{
		}

		[global::OpenPCR.Migration("416db291-63da-45ee-a484-f0599e94baf2")]
		public void Init()
		{
			if (BaseSingletonTree.rootNode != null)
			{
				string key = typeof(TOwnerClass).ToString();
				currentNode = BaseSingletonTree.rootNode.FindChild(key);
			}
		}

		protected abstract void setupTree();

		[global::OpenPCR.Migration("f60ec14e-6c6e-47a3-aa7a-abda84c7027f")]
		protected void addNode(SingletonNode _parentNode, Type _childKey, SingletonNode _childNode)
		{
			string key = _childKey.ToString();
			BaseSingletonTree.dicNode.Add(key, _childNode);
			_parentNode.AddChild(key, _childNode);
		}

		[global::OpenPCR.Migration("e519fd0e-269d-4b18-b814-eda6d79ba37d")]
		public SingletonNode AddNode(ISingletonNodeForInstanceEditing _parentNode, Type _childKey)
		{
			SingletonNode singletonNode = new SingletonNode();
			SingletonNode parentNode = (SingletonNode)_parentNode;
			addNode(parentNode, _childKey, singletonNode);
			return singletonNode;
		}

		[global::OpenPCR.Migration("40329b35-8a06-46fe-8cc6-4fcb2cd14e15")]
		public void RemoveNode(ISingletonNodeForInstanceEditing _parentNode, string _key)
		{
			BaseSingletonTree.dicNode.Remove(_key);
			((SingletonNode)_parentNode).RemoveChild(_key);
		}

		[global::OpenPCR.Migration("7db58d4f-5346-41d5-bc6b-43c68b46102b")]
		public void RemoveNode(ISingletonNodeForInstanceEditing _parentNode, Type _childKey)
		{
			string key = _childKey.ToString();
			RemoveNode(_parentNode, key);
		}

		[global::OpenPCR.Migration("52a93ae9-7dba-4da7-8c77-b0870c8d3d3e")]
		public T Get<T>()
		{
			if (currentNode == null)
			{
				return default(T);
			}
			return currentNode.Get<T>();
		}

		public T Get<T>(string _key)
		{
			if (currentNode == null)
			{
				return default(T);
			}
			return currentNode.Get<T>(_key);
		}

		[global::OpenPCR.Migration("d3f11053-3317-4488-9522-aed0553e8647")]
		public ISingletonNodeForInstanceEditing GetCurrentNodeForInstanceEditing()
		{
			return currentNode;
		}
	}

}
