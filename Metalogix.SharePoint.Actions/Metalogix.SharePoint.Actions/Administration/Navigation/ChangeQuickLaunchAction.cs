using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.Licensing;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Options.Administration.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Administration.Navigation
{
	[Image("Metalogix.SharePoint.Actions.Icons.Administration.QuickLaunch.ico")]
	[LaunchAsJob(false)]
	[LicensedProducts(ProductFlags.CMCSharePoint | ProductFlags.CMCFileShare)]
	[Name("Change Quick Launch Settings")]
	[RequiresWriteAccess(true)]
	[RunAsync(true)]
	[ShowInMenus(false)]
	[SourceCardinality(Cardinality.Zero)]
	[TargetCardinality(Cardinality.One)]
	[TargetType(typeof(SPWeb), true)]
	public class ChangeQuickLaunchAction : SharePointAction<ChangeNavigationSettingsOptions>
	{
		public ChangeQuickLaunchAction()
		{
		}

		public void ChangeQuickLaunchSettings(SPWeb webToModify, string sNavSettingsWebXml)
		{
			ModifyNavigationOptions modifyNavigationOption = new ModifyNavigationOptions();
			webToModify.Adapter.Writer.ModifyWebNavigationSettings(sNavSettingsWebXml, modifyNavigationOption);
			if (base.SharePointOptions.ApplyChangesToParentSites)
			{
				this.PushChangesToParentSites(webToModify, sNavSettingsWebXml);
			}
			if (base.SharePointOptions.ApplyChangesToSubSites)
			{
				this.PushChangesToChildSites(webToModify, sNavSettingsWebXml);
			}
		}

		private string GetQuickLaunchSettingsXml()
		{
			StringWriter stringWriter = new StringWriter(new StringBuilder());
			XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
			base.SharePointOptions.ToXML(xmlTextWriter, ChangeNavigationSettingsOptions.NavigationXmlOutput.QuickLaunchSettingsOnly);
			xmlTextWriter.Flush();
			xmlTextWriter.Close();
			return stringWriter.ToString();
		}

		public ChangeNavigationSettingsOptions GetWebNavigationSettings(SPWeb web)
		{
			return new ChangeNavigationSettingsOptions(web.Adapter.Reader.GetWebNavigationSettings(), web.Adapter.SharePointVersion, web.Adapter.DisplayedShortName);
		}

		private void PushChangesToChildSites(SPWeb parentWeb, string sNavSettingsWebXml)
		{
			try
			{
				if (parentWeb != null)
				{
					ModifyNavigationOptions modifyNavigationOption = new ModifyNavigationOptions();
					foreach (SPWeb subWeb in parentWeb.SubWebs)
					{
						subWeb.Adapter.Writer.ModifyWebNavigationSettings(sNavSettingsWebXml, modifyNavigationOption);
						this.PushChangesToChildSites(subWeb, sNavSettingsWebXml);
					}
				}
			}
			catch (ChangeQuickLaunchAction.QuickLaunchNavigationException quickLaunchNavigationException)
			{
				throw quickLaunchNavigationException;
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				throw new ChangeQuickLaunchAction.QuickLaunchNavigationException(string.Concat("An error occurred pushing quick launch navigation changes to subsites. The parent site is '", parentWeb.Url, "'. Error: ", exception.Message), exception);
			}
		}

		private void PushChangesToParentSites(SPWeb childWeb, string sNavSettingsWebXml)
		{
			SPWeb parent = null;
			try
			{
				if (childWeb != null && childWeb.Parent != null && childWeb.Parent is SPWeb)
				{
					ModifyNavigationOptions modifyNavigationOption = new ModifyNavigationOptions();
					parent = childWeb.Parent as SPWeb;
					parent.Adapter.Writer.ModifyWebNavigationSettings(sNavSettingsWebXml, modifyNavigationOption);
					this.PushChangesToParentSites(parent, sNavSettingsWebXml);
				}
			}
			catch (ChangeQuickLaunchAction.QuickLaunchNavigationException quickLaunchNavigationException)
			{
				throw quickLaunchNavigationException;
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				string[] displayUrl = new string[] { "An error occurred pushing quick launch navigation changes to a parent site. The child site is '", childWeb.DisplayUrl, "' and the parent site is '", parent.DisplayUrl, "'. Error: ", exception.Message };
				throw new ChangeQuickLaunchAction.QuickLaunchNavigationException(string.Concat(displayUrl), exception);
			}
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			SPWeb item = target[0] as SPWeb;
			this.ChangeQuickLaunchSettings(item, this.GetQuickLaunchSettingsXml());
		}

		private class QuickLaunchNavigationException : Exception
		{
			public QuickLaunchNavigationException(string sMessage) : base(sMessage)
			{
			}

			public QuickLaunchNavigationException(string sMessage, Exception ex) : base(sMessage, ex)
			{
			}
		}
	}
}