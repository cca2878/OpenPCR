using global::OpenPCR.AnimKit;
using global::OpenPCR.AnimKit.Scene;
namespace OpenPCR.AnimKit.Scene
{
	public interface ISkeletonAnimation
	{
		Skeleton Skeleton { get; }

		event UpdateBonesDelegate UpdateLocal;
		event UpdateBonesDelegate UpdateWorld;
		event UpdateBonesDelegate UpdateComplete;
		void LateUpdate();
	}
}
