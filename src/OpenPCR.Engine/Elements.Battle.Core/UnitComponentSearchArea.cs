using System.Collections;
using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class UnitComponentSearchArea : UnitComponentBase, IUnitComponent
	{
		[global::OpenPCR.Migration("b48dd8ca-997a-46bb-93c4-0cfaf8749415")] public float SearchAreaSize { [global::OpenPCR.Migration("a62a7f8c-9b5c-4ed5-b425-50ccef9bf2b6")] get; [global::OpenPCR.Migration("8765045c-cfee-4903-b713-01d26f578d85")] private set; }
		public float StartSearchAreaSize { get; private set; }
		public Dictionary<int, float> SkillAreaWidthList { get; private set; }
		public List<UnitCtrl> SkillTargetList { get; private set; }

		[global::OpenPCR.Migration("51d19bbe-b19e-421c-8c8c-c6d17b9775aa")]
		public bool HasSkillTarget
		{
			[global::OpenPCR.Migration("15d414a3-a101-4cef-8e60-d9be657de472")]
			get
			{ return global::OpenPCR.Presentation.Noop<bool>(); }
		}

		public List<UnitCtrl> TargetEnemyList { get; private set; }
		public bool IsTauntSourceInSearchArea { get; private set; }
		public List<UnitCtrl> TargetPlayerList { get; private set; }
		private UnitComponentActionPattern componentAttackPattern { get; set; }
		private UnitComponentParameter componentParameter { get; set; }
		private UnitComponentState componentState { get; set; }
		private UnitComponentUnionBurst componentUnionBurst { get; set; }

		[global::OpenPCR.Migration("cf7e41ed-a3dc-43be-9b40-fa844fb3f7f1")]
		public UnitComponentSearchArea(UnitCtrl _unitController) : base(_unitController)
		{
			SkillAreaWidthList = new Dictionary<int, float>();
			SkillTargetList = new List<UnitCtrl>();
			TargetEnemyList = new List<UnitCtrl>();
			TargetPlayerList = new List<UnitCtrl>();
		}

		[global::OpenPCR.Migration("c63e76a2-81a7-4698-989a-af9c80a33257")]
		public void RegisterComponentSet(IUnitComponentContainer _container)
		{
			componentAttackPattern = _container.ComponentActionPattern;
			componentParameter = _container.ComponentParameter;
			componentUnionBurst = _container.ComponentUnionBurst;
			componentState = _container.ComponentState;
		}

		[global::OpenPCR.Migration("1f574458-4d4d-446a-abb1-272a4dc368d6")]
		public void InitializeSearchArea(float _value)
		{
			SearchAreaSize = _value;
			StartSearchAreaSize = _value;
			ResetTargetList();
		}

		[global::OpenPCR.Migration("ee984ea2-9f97-4939-a6fb-06a41fd53a95")]
		public void ResetTargetList()
		{
			TargetEnemyList.Clear();
			TargetPlayerList.Clear();
			SkillTargetList.Clear();
		}

		[global::OpenPCR.Migration("5f990ef7-aa00-4b74-a497-f86dcddd5971")]
		public void UpdateSkillTarget()
		{
			SkillTargetList.Clear();
			List<UnitCtrl> targets = unitController.GetEnemyList();
			for (int i = 0; i < targets.Count; i++)
			{
				UnitCtrl target = targets[i];
				if (judgeFrontAreaTarget(target, SkillAreaWidthList[unitController.UnionBurstSkillId]))
					SkillTargetList.Add(target);
			}
		}

		[global::OpenPCR.Migration("46028f40-5b68-4126-8e04-2cc137be99a9")]
		private bool judgeFrontAreaTarget(UnitCtrl _target, float _distance)
		{
			if (_target.IsPartsBoss)
				return judgeFrontAreaTargetForBossParts(_target, _distance);
			var ownerTransform = unitController.transform;
			float scale = ownerTransform.parent.lossyScale.x;
			float targetX = _target.transform.position.x;
			float ownerX = ownerTransform.position.x;
			float width = (_target.BodyWidth + unitController.BodyWidth) * 0.5f;
			bool left = unitController.IsLeftDir;
			float distance = targetX / scale - ownerX / scale;
			float min = (left ? -_distance : 0f) - width;
			float max = width + (left ? 0f : _distance);
			if ((!(distance >= min) || !(distance <= max)) && !BattleUtil.Approximately(distance, min) && !BattleUtil.Approximately(distance, max))
				return false;
			var state = _target.ComponentState;
			var abnormal = _target.ComponentAbnormal;
			if ((state.IsDead || (long)_target.ComponentParameter.Hp <= 0L) && !state.HasUnDeadTime)
				return false;
			if (_target.IsPhantom)
				return false;
			if (abnormal.JudgeIsEtherialization(unitController) || abnormal.IsStealth)
				return false;
			if (!abnormal.IsAbnormalState(eAbnormalState.SPY))
				return true;
			return !(_target != unitController);
		}

		[global::OpenPCR.Migration("09007fa0-f5a8-4a8b-ad16-578932870e5f")]
		private bool judgeFrontAreaTargetForBossParts(UnitCtrl _target, float _distance)
		{
			UnitComponentMultiPartsBoss parts = _target.ComponentMultiPartsBoss;
			for (int i = 0; i < parts.BossPartsListForBattle.Count; i++)
			{
				PartsData data = parts.BossPartsListForBattle[i];
				if (data.GetTargetable() && judgePartsInDistance(_target, _distance, data))
					return true;
				parts = _target.ComponentMultiPartsBoss;
			}

			return false;
		}

		[global::OpenPCR.Migration("3d9a0a53-ad5c-444b-967b-a2cbedd41836")]
		private bool judgePartsInDistance(UnitCtrl _target, float _distance, PartsData _data)
		{
			float targetX = _target.transform.localPosition.x;
			float partX = _data.PositionX;
			float ownerX = unitController.transform.localPosition.x;
			float ownerWidth = unitController.BodyWidth;
			bool left = unitController.IsLeftDir;
			float distance = (targetX + partX) - ownerX;
			float width = (_data.BodyWidthValue + ownerWidth) * 0.5f;
			float min = (left ? -_distance : 0f) - width;
			float max = width + (left ? 0f : _distance);
			if ((!(distance >= min) || !(distance <= max)) && !BattleUtil.Approximately(distance, min) && !BattleUtil.Approximately(distance, max))
				return false;
			var state = _target.ComponentState;
			if ((state.IsDead || (long)_target.ComponentParameter.Hp <= 0L) && !state.HasUnDeadTime)
				return false;
			return !_target.IsPhantom;
		}

		[global::OpenPCR.Migration("9e326140-dce3-4e04-a631-099d1c800011")]
		public void UpdateAttackTargetImpl(BasePartsData _tauntSource)
		{
			List<UnitCtrl> enemies = unitController.GetEnemyList();
			float distance = SearchAreaSize;
			int firstSkillId = componentAttackPattern.GetFirstActionSkillId();
			if (firstSkillId >= 2)
				distance = SkillAreaWidthList[firstSkillId];
			IsTauntSourceInSearchArea = !System.Object.ReferenceEquals(_tauntSource, null) && (_tauntSource is PartsData parts ? judgePartsInDistance(_tauntSource.Owner, distance, parts) : judgeFrontAreaTarget(_tauntSource.Owner, distance));
			for (int i = 0; i < enemies.Count; i++)
			{
				UnitCtrl target = enemies[i];
				if (judgeFrontAreaTarget(target, distance))
				{
					if (!TargetEnemyList.Contains(target))
						TargetEnemyList.Add(target);
				}
				else
					TargetEnemyList.Remove(target);
			}

			for (int i = TargetEnemyList.Count - 1; i >= 0; i--)
			{
				UnitCtrl target = TargetEnemyList[i];
				if (!enemies.Contains(target))
					TargetEnemyList.Remove(target);
			}

			List<UnitCtrl> friends = unitController.GetFriendList();
			for (int i = 0; i < friends.Count; i++)
			{
				UnitCtrl target = friends[i];
				if (target == null)
					continue;
				if (judgeFrontAreaTarget(target, distance))
				{
					if (!TargetPlayerList.Contains(target))
						TargetPlayerList.Add(target);
				}
				else
					TargetPlayerList.Remove(target);
			}

			for (int i = TargetPlayerList.Count - 1; i >= 0; i--)
			{
				UnitCtrl target = TargetPlayerList[i];
				if (!friends.Contains(target))
					TargetPlayerList.Remove(target);
			}
		}
	}
}
