using global::OpenPCR.AnimKit;
using global::OpenPCR.AnimKit.Scene;
using global::OpenPCR.Runtime.Api;
using global::OpenPCR.Runtime.Api.Serialization;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace OpenPCR.AnimKit.Scene
{

	public partial class SkeletonRenderer : MonoBehaviour, ISkeletonComponent
	{
		public SkeletonDataAsset skeletonDataAsset;
		[FormerlySerializedAs("submeshSeparators")]
		public string[] separatorSlotNames = new string[0];
		[NonSerialized]
		public readonly List<Slot> separatorSlots = new List<Slot>();
		public bool useClipping = true;
		public bool pmaVertexColors = true;
		public bool disableRenderingOnOverride = true;
		[NonSerialized]
		private readonly Dictionary<Material, Material> customMaterialOverride = new Dictionary<Material, Material>();
		[NonSerialized]
		private readonly Dictionary<Slot, Material> customSlotMaterials = new Dictionary<Slot, Material>();
		protected Shader curShader;
		[NonSerialized]
		public bool valid;
		[NonSerialized]
		public Skeleton skeleton;
		[NonSerialized]
		private readonly SkeletonRendererInstruction currentInstructions = new SkeletonRendererInstruction();
		private readonly MeshGenerator meshGenerator = new MeshGenerator();
		[NonSerialized]
		private readonly MeshRendererBuffers rendererBuffers = new MeshRendererBuffers();
		public SkeletonDataAsset SkeletonDataAsset => skeletonDataAsset;

		public Skeleton Skeleton
		{
			get
			{
				Initialize(false);
				return skeleton;
			}
		}

		[global::OpenPCR.Migration("35398274-1719-48a5-a0da-750d84245471")]
		public SkeletonRenderer()
		{
		}

		public virtual void Initialize(bool overwrite)
		{
			if (valid && !overwrite) return;
			throw new global::OpenPCR.OutOfBoundaryException("OpenPCR.AnimKit.Scene.SkeletonRenderer::Initialize(System.Boolean):System.Void: runtime capability is unavailable.");
		}

		public virtual void LateUpdate()
		{
			if (!valid) return;
			throw new global::OpenPCR.OutOfBoundaryException("OpenPCR.AnimKit.Scene.SkeletonRenderer::LateUpdate():System.Void: runtime capability is unavailable.");
		}
	}
}
