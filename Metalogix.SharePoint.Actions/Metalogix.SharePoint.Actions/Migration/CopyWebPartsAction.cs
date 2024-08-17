using Metalogix.Actions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Actions.Properties;
using Metalogix.SharePoint.Actions.Transform;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Migration;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.Transformers;
using Metalogix.Transformers.Attributes;
using Metalogix.Transformers.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Migration
{
	[MandatoryTransformers(new Type[] { typeof(WebPartsProcessor) })]
	[Name("Paste Web Parts")]
	[ShowInMenus(false)]
	[SourceCardinality(Cardinality.One)]
	[SourceType(typeof(SPListItem), true)]
	[TargetCardinality(Cardinality.One)]
	[TargetType(typeof(SPListItem))]
	public class CopyWebPartsAction : PasteAction<WebPartOptions>
	{
		protected static TransformerDefinition<SPWebPart, PasteAction<WebPartOptions>, SPWebPartCollection, SPWebPartCollection> s_webPartTransformer;

		static CopyWebPartsAction()
		{
			CopyWebPartsAction.s_webPartTransformer = new TransformerDefinition<SPWebPart, PasteAction<WebPartOptions>, SPWebPartCollection, SPWebPartCollection>("SharePoint Web Parts", false);
		}

		public CopyWebPartsAction()
		{
		}

		private void CopyWebParts(SPListItem sourceItem, SPListItem targetItem, LogItem logItem = null)
		{
			if (sourceItem == null || !SPWebPartPage.IsWebPartPage(sourceItem))
			{
				throw new ArgumentException("Error: A web parts copy was attempted without a valid source page.");
			}
			if (targetItem == null || !SPWebPartPage.IsWebPartPage(targetItem))
			{
				throw new ArgumentException("Error: A web parts copy was attempted without a valid target page.");
			}
			this.CopyWebParts(new SPWebPartPage(sourceItem, this), new SPWebPartPage(targetItem, this), null);
		}

		private void CopyWebParts(SPWebPartPage sourceWebPartPage, SPWebPartPage targetWebPartPage, LogItem logItem = null)
		{
			try
			{
				CopyWebPartsAction.s_webPartTransformer.BeginTransformation(this, sourceWebPartPage.WebParts, targetWebPartPage.WebParts, this.Options.Transformers);
				XmlNode xmlNodes = this.ProcessWebPartsForWriting(sourceWebPartPage, targetWebPartPage);
				string str = null;
				if (base.SharePointOptions.CopyContentZoneContent && sourceWebPartPage.Adapter.SharePointVersion.IsSharePoint2010OrLater && targetWebPartPage.Adapter.SharePointVersion.IsSharePoint2010OrLater)
				{
					str = base.LinkCorrector.CorrectHtml(sourceWebPartPage.EmbeddedContentFieldContent);
				}
				if (sourceWebPartPage.WebPartConnections != null && (string.Equals(targetWebPartPage.Adapter.DisplayedShortName, "OM", StringComparison.OrdinalIgnoreCase) || string.Equals(targetWebPartPage.Adapter.DisplayedShortName, "WS", StringComparison.OrdinalIgnoreCase)))
				{
					XmlNode innerXml = xmlNodes.OwnerDocument.CreateNode(XmlNodeType.Element, "webPartConnections", "");
					innerXml.InnerXml = sourceWebPartPage.WebPartConnections.InnerXml;
					xmlNodes.AppendChild(innerXml);
				}
				targetWebPartPage.AddWebParts(xmlNodes.OuterXml, str);
				CopyWebPartsAction.s_webPartTransformer.EndTransformation(this, sourceWebPartPage.WebParts, targetWebPartPage.WebParts, this.Options.Transformers);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				if (logItem == null)
				{
					logItem = new LogItem(Resources.FailedToCopyWebParts, sourceWebPartPage.FileLeafRef, sourceWebPartPage.DisplayUrl, targetWebPartPage.DisplayUrl, ActionOperationStatus.Failed);
					base.FireOperationStarted(logItem);
					logItem.Exception = exception;
					base.FireOperationFinished(logItem);
				}
				else
				{
					logItem.Status = ActionOperationStatus.Failed;
					logItem.Exception = exception;
				}
			}
		}

		protected override List<ITransformerDefinition> GetSupportedDefinitions()
		{
			List<ITransformerDefinition> supportedDefinitions = base.GetSupportedDefinitions();
			supportedDefinitions.Add(CopyWebPartsAction.s_webPartTransformer);
			return supportedDefinitions;
		}

		private XmlNode ProcessWebPartsForWriting(SPWebPartPage sourceWebPartPage, SPWebPartPage targetWebPartPage)
		{
			StringBuilder stringBuilder = new StringBuilder(1024);
			List<SPWebPart> sPWebParts = new List<SPWebPart>();
			foreach (SPWebPart webPart in sourceWebPartPage.WebParts)
			{
				SPWebPart sPWebPart = CopyWebPartsAction.s_webPartTransformer.Transform(webPart, this, sourceWebPartPage.WebParts, targetWebPartPage.WebParts, this.Options.Transformers);
				if (sPWebPart == null)
				{
					continue;
				}
				sPWebParts.Add(sPWebPart);
			}
			sPWebParts.Sort((SPWebPart wp1, SPWebPart wp2) => wp1.PartOrder.CompareTo(wp2.PartOrder));
			foreach (SPWebPart sPWebPart1 in sPWebParts)
			{
				stringBuilder.AppendLine(sPWebPart1.Xml);
			}
			XmlNode str = (new XmlDocument()).CreateElement("WebParts");
			str.InnerXml = stringBuilder.ToString();
			return str;
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			this.InitializeSharePointCopy(source, target, this.ActionOptions.ForceRefresh);
			SPListItem item = source[0] as SPListItem;
			this.CopyWebParts(item, target[0] as SPListItem, null);
		}

		protected override void RunOperation(object[] oParams)
		{
			LogItem logItem;
			if (oParams == null || (int)oParams.Length < 2)
			{
				throw new Exception(string.Format("{0} is missing parameters", this.Name));
			}
			if ((int)oParams.Length < 3)
			{
				logItem = null;
			}
			else
			{
				logItem = oParams[2] as LogItem;
			}
			LogItem logItem1 = logItem;
			if (oParams[0] is SPWebPartPage)
			{
				this.CopyWebParts(oParams[0] as SPWebPartPage, oParams[1] as SPWebPartPage, logItem1);
				return;
			}
			this.CopyWebParts(oParams[0] as SPListItem, oParams[1] as SPListItem, logItem1);
		}
	}
}