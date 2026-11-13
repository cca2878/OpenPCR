using global::OpenPCR.Runtime.Api;
using System;

namespace Elements
{
	public class PartsSkillWindow : MonoBehaviour
	{

		[global::OpenPCR.Migration("f3176410-47dc-412c-b347-b73bad810226")]
		public void Pause(bool @value)
		{
			global::OpenPCR.Presentation.Noop();
		}

		[global::OpenPCR.Migration("2620adf3-a4c8-46e0-9e28-7cbb83a35b90")]
		public void Indicate(string _skillName, bool isBoss)
		{
			global::OpenPCR.Presentation.Noop();
		}

		public PartsSkillWindow()
		{
			throw new global::OpenPCR.MigrationPendingException("Elements/PartsSkillWindow.cs::.ctor", "Native consumer declaration; body migration pending");
		}
	}
}
