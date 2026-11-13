using global::OpenPCR.Runtime.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Elements
{
	public class ColorShader
	{
		private Color curColor = Color.white;
		private Color curColorOffset = Color.white;

		public Color CurColor
		{
			[global::OpenPCR.Migration("452a02dd-b0fa-4dfc-8abc-3fe60141fc52")]
			get { return curColor; }
			[global::OpenPCR.Migration("4823241b-7fc7-4e3d-b12e-17b79ef098e0")]
			set { curColor = value; }
		}

		public Color CurColorOffset
		{
			[global::OpenPCR.Migration("b44ccf1a-4583-47b9-93ca-284061686ec1")]
			get { return global::OpenPCR.Presentation.Noop<Color>(); }
			[global::OpenPCR.Migration("61d4254c-dc5e-4c86-b7d5-63d67a068151")]
			set { global::OpenPCR.Presentation.Noop(); }
		}

		[global::OpenPCR.Migration("90550c93-432b-477d-837b-8bb0fe60013f")]
		public ColorShader()
		{
		}
	}
}
