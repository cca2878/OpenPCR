using System.Collections.Generic;
using System.Reflection;

namespace Elements
{
	public class UnitSkillReserve
	{
		public bool this[int unitId]
		{
			[global::OpenPCR.Migration("fdbab775-8d02-4bfb-bd1a-12c8cb1b1443")]
			get { return skillReserves.TryGetValue(unitId, out bool value) && value; }
			[global::OpenPCR.Migration("2929f974-f599-4182-8f8a-13685082d576")]
			set { skillReserves[unitId] = value; }
		}

		private Dictionary<int, bool> skillReserves;

		[global::OpenPCR.Migration("66db38d5-0f4a-4837-8bd5-6eabfb60bbd8")]
		public UnitSkillReserve(Dictionary<int, bool> _copyData = null)
		{
			if (_copyData != null)
			{
				skillReserves = new Dictionary<int, bool>(_copyData);
			}
			else
			{
				skillReserves = new Dictionary<int, bool>();
			}
		}

		public void Reset()
		{
			foreach (int item in new List<int>(skillReserves.Keys))
			{
				skillReserves[item] = false;
			}
		}
	}
}
