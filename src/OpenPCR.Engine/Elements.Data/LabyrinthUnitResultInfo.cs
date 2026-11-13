namespace Elements.Data
{
	public class LabyrinthUnitResultInfo
	{
		public readonly long Hp;
		public readonly long MaxHp;
		public readonly double Energy;

		[global::OpenPCR.Migration("131265b2-bc33-4931-8d6c-d2e1218a9b23")]
		public LabyrinthUnitResultInfo(long _hp, long _maxHp, double _energy)
		{
			Hp = _hp;
			MaxHp = _maxHp;
			Energy = _energy;
		}
	}
}
