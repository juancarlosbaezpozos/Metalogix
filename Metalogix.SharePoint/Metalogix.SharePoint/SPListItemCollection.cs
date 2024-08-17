using Metalogix.Core.OperationLog;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.ObjectResolution;
using Metalogix.Permissions;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint
{
    public class SPListItemCollection : ListItemCollection
    {
        private const float TargetFetchSize = 400000f;

        private SPList _parentList;

        private SPFolder _parentFolder;

        private int m_iNextFetchSize = 10;

        public override Folder ParentFolder
        {
            get
            {
                return this._parentFolder;
            }
        }

        public override List ParentList
        {
            get
            {
                if (this._parentList == null)
                {
                    if ((base.Count <= 0 || !(this[0] is SPListItem) ? false : ((SPListItem)this[0]).Parent is SPList))
                    {
                        this._parentList = (SPList)((SPListItem)this[0]).Parent;
                    }
                }
                return this._parentList;
            }
        }

        public SPList ParentSPList
        {
            get
            {
                return this._parentList;
            }
        }

        public SPListItemCollection(SPList parentList, SPFolder parentFolder, Node[] items) : base(parentList, parentFolder, items)
        {
            this._parentList = parentList;
            this._parentFolder = parentFolder;
        }

        public SPListItemCollection(XmlNode itemCollectionNode) : base(null, null, null)
        {
            this.FromXML(itemCollectionNode);
        }

        public SPListItemCollection(ListItemCollection baseItemCollection) : base(baseItemCollection.ParentList, baseItemCollection.ParentFolder, baseItemCollection.ToArray())
        {
            if (baseItemCollection.ParentList != null)
            {
                this._parentList = (SPList)baseItemCollection.ParentList;
            }
            else if (base.Count > 0)
            {
                this._parentList = ((SPListItem)this[0]).ParentList;
            }
            if (baseItemCollection.ParentFolder != null)
            {
                this._parentFolder = (SPFolder)baseItemCollection.ParentFolder;
            }
            else if (base.Count > 0)
            {
                this._parentFolder = ((SPListItem)this[0]).ParentFolder;
            }
        }

        public override void Add(Node item)
        {
            byte[][] fileContents;
            string[] fileNames;
            if (!(item is SPListItem))
            {
                throw new Exception("The node being added is not a SPListItem");
            }
            try
            {
                if (((SPListItem)item).ItemType == SPListItemType.Folder)
                {
                    this.AddFolder(item.XML, new AddFolderOptions());
                }
                else if (!((SPListItem)item).ParentList.IsDocumentLibrary)
                {
                    if (((SPListItem)item).HasAttachments)
                    {
                        fileContents = ((SPListItem)item).Attachments.GetFileContents();
                    }
                    else
                    {
                        fileContents = null;
                    }
                    byte[][] numArray = fileContents;
                    if (((SPListItem)item).HasAttachments)
                    {
                        fileNames = ((SPListItem)item).Attachments.GetFileNames();
                    }
                    else
                    {
                        fileNames = null;
                    }
                    string[] strArrays = fileNames;
                    AddListItemOptions addListItemOption = new AddListItemOptions();
                    this.AddItem(((SPListItem)item).ParentRelativePath, item.XML, strArrays, numArray, addListItemOption);
                }
                else
                {
                    AddDocumentOptions addDocumentOption = new AddDocumentOptions()
                    {
                        Overwrite = false
                    };
                    this.AddDocument(item.XML, addDocumentOption, ((SPListItem)item).Binary);
                }
            }
            catch (Exception exception)
            {
                throw new Exception(string.Concat("Error adding node ", exception.Message));
            }
        }

        public SPListItem AddDocument(string sItemXML, AddDocumentOptions Options, byte[] fileContents)
        {
            return this.AddDocument("", sItemXML, Options, fileContents, AddDocumentMode.Comprehensive);
        }

        public SPListItem AddDocument(string sItemXML, AddDocumentOptions Options, byte[] fileContents, AddDocumentMode Mode)
        {
            return this.AddDocument("", sItemXML, Options, fileContents, Mode);
        }

        public SPListItem AddDocument(string sFolder, string sItemXML, AddDocumentOptions AddDocOptions, byte[] fileContents, AddDocumentMode mode = 0)
        {
            SPListItem collection;
            if (!this._parentFolder.WriteVirtually)
            {
                if (this._parentList.Adapter.Writer == null)
                {
                    throw new Exception("The underlying SharePoint adapter does not support write operations");
                }
                string str = null;
                if (mode == AddDocumentMode.Optimistic)
                {
                    FieldsLookUp fieldsSchemaLookup = this._parentList.FieldCollection.GetFieldsSchemaLookup();
                    str = this._parentList.Adapter.Writer.AddDocumentOptimistically(new Guid(this._parentList.ID), this._parentList.Name, this.GetFullPath(sFolder), sItemXML, fileContents, AddDocOptions, ref fieldsSchemaLookup);
                }
                else
                {
                    str = this._parentList.Adapter.Writer.AddDocument(this._parentList.ConstantID, this.GetFullPath(sFolder), sItemXML, fileContents, this._parentList.ListSettingsXML, AddDocOptions);
                }
                OperationReportingResult operationReportingResult = new OperationReportingResult(str);
                if (string.IsNullOrEmpty(operationReportingResult.ObjectXml))
                {
                    OperationReportingException operationReportingException = new OperationReportingException(string.Format("AddDocument - No ObjectXml [{0}]", operationReportingResult.GetMessageOfFirstErrorElement), operationReportingResult.AllReportElementsAsString);
                    throw operationReportingException;
                }
                collection = this.AddItemToCollection(operationReportingResult.ObjectXml, true);
            }
            else
            {
                SPListItem sPListItem = this.AddItemToCollection(sItemXML, true);
                sPListItem.Binary = fileContents;
                collection = sPListItem;
            }
            return collection;
        }

        public SPListItem AddFolder(string sItemXML, AddFolderOptions Options)
        {
            return this.AddFolder("", sItemXML, Options);
        }

        public SPListItem AddFolder(string sItemXML, AddFolderOptions Options, AddFolderMode Mode)
        {
            return this.AddFolder("", sItemXML, Options, Mode);
        }

        public SPListItem AddFolder(string sFolder, string sItemXML, AddFolderOptions Options)
        {
            return this.AddFolder("", sItemXML, Options, AddFolderMode.Comprehensive);
        }

        public SPListItem AddFolder(string sFolder, string sItemXML, AddFolderOptions Options, AddFolderMode Mode)
        {
            if (this._parentList.Adapter.Writer == null)
            {
                throw new Exception("The underlying SharePoint adapter does not support write operations");
            }
            string str = null;
            if (Mode == AddFolderMode.Optimistic)
            {
                FieldsLookUp fieldsSchemaLookup = this._parentList.FieldCollection.GetFieldsSchemaLookup();
                str = this._parentList.Adapter.Writer.AddFolderOptimistically(new Guid(this._parentList.ID), this._parentList.Name, this.GetFullPath(sFolder), sItemXML, Options, ref fieldsSchemaLookup);
            }
            else
            {
                str = this._parentList.Adapter.Writer.AddFolder(this._parentList.ConstantID, this.GetFullPath(sFolder), sItemXML, Options);
            }
            return this.AddItemToCollection(str, false);
        }

        public SPListItem AddItem(string sFolder, string sItemXML, string[] attachmentNames, byte[][] attachmentContents, int? iParentItemID)
        {
            AddListItemOptions addListItemOption = new AddListItemOptions()
            {
                ParentID = iParentItemID
            };
            return this.AddItem(sFolder, sItemXML, attachmentNames, attachmentContents, addListItemOption);
        }

        public SPListItem AddItem(string sFolder, string sItemXML, string[] attachmentNames, byte[][] attachmentContents, AddListItemOptions Options)
        {
            if (this._parentList.Adapter.Writer == null)
            {
                throw new Exception(Resources.TargetIsReadOnly);
            }
            Options.PredictedNextAvailableID = this.ParentSPList.PredictedNextAvailableID;
            string str = this._parentList.Adapter.Writer.AddListItem(this._parentList.ConstantID, this.GetFullPath(sFolder), sItemXML, attachmentNames, attachmentContents, this._parentList.ListSettingsXML, Options);
            OperationReportingResult operationReportingResult = new OperationReportingResult(str);
            if (string.IsNullOrEmpty(operationReportingResult.ObjectXml))
            {
                OperationReportingException operationReportingException = new OperationReportingException(string.Format("AddListItem - No ObjectXml [{0}]", operationReportingResult.GetMessageOfFirstErrorElement), operationReportingResult.AllReportElementsAsString);
                throw operationReportingException;
            }
            return this.AddItemToCollection(operationReportingResult.ObjectXml, true);
        }

        private SPListItem AddItemToCollection(string sListItemXML, bool bOnlyPartialXmlProvided)
        {
            SPListItem sPListItem;
            if (sListItemXML == null)
            {
                throw new Exception("Internal Error: cannot add an empty item.");
            }
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(sListItemXML);
            XmlNode xmlNodes = xmlDocument.SelectSingleNode("//ListItem");
            if (xmlNodes == null)
            {
                throw new Exception("Internal Error: cannot add an empty item.");
            }
            sPListItem = (!bOnlyPartialXmlProvided ? SPListItem.CreateListItem(this._parentList, this._parentFolder, this, xmlNodes) : SPListItem.CreateListItemFromTerseXml(this._parentList, this._parentFolder, this, xmlNodes));
            if ((sPListItem.ItemType != SPListItemType.Folder ? false : this.ParentSPList.HasFolders))
            {
                this.ParentSPList.SubFolders.AddFolderFromListItem(sPListItem);
            }
            bool flag = this.RemoveByID(sPListItem.ID);
            base.AddToCollection(sPListItem);
            this.UpdateParentListStatistics(sPListItem);
            this.FireNodeCollectionChanged((flag ? NodeCollectionChangeType.NodeChanged : NodeCollectionChangeType.NodeAdded), sPListItem);
            return sPListItem;
        }

        public SPListItem CatalogDocumentToStoragePoint(string sSourceFilePath, string sItemXML, AddDocumentOptions addDocOptions)
        {
            bool flag = false;
            bool.TryParse(this._parentList.Adapter.Reader.StoragePointAvailable(string.Empty), out flag);
            if (!flag)
            {
                throw new StoragePointNotAvailableException();
            }
            string storagePointFileShareEndpoint = this._parentList.Adapter.Writer.CatalogDocumentToStoragePointFileShareEndpoint(sSourceFilePath, this._parentList.ID, this.GetFullPath(""), sItemXML, addDocOptions);
            return this.AddItemToCollection(storagePointFileShareEndpoint, true);
        }

        public void CorrectDefaultPageVersions(string sFolder, string sListItemXml)
        {
            this._parentList.Adapter.Writer.CorrectDefaultPageVersions(this._parentList.ConstantID, this.GetFullPath(sFolder), sListItemXml);
        }

        public bool DeleteAllItems()
        {
            if (this._parentList.Adapter.Writer == null)
            {
                throw new Exception(Resources.TargetIsReadOnly);
            }
            this._parentList.Adapter.Writer.DeleteItems(this._parentList.ConstantID, true, null);
            this.ClearCollection();
            this.FireNodeCollectionChanged(NodeCollectionChangeType.FullReset, null);
            return base.Count == 0;
        }

        private bool DeleteAssociateItem(SPListItem item)
        {
            bool flag = this.RemoveByID(item.ID);
            this.FireNodeCollectionChanged(NodeCollectionChangeType.NodeRemoved, item);
            return flag;
        }

        public bool DeleteDiscussionFolder(SPListItem folderItem)
        {
            if (folderItem.ParentList != this.ParentList)
            {
                throw new Exception("The folder to be deleted is not in this collection.");
            }
            string item = folderItem["ContentType"];
            if ((!(folderItem is SPDiscussionItem) ? true : item != "Discussion"))
            {
                throw new Exception("The item to be deleted is not a discussion folder.");
            }
            if (this._parentList.Adapter.Writer == null)
            {
                throw new Exception(Resources.TargetIsReadOnly);
            }
            this._parentList.Adapter.Writer.DeleteFolder(this._parentList.ConstantID, folderItem.ConstantID, folderItem.ConstantFileLeafRef);
            bool flag = this.RemoveByID(folderItem.ID);
            this.FireNodeCollectionChanged(NodeCollectionChangeType.NodeRemoved, folderItem);
            return flag;
        }

        public bool DeleteItem(SPListItem item)
        {
            if (item.ParentList != this.ParentList)
            {
                throw new Exception("The item to be deleted is not in this collection");
            }
            if (this._parentList.Adapter.Writer == null)
            {
                throw new Exception(Resources.TargetIsReadOnly);
            }
            SPListItem matchingItem = null;
            if ((this._parentList.BaseTemplate != ListTemplateType.MasterPageCatalog || !this._parentList.Adapter.SharePointVersion.IsSharePoint2013OrLater ? false : this._parentList.Adapter.IsClientOM))
            {
                XmlAttributeCollection attributes = item.GetNodeXML().Attributes;
                bool flag = (attributes == null ? false : string.Equals(attributes["HtmlDesignAssociated"].Value, "true", StringComparison.OrdinalIgnoreCase));
                if ((!item.FileLeafRef.EndsWith(".html", StringComparison.OrdinalIgnoreCase) ? false : flag))
                {
                    matchingItem = this.GetMatchingItem(item, string.Concat(item.FileLeafRef.Remove(item.FileLeafRef.Length - 5), ".js"), item.ParentRelativePath);
                }
            }
            this._parentList.Adapter.Writer.DeleteItem(this._parentList.ConstantID, item.ConstantID);
            bool flag1 = this.RemoveByID(item.ID);
            this.FireNodeCollectionChanged(NodeCollectionChangeType.NodeRemoved, item);
            if (matchingItem != null)
            {
                this.DeleteAssociateItem(matchingItem);
            }
            return flag1;
        }

        public void DeleteItems(SPListItem[] items)
        {
            SPListItem sPListItem;
            int i;
            if (this.ParentList != this.ParentList)
            {
                throw new Exception("The item to be deleted is not in this collection");
            }
            if (this._parentList.Adapter.Writer == null)
            {
                throw new Exception(Resources.TargetIsReadOnly);
            }
            StringBuilder stringBuilder = new StringBuilder();
            int num = 0;
            SPListItem[] sPListItemArray = items;
            for (i = 0; i < (int)sPListItemArray.Length; i++)
            {
                sPListItem = sPListItemArray[i];
                stringBuilder.Append((stringBuilder.Length > 0 ? "," : ""));
                stringBuilder.Append(sPListItem.ID.ToString());
                if ((this.ParentSPList.Adapter.AdapterShortName != "NW" ? false : sPListItem.ParentList.IsDocumentLibrary))
                {
                    stringBuilder.Append(string.Concat("#", sPListItem.FileRef));
                }
                num++;
                if (num >= 50)
                {
                    this._parentList.Adapter.Writer.DeleteItems(this._parentList.ConstantID, false, stringBuilder.ToString());
                    stringBuilder.Remove(0, stringBuilder.Length);
                    num = 0;
                }
            }
            if (stringBuilder.Length > 0)
            {
                this._parentList.Adapter.Writer.DeleteItems(this._parentList.ConstantID, false, stringBuilder.ToString());
            }
            sPListItemArray = items;
            for (i = 0; i < (int)sPListItemArray.Length; i++)
            {
                sPListItem = sPListItemArray[i];
                int indexByID = this.GetIndexByID(sPListItem.ID);
                if (indexByID != -1)
                {
                    base.RemoveIndex(indexByID);
                    this.FireNodeCollectionChanged(NodeCollectionChangeType.NodeRemoved, sPListItem);
                }
            }
        }

        public void FetchData()
        {
            this.FetchData(false, ListItemQueryType.ListItem, null);
        }

        public void FetchData(bool bRecursive, ListItemQueryType itemTypes)
        {
            this.FetchData(bRecursive, itemTypes, null);
        }

        public void FetchData(bool bRecursive, ListItemQueryType itemType, string sFields)
        {
            this.FetchData(bRecursive, itemType, sFields, true, true);
        }

        public void FetchData(bool bRecursive, ListItemQueryType itemType, string sFields, bool bIncludeExternalizationData, bool bIncludePermissionsInheritance)
        {
            bool flag;
            GetListItemOptions getListItemOption;
            sFields = this.GetFieldsAndOptionsForFetch(sFields, bIncludeExternalizationData, bIncludePermissionsInheritance, out flag, out getListItemOption);
            string listItems = this._parentList.Adapter.Reader.GetListItems(this._parentList.ConstantID, null, sFields, this._parentFolder.ConstantDirName, bRecursive, itemType, this._parentList.GetListXML(false).OuterXml, getListItemOption);
            this.ParseFetchedDataIntoCollection(listItems, flag, false);
        }

        public void FetchDataAt(int iItemIndex)
        {
            if (!((SPListItem)this[iItemIndex]).HasFullXML)
            {
                int num = this.FetchDataForItems(iItemIndex, this.m_iNextFetchSize);
                this.m_iNextFetchSize = (int)(400000f / (float)num * (float)this.m_iNextFetchSize);
                this.m_iNextFetchSize = (this.m_iNextFetchSize == 0 ? 1 : this.m_iNextFetchSize);
                this.m_iNextFetchSize = (this.m_iNextFetchSize > 50 ? 50 : this.m_iNextFetchSize);
            }
        }

        public void FetchDataByQuery(string query, string fields = null, bool includeExternalizationData = true, bool includePermissionsInheritance = true)
        {
            bool flag;
            GetListItemOptions getListItemOption;
            fields = this.GetFieldsAndOptionsForFetch(fields, includeExternalizationData, includePermissionsInheritance, out flag, out getListItemOption);
            string listItemsByQuery = this._parentList.Adapter.Reader.GetListItemsByQuery(this._parentList.ID, fields, query, this._parentList.GetListXML(false).OuterXml, getListItemOption);
            this.ParseFetchedDataIntoCollection(listItemsByQuery, flag, false);
        }

        private int FetchDataForItems(int iItemIndex, int iNumToFetch)
        {
            Dictionary<int, SPListItem> nums = new Dictionary<int, SPListItem>();
            StringBuilder stringBuilder = new StringBuilder();
            int num = iItemIndex;
            while (true)
            {
                if ((num >= iItemIndex + iNumToFetch ? true : num >= base.Count))
                {
                    break;
                }
                SPListItem item = (SPListItem)this[num];
                if (!item.HasFullXML)
                {
                    nums.Add(item.ID, item);
                    stringBuilder.Append(string.Concat((stringBuilder.Length > 0 ? "," : ""), item.ID));
                }
                num++;
            }
            GetListItemOptions getListItemOption = new GetListItemOptions()
            {
                IncludeExternalizationData = true,
                IncludePermissionsInheritance = true
            };
            string listItems = this._parentList.Adapter.Reader.GetListItems(this._parentList.ConstantID, stringBuilder.ToString(), null, null, true, ListItemQueryType.ListItem | ListItemQueryType.Folder, this._parentList.ListSettingsXML, getListItemOption);
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(listItems);
            foreach (XmlNode xmlNodes in xmlDocument.SelectNodes(".//ListItem"))
            {
                int num1 = Convert.ToInt32(xmlNodes.Attributes["ID"].Value);
                if (nums.ContainsKey(num1))
                {
                    nums[num1].SetFullXML(xmlNodes);
                }
            }
            return listItems.Length;
        }

        public void FetchDataWithAllFieldData(bool bRecursive, ListItemQueryType itemTypes)
        {
            GetListItemOptions getListItemOption = new GetListItemOptions()
            {
                IncludeExternalizationData = true,
                IncludePermissionsInheritance = true
            };
            string listItems = this._parentList.Adapter.Reader.GetListItems(this._parentList.ConstantID, null, null, this._parentFolder.ConstantDirName, bRecursive, itemTypes, this._parentList.GetListXML(false).OuterXml, getListItemOption);
            this.ParseFetchedDataIntoCollection(listItems, false, true);
        }

        public void FetchTerseData(bool bRecursive, ListItemQueryType itemTypes, string sFields, GetListItemOptions getOptions)
        {
            this.FetchTerseData(bRecursive, itemTypes, null, sFields, getOptions, true);
        }

        public void FetchTerseData(bool bRecursive, ListItemQueryType itemTypes, string sFields, GetListItemOptions getOptions, bool fireEventsPerItem)
        {
            this.FetchTerseData(bRecursive, itemTypes, null, sFields, getOptions, fireEventsPerItem);
        }

        public void FetchTerseData(bool bRecursive, ListItemQueryType itemTypes, string sIDs, string sFields, GetListItemOptions getOptions, bool fireEventsPerItem)
        {
            if (sFields == null)
            {
                sFields = this.GetDefaultFields();
            }
            string listItems = this._parentList.Adapter.Reader.GetListItems(this._parentList.ConstantID, sIDs, sFields, this._parentFolder.ConstantDirName, bRecursive, itemTypes, this._parentList.GetListXML(false).OuterXml, getOptions);
            this.ParseFetchedDataIntoCollection(listItems, true, fireEventsPerItem);
        }

        public override void FireNodeCollectionChanged(NodeCollectionChangeType changeType, Node changedNode)
        {
            base.FireNodeCollectionChanged(changeType, changedNode);
            this._parentFolder.CollectionChanged(this);
        }

        public override void FromXML(XmlNode xmlNode)
        {
            XmlNode xmlNodes = xmlNode.SelectSingleNode("./ParentList/Location");
            this._parentList = (SPList)(new Location(xmlNodes)).GetNode();
            XmlNode xmlNodes1 = xmlNode.SelectSingleNode("./ParentFolder/Location");
            this._parentFolder = (SPFolder)(new Location(xmlNodes1)).GetNode();
            XmlNode xmlNodes2 = xmlNode.SelectSingleNode("./IDs");
            string listItems = this._parentList.Adapter.Reader.GetListItems(this._parentList.ConstantID, xmlNodes2.InnerText, this.GetDefaultFields(), this._parentFolder.ConstantDirName, true, ListItemQueryType.ListItem | ListItemQueryType.Folder, this._parentList.GetListXML(false).OuterXml, new GetListItemOptions());
            this.ParseFetchedDataIntoCollection(listItems, true, false);
        }

        private string GetDefaultFields()
        {
            StringBuilder stringBuilder = new StringBuilder(512);
            stringBuilder.Append("<Fields>");
            stringBuilder.Append("<Field Name=\"ID\" ColName=\"tp_ID\" Type=\"Counter\"/><Field ColName=\"tp_Modified\" Name=\"Modified\" Type=\"DateTime\"/>");
            if ((!this._parentList.Adapter.SharePointVersion.IsSharePoint2003 ? true : this._parentList.IsDocumentLibrary))
            {
                stringBuilder.Append("<Field Name=\"FileRef\" Type=\"File\" /><Field Name=\"FSObjType\" ColName=\"Type\" Type=\"Lookup\" FromBaseType=\"TRUE\" />");
            }
            if (this._parentList is SPDiscussionList)
            {
                if (!this._parentList.Adapter.SharePointVersion.IsSharePoint2003)
                {
                    stringBuilder.Append("<Field Name=\"ThreadIndex\" ColName=\"tp_ThreadIndex\" Type=\"ThreadIndex\" />");
                    SPField fieldByName = this._parentList.FieldCollection.GetFieldByName("ParentFolderId");
                    if ((fieldByName == null ? false : !string.IsNullOrEmpty(fieldByName.ColName)))
                    {
                        stringBuilder.Append(string.Format("<Field Name=\"ParentFolderId\" ColName=\"{0}\" Type=\"Integer\" />", fieldByName.ColName));
                    }
                }
                else
                {
                    stringBuilder.Append("<Field Name=\"Ordering\" ColName=\"tp_Ordering\" Type=\"Threading\" />");
                }
            }
            stringBuilder.Append("</Fields>");
            return stringBuilder.ToString();
        }

        private string GetFieldsAndOptionsForFetch(string sFields, bool bIncludeExternalizationData, bool bIncludePermissionsInheritance, out bool bFetchOnlyIDFields, out GetListItemOptions getOptions)
        {
            bFetchOnlyIDFields = sFields == null;
            getOptions = new GetListItemOptions();
            if (!bFetchOnlyIDFields)
            {
                getOptions.IncludeExternalizationData = bIncludeExternalizationData;
                getOptions.IncludePermissionsInheritance = bIncludePermissionsInheritance;
            }
            else
            {
                sFields = this.GetDefaultFields();
            }
            return sFields;
        }

        private string GetFullPath(string sFolder)
        {
            return string.Concat(this._parentFolder.FolderPath, (sFolder == "" || sFolder.StartsWith("/") ? "" : "/"), sFolder);
        }

        private int GetIndexByID(int iID)
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
                else if (iID != ((SPListItem)this[num1]).ID)
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

        public SPListItem GetItemByFileName(string sFileName)
        {
            SPListItem sPListItem;
            foreach (SPListItem sPListItem1 in this)
            {
                if (sPListItem1.FileLeafRef.Equals(sFileName, StringComparison.CurrentCultureIgnoreCase))
                {
                    sPListItem = sPListItem1;
                    return sPListItem;
                }
            }
            sPListItem = null;
            return sPListItem;
        }

        public SPListItem GetItemByFileNameAndDir(string sFileName, string sFolderPath)
        {
            SPListItem sPListItem;
            foreach (SPListItem sPListItem1 in this)
            {
                if ((!sPListItem1.FileLeafRef.Equals(sFileName, StringComparison.CurrentCultureIgnoreCase) ? false : sPListItem1.ParentRelativePath.Equals(sFolderPath, StringComparison.CurrentCultureIgnoreCase)))
                {
                    sPListItem = sPListItem1;
                    return sPListItem;
                }
            }
            sPListItem = null;
            return sPListItem;
        }

        public SPListItem GetItemByGuid(Guid guid)
        {
            SPListItem sPListItem;
            foreach (SPListItem sPListItem1 in this)
            {
                if (sPListItem1.GUID == guid)
                {
                    sPListItem = sPListItem1;
                    return sPListItem;
                }
            }
            sPListItem = null;
            return sPListItem;
        }

        public SPListItem GetItemByID(int iID)
        {
            SPListItem item;
            int indexByID = this.GetIndexByID(iID);
            if (indexByID < 0)
            {
                item = null;
            }
            else
            {
                item = (SPListItem)this[indexByID];
            }
            return item;
        }

        public SPListItem GetItemByServerRelativeUrl(string serverRelativeUrl)
        {
            SPListItem sPListItem;
            string str = serverRelativeUrl;
            str = UrlUtils.EnsureLeadingSlash(str);
            Node[] array = (
                from item in this
                where UrlUtils.Equal(UrlUtils.EnsureLeadingSlash(item.ServerRelativeUrl), str)
                select item).ToArray<Node>();
            if ((array == null ? false : (int)array.Length != 0))
            {
                sPListItem = array[0] as SPListItem;
            }
            else
            {
                sPListItem = null;
            }
            return sPListItem;
        }

        public SPListItem GetItemByUniqueId(string id)
        {
            SPListItem sPListItem;
            foreach (SPListItem sPListItem1 in this)
            {
                XmlAttributeCollection attributes = sPListItem1.GetNodeXML().Attributes;
                if ((attributes == null || attributes["UniqueId"] == null ? false : attributes["UniqueId"].Value == id))
                {
                    sPListItem = sPListItem1;
                    return sPListItem;
                }
            }
            sPListItem = null;
            return sPListItem;
        }

        private string GetItemIDs()
        {
            StringBuilder stringBuilder = new StringBuilder(1024);
            foreach (SPListItem sPListItem in this)
            {
                if (stringBuilder.Length > 0)
                {
                    stringBuilder.Append(",");
                }
                stringBuilder.Append(sPListItem.ID);
            }
            return stringBuilder.ToString();
        }

        public SPListItem GetMatchingItem(SPListItem exampleItem, string sFileLeafRef, string sParentRelativePath)
        {
            SPListItem sPListItem;
            SPListItem sPListItem1 = null;
            sPListItem1 = ((exampleItem.ItemType == SPListItemType.Folder ? false : !exampleItem.ParentList.IsDocumentLibrary) ? this.GetItemByID(exampleItem.ID) : this.GetItemByFileNameAndDir(sFileLeafRef, sParentRelativePath));
            if ((sPListItem1 == null ? false : sPListItem1.ItemType == exampleItem.ItemType))
            {
                sPListItem = sPListItem1;
            }
            else
            {
                sPListItem = null;
            }
            return sPListItem;
        }

        // Metalogix.SharePoint.SPListItemCollection
        public SecurityPrincipalCollection GetReferencedPrincipals()
        {
            SecurityPrincipalCollection result;
            if (base.Count == 0)
            {
                result = new SecurityPrincipalCollection();
            }
            else
            {
                SPFieldCollection fieldsOfTypes = ((SPFieldCollection)this._parentList.Fields).GetFieldsOfTypes(new string[]
                {
                    "User",
                    "UserMulti"
                });
                bool flag = false;
                if (this._parentList.Adapter.SharePointVersion.IsSharePoint2007OrLater)
                {
                    IEnumerator enumerator = fieldsOfTypes.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        SPField sPField = (SPField)enumerator.Current;
                        XmlAttribute xmlAttribute = sPField.FieldXML.Attributes["UserSelectionMode"];
                        if ((xmlAttribute != null && xmlAttribute.Value == "PeopleAndGroups") || (xmlAttribute == null && sPField.FieldXML.Attributes["ColName"] != null && !sPField.FieldXML.Attributes["ColName"].Value.StartsWith("tp_")))
                        {
                            flag = true;
                            break;
                        }
                    }
                }
                string sIDs = null;
                if (this.m_collection != null && this.m_collection.Count < SharePointConfigurationVariables.ReferencedPrincipalsMaximumIDQuery)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    using (List<Node>.Enumerator enumerator2 = this.m_collection.GetEnumerator())
                    {
                        while (enumerator2.MoveNext())
                        {
                            SPListItem sPListItem = (SPListItem)enumerator2.Current;
                            if (stringBuilder.Length > 0)
                            {
                                stringBuilder.Append(",");
                            }
                            stringBuilder.Append(sPListItem.ID);
                        }
                    }
                    sIDs = stringBuilder.ToString();
                }
                string listItems = this._parentList.Adapter.Reader.GetListItems(this._parentList.ConstantID, sIDs, fieldsOfTypes.XML, this._parentFolder.ConstantDirName, false, ListItemQueryType.ListItem | ListItemQueryType.Folder, this._parentList.ListSettingsXML, new GetListItemOptions());
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(listItems);
                XmlNodeList xmlNodeList = xmlDocument.SelectNodes("//ListItem");
                Hashtable hashtable = new Hashtable();
                foreach (XmlNode xmlNode in xmlNodeList)
                {
                    foreach (SPField sPField2 in fieldsOfTypes)
                    {
                        string text = (xmlNode.Attributes[sPField2.Name] != null) ? xmlNode.Attributes[sPField2.Name].Value : null;
                        if (text != null)
                        {
                            string[] array = text.Split(new char[]
                            {
                                ','
                            }, StringSplitOptions.RemoveEmptyEntries);
                            string[] array2 = array;
                            for (int i = 0; i < array2.Length; i++)
                            {
                                string text2 = array2[i];
                                if (!hashtable.ContainsKey(text2))
                                {
                                    hashtable.Add(text2, text2);
                                }
                            }
                        }
                    }
                }
                List<SecurityPrincipal> list = new List<SecurityPrincipal>();
                SecurityPrincipalCollection securityPrincipalCollection = flag ? this._parentList.ParentWeb.Principals : this._parentList.ParentWeb.SiteUsers;
                foreach (string text2 in hashtable.Keys)
                {
                    //string text2;
                    SecurityPrincipal securityPrincipal = securityPrincipalCollection[text2];
                    if (securityPrincipal == null)
                    {
                        XmlDocument xmlDocument2 = new XmlDocument();
                        xmlDocument2.LoadXml("<User LoginName='" + text2 + "'/>");
                        securityPrincipal = new SPUser(xmlDocument2.FirstChild);
                    }
                    list.Add(securityPrincipal);
                    if (flag && securityPrincipal is SPGroup)
                    {
                        SPGroup sPGroup = (SPGroup)securityPrincipal;
                        while (sPGroup.Owner != null && !list.Contains(sPGroup.Owner))
                        {
                            if (sPGroup.OwnerIsUser)
                            {
                                list.Add((SPUser)sPGroup.Owner);
                            }
                            else
                            {
                                sPGroup = (SPGroup)sPGroup.Owner;
                                list.Add(sPGroup);
                            }
                        }
                    }
                }
                result = new SecurityPrincipalCollection(list.ToArray());
            }
            return result;
        }

        public bool ParentFolderExists(string sParentRelativePath)
        {
            bool flag;
            if (!string.IsNullOrEmpty(sParentRelativePath))
            {
                int num = sParentRelativePath.LastIndexOf('/');
                string str = (num < 0 ? sParentRelativePath : sParentRelativePath.Substring(num + 1));
                SPListItem itemByFileNameAndDir = this.GetItemByFileNameAndDir(str, (num < 0 ? "" : sParentRelativePath.Substring(0, num)));
                flag = ((itemByFileNameAndDir == null ? false : itemByFileNameAndDir.ItemType == SPListItemType.Folder) ? true : false);
            }
            else
            {
                flag = true;
            }
            return flag;
        }

        private void ParseFetchedDataIntoCollection(string sItemsXML, bool bFetchedDataIsTerse, bool fireEventsPerItem = true)
        {
            SPListItem item;
            this.ClearCollection();
            if (fireEventsPerItem)
            {
                this.FireNodeCollectionChanged(NodeCollectionChangeType.FullReset, null);
            }
            if (!string.IsNullOrEmpty(sItemsXML))
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(sItemsXML);
                foreach (XmlNode xmlNodes in xmlDocument.SelectNodes(".//ListItem"))
                {
                    SPListItem sPListItem = null;
                    sPListItem = (!bFetchedDataIsTerse ? SPListItem.CreateListItem(this._parentList, this._parentFolder, this, xmlNodes) : SPListItem.CreateListItemFromTerseXml(this._parentList, this._parentFolder, this, xmlNodes));
                    base.AddToCollection(sPListItem, false);
                    if (fireEventsPerItem)
                    {
                        this.FireNodeCollectionChanged(NodeCollectionChangeType.NodeAdded, sPListItem);
                    }
                }
                if (base.Count == 0)
                {
                    item = null;
                }
                else
                {
                    item = this[base.Count - 1] as SPListItem;
                }
                SPListItem sPListItem1 = item;
                if (sPListItem1 != null)
                {
                    this.UpdateParentListStatistics(sPListItem1);
                }
                if (!fireEventsPerItem)
                {
                    this.FireNodeCollectionChanged(NodeCollectionChangeType.FullReset, null);
                }
            }
        }

        public override bool Remove(Node item)
        {
            bool flag;
            if (!(item is SPListItem))
            {
                throw new Exception("The node being removed is not a SPListItem");
            }
            SPListItem sPListItem = (SPListItem)item;
            flag = ((!(sPListItem is SPDiscussionItem) ? true : ((SPDiscussionItem)sPListItem).ParentID != 0) ? this.DeleteItem(sPListItem) : this.DeleteDiscussionFolder(sPListItem));
            return flag;
        }

        public bool RemoveByID(int iID)
        {
            bool flag;
            int indexByID = this.GetIndexByID(iID);
            flag = (indexByID == -1 ? false : base.RemoveIndex(indexByID));
            return flag;
        }

        public void SortItemsToEnsureFolderExistence()
        {
            List<SPListItem> sPListItems = new List<SPListItem>();
            Dictionary<string, SPListItem> strs = new Dictionary<string, SPListItem>();
            foreach (SPListItem sPListItem in this)
            {
                if (!(string.IsNullOrEmpty(sPListItem.ParentRelativePath) ? true : strs.ContainsKey(sPListItem.FileDirRef)))
                {
                    sPListItems.Add(sPListItem);
                }
                else if (sPListItem.ItemType == SPListItemType.Folder)
                {
                    strs.Add(sPListItem.FileRef, sPListItem);
                }
            }
            if (sPListItems.Count > 0)
            {
                sPListItems.Sort((SPListItem item1, SPListItem item2) =>
                {
                    int num = item1.FileDirRef.CompareTo(item2.FileDirRef);
                    return (num == 0 ? item1.ID.CompareTo(item2.ID) : num);
                });
                Node[] array = sPListItems.ToArray();
                base.RemoveRangeFromCollection(array);
                base.AddRangeToCollection(array);
            }
        }

        public override void ToXML(XmlWriter xmlTextWriter)
        {
            xmlTextWriter.WriteStartElement("SPListItemCollection");
            xmlTextWriter.WriteStartElement("ParentList");
            this._parentList.Location.ToXML(xmlTextWriter);
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteStartElement("ParentFolder");
            this._parentFolder.Location.ToXML(xmlTextWriter);
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteElementString("IDs", this.GetItemIDs());
            xmlTextWriter.WriteEndElement();
        }

        protected void UpdateParentListStatistics(SPListItem item)
        {
            if (this.ParentSPList != null)
            {
                this.ParentSPList.UpdatePredictedNextAvailableID(item);
            }
        }
    }
}