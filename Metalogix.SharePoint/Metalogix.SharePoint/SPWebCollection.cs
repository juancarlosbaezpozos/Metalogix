using Metalogix.Actions;
using Metalogix.Core.OperationLog;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.SharePoint.Adapters;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class SPWebCollection : NodeCollection
	{
		private SPWeb m_parentWeb = null;

		public SPWeb ParentWeb
		{
			get
			{
				return this.m_parentWeb;
			}
		}

		public SPWebCollection(SPWeb parentWeb)
		{
			this.m_parentWeb = parentWeb;
		}

		public override void Add(Node item)
		{
			if (!(item is SPWeb))
			{
				throw new Exception("Can't add node because it is not a SPWeb");
			}
			this.AddWeb(item.XML, new AddWebOptions(), null);
		}

		public virtual SPWeb AddWeb(string sWebXML, AddWebOptions addOptions, LogItem logItem = null)
		{
			bool flag;
			string str = null;
			if (AdapterConfigurationVariables.MigrateLanguageSettings)
			{
				sWebXML = XmlUtility.AddLanguageSettingsAttribute(sWebXML, "Web", "Site");
			}
			if (!this.ParentWeb.WriteVirtually)
			{
				if (this.ParentWeb.Adapter.Writer == null)
				{
					throw new Exception("The underlying SharePoint adapter does not support write operations");
				}
				str = this.ParentWeb.Adapter.Writer.AddWeb(sWebXML, addOptions);
			}
			else
			{
				str = sWebXML;
			}
			OperationReportingResult operationReportingResult = new OperationReportingResult(str);
			if (string.IsNullOrEmpty(operationReportingResult.ObjectXml))
			{
				OperationReportingException operationReportingException = new OperationReportingException(string.Format("AddWeb - No ObjectXml [{0}]", operationReportingResult.GetMessageOfFirstErrorElement), operationReportingResult.AllReportElementsAsString);
				throw operationReportingException;
			}
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(operationReportingResult.ObjectXml);
			this.RemoveByName(xmlDocument.FirstChild.Attributes["Name"].Value);
			SPWeb collection = this.AddWebToCollection(this.ParentWeb, xmlDocument.FirstChild, true);
			this.FireNodeCollectionChanged(NodeCollectionChangeType.NodeAdded, collection);
			if (logItem == null)
			{
				flag = true;
			}
			else
			{
				flag = (operationReportingResult.ErrorOccured || operationReportingResult.WarningOccured ? false : !operationReportingResult.HasInformation);
			}
			if (!flag)
			{
				logItem.Information = "Site has been added however some issues were encountered, please review details";
				logItem.Details = operationReportingResult.AllReportElementsAsString;
				if ((operationReportingResult.ErrorOccured ? true : operationReportingResult.WarningOccured))
				{
					logItem.Status = ActionOperationStatus.Warning;
				}
			}
			return collection;
		}

		private SPWeb AddWebToCollection(SPWeb parentWeb, XmlNode webXml, bool shouldSort)
		{
			SharePointAdapter value = parentWeb.Adapter.Clone();
			value.Url = webXml.Attributes["ServerRelativeUrl"].Value;
			if (webXml.Attributes["ID"] != null)
			{
				value.WebID = webXml.Attributes["ID"].Value;
			}
			SPWeb sPWeb = new SPWeb(parentWeb, value, webXml);
			base.AddToCollection(sPWeb);
			if (shouldSort)
			{
				base.Sort(new WebTitleSorter());
			}
			return sPWeb;
		}

		public virtual bool DeleteWeb(string sWebName)
		{
			bool writeVirtually = this.ParentWeb.WriteVirtually;
			if ((this.ParentWeb.Adapter.Writer != null ? false : !writeVirtually))
			{
				throw new Exception("The underlying adapter does not support write operations");
			}
			SPWeb item = (SPWeb)base[sWebName];
			if (!writeVirtually)
			{
				this.ParentWeb.Adapter.Writer.DeleteWeb(item.ServerRelativeUrl);
			}
			bool flag = base.RemoveFromCollection(item);
			this.FireNodeCollectionChanged(NodeCollectionChangeType.NodeRemoved, item);
			return flag;
		}

		public virtual void FetchData()
		{
			XmlDocument xmlDocument = new XmlDocument();
			this.ClearCollection();
			xmlDocument.LoadXml(this.m_parentWeb.Adapter.Reader.GetSubWebs());
			XmlNodeList xmlNodeLists = xmlDocument.SelectNodes("//Web");
			this.RangedAddWebToCollection(this.ParentWeb, xmlNodeLists);
		}

		private void RangedAddWebToCollection(SPWeb parentWeb, XmlNodeList webXmlNodes)
		{
			foreach (XmlNode webXmlNode in webXmlNodes)
			{
				this.AddWebToCollection(this.ParentWeb, webXmlNode, false);
			}
			base.Sort(new WebTitleSorter());
		}

		public override bool Remove(Node item)
		{
			if (!(item is SPWeb))
			{
				throw new Exception("Can't remove node because it is not a SPWeb");
			}
			return this.DeleteWeb(((SPWeb)item).Name);
		}

		public override void RemoveAt(int index)
		{
			SPWeb item = this[index] as SPWeb;
			base.RemoveAt(index);
			this.FireNodeCollectionChanged(NodeCollectionChangeType.NodeRemoved, item);
		}

		private void RemoveByName(string sName)
		{
			int indexByName = base.GetIndexByName(sName);
			if (indexByName >= 0)
			{
				base.RemoveIndex(indexByName);
			}
		}
	}
}