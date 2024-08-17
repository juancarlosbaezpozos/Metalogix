using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Migration;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Migration;
using System;
using System.Collections;

namespace Metalogix.SharePoint.Actions.Migration
{
	[CmdletEnabled(true, "Copy-MLSharePointWebParts", new string[] { "Metalogix.SharePoint.Migration.Commands" })]
	[Image("Metalogix.SharePoint.Actions.Icons.Migration.WebParts.ico")]
	[MenuText("3:Paste Site Objects {0-Paste} > Web Parts...")]
	[Name("Paste Web Parts")]
	[RunAsync(true)]
	[ShowStatusDialog(true)]
	[SourceCardinality(Cardinality.One)]
	[SourceType(typeof(SPWeb))]
	[SubActionTypes(typeof(CopyWebPartPageAction))]
	[SupportsThreeStateConfiguration(true)]
	[TargetCardinality(Cardinality.OneOrMore)]
	[TargetType(typeof(SPWeb))]
	public class PasteDefaultWebPartPageAction : PasteAction<WebPartOptions>
	{
		public PasteDefaultWebPartPageAction()
		{
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			using (SPWeb item = source[0] as SPWeb)
			{
				foreach (SPWeb sPWeb in target)
				{
					Node[] nodeArray = new Node[] { sPWeb };
					this.InitializeSharePointCopy(source, new NodeCollection(nodeArray), base.SharePointOptions.ForceRefresh);
					CopyWebPartPageAction copyWebPartPageAction = new CopyWebPartPageAction();
					copyWebPartPageAction.SharePointOptions.CopySiteWebParts = true;
					copyWebPartPageAction.SharePointOptions = (WebPartOptions)this.Options;
					copyWebPartPageAction.LinkCorrector = base.LinkCorrector;
					base.SubActions.Add(copyWebPartPageAction);
					copyWebPartPageAction.LinkCorrector.AddWebMappings(item, sPWeb);
					copyWebPartPageAction.AddGuidMappings(item.ID, sPWeb.ID);
					object[] objArray = new object[] { item, sPWeb };
					copyWebPartPageAction.RunAsSubAction(objArray, new ActionContext(item, sPWeb), null);
				}
			}
		}
	}
}