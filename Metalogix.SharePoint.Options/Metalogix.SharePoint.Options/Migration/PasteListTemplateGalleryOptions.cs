using Metalogix.SharePoint.Options;
using System;

namespace Metalogix.SharePoint.Options.Migration
{
	public class PasteListTemplateGalleryOptions : SharePointActionOptions
	{
		private bool _copyListTemplateGallery = true;

		public bool CopyListTemplateGallery
		{
			get
			{
				return this._copyListTemplateGallery;
			}
			set
			{
				this._copyListTemplateGallery = value;
			}
		}

		public PasteListTemplateGalleryOptions()
		{
		}
	}
}