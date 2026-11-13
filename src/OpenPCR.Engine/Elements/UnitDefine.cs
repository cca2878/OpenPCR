using System;
using System.Collections.Generic;
using global::OpenPCR.Runtime.Api;

namespace Elements
{
	public class UnitDefine
	{
		public enum eSkinType
		{
			Icon = 0,
			Sd = 1,
			Still = 2,
			Motion = 3
		}

		public static readonly int MAX_ENERGY = 1000;
		public enum eParameterEnhanceType
		{
			KNIGHT_ENHANCE = 0,
			ROLE = 1,
			LABYRINTH = 2,
			RELIC = 3
		}



		public enum UnitType
		{
			NONE = 0,
			PERSON = 1,
			MONSTER = 2,
			BOSS = 3,
			SUMMON_PERSON = 4,
			SUMMON_MONSTER = 5,
			GUEST = 9,
			SKILL_EFFECT = 10,
		}
	}
}
