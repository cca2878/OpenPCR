using System.Collections.Generic;
using Elements.Battle.Core;

namespace Elements
{
	public class UnitComponentRelicSkill : UnitComponentBase, ISingletonField
	{
		private List<RelicSkillData> relicSkillList = new List<RelicSkillData>();
		private List<RelicSkillData> execSkillList = new List<RelicSkillData>();
		[global::OpenPCR.Migration("76987370-aa8e-4f15-bda8-931c555351f4")]
		public UnitComponentRelicSkill(UnitCtrl _unitController) : base(_unitController)
		{
		}

		[global::OpenPCR.Migration("f4dceaed-280b-4f30-94ea-c3b6e5dd3fa5")]
		public void Initialize()
		{
			if (!unitController.IsRelicEnemy) return;
			var context = this.CreateSingletonTree().Get<Elements.Data.LabyrinthTempData>();
			foreach (int group in global::OpenPCR.PreparedLabyrinthInputs.Require(context).ActiveEffectGroups)
				relicSkillList.Add(new RelicSkillData(unitController, group));
			InitializeRelicMarkSkill();
		}

		[global::OpenPCR.Migration("8120b81a-3512-4d53-8147-a8df46ffdd37")]
		private void InitializeRelicMarkSkill()
		{
			var context = this.CreateSingletonTree().Get<Elements.Data.LabyrinthTempData>();
			foreach (int group in global::OpenPCR.PreparedLabyrinthInputs.Require(context).MarkEffectGroups)
				relicSkillList.Add(new RelicSkillData(unitController, group));
		}

		[global::OpenPCR.Migration("4d6d6eaa-2e14-42d4-86f2-4993937e0f5d")]
		public void TryExec(float _passedTime)
		{
			for (int i = execSkillList.Count - 1; i >= 0; --i)
			{
				var data = execSkillList[i];
				data.TryExec(_passedTime);
				if (data.ExecSkillReaminCount == 0) execSkillList.RemoveAt(i);
			}
		}

		[global::OpenPCR.Migration("96063a7f-1d9e-4f51-bc1d-74988ac9cc25")]
		public void ResetExecedFlag()
		{
			execSkillList = new List<RelicSkillData>(relicSkillList);
			foreach (var data in execSkillList)
				data.ResetExecStatus();
		}
	}
}
