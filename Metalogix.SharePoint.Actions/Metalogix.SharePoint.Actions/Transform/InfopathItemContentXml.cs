using Metalogix.Actions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Actions.Properties;
using Metalogix.SharePoint.Migration;
using Metalogix.SharePoint.Options;
using Metalogix.Transformers;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Transform
{
	public class InfopathItemContentXml : PreconfiguredTransformer<SPListItem, PasteListItemAction, SPListItemCollection, SPListItemCollection>
	{
		public override string Name
		{
			get
			{
				return "Infopath Item Content Link Corrector";
			}
		}

		public InfopathItemContentXml()
		{
		}

		public override void BeginTransformation(PasteListItemAction action, SPListItemCollection sources, SPListItemCollection targets)
		{
		}

		public override void EndTransformation(PasteListItemAction action, SPListItemCollection sources, SPListItemCollection targets)
		{
		}

		public override SPListItem Transform(SPListItem dataObject, PasteListItemAction action, SPListItemCollection sources, SPListItemCollection targets)
		{
			SPListItem sPListItem;
			if (action.SharePointOptions.CorrectingLinks && dataObject.Name.Contains(".xml") && (dataObject.ParentList.BaseTemplate == ListTemplateType.FormLibrary || dataObject.IsInfoPathDocument))
			{
				bool flag = false;
				LogItem logItem = null;
				StringBuilder stringBuilder = null;
				if (action.SharePointOptions.Verbose)
				{
					stringBuilder = new StringBuilder(512);
				}
				try
				{
					try
					{
						int num = (dataObject.Binary == null ? 0 : (int)dataObject.Binary.Length);
						if (num > 1)
						{
							MemoryStream memoryStream = new MemoryStream(dataObject.Binary);
							XmlReaderSettings xmlReaderSetting = new XmlReaderSettings()
							{
								CloseInput = true
							};
							using (XmlReader xmlReader = XmlReader.Create(memoryStream, xmlReaderSetting))
							{
								XmlDocument xmlDocument = new XmlDocument();
								xmlDocument.Load(xmlReader);
								XmlProcessingInstruction xmlProcessingInstruction = xmlDocument.SelectSingleNode("processing-instruction('mso-infoPathSolution')") as XmlProcessingInstruction;
								if (xmlProcessingInstruction != null)
								{
									Match match = Regex.Match(xmlProcessingInstruction.Data, "href=\"(?<url>.*?)\"");
									if (match.Success && match.Groups["url"].Success)
									{
										string str = action.LinkCorrector.CorrectUrl(match.Groups["url"].Value);
										xmlProcessingInstruction.Data = xmlProcessingInstruction.Data.Replace(match.Groups["url"].Value, str);
										dataObject.Binary = Encoding.UTF8.GetBytes(xmlDocument.OuterXml);
										flag = true;
										if (stringBuilder != null)
										{
											stringBuilder.AppendLine(string.Format(Resources.FS_CorrectedInfoPathHREF, match.Groups["url"].Value, str));
										}
									}
								}
							}
							if (action.SharePointOptions.Verbose && flag)
							{
								logItem = new LogItem(this.Name, Resources.Transform, dataObject.Name, string.Empty, ActionOperationStatus.Completed)
								{
									Information = Resources.SuccessReviewDetails,
									Details = stringBuilder.ToString()
								};
								base.FireOperationStarted(logItem);
							}
						}
						else
						{
							LogItem logItem1 = new LogItem(this.Name, Resources.Transform, dataObject.Name, string.Empty, ActionOperationStatus.Running)
							{
								Information = "Skip InfoPath Xml", //Resources.SkippingInfoPathXmlFileAsEmpty,
								Details = string.Format("Binary size was {0} bytes", num),
								Status = ActionOperationStatus.Warning
							};
							base.FireOperationStarted(logItem1);
							base.FireOperationFinished(logItem1);
							sPListItem = null;
							return sPListItem;
						}
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						if (logItem == null)
						{
							logItem = new LogItem(this.Name, Resources.Transform, dataObject.Name, string.Empty, ActionOperationStatus.Running);
							base.FireOperationStarted(logItem);
						}
						logItem.Exception = exception;
					}
					return dataObject;
				}
				finally
				{
					if (logItem != null)
					{
						base.FireOperationFinished(logItem);
					}
				}
				return sPListItem;
			}
			return dataObject;
		}
	}
}