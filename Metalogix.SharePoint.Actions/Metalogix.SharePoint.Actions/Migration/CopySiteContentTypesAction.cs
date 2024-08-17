using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.Licensing;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Options.Migration;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Metalogix.SharePoint.Actions.Migration
{
	[CmdletEnabled(true, "Copy-MLSharePointContentTypes", new string[] { "Metalogix.SharePoint.Migration.Commands" })]
	[Image("Metalogix.SharePoint.Actions.Icons.Migration.ContentTypes.ico")]
	[LicensedProducts(ProductFlags.CMCSharePoint | ProductFlags.CMCPublicFolder | ProductFlags.CMWebComponents)]
	[MenuText("3:Paste Site Objects {0-Paste} > Content Types...")]
	[Name("Paste Site Content Types")]
	[RunAsync(true)]
	[ShowInMenus(true)]
	[ShowStatusDialog(true)]
	[SourceCardinality(Cardinality.One)]
	[SourceType(typeof(SPWeb))]
	[SupportsThreeStateConfiguration(true)]
	[TargetCardinality(Cardinality.OneOrMore)]
	[TargetType(typeof(SPWeb))]
	public class CopySiteContentTypesAction : CopyContentTypesAction
	{
		public CopySiteContentTypesAction()
		{
		}

		public void CopyContentTypes(SPWeb sourceWeb, SPWeb targetWeb)
		{
			using (sourceWeb)
			{
				LogItem logItem = new LogItem("Initializing Content Types", sourceWeb.Name, sourceWeb.DisplayUrl, targetWeb.DisplayUrl, ActionOperationStatus.Running);
				base.FireOperationStarted(logItem);
				SPContentTypeCollection contentTypes = sourceWeb.ContentTypes;
				SPContentTypeCollection sPContentTypeCollections = targetWeb.ContentTypes;
				logItem.Status = ActionOperationStatus.Completed;
				base.FireOperationFinished(logItem);
				base.CopyReferencedSiteColumns(sourceWeb, targetWeb);
				base.CopyWebContentTypes(sourceWeb.ContentTypes, targetWeb.ContentTypes);
				if (base.SharePointOptions.Recursive)
				{
					foreach (SPWeb subWeb in sourceWeb.SubWebs)
					{
						if (!base.CheckForAbort())
						{
							SPWeb item = (SPWeb)targetWeb.SubWebs[subWeb.Name];
							if (item != null)
							{
								this.CopyContentTypes(subWeb, item);
							}
							else
							{
								LogItem logItem1 = new LogItem("Copying Content Types", subWeb.Name, subWeb.DisplayUrl, null, ActionOperationStatus.MissingOnTarget)
								{
									Information = string.Concat("The site: '", subWeb.Name, "' does not exist on the target")
								};
								base.FireOperationStarted(logItem1);
							}
						}
						else
						{
							return;
						}
					}
				}
			}
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			using (SPWeb item = source[0] as SPWeb)
			{
				foreach (SPWeb sPWeb in target)
				{
					using (sPWeb)
					{
						this.CopyContentTypes(item, sPWeb);
					}
				}
			}
		}
	}
}