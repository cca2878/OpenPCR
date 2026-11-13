using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class BuffDebuffConstData
	{
		public global::Elements.eStateIconType BuffIcon { get; set; }
		public global::Elements.eStateIconType DebuffIcon { get; set; }

		[global::OpenPCR.Migration("6f25b46e-5aac-4837-a646-f9fa325f8c16")]
		public BuffDebuffConstData()
		{
		}
	}
}
