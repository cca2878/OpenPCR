using global::Elements;
using System.Runtime.CompilerServices;

namespace Elements
{
	public class AnimKitResourceInfo
	{
		public eAnimKitType spineType { get; private set; }
		public eBundleId bundleId { get; private set; }
		public eResourceId skelton { get; private set; }
		public eResourceId controller { get; private set; }
		public eResourceId animation { get; private set; }
		public float animationScale { get; private set; }
		public bool IsSkinId { get; private set; }

		[global::OpenPCR.Migration("4a5807d8-33ee-4371-a866-73e90bbad816")]
		public AnimKitResourceInfo(eAnimKitType _spineType, eBundleId _bundleId, eResourceId _skelton, eResourceId _controller, eResourceId _animation = eResourceId.NONE, float _animationScale = 1f, bool _isSkinId = false)
		{
			spineType = _spineType;
			bundleId = _bundleId;
			skelton = _skelton;
			controller = _controller;
			animation = _animation;
			animationScale = _animationScale;
			IsSkinId = _isSkinId;
		}
	}
}
