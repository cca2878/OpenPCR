using global::OpenPCR.Runtime.Api;
using global::OpenPCR.Foundation;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Elements
{
	public class PartsBattleSkipAnimation : MonoBehaviour
	{
		[StructLayout((LayoutKind)0, Pack = 1, Size = 16)]
		private readonly struct Resource
		{
			public readonly eResourceId Id;
			public Resource(eResourceId _id, long _index)
			{
				throw new global::OpenPCR.MigrationPendingException("Elements.PartsBattleSkipAnimation+Resource::.ctor(Elements.eResourceId,System.Int64)", "structural declaration migration pending");
			}
		}
		private static readonly List<Resource> ANIMATION_RESOURCES = new List<Resource>
		{
			new Resource(eResourceId.BATTLE_SKIP_TEXTURE, 1L),
			new Resource(eResourceId.BATTLE_SKIP_TEXTURE, 2L),
			new Resource(eResourceId.BATTLE_SKIP_TEXTURE, 3L),
			new Resource(eResourceId.BATTLE_SKIP_TEXTURE, 4L)
		};
		public const float FLIP_INTERVAL_SECONDS = 0.7f;
		public PartsBattleSkipAnimation()
		{
			throw new global::OpenPCR.MigrationPendingException("Elements.PartsBattleSkipAnimation::.ctor()", "body migration pending");
		}

		[global::OpenPCR.Migration("28339e36-7beb-4469-a2d3-42597bb06fe9")]
		public void Load()
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.PartsBattleSkipAnimation::Load():System.Void: periphery input member; behavior is supplied by the wiring phase");
		}
	}
}
