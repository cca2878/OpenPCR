namespace Elements
{
	public class DungeonQueryUnit
	{
		public long owner_viewer_id { get; private set; }
		public int unit_id { get; private set; }
		public int retired { get; private set; }
		public long hp { get; private set; }
		public int energy { get; private set; }
		public SkillLimitCounter[] skill_limit_counter { get; private set; }
		public long damage { get; private set; }
		public PartsInfo[] parts_list { get; private set; }

		[global::OpenPCR.Migration("53701776-5380-4e37-8898-886e6d93292a")]
		public DungeonQueryUnit(long _ownerViewerId, int _unitId, int _retired, long _hp, int _energy, SkillLimitCounter[] _skillLimitCounter, long _damage, PartsInfo[] _partsList)
		{
			owner_viewer_id = _ownerViewerId;
			unit_id = _unitId;
			retired = _retired;
			hp = _hp;
			energy = _energy;
			skill_limit_counter = _skillLimitCounter;
			damage = _damage;
			parts_list = _partsList;
		}
	}
}
