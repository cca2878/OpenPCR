using System.Collections.Generic;

namespace Elements
{
	public class TowerUnit
	{
		public int UnitId { get; private set; }
		public int IdentifyNum { get; private set; }
		public long Hp { get; private set; }
		public int Energy { get; private set; }
		public List<SkillLimitCounter> SkillLimitCounter { get; private set; }
		public long MaxHp { get; private set; }
		public long SavedHp { get; private set; }
		public int Level { get; private set; }
		public int Rarity { get; private set; }
		public int PromotionLevel { get; private set; }
		public int EnemyColor { get; private set; }
		public SkinData SkinData { get; private set; }

		[global::OpenPCR.Migration("2f6f9f0b-7e4a-4494-835a-2cf8752fd249")]
		public TowerUnit() { initializeTowerUnit(); }
		[global::OpenPCR.Migration("61d98dd8-1277-49f6-861f-e44d6f8c8086")]
		public void SetUnitId(int _unitId) { UnitId = _unitId; }
		[global::OpenPCR.Migration("8c112507-c7f6-4934-b9a2-c9adf55ed211")]
		public void SetIdentifyNum(int _identifyNum) { IdentifyNum = _identifyNum; }
		[global::OpenPCR.Migration("2336c66d-9ba7-45f3-afca-6e7124d61bcc")]
		public void SetHp(long _hp) { Hp = _hp; }
		[global::OpenPCR.Migration("98275be5-cc65-453f-9c42-432887bf19cf")]
		public void SetEnergy(int _energy) { Energy = _energy; }
		[global::OpenPCR.Migration("97a37f6d-f427-464f-b944-299433bd7281")]
		public void SetSkillLimitCounter(List<SkillLimitCounter> _skillLimitCounter) { SkillLimitCounter = _skillLimitCounter; }
		[global::OpenPCR.Migration("67524c37-6199-4d80-b69e-ad431e9f8c44")]
		public void SetMaxHp(long _maxHp) { MaxHp = _maxHp; }
		[global::OpenPCR.Migration("e8c794ba-1310-43ba-a8f7-8f87cdf91c53")]
		public void SetSavedHp(long _savedHp) { SavedHp = _savedHp; }
		[global::OpenPCR.Migration("88d7aa1d-5b53-44b0-ae19-123a522cd456")]
		public void SetLevel(int _level) { Level = _level; }
		[global::OpenPCR.Migration("c412bab0-e94b-4792-8d06-0cb6ef1f3ba6")]
		public void SetRarity(int _rarity) { Rarity = _rarity; }
		[global::OpenPCR.Migration("588ff7e9-1b8f-4aca-8f01-4631323cda50")]
		public void SetPromotionLevel(int _promotionLevel) { PromotionLevel = _promotionLevel; }
		[global::OpenPCR.Migration("eca3d968-bfb8-4720-ba3f-fce8070c0ecc")]
		public void SetEnemyColor(int _enemyColor) { EnemyColor = _enemyColor; }

		[global::OpenPCR.Migration("84b959df-a069-4282-904a-ef33aced258b")]
		private void initializeTowerUnit()
		{
			UnitId = 0;
			IdentifyNum = 0;
			Hp = 0;
			Energy = 0;
			SkillLimitCounter = new List<SkillLimitCounter>();
			MaxHp = 0;
			SavedHp = 0;
			Level = 0;
			Rarity = 0;
			PromotionLevel = 0;
			EnemyColor = 0;
			SkinData = null;
		}
	}
}
