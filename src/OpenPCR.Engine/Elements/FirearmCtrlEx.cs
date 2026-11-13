using global::Elements;
using global::OpenPCR.Runtime.Api;
namespace Elements
{
	public class FirearmCtrlEx : FirearmCtrl
	{

		[global::OpenPCR.Migration("711d2dd9-cfb9-458b-8752-1dc8443fbe9b")]
		protected override Vector3 getHeadBonePos(BasePartsData _target)
		{
			return BattleUnitBaseAnimKitController.BoneWorldToGlobalPosConsiderRotate(_target.GetStateBone(), _target.Owner.RotateCenter, _target.GetBottomLossyScale(), false);
		}
	}
}
