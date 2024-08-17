using Metalogix.Actions;
using Metalogix.Commands;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Options.Migration;
using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Migration.Commands
{
	[Cmdlet("Copy", "MLSharePointListTemplateGallery")]
	public class CopyListTemplateGalleryCmdlet : CopyListCmdlet
	{
		protected override Type ActionType
		{
			get
			{
				return typeof(CopyListTemplateGalleryAction);
			}
		}

		[Parameter(Mandatory=true, HelpMessage="Indicates if the list template gallery should be copied.")]
		public SwitchParameter CopyListTemplateGallery
		{
			get
			{
				return this.PasteListTemplateGalleryOptions.CopyListTemplateGallery;
			}
			set
			{
				this.PasteListTemplateGalleryOptions.CopyListTemplateGallery = value;
			}
		}

		protected virtual Metalogix.SharePoint.Options.Migration.PasteListTemplateGalleryOptions PasteListTemplateGalleryOptions
		{
			get
			{
				return base.Action.Options as Metalogix.SharePoint.Options.Migration.PasteListTemplateGalleryOptions;
			}
		}

		public CopyListTemplateGalleryCmdlet()
		{
		}
	}
}