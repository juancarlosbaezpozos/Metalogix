using Metalogix.SharePoint.Adapters;
using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

namespace Metalogix.SharePoint.Adapters.OM
{
    public class FolderItemProcessor
    {
        private readonly OMAdapter _adapter;

        private readonly SPWeb _currentWeb;

        private readonly SPList _list;

        private readonly List<string> _listIds;

        private readonly string[] _iDArray;

        private readonly XmlWriter _xmlWriter;

        public FolderItemProcessor(string[] stringIds, List<string> listIds, XmlWriter xmlWriter, SPWeb currentWeb,
            SPList list, OMAdapter adapter)
        {
            this._iDArray = stringIds;
            this._listIds = listIds;
            this._xmlWriter = xmlWriter;
            this._currentWeb = currentWeb;
            this._list = list;
            this._adapter = adapter;
        }

        private string GetFieldValueByInternalName(SPWeb currentWeb, SPList list, SPListItem listItem,
            string internalFieldName, bool datesInUtc)
        {
            object item = null;
            SPField fieldByInternalName = null;
            try
            {
                fieldByInternalName = list.Fields.GetFieldByInternalName(internalFieldName);
                item = listItem[fieldByInternalName.Id];
            }
            catch (Exception exception)
            {
                item = null;
            }

            if (item == null)
            {
                return null;
            }

            return this._adapter.GetFieldValue(item, fieldByInternalName, currentWeb, datesInUtc);
        }

        public void ProcessListItem(SPListItem folderItem)
        {
            if (folderItem.FileSystemObjectType != (SPFileSystemObjectType)1)
            {
                return;
            }

            if (folderItem != null &&
                ((int)this._iDArray.Length == 0 || this._listIds.Contains(folderItem.ID.ToString())))
            {
                this._xmlWriter.WriteStartElement("Folder");
                this._xmlWriter.WriteAttributeString("ContentTypeId",
                    this.GetFieldValueByInternalName(this._currentWeb, this._list, folderItem, "ContentTypeId", false));
                this._xmlWriter.WriteAttributeString("ID", folderItem.ID.ToString());
                this._xmlWriter.WriteAttributeString("FileLeafRef",
                    this.GetFieldValueByInternalName(this._currentWeb, this._list, folderItem, "FileLeafRef", false));
                this._xmlWriter.WriteAttributeString("FileDirRef",
                    this.GetFieldValueByInternalName(this._currentWeb, this._list, folderItem, "FileDirRef", false));
                string fieldValueByInternalName =
                    this.GetFieldValueByInternalName(this._currentWeb, this._list, folderItem, "Editor", false);
                if (fieldValueByInternalName != null)
                {
                    this._xmlWriter.WriteAttributeString("Editor", fieldValueByInternalName);
                }

                string str =
                    this.GetFieldValueByInternalName(this._currentWeb, this._list, folderItem, "Author", false);
                if (str != null)
                {
                    this._xmlWriter.WriteAttributeString("Author", str);
                }

                this._xmlWriter.WriteAttributeString("Created",
                    Utils.FormatDate(
                        this._currentWeb.RegionalSettings.TimeZone.LocalTimeToUTC((DateTime)folderItem["Created"])));
                this._xmlWriter.WriteAttributeString("Modified",
                    Utils.FormatDate(
                        this._currentWeb.RegionalSettings.TimeZone.LocalTimeToUTC((DateTime)folderItem["Modified"])));
                this._xmlWriter.WriteAttributeString("HasUniquePermissions",
                    folderItem.HasUniqueRoleAssignments.ToString());
                this._xmlWriter.WriteEndElement();
            }
        }

        public bool ProcessListItemError(SPListItem item, Exception ex)
        {
            Trace.WriteLine(string.Concat("Failed to process folder: ", ex));
            return true;
        }
    }
}