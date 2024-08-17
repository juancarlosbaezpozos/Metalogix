using Metalogix.SharePoint.Adapters;
using Metalogix.Utilities;
using Microsoft.SharePoint;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

namespace Metalogix.SharePoint.Adapters.OM
{
    public class ListItemProcessor
    {
        private readonly OMAdapter _adapter;

        private readonly SPWeb _currentWeb;

        private List<string> _fields;

        private readonly XmlWriter _xmlWriter;

        private GetListItemOptions _options;

        public ListItemProcessor(string fieldsXml, XmlWriter writer, SPWeb currentWeb, OMAdapter adapter,
            GetListItemOptions options)
        {
            this._xmlWriter = writer;
            this._currentWeb = currentWeb;
            this._adapter = adapter;
            this._options = options;
            this.InitializeFields(fieldsXml);
        }

        private void InitializeFields(string fieldsXml)
        {
            this._fields = new List<string>();
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(fieldsXml);
                foreach (XmlNode xmlNodes in xmlDocument.SelectNodes("./Fields/Field"))
                {
                    this._fields.Add(xmlNodes.GetAttributeValueAsString("Name"));
                }
            }
            finally
            {
                Trace.WriteLine("Failed to initialize fields: ");
            }
        }

        public void ProcessListItemCollection(SPListItemCollection listItems)
        {
            foreach (SPListItem listItem in listItems)
            {
                this._adapter.GetItemXML(this._currentWeb, this._xmlWriter, this._fields, listItem, true, null,
                    this._options);
            }
        }

        public bool ProcessListItemCollectionError(SPListItemCollection item, Exception ex)
        {
            Trace.WriteLine(string.Concat("Failed to process list item collection: ", ex));
            return true;
        }
    }
}