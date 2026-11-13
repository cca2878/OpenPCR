namespace Elements
{
	public class SkillLimitCounter
	{
		public int skill_id { get; private set; }

		public int counter { get; private set; }

		public SkillLimitCounter()
		{
			initializeSkillLimitCounter();
		}

		public SkillLimitCounter(int _skillId, int _counter)
		{
			skill_id = _skillId;
			counter = _counter;
		}

		private void initializeSkillLimitCounter()
		{
			skill_id = 0;
			counter = 0;
		}
	}
}
