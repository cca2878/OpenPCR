namespace Elements.Utility
{
	public class ActionData
	{
		public int ActionId { get; private set; }

		public SkillActionValue ActionValue { get; private set; }

		[global::OpenPCR.Migration("6e08a35d-7634-405a-84c2-740bb22d944f")]
		public void SetActionId(int _actionId)
		{
			ActionId = _actionId;
		}

		[global::OpenPCR.Migration("36dbfe04-c983-4bb2-9007-28fdee08265c")]
		public void SetActionValue(SkillActionValue _actionValue)
		{
			ActionValue = _actionValue;
		}

		[global::OpenPCR.Migration("a96ebd73-3795-4c5d-b3ad-ac7d987be94b")]
		private void initializeActionData()
		{
			ActionId = 0;
			ActionValue = null;
		}

		[global::OpenPCR.Migration("9140deea-d458-4a45-9a81-137b0d7db067")]
		public ActionData()
		{
			initializeActionData();
		}
	}
}
