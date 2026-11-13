using global::OpenPCR.UICompat;
using global::OpenPCR.Runtime.Api;
using System;
using System.Collections.Generic;

namespace OpenPCR.UICompat
{

	public class UISprite : UIBasicSprite
	{

		public string spriteName
		{
			get
			{
				throw new global::OpenPCR.OutOfBoundaryException("UISprite::spriteName");
			}

			set
			{
				throw new global::OpenPCR.OutOfBoundaryException("UISprite::spriteName");
			}
		}

		public UISprite()
		{
			throw new global::OpenPCR.MigrationPendingException("UISprite::.ctor()", "body migration pending");
		}
	}

}
