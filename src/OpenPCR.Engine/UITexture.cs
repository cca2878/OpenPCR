using global::OpenPCR.UICompat;
using global::OpenPCR.Runtime.Api;
using System;

namespace OpenPCR.UICompat
{

	public class UITexture : UIBasicSprite
	{
		public override Texture mainTexture
		{
			get
			{
				throw new global::OpenPCR.OutOfBoundaryException("UITexture::mainTexture");
			}

			[global::OpenPCR.Migration("8dc9e9b4-4184-4123-a6a3-668698f94169")]
			set
			{
				throw new global::OpenPCR.OutOfBoundaryException("UITexture::mainTexture");
			}
		}

		public override Material material
		{
			get
			{
				throw new global::OpenPCR.OutOfBoundaryException("UITexture::material");
			}

			[global::OpenPCR.Migration("142940aa-7b6a-4ee4-b653-36bd53acef9d")]
			set
			{
				throw new global::OpenPCR.OutOfBoundaryException("UITexture::material");
			}
		}

		public UITexture()
		{
			throw new global::OpenPCR.MigrationPendingException("UITexture::.ctor()", "body migration pending");
		}
	}

}
