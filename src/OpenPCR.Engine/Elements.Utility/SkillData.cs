using System.Collections.Generic;

namespace Elements.Utility
{
	public class SkillData
	{
		public int SkillId { get; private set; }

		public List<ActionData> ActionInfoList { get; private set; }

		[global::OpenPCR.Migration("dc87ddf7-3388-4473-b15e-5fb2c574367c")]
		public void SetSkillId(int _skillId)
		{
			SkillId = _skillId;
		}

		[global::OpenPCR.Migration("1750a788-3615-4bb7-9c97-91384f99e754")]
		public void SetActionInfoList(List<ActionData> _actionInfoList)
		{
			ActionInfoList = _actionInfoList;
		}

		[global::OpenPCR.Migration("6dc822c7-5168-4a2f-81e3-b57258603b07")]
		private void initializeSkillData()
		{
			SkillId = 0;
			ActionInfoList = new List<ActionData>();
		}

		[global::OpenPCR.Migration("935f9e94-d843-462b-8ee1-e6174734cdf2")]
		public SkillData()
		{
			initializeSkillData();
		}
	}
}
