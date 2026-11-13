using System;
using System.Collections.Generic;

namespace Elements
{
	public class SkillLevelInfoLight
	{
		public global::System.Int32 SkillLevel { get; private set; }

		public SkillLevelInfoLight()
		{
			initializeSkillLevelInfoLight();
		}

		private void initializeSkillLevelInfoLight()
		{
			SkillLevel = 0;
		}
	}
}
