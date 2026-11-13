namespace Elements
{

	public class SingletonTreeTreeFunc : ISingletonField
	{


		[global::OpenPCR.Migration("cb018ee2-c837-46c1-a3ff-3aaab9656812")]
		public static Yggdrasil<TNodeType> CreateSingletonTree<TNodeType>() where TNodeType : class, ISingletonField
		{
			Yggdrasil<TNodeType> yggdrasil = Yggdrasil<TNodeType>.Create();
			yggdrasil.Init();
			return yggdrasil;
		}
	}

}
