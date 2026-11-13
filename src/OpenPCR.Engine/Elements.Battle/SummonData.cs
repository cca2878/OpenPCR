using global::OpenPCR.Runtime.Api;
using System.Runtime.CompilerServices;
using Elements.Battle.Core;

namespace Elements.Battle
{
	public class SummonData
	{

		public global::Elements.Battle.Core.SummonAction.eSummonSide SummonSide { get; set; }

		public global::Elements.Battle.Core.SummonAction.eSummonType SummonType { get; set; }

		public int SummonId { get; set; }
		public Vector3 TargetPosition { get; set; }
		public Vector3 FromPosition { get; set; }
		public UnitCtrl Owner { get; set; }
		public Skill Skill { get; set; }
		public GameObject Prefab { get; set; }
		public float MoveSpeed { get; set; }
		public bool UseRespawnPos { get; set; }
		public eUnitRespawnPos RespawnPos { get; set; }
		public bool ConsiderEquipmentAndBonus { get; set; }

		[global::OpenPCR.Migration("780abb66-c653-41a8-b660-15722f8586f5")]
		public SummonData()
		{
		}
	}
}
