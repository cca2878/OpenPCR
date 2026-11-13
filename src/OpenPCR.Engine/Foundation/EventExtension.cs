using System;

namespace OpenPCR.Foundation
{
	public static class EventExtension
	{
		[global::OpenPCR.Migration("1dcc37f9-4b78-45ce-9329-588eda69ecaf")]
		public static TR Call<TR>(this Func<TR> func)
		{
			if (func != null)
			{
				return func();
			}
			return default(TR);
		}

		[global::OpenPCR.Migration("93767075-a641-4a61-bf15-761ed884a7e0")]
		public static void Call(this Action action)
		{
			if (action != null)
			{
				action();
			}
		}

		[global::OpenPCR.Migration("899efb56-7f4f-40a3-94aa-ccd9e85b6c0d")]
		public static void Call<T1>(this Action<T1> action, T1 arg1)
		{
			if (action != null)
			{
				action(arg1);
			}
		}

		[global::OpenPCR.Migration("c4f8b59b-dd19-474c-af01-737eafc8c6b1")]
		public static void Call<T1, T2>(this Action<T1, T2> action, T1 arg1, T2 arg2)
		{
			if (action != null)
			{
				action(arg1, arg2);
			}
		}

		[global::OpenPCR.Migration("8cd8515e-30f1-47b4-9aa5-eb03e8628e25")]
		public static void Call<T1, T2, T3>(this Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
		{
			if (action != null)
			{
				action(arg1, arg2, arg3);
			}
		}
	}
}
