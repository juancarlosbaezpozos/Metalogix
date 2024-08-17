using Metalogix.Core.OperationLog;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Nintex;
using Metalogix.SharePoint.Properties;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class SPListCollection : SPFolderCollection
	{
		private SPWeb _parentWeb = null;

		public new SPList this[string sName]
		{
			get
			{
				SPList item;
				int indexByName = base.GetIndexByName(sName);
				if (indexByName >= 0)
				{
					item = (SPList)this[indexByName];
				}
				else
				{
					item = null;
				}
				return item;
			}
		}

		public SPWeb ParentWeb
		{
			get
			{
				return this._parentWeb;
			}
		}

		public SPListCollection(SPWeb parentWeb) : base(null, null)
		{
			this._parentWeb = parentWeb;
		}

		public override void Add(Node item)
		{
			if (!(item is SPList))
			{
				if (!(item is SPFolder))
				{
					throw new Exception("The node being added is not a SPFolder or SPList");
				}
				base.AddFolder(item.XML, new AddFolderOptions(), AddFolderMode.Comprehensive);
			}
			else
			{
				this.AddList(item.XML, new AddListOptions());
			}
		}

		public SPList AddList(string sListXML, AddListOptions options)
		{
			return this.AddList(sListXML, null, options).ResultObject;
		}

		public OperationReportingResultObject<SPList> AddList(string sListXML, byte[] documentTemplateFile, AddListOptions options)
		{
			SPList sPList;
			string str = null;
			string objectXml = null;
			if (AdapterConfigurationVariables.MigrateLanguageSettings)
			{
				sListXML = XmlUtility.AddLanguageSettingsAttribute(sListXML, "List", null);
				sListXML = XmlUtility.AddLanguageSettingsAttribute(sListXML, "List/Fields/Field", null);
			}
			if (!this.ParentWeb.WriteVirtually)
			{
				if (this.ParentWeb.Adapter.Writer == null)
				{
					throw new Exception("The underlying SharePoint adapter does not support write operations");
				}
				str = this.ParentWeb.Adapter.Writer.AddList(sListXML, options, documentTemplateFile);
				if ((!AdapterConfigurationVariables.MigrateLanguageSettings || !AdapterConfigurationVariables.MigrateLanguageSettingForViews ? false : options.CopyViews))
				{
					SPUtils.SetLanguageResourcesCollection(sListXML, this.ParentWeb);
				}
				OperationReportingResult operationReportingResult = new OperationReportingResult(str);
				if (string.IsNullOrEmpty(operationReportingResult.ObjectXml))
				{
					OperationReportingException operationReportingException = new OperationReportingException(string.Format("AddList - Error [{0}]", operationReportingResult.GetMessageOfFirstErrorElement), operationReportingResult.AllReportElementsAsString);
					throw operationReportingException;
				}
				objectXml = operationReportingResult.ObjectXml;
			}
			else
			{
				objectXml = sListXML;
			}
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(objectXml);
			if (!(xmlDocument.FirstChild.Attributes["BaseTemplate"].Value == "108"))
			{
				sPList = new SPList(this.ParentWeb, xmlDocument.FirstChild);
			}
			else
			{
				sPList = new SPDiscussionList(this.ParentWeb, xmlDocument.FirstChild);
			}
			this.RemoveByName(sPList.Name);
			base.AddToCollection(sPList);
			base.Sort(new ListTitleSorter());
			this.FireNodeCollectionChanged(NodeCollectionChangeType.NodeAdded, sPList);
			return new OperationReportingResultObject<SPList>(str)
			{
				ResultObject = sPList
			};
		}

		public bool DeleteList(string sListID)
		{
			bool flag;
			bool writeVirtually = this.ParentWeb.WriteVirtually;
			if ((this._parentWeb.Adapter.Writer != null ? false : !writeVirtually))
			{
				throw new Exception(Resources.TargetIsReadOnly);
			}
			if (this.GetListIndexByGUID(sListID) != -1)
			{
				if (!writeVirtually)
				{
					this.ParentWeb.Adapter.Writer.DeleteList(sListID);
				}
				SPNode sPNode = this.RemoveByID(sListID);
				this.FireNodeCollectionChanged(NodeCollectionChangeType.NodeRemoved, sPNode);
				if (sPNode != null)
				{
					flag = true;
					return flag;
				}
			}
			flag = false;
			return flag;
		}

		public override void FetchData()
		{
			string[] strArrays;
			bool flag;
			bool flag1;
			XmlDocument xmlDocument = new XmlDocument();
			this.ClearCollection();
			xmlDocument.LoadXml(this._parentWeb.Adapter.Reader.GetLists());
			foreach (XmlNode xmlNodes in xmlDocument.SelectNodes("//List"))
			{
				if (xmlNodes.Attributes["DirName"].Value.EndsWith("_catalogs"))
				{
					flag = false;
				}
				else
				{
					strArrays = new string[] { "110", "200" };
					flag = !xmlNodes.IsAttributeValueInSet("BaseTemplate", strArrays);
				}
				if (flag)
				{
					strArrays = new string[] { "Relationships List", "TaxonomyHiddenList", "WmaAggregatorList_User", "ProjectPolicyItemList", "ContentTypeSyncLog" };
					if ((!xmlNodes.IsAttributeValueInSet("Name", strArrays) || !(xmlNodes.Attributes["BaseTemplate"].Value == "100") ? true : !string.Equals(xmlNodes.GetAttributeValueAsString("Hidden"), "True", StringComparison.OrdinalIgnoreCase)))
					{
						strArrays = new string[] { "PublishedFeed", "Social" };
						if (!xmlNodes.IsAttributeValueInSet("Name", strArrays))
						{
							flag1 = true;
						}
						else
						{
							strArrays = new string[] { "544", "550" };
							flag1 = !xmlNodes.IsAttributeValueInSet("BaseTemplate", strArrays);
						}
						if (!flag1)
						{
							continue;
						}
					}
					else
					{
						continue;
					}
				}
				else if (!xmlNodes.GetAttributeValueAsString("Name").Equals("wfpub", StringComparison.InvariantCultureIgnoreCase))
				{
					continue;
				}
				if (xmlNodes.Attributes["BaseTemplate"].Value == "108")
				{
					base.AddToCollection(new SPDiscussionList(this.ParentWeb, xmlNodes));
				}
				else if ((xmlNodes.Attributes["BaseTemplate"].Value != "5001" ? true : !xmlNodes.GetAttributeValueAsString("Title").Equals("NintexWorkflows", StringComparison.InvariantCultureIgnoreCase)))
				{
					base.AddToCollection(new SPList(this.ParentWeb, xmlNodes));
				}
				else
				{
					base.AddToCollection(new SPNintexWorkflowList(this.ParentWeb, xmlNodes));
				}
			}
			this._hasFetched = true;
		}

		public IList<SPList> GetDependencies()
		{
			IList<SPList> sPLists = new List<SPList>();
			foreach (SPList sPList in this)
			{
				try
				{
					if (sPList.HasDependencies)
					{
						SPList[] dependencies = this.GetDependencies(sPList);
						for (int i = 0; i < (int)dependencies.Length; i++)
						{
							SPList sPList1 = dependencies[i];
							if (!sPLists.Contains(sPList1))
							{
								sPLists.Add(sPList1);
							}
						}
					}
				}
				catch
				{
				}
			}
			return sPLists;
		}

		public SPList[] GetDependencies(SPList list)
		{
			ArrayList arrayLists = new ArrayList();
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(list.XML);
			foreach (XmlNode xmlNodes in xmlDocument.SelectNodes("//Field[@Type='Lookup' or @Type='LookupMulti']"))
			{
				if (xmlNodes.Attributes["TargetListName"] == null)
				{
					continue;
				}
				SPList listByTitle = this.GetListByTitle(xmlNodes.Attributes["TargetListName"].Value);
				if ((listByTitle == null ? false : !arrayLists.Contains(listByTitle)))
				{
					arrayLists.Add(listByTitle);
				}
			}
			SPList[] sPListArray = new SPList[arrayLists.Count];
			arrayLists.CopyTo(sPListArray);
			return sPListArray;
		}

		public SPList GetListByGuid(string sListID)
		{
			SPList item;
			int listIndexByGUID = this.GetListIndexByGUID(sListID);
			if (listIndexByGUID >= 0)
			{
				item = (SPList)this[listIndexByGUID];
			}
			else
			{
				item = null;
			}
			return item;
		}

		public SPList GetListByServerRelativeUrl(string serverRelativeUrl)
		{
			SPList sPList;
			SPList sPList1 = (SPList)this.Where<Node>((Node lst) => {
				string str2 = lst.ServerRelativeUrl;
				char[] chrArray = new char[] { '/' };
				string str = str2.TrimStart(chrArray);
				string str1 = serverRelativeUrl;
				chrArray = new char[] { '/' };
				return str.Equals(str1.TrimStart(chrArray), StringComparison.OrdinalIgnoreCase);
			}).FirstOrDefault<Node>();
			if (sPList1 == null)
			{
				sPList = null;
			}
			else
			{
				sPList = sPList1;
			}
			return sPList;
		}

		public SPList GetListByTitle(string sTitle)
		{
			SPList sPList;
			int num = 0;
			while (true)
			{
				if (num < base.Count)
				{
					SPList item = (SPList)this[num];
					if (!item.Title.Equals(sTitle, StringComparison.OrdinalIgnoreCase))
					{
						num++;
					}
					else
					{
						sPList = item;
						break;
					}
				}
				else
				{
					sPList = null;
					break;
				}
			}
			return sPList;
		}

		private int GetListIndexByGUID(string sListID)
		{
			int num;
			if (sListID != null)
			{
				int num1 = 0;
				while (num1 < base.Count)
				{
					if (!(((SPList)this[num1]).ID.ToUpper() == sListID.ToUpper()))
					{
						num1++;
					}
					else
					{
						num = num1;
						return num;
					}
				}
			}
			num = -1;
			return num;
		}

		private int GetListIndexByName(string sName)
		{
			int num;
			int num1 = 0;
			while (true)
			{
				if (num1 >= base.Count)
				{
					num = -1;
					break;
				}
				else if (!(((SPList)this[num1]).Name == sName))
				{
					num1++;
				}
				else
				{
					num = num1;
					break;
				}
			}
			return num;
		}

		public IEnumerable<SPList> GetListsByListTemplate(ListTemplateType template)
		{
			bool flag;
			foreach (SPList sPList in this)
			{
				flag = (sPList == null ? false : sPList.BaseTemplate == template);
				if (flag)
				{
					yield return sPList;
				}
			}
		}

		public override bool Remove(Node item)
		{
			bool flag;
			if (!(item is SPList))
			{
				if (!(item is SPFolder))
				{
					throw new Exception("The node being removed is not a SPFolder or SPList");
				}
				flag = base.DeleteFolder(((SPFolder)item).Name);
			}
			else
			{
				flag = this.DeleteList(((SPList)item).ID);
			}
			return flag;
		}

		private SPNode RemoveByID(string sListID)
		{
			SPNode item = null;
			int listIndexByGUID = this.GetListIndexByGUID(sListID);
			if (listIndexByGUID >= 0)
			{
				item = (SPNode)this[listIndexByGUID];
				base.RemoveIndex(listIndexByGUID);
			}
			return item;
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