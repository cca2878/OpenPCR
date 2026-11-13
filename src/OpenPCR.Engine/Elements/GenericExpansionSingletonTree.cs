namespace Elements
{

	public static class GenericExpansionSingletonTree
	{
		[global::OpenPCR.Migration("e6b6566c-33e4-4f4f-addd-ce9a4f03244e")]
		public static Yggdrasil<T> CreateSingletonTree<T>(this T _current) where T : class, ISingletonField
		{
			return SingletonTreeTreeFunc.CreateSingletonTree<T>();
		}
	}

}
