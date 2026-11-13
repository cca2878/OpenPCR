using global::OpenPCR.Runtime.Api;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class UnitComponentColor : UnitComponentBase, IUnitComponent
	{
		private static readonly Color WEAK_COLOR = new Color(0.5411f, 0.6274f, 0.996f, 1f);
		private static readonly Color32 DAMAGE_FLASH_COLOR_32 = new Color32(150, 150, 150, 255);
		private static readonly Color DAMAGE_FLASH_COLOR = DAMAGE_FLASH_COLOR_32;

		private UnitComponentAbnormal componentAbnormal { get; set; }

		private Color curColor { get; set; }

		private Dictionary<ChangeColorEffect, Color> curColorChannel { get; set; }

		private Dictionary<ChangeColorEffect, Color> curColorOffsetChannel { get; set; }

		[global::OpenPCR.Migration("129e5740-2a01-440e-82b9-3560b75bec14")]
		public UnitComponentColor(UnitCtrl _unitController) : base(_unitController)
		{
			curColorChannel = new Dictionary<ChangeColorEffect, Color>();
			curColorOffsetChannel = new Dictionary<ChangeColorEffect, Color>();
			curColor = new Color(1f, 1f, 1f, 1f);
		}

		[global::OpenPCR.Migration("9a90d820-bc63-4221-9ebc-39ba41e173ea")]
		public void RegisterComponentSet(IUnitComponentContainer _container)
		{
			componentAbnormal = _container.ComponentAbnormal;
		}

		[global::OpenPCR.Migration("e982e5f9-0750-4aa5-a261-33041d85b98c")]
		public void SetCurColor(ChangeColorEffect _key, Color _color)
		{
			if (!curColorChannel.ContainsKey(_key))
			{
				curColorChannel.Add(_key, _color);
			}
			else
			{
				curColorChannel[_key] = _color;
			}

			updateCurColor();
		}

		[global::OpenPCR.Migration("c7a3d9c3-4226-4c4f-abec-1200f3f2f23f")]
		private void updateCurColor()
		{
			Dictionary<ChangeColorEffect, Color>.Enumerator enumerator = curColorChannel.GetEnumerator();
			Color color = curColor;
			while (enumerator.MoveNext())
			{
				color *= enumerator.Current.Value;
			}
			foreach (ToadData toadData in componentAbnormal.ToadDatas)
			{
				toadData.BattleSpineController.SetCurColor(color);
			}
			unitController.UnitSpineCtrl.SetCurColor(color);
			if (unitController.UnitSpineCtrlModeChange != null)
			{
				unitController.UnitSpineCtrlModeChange.SetCurColor(color);
			}
		}

		[global::OpenPCR.Migration("97f22eaa-7024-4f7d-ab85-6ca23b9751f7")]
		public void SetCurColorOffset(ChangeColorEffect _key, Color _color)
		{
			if (!curColorOffsetChannel.ContainsKey(_key))
			{
				curColorOffsetChannel.Add(_key, _color);
			}
			else
			{
				curColorOffsetChannel[_key] = _color;
			}
			Dictionary<ChangeColorEffect, Color>.Enumerator enumerator = curColorOffsetChannel.GetEnumerator();
			Color curColorOffset = new Color(0f, 0f, 0f, 0f);
			while (enumerator.MoveNext())
			{
				curColorOffset += enumerator.Current.Value;
			}
			foreach (ToadData toadData in componentAbnormal.ToadDatas)
			{
				toadData.BattleSpineController.CurColorOffset = curColorOffset;
			}
			unitController.UnitSpineCtrl.CurColorOffset = curColorOffset;
			if (unitController.UnitSpineCtrlModeChange != null)
			{
				unitController.UnitSpineCtrlModeChange.CurColorOffset = curColorOffset;
			}
		}

		[global::OpenPCR.Migration("cfc99d8a-198c-4162-ab5c-43e7ac067b02")]
		public void SetDamageColor()
		{
			curColor = DAMAGE_FLASH_COLOR;
			updateCurColor();
		}

		[global::OpenPCR.Migration("d04b55e2-12f4-4547-aba8-c43fb4ed8009")]
		public void SetNormalColor()
		{
			curColor = new Color(1f, 1f, 1f, 1f);
			updateCurColor();
		}

		[global::OpenPCR.Migration("b20233fe-e038-438f-ae19-1f9514dda623")]
		public void SetWeakColor()
		{
			curColor = WEAK_COLOR;
			updateCurColor();
		}
		[global::OpenPCR.Migration("2682e475-edec-422c-aef7-ddc02647a4d8")]
		private void removeColorEffect(ChangeColorEffect _colorEffect)
		{
			if (_colorEffect != null)
			{
				curColorOffsetChannel.Remove(_colorEffect);
				curColorChannel.Remove(_colorEffect);
			}
		}

		[global::OpenPCR.Migration("d00cc4f1-deed-4b89-87d5-a13a6fea3fdb")]
		public void ResetColor(ChangeColorEffect _colorEffect)
		{
			removeColorEffect(_colorEffect);
			if (componentAbnormal.IsSlowSpeed()) SetWeakColor();
			else SetNormalColor();
		}
	}
}
