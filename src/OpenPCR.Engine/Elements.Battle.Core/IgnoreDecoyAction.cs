namespace Elements.Battle.Core
{
	public class IgnoreDecoyAction : ActionParameter
	{
		[global::OpenPCR.Migration("e8334d24-c22b-44d4-9ca0-cc482ae188f8")]
		public IgnoreDecoyAction() : base()
		{
		}

		[global::OpenPCR.Migration("b9fa79b4-22db-478f-825f-65f4b93c3fc8")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
			base.IgnoreDecoy = true;
			foreach (int actionChildrenIndex in base.ActionChildrenIndexes)
			{
				_skill.ActionParameters[actionChildrenIndex].IgnoreDecoy = true;
			}
		}

		[global::OpenPCR.Migration("4fb46425-e4ed-48f1-83cd-5e35691eb290")]
		public override bool IsIgnoreDebuffImmune()
		{
			return ActionDetail2 == 1;
		}
	}
}
