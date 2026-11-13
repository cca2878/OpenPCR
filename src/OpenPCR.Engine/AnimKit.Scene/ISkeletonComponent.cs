using global::OpenPCR.AnimKit;
using global::OpenPCR.AnimKit.Scene;
namespace OpenPCR.AnimKit.Scene
{
	public interface ISkeletonComponent
	{
		SkeletonDataAsset SkeletonDataAsset { get; }

		Skeleton Skeleton { get; }
	}
}
