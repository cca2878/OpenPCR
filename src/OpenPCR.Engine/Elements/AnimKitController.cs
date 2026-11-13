using global::OpenPCR.AnimKit;
using global::OpenPCR.AnimKit.Scene;
using global::OpenPCR.Runtime.Api;
using global::OpenPCR.Foundation;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Elements
{
	public class AnimKitController : SkeletonAnimation
	{

		private Skin DefaultSkin;
		private Dictionary<Skin.AttachmentKeyTuple, Attachment> defaultAttachmentDic = new Dictionary<Skin.AttachmentKeyTuple, Attachment>();
		protected global::OpenPCR.AnimKit.AnimationState.TrackEntryDelegate startEndDelegate;
		protected global::OpenPCR.AnimKit.AnimationState.TrackEntryDelegate startStartDelegate;
		protected global::OpenPCR.AnimKit.AnimationState.TrackEntryEventDelegate eventDelegate;
		private ColorShader colorShader = new ColorShader();
		protected bool isSyncUpdate;

		public bool IsPlayAnime { get; protected set; }
		public string AnimeName { get; protected set; }

		public int Depth
		{
			[global::OpenPCR.Migration("abc04c2e-a1ca-40a0-9ae1-670b84b54936")]
			get
			{
				return global::OpenPCR.Presentation.Noop<int>();
			}

			[global::OpenPCR.Migration("0bc4bbdf-7e89-41a9-9d46-4294170f9141")]
			set
			{
				global::OpenPCR.Presentation.Noop();
			}
		}

		public Color CurColor
		{
			[global::OpenPCR.Migration("2078daa4-957f-4b4f-8d79-1e178e2d5c0a")]
			get
			{
				return colorShader.CurColor;
			}

			[global::OpenPCR.Migration("a64fa5db-a5aa-497e-a70e-8ed954c26e17")]
			set
			{
				colorShader.CurColor = value;
			}
		}

		public Color CurColorOffset
		{
			get
			{
				return colorShader.CurColorOffset;
			}

			set
			{
				colorShader.CurColorOffset = value;
			}
		}

		protected Shader defaultShader { get; private set; }

		[global::OpenPCR.Migration("9471b2df-d6da-4c88-86d5-77a1ce757977")]
		public AnimKitController()
		{

		}

		[global::OpenPCR.Migration("1c7a9e16-1d66-4809-b283-cc0961d55e69")]
		public override void Update()
		{
			if (!isSyncUpdate)
			{
				base.Update();
			}
		}


		[global::OpenPCR.Migration("5c29034c-608e-430f-8deb-a6349d909328")]
		public void ChangeDefaultShader()
		{
			ChangeShader(defaultShader);
		}

		public void ChangeShader(Shader _shader)
		{
			curShader = _shader;
		}

		[global::OpenPCR.Migration("b2ecf671-df29-4fda-b862-4913a1c46886")]
		public virtual void PlayAnime(string _playAnimeName, bool _playLoop = true)
		{
			loop = _playLoop;
			base.AnimationName = _playAnimeName;
		}

		[global::OpenPCR.Migration("e5a1d8b4-d009-45d8-a120-3f5512c1e88b")]
		public void PlayAnimeNoOverlap(string _playAnimeName, bool _playLoop = true)
		{
			if (!(AnimeName == _playAnimeName))
			{
				PlayAnime(_playAnimeName, _playLoop);
			}
		}

		[global::OpenPCR.Migration("3e725ea4-74b5-4742-8d85-469271123528")]
		public bool IsAnimation(string _animeName)
		{
			return skeleton.data.IsAnimation(_animeName);
		}

		protected virtual void EntryCallBack()
		{
			startEndDelegate = delegate (TrackEntry _animationState)
			{
				if (_animationState.Animation.Name == base.AnimationName)
				{
					IsPlayAnime = false;
				}
			};
			state.Complete += startEndDelegate;
			startStartDelegate = delegate
			{
				IsPlayAnime = true;
				AnimeName = base.AnimationName;
			};
			state.Start += startStartDelegate;
		}
	}
}
