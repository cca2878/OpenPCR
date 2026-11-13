namespace Elements
{
	public class Yggdrasil<TOwnerClass> : SingletonTree<TOwnerClass> where TOwnerClass : ISingletonField
	{
		[global::OpenPCR.Migration("4f6fda49-95f5-4987-8c85-7026c6681e73")]
		protected Yggdrasil()
		{
		}

		protected override void setupTree()
		{
			// The complete Native product tree belongs to host bootstrap.
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Yggdrasil`1::setupTree():System.Void");
		}

		[global::OpenPCR.Migration("85e41a42-632d-4557-855d-d1a242064fa3")]
		public static Yggdrasil<TOwnerClass> Create()
		{
			return new Yggdrasil<TOwnerClass>();
		}
	}
}
