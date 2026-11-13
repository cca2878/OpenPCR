using global::OpenPCR.AnimKit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace OpenPCR.AnimKit
{
	public class Atlas : IEnumerable<AtlasRegion>, IEnumerable
	{
		private readonly List<AtlasPage> pages = new List<AtlasPage>();
		private List<AtlasRegion> regions = new List<AtlasRegion>();
		private TextureLoader textureLoader;

		public Atlas(List<AtlasPage> pages, List<AtlasRegion> regions)
		{
			this.pages = pages;
			this.regions = regions;
			textureLoader = null;
		}

		public IEnumerator<AtlasRegion> GetEnumerator()
		{
			throw new global::OpenPCR.MigrationPendingException("OpenPCR.AnimKit.Atlas::GetEnumerator():System.Collections.Generic.IEnumerator`1<OpenPCR.AnimKit.AtlasRegion>", "body migration pending");
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new global::OpenPCR.MigrationPendingException("OpenPCR.AnimKit.Atlas::System.Collections.IEnumerable.GetEnumerator():System.Collections.IEnumerator", "body migration pending");
		}

		public AtlasRegion FindRegion(string name)
		{
			int i = 0;
			for (int count = regions.Count; i < count; i++)
			{
				if (regions[i].name == name)
				{
					return regions[i];
				}
			}

			return null;
		}
	}
}
