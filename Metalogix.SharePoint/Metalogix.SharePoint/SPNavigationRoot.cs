using Metalogix.Core.OperationLog;
using Metalogix.SharePoint.Adapters;
using Metalogix.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class SPNavigationRoot : SPNavigationNode
	{
		private SPWeb m_parentWeb;

		public SPWeb ParentWeb
		{
			get
			{
				return this.m_parentWeb;
			}
		}

		public SPNavigationNode QuickLaunchNode
		{
			get
			{
				SPNavigationNode nodeByIDRecursive;
				if (!this.ParentWeb.Adapter.SharePointVersion.IsSharePoint2003)
				{
					nodeByIDRecursive = this.GetNodeByIDRecursive(SPNavigationNode.QUICK_LAUNCH_NODE_ID, this);
				}
				else
				{
					nodeByIDRecursive = this;
				}
				return nodeByIDRecursive;
			}
		}

		public SPNavigationNode TopNavigationBarNode
		{
			get
			{
				return this.GetNodeByIDRecursive(SPNavigationNode.TOP_NAV_BAR_NODE_ID, this);
			}
		}

		public SPNavigationRoot(SPWeb web)
		{
			base.BeginUpdates();
			this.m_parentWeb = web;
			string webNavigationStructure = web.Adapter.Reader.GetWebNavigationStructure();
			base.InitializePrivateFieldValues(XmlUtility.StringToXmlNode(webNavigationStructure));
			base.EndUpdates();
		}

		private static void AddNavigationElements(object attributeValue, StringBuilder changeBuilder, string elementName, string attributeName)
		{
			StringBuilder stringBuilder = new StringBuilder();
			XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, XmlUtility.WriterSettings);
			try
			{
				xmlWriter.WriteStartElement(elementName);
				xmlWriter.WriteAttributeString(attributeName, Convert.ToString(attributeValue));
				xmlWriter.WriteEndElement();
				xmlWriter.Flush();
			}
			finally
			{
				if (xmlWriter != null)
				{
					((IDisposable)xmlWriter).Dispose();
				}
			}
			changeBuilder.Append(stringBuilder.ToString());
		}

		public OperationReportingResult CommitChanges(StringBuilder navigationChangesCopy, bool isSourceCSOMorNWS, bool isTargetCSOM, bool isPublishingFeatureEnabled)
		{
			if (navigationChangesCopy == null)
			{
				throw new ArgumentNullException("navigationChangesCopy");
			}
			OperationReportingResult operationReportingResult = null;
			bool flag = false;
			try
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("<Changes>");
				StringBuilder stringBuilder1 = new StringBuilder();
				XmlWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder1));
				StringBuilder stringBuilder2 = new StringBuilder();
				XmlWriter xmlWriter = new XmlTextWriter(new StringWriter(stringBuilder2));
				xmlTextWriter.WriteStartElement("AdditionsAndUpdates");
				xmlWriter.WriteStartElement("Deletions");
				base.WriteChanges(xmlTextWriter, xmlWriter);
				xmlTextWriter.WriteEndElement();
				xmlWriter.WriteEndElement();
				xmlTextWriter.Flush();
				xmlWriter.Flush();
				stringBuilder.Append(stringBuilder1.ToString());
				stringBuilder.Append(stringBuilder2.ToString());
				if (isTargetCSOM)
				{
					SPNavigationRoot.AddNavigationElements(isSourceCSOMorNWS, stringBuilder, "SoureAdapterInformation", "IsCSOMOrNWS");
					bool flag1 = (!AdapterConfigurationVariables.MigrateLanguageSettings ? false : AdapterConfigurationVariables.MigrateLanguageSettingForNavigationStructure);
					if (flag1)
					{
						SPNavigationRoot.AddNavigationElements(flag1, stringBuilder, "MigrateLanguageSettingForNavigationStructure", "IsMigrateLanguageSettingForNavigationStructure");
						SPNavigationRoot.AddNavigationElements(isPublishingFeatureEnabled, stringBuilder, "PublishingFeatureInformation", "IsPublishingFeatureEnabled");
						SPNavigationRoot.AddNavigationElements(AdapterConfigurationVariables.LanguageSettingsMaximumInterval, stringBuilder, "LanguageSettingsMaximumIntervalInformation", "LanguageSettingsMaximumInterval");
						SPNavigationRoot.AddNavigationElements(AdapterConfigurationVariables.LanguageSettingsRefreshInterval, stringBuilder, "LanguageSettingsRefreshIntervalInformation", "LanguageSettingsRefreshInterval");
					}
				}
				stringBuilder.Append("</Changes>");
				navigationChangesCopy.AppendLine("Proposed changes to navigation:");
				navigationChangesCopy.AppendLine(stringBuilder.ToString());
				navigationChangesCopy.AppendLine();
				string str = this.ParentWeb.Adapter.Writer.UpdateWebNavigationStructure(stringBuilder.ToString());
				operationReportingResult = new OperationReportingResult(str);
				if (string.IsNullOrEmpty(operationReportingResult.ObjectXml))
				{
					navigationChangesCopy.AppendLine("No Object Xml, Errors:");
					navigationChangesCopy.AppendLine(operationReportingResult.AllReportElementsAsString);
					OperationReportingException operationReportingException = new OperationReportingException(string.Format("UpdateWebNavigationStructure Error: [{0}]", operationReportingResult.GetMessageOfFirstErrorElement), operationReportingResult.AllReportElementsAsString);
					throw operationReportingException;
				}
				base.UpdateNodeData(XmlUtility.StringToXmlNode(operationReportingResult.ObjectXml));
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					this.RefreshData();
				}
			}
			return operationReportingResult;
		}

		public SPNavigationNode GetNodeByID(int iID)
		{
			SPNavigationNode nodeByIDRecursive;
			if (iID != base.ID)
			{
				nodeByIDRecursive = this.GetNodeByIDRecursive(iID, this);
			}
			else
			{
				nodeByIDRecursive = this;
			}
			return nodeByIDRecursive;
		}
        
	    private SPNavigationNode GetNodeByIDRecursive(int iID, SPNavigationNode currentNode)
	    {
	        SPNavigationNode result;
	        foreach (SPNavigationNode current in currentNode.Children)
	        {
	            if (current.ID == iID)
	            {
	                result = current;
	                return result;
	            }
	        }
	        foreach (SPNavigationNode current in currentNode.Children)
	        {
	            SPNavigationNode nodeByIDRecursive = this.GetNodeByIDRecursive(iID, current);
	            if (nodeByIDRecursive != null)
	            {
	                result = nodeByIDRecursive;
	                return result;
	            }
	        }
	        result = null;
	        return result;
	    }

        public void RefreshData()
		{
			string webNavigationStructure = this.ParentWeb.Adapter.Reader.GetWebNavigationStructure();
			base.UpdateNodeData(XmlUtility.StringToXmlNode(webNavigationStructure));
		}
	}
}