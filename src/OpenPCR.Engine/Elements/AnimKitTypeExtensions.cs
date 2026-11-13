using global::Elements;
namespace Elements
{
	public static class AnimKitTypeExtensions
	{
		[global::OpenPCR.Migration("5ae57981-01c3-47db-9c2d-d13c9b5820b9")]
		public static AnimKitResourceInfo GetResourceInfo(this eAnimKitType _spineType)
		{
			return ResourceDefineAnimKit.GetResource(_spineType);
		}
	}
}
