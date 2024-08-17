using Metalogix;
using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.Threading;
using System;
using System.Collections;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Migration
{
	[CmdletEnabled(true, "Copy-MLSharePointListViews", new string[] { "Metalogix.SharePoint.Migration.Commands" })]
	[Image("Metalogix.SharePoint.Actions.Icons.Migration.Views.ico")]
	[MenuText("3:Paste List Objects {0-Paste} > Views... ")]
	[MenuTextPlural("", PluralCondition.None)]
	[Name("Paste List Views")]
	[RunAsync(true)]
	[ShowStatusDialog(true)]
	[SourceCardinality(Cardinality.One)]
	[SourceType(typeof(SPList), true)]
	[SupportsThreeStateConfiguration(true)]
	[TargetCardinality(Cardinality.OneOrMore)]
	[TargetType(typeof(SPList), true)]
	public class PasteListViewsAction : PasteAction<PasteListViewsOptions>
	{
		public PasteListViewsAction()
		{
		}

		private void CopyListViews(SPList sourceList, SPList targetList)
		{
			if (base.CheckForAbort())
			{
				return;
			}
			LogItem logItem = null;
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(sourceList.XML);
			XmlNode documentElement = xmlDocument.DocumentElement;
			XmlNode xmlNodes = documentElement.SelectSingleNode("./Views").Clone();
			if (documentElement == null || xmlNodes == null || xmlNodes.ChildNodes.Count <= 0)
			{
				logItem = new LogItem("Copying List Views", sourceList.Name, sourceList.Url, targetList.Url, ActionOperationStatus.Running)
				{
					Information = "The source list does not have any views."
				};
				base.FireOperationStarted(logItem);
				logItem.Status = ActionOperationStatus.Skipped;
				base.FireOperationFinished(logItem);
			}
			else
			{
				if (!base.SharePointOptions.OverwriteExistingViews)
				{
					xmlDocument = new XmlDocument();
					xmlDocument.LoadXml(targetList.XML);
					XmlNode documentElement1 = xmlDocument.DocumentElement;
					if (xmlNodes != null && documentElement1 != null)
					{
						XmlNode xmlNodes1 = documentElement1.SelectSingleNode("./Views");
						if (xmlNodes1.ChildNodes.Count > 0)
						{
							foreach (XmlNode childNode in xmlNodes1.ChildNodes)
							{
								string value = childNode.Attributes["Url"].Value;
								string str = value.Substring(value.LastIndexOf('/') + 1);
								str = str.Substring(0, str.IndexOf(".aspx"));
								foreach (XmlNode childNode1 in xmlNodes.ChildNodes)
								{
									string value1 = childNode1.Attributes["Url"].Value;
									string str1 = value1.Substring(value1.LastIndexOf('/') + 1);
									str1 = str1.Substring(0, str1.IndexOf(".aspx"));
									if (!str1.Equals(str, StringComparison.OrdinalIgnoreCase))
									{
										continue;
									}
									xmlNodes.RemoveChild(childNode1);
									break;
								}
							}
						}
					}
				}
				if (xmlNodes.ChildNodes.Count <= 0)
				{
					logItem = new LogItem("Copying List Views", sourceList.Name, sourceList.Url, targetList.Url, ActionOperationStatus.Running)
					{
						Information = "The source list does not have any new views."
					};
					base.FireOperationStarted(logItem);
					logItem.Status = ActionOperationStatus.Skipped;
					base.FireOperationFinished(logItem);
				}
				else
				{
					try
					{
						try
						{
							logItem = new LogItem("Adding List Views", "List Views", sourceList.DisplayUrl, targetList.DisplayUrl, ActionOperationStatus.Running);
							base.FireOperationStarted(logItem);
							long count = (long)xmlNodes.ChildNodes.Count * SPObjectSizes.GetObjectTypeSize(typeof(SPView));
							string outerXml = xmlNodes.OuterXml;
							if (targetList.Adapter.SharePointVersion.IsSharePoint2010OrLater)
							{
								outerXml = MigrationUtils.UpdateCheckedOutToFieldRefsInViewsXml(outerXml);
							}
							targetList.UpdateList(sourceList.XML, outerXml, false, true);
							logItem.LicenseDataUsed = count;
							logItem.Status = ActionOperationStatus.Completed;
						}
						catch (Exception exception1)
						{
							Exception exception = exception1;
							logItem.Exception = exception;
							logItem.Status = ActionOperationStatus.Failed;
							logItem.Information = string.Concat("Exception thrown: ", exception.Message);
							logItem.Details = exception.StackTrace;
						}
					}
					finally
					{
						base.FireOperationFinished(logItem);
					}
				}
			}
			sourceList.Dispose();
			targetList.Dispose();
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			if (source[0] is SPList && target[0] is SPList)
			{
				SPList item = (SPList)source[0];
				SPList sPList = (SPList)target[0];
				this.CopyListViews(item, sPList);
				if (base.SharePointOptions.CopyViewWebParts)
				{
					WebPartOptions webPartOption = new WebPartOptions();
					webPartOption.SetFromOptions(base.SharePointOptions);
					base.QueueListViewWebPartCopies(item, sPList, webPartOption);
					base.ThreadManager.SetBufferedTasks(base.GetWebPartCopyBufferKey(sPList.ParentWeb), false, false);
				}
			}
		}
	}
}