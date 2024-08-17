using Metalogix.SharePoint.Options;
using System;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.Options.Migration
{
	public class PasteListFormsOptions : SharePointActionOptions
	{
		public bool CopyCustomizedFormPages
		{
			get;
			set;
		}

		public bool CopyFormWebParts
		{
			get;
			set;
		}

		public PasteListFormsOptions()
		{
		}
	}
}