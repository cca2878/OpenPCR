using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Elements.Data
{
	public class LabyrinthEnhanceParameter
	{
		private global::System.Collections.Generic.Dictionary<Elements.eParamType, System.Int32> labyrinthBuffParameterDictionary { get; set; }
		public global::System.Collections.Generic.Dictionary<Elements.eParamType, System.Int32> LabyrinthBuffParameterDictionary => labyrinthBuffParameterDictionary;
		private global::System.Collections.Generic.Dictionary<Elements.eParamType, System.Int32> labyrinthRelicPassiveParameter { get; set; }
		public global::System.Collections.Generic.Dictionary<Elements.eParamType, System.Int32> LabyrinthRelicPassiveParameter => labyrinthRelicPassiveParameter;

		public LabyrinthEnhanceParameter(LabyrinthTempData _labyrinthTempData)
		{
			labyrinthRelicPassiveParameter = new Dictionary<eParamType, int>();
			labyrinthBuffParameterDictionary = Elements.Utility.LabyrinthUtility.CalcLabyrinthEnhancedBuffParam(_labyrinthTempData.ReinforcementNodeList);
			labyrinthRelicPassiveParameter = Elements.Utility.LabyrinthUtility.CalcLabyrinthRelicPassiveParam(_labyrinthTempData);
		}
	}
}
