using System;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.Options.Migration
{
	public class PasteMasterPagesOptions : PasteMasterPageGalleryOptions
	{
		public string UIVersionToCopyPagesFor
		{
			get;
			set;
		}

		public bool UpdateMasterPagesForUseBySpecificUIVersion
		{
			get;
			set;
		}

		public PasteMasterPagesOptions()
		{
		}
	}
}