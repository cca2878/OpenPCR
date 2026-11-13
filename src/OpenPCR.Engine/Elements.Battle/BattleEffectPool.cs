using global::Elements;
using global::OpenPCR.Runtime.Api;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Elements.Battle
{
	public class BattleEffectPool : IBattleEffectPool
	{
		private Dictionary<string, List<SkillEffectCtrl>> effectDictionary;
		private Dictionary<string, List<DamageEffectCtrlBase>> numberEffectDictionary;
		public bool ForceNormalSD { get; set; }

		[global::OpenPCR.Migration("c568446f-a1d8-464f-a169-c0ce755387c7")]
		public BattleEffectPool()
		{
			effectDictionary = new Dictionary<string, List<SkillEffectCtrl>>();
			numberEffectDictionary = new Dictionary<string, List<DamageEffectCtrlBase>>();
		}

		[global::OpenPCR.Migration("590d59ba-10d5-4e80-a2e4-3d492dfac4d4")]
		public SkillEffectCtrl GetEffect(GameObject _prefab, UnitCtrl _owner = null)
		{
			bool isShadow = !(_owner == null) && !ForceNormalSD && _owner.IsShadow;
			SkillEffectCtrl skillEffectCtrl = null;
			if (!effectDictionary.ContainsKey(_prefab.name))
			{
				skillEffectCtrl = Object.Instantiate(_prefab).GetComponent<SkillEffectCtrl>();
				List<SkillEffectCtrl> value = new List<SkillEffectCtrl>
				{
					skillEffectCtrl
				};
				effectDictionary.Add(_prefab.name, value);
			}
			else
			{
				List<SkillEffectCtrl> list = effectDictionary[_prefab.name];
				for (int i = 0; i < list.Count; i++)
				{
					SkillEffectCtrl skillEffectCtrl2 = list[i];
					if (!skillEffectCtrl2.IsPlaying)
					{
						skillEffectCtrl2.gameObject.SetActive(value: true);
						skillEffectCtrl2.SetSource(_owner);
						skillEffectCtrl2.ResetParameter(_prefab, (_owner != null) ? UnitUtility.GetSkinId(_owner.ComponentSound.SoundUnitId, _owner.SDSkin) : 0, isShadow);
						return skillEffectCtrl2;
					}
				}

				skillEffectCtrl = Object.Instantiate(_prefab).GetComponent<SkillEffectCtrl>();
				list.Add(skillEffectCtrl);
			}

			skillEffectCtrl.SetSource(_owner);
			skillEffectCtrl.ResetParameter(_prefab, (_owner != null) ? UnitUtility.GetSkinId(_owner.ComponentSound.SoundUnitId, _owner.SDSkin) : 0, isShadow);
			return skillEffectCtrl;
		}

		[global::OpenPCR.Migration("508c18e9-fc3d-4ebb-b8d1-881f5a713d05")]
		public DamageEffectCtrlBase LoadNumberEffect(GameObject _prefab, UnitCtrl _source)
		{
			DamageEffectCtrlBase damageEffectCtrlBase = null;
			List<DamageEffectCtrlBase> value = null;
			if (_prefab == null)
			{
				return null;
			}
			if (!numberEffectDictionary.TryGetValue(_prefab.name, out value))
			{
				damageEffectCtrlBase = Object.Instantiate(_prefab).GetComponent<DamageEffectCtrlBase>();
				damageEffectCtrlBase.transform.parent = ExceptUIRoot.Transform;
				List<DamageEffectCtrlBase> value2 = new List<DamageEffectCtrlBase> { damageEffectCtrlBase };
				numberEffectDictionary.Add(_prefab.name, value2);
			}
			else
			{
				for (int i = 0; i < value.Count; i++)
				{
					DamageEffectCtrlBase damageEffectCtrlBase2 = value[i];
					if (damageEffectCtrlBase2 != null && !damageEffectCtrlBase2.IsPlaying)
					{
						damageEffectCtrlBase2.gameObject.SetActive(value: true);
						damageEffectCtrlBase2.ResetParameter(_prefab);
						damageEffectCtrlBase2.SetSource(_source);
						return damageEffectCtrlBase2;
					}
				}
				damageEffectCtrlBase = Object.Instantiate(_prefab).GetComponent<DamageEffectCtrlBase>();
				damageEffectCtrlBase.transform.parent = ExceptUIRoot.Transform;
				value.Add(damageEffectCtrlBase);
			}
			damageEffectCtrlBase.ResetParameter(_prefab);
			damageEffectCtrlBase.SetSource(_source);
			return damageEffectCtrlBase;
		}

		[global::OpenPCR.Migration("35eeb687-af74-4af7-adfe-517ca435dc2d")]
		public void DisappearUnitNumberEffect(UnitCtrl _owner)
		{
			foreach (KeyValuePair<string, List<DamageEffectCtrlBase>> item in numberEffectDictionary)
			{
				for (int i = 0; i < item.Value.Count; i++)
				{
					DamageEffectCtrlBase damageEffectCtrlBase = item.Value[i];
					if (damageEffectCtrlBase.IsPlaying && !(damageEffectCtrlBase.Source != _owner))
					{
						damageEffectCtrlBase.SetActiveWithCheck(_isActive: false);
					}
				}
			}
		}

		[global::OpenPCR.Migration("efb1bdf8-214d-475d-9a32-5ec950c9e484")]
		public void Release()
		{
			effectDictionary = null;
			numberEffectDictionary = null;
		}
	}
}
