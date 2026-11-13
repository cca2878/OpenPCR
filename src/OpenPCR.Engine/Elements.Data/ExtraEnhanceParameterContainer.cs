using System.Runtime.CompilerServices;

namespace Elements.Data
{
	public class ExtraEnhanceParameterContainer
	{

		public global::Elements.Data.TeamEnhanceParameter TeamEnhanceParameter { get; private set; }

		public global::Elements.Data.LabyrinthEnhanceParameter LabyrinthEnhanceParameter { get; private set; }


		[global::OpenPCR.Migration("0c52c977-05f4-436b-ae2f-3db46030a941")]
		public ExtraEnhanceParameterContainer(TeamEnhanceParameter _teamEnhanceParameter, LabyrinthEnhanceParameter _labyrinthEnhanceParameter = null)
		{
			TeamEnhanceParameter = _teamEnhanceParameter;
			LabyrinthEnhanceParameter = _labyrinthEnhanceParameter;
		}
	}
}
