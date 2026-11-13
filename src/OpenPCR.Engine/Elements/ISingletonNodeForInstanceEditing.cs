using System;

namespace Elements
{

	public interface ISingletonNodeForInstanceEditing
	{
		void AddInstance(object _instance);

		void AddInstance(Type _type, object _instance);

		void AddInstance(string _className, object _instance);

		void RemoveInstance(object _instance);

		void RemoveInstance(Type _type);

		void RemoveInstance(string _key);

		void AllRemoveInstance();
	}

}
