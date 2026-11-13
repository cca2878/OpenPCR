using global::OpenPCR.Runtime.Api;
namespace Elements
{
	public static class TransformUtility
	{
		[global::OpenPCR.Migration("4be14072-29af-4998-b30d-8a4ad92e4eb6")]
		public static void ResetLocal(Transform _transform)
		{
			_transform.localPosition = Vector3.zero;
			_transform.localRotation = Quaternion.identity;
			_transform.localScale = Vector3.one;
		}
		[global::OpenPCR.Migration("192d4fb9-2dfa-4e39-94eb-ec26caef4156")]
		public static void ResetLocal(FixedTransformMonoBehavior.FixedTransform _transform)
		{
			_transform.localPosition = Vector3.zero;
			_transform.TargetTransform.localRotation = Quaternion.identity;
			_transform.TargetTransform.localScale = Vector3.one;
		}
	}
}
