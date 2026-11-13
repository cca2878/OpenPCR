using System.Collections.Generic;

namespace Elements
{
	public class TowerUtility
	{
		[global::OpenPCR.Migration("aa04e717-445b-4d04-a818-fd6cf0302b10")]
		public static TowerWaveResultInfo CreateWaveResultInfo(int _waveIndex, List<UnitCtrl> _unitList, List<UnitDamageInfo> _myUnitDamageList, int _miliLimitTime)
		{
			var result = new List<TowerWaveResultUnitInfo>();
			for (int i = 0; i < _myUnitDamageList.Count; i++)
			{
				UnitDamageInfo damage = _myUnitDamageList[i];
				UnitCtrl unit = _unitList.Find(value => value.OriginalUnitId == damage.unit_id);
				int alive = unit.ComponentParameter.Hp > 0 ? 1 : 0;
				result.Add(new TowerWaveResultUnitInfo(damage.unit_id, damage.viewer_id, damage.damage, alive));
			}
			return new TowerWaveResultInfo(_waveIndex + 1, result.ToArray(), _miliLimitTime);
		}

		[global::OpenPCR.Migration("9b969313-36fd-461f-a463-9eb8ef392636")]
		public static eBattleClearType SearchExtraClearType(List<eBattleClearType> _autoFlagList)
		{
			eBattleClearType result = _autoFlagList[0];
			for (int i = 1; i < _autoFlagList.Count; i++)
				if (_autoFlagList[i] != result) return eBattleClearType.MANUAL;
			return result;
		}
	}
}
