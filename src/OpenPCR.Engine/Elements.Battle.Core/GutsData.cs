using global::OpenPCR.Foundation;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class GutsData
	{
		private global::Elements.Battle.Core.eInhibitHealType inhibitHealType { get; } = global::Elements.Battle.Core.eInhibitHealType.NO_EFFECT;
		private global::Elements.UnitCtrl target { get; }
		private global::Elements.UnitCtrl source { get; }

		[global::OpenPCR.Migration("275f417f-30b1-425d-8a8c-9d99f7cc4d5a")]
		public bool IsEnable
		{
			[global::OpenPCR.Migration("b3c1a767-c0ee-4e48-9bef-74633deb95c0")]
			get
			{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.Core.GutsData::get_IsEnable (reachable only through Excluded GutsAction: no first-phase loader)"); }
		}

		[global::OpenPCR.Migration("c15e9ec2-fc3e-4b2c-beae-0099260c0b84")]
		public GutsData(eStateIconType _stateIconType, eInhibitHealType _inhibitHealType, long _healValue, float _limitTime, int _remainCount, int _remainMaxCount, SkillEffectCtrl _effect, NormalSkillEffect _execEffectData, Skill _skill, UnitCtrl _target, UnitCtrl _source)
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.Core.GutsData::.ctor (reachable only through Excluded GutsAction: no first-phase loader)"); }

		[global::OpenPCR.Migration("60ae7213-f793-44a1-88aa-a8af17764179")]
		public void Exec()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.Core.GutsData::Exec (reachable only through Excluded GutsAction: no first-phase loader)"); }
	}
}
