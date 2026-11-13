using global::OpenPCR.Runtime.Api;
using global::OpenPCR.Foundation;
using System;
using System.Runtime.CompilerServices;
using Elements.Data;

namespace Elements
{
	public abstract class DialogBase : MonoBehaviour, ISingletonField
	{

		[global::OpenPCR.Migration("732aa826-29f7-47b9-8eb1-f9f5e1c6a3c5")]
		protected DialogBase()
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.DialogBase::.ctor(): periphery timing member; behavior is supplied by the wiring phase");
		}

		[global::OpenPCR.Migration("7882a79b-a610-4e44-953a-d1d615b58b57")]
		public static void StaticRelease()
		{
			global::OpenPCR.Presentation.Noop();
		}
	}
}
