namespace Elements
{

	public class BaseSingletonTree
	{
		protected static SingletonNode rootNode;
		protected static global::System.Collections.Generic.Dictionary<string, SingletonNode> dicNode;

		[global::OpenPCR.Migration("3e7c31d2-42a3-4e6b-941d-627c7581bfd2")]
		public BaseSingletonTree()
		{
		}
	}
}
