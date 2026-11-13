using System;
using System.Collections.Generic;

namespace Elements
{
	[Serializable]
	public class CutInData
	{
		public List<int> CutInPlayedCharaList = new List<int>();
		public ePlaybackType PlaybackType;
		public CutInData()
		{
			PlaybackType = ePlaybackType.EVERY_TIME;
		}

		public void AddCharaIdPlayedCutIn(int charaId)
		{
			CutInPlayedCharaList.Add(charaId);
		}

		public bool IsCutInPlayedChara(int unitId)
		{
			return CutInPlayedCharaList.Contains(UnitUtility.GetCharaId(unitId));
		}
	}
}
