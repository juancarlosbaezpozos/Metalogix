using Metalogix.Actions;
using Metalogix.Commands;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Options.Migration;
using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Migration.Commands
{
	[Cmdlet("Copy", "MLSharePointMasterPageGallery")]
	public class CopyMasterPageGalleryCmdlet : CopyListCmdlet
	{
		protected override Type ActionType
		{
			get
			{
				return typeof(CopyMasterPageGalleryAction);
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should include master pages.")]
		public SwitchParameter CopyMasterPages
		{
			get
			{
				return this.PasteMasterPageGalleryOptions.CopyMasterPages;
			}
			set
			{
				this.PasteMasterPageGalleryOptions.CopyMasterPages = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should include other resources in the gallery.")]
		public SwitchParameter CopyOtherResources
		{
			get
			{
				return this.PasteMasterPageGalleryOptions.CopyOtherResources;
			}
			set
			{
				this.PasteMasterPageGalleryOptions.CopyOtherResources = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the copy operation should include page layouts.")]
		public SwitchParameter CopyPageLayouts
		{
			get
			{
				return this.PasteMasterPageGalleryOptions.CopyPageLayouts;
			}
			set
			{
				this.PasteMasterPageGalleryOptions.CopyPageLayouts = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="Indicates if the links within the master page should be corrected or not.")]
		public SwitchParameter CorrectMasterPageLinks
		{
			get
			{
				return this.PasteMasterPageGalleryOptions.CorrectMasterPageLinks;
			}
			set
			{
				this.PasteMasterPageGalleryOptions.CorrectMasterPageLinks = value;
			}
		}

		protected virtual Metalogix.SharePoint.Options.Migration.PasteMasterPageGalleryOptions PasteMasterPageGalleryOptions
		{
			get
			{
				return base.Action.Options as Metalogix.SharePoint.Options.Migration.PasteMasterPageGalleryOptions;
			}
		}

		public CopyMasterPageGalleryCmdlet()
		{
		}
	}
}