using Metalogix.Actions;
using System;
using System.Xml;

namespace Metalogix.Metabase.Options
{
    public class ImportFromCSVOptions : ActionOptions
    {
        private bool m_bAddNewRows;

        private bool m_bCreateNewColumns;

        private bool m_bOverwriteFullRows;

        private string m_sSourceFilename = string.Empty;

        private bool m_bFailureLogging = true;

        private bool m_bPreview;

        private bool m_bIgnoreMetalogixId;

        public bool AddNewRows
        {
            get { return this.m_bAddNewRows; }
            set { this.m_bAddNewRows = value; }
        }

        public bool CreateNewColumns
        {
            get { return this.m_bCreateNewColumns; }
            set { this.m_bCreateNewColumns = value; }
        }

        public bool FailureLogging
        {
            get { return this.m_bFailureLogging; }
            set { this.m_bFailureLogging = value; }
        }

        public bool IgnoreMetalogixId
        {
            get { return this.m_bIgnoreMetalogixId; }
            set { this.m_bIgnoreMetalogixId = value; }
        }

        public bool OverwriteFullRows
        {
            get { return this.m_bOverwriteFullRows; }
            set { this.m_bOverwriteFullRows = value; }
        }

        public bool Preview
        {
            get { return this.m_bPreview; }
            set { this.m_bPreview = value; }
        }

        public string SourceFileName
        {
            get { return this.m_sSourceFilename; }
            set { this.m_sSourceFilename = value; }
        }

        public ImportFromCSVOptions()
        {
        }

        public override void FromXML(XmlNode xmlNode)
        {
            bool flag;
            bool flag1;
            bool flag2;
            bool flag3;
            bool flag4;
            bool flag5;
            if (xmlNode == null)
            {
                return;
            }

            XmlNode xmlNodes = xmlNode.SelectSingleNode("./ImportFromCSVOptions");
            if (xmlNodes == null)
            {
                return;
            }

            XmlNode xmlNodes1 = xmlNodes.SelectSingleNode("./OverwriteFullRows");
            if (xmlNodes1 != null && bool.TryParse(xmlNodes1.InnerText, out flag))
            {
                this.m_bOverwriteFullRows = flag;
            }

            XmlNode xmlNodes2 = xmlNodes.SelectSingleNode("./CreateNewColumns");
            if (xmlNodes2 != null && bool.TryParse(xmlNodes2.InnerText, out flag1))
            {
                this.m_bCreateNewColumns = flag1;
            }

            XmlNode xmlNodes3 = xmlNodes.SelectSingleNode("./AddNewRows");
            if (xmlNodes3 != null && bool.TryParse(xmlNodes3.InnerText, out flag2))
            {
                this.m_bAddNewRows = flag2;
            }

            XmlNode xmlNodes4 = xmlNodes.SelectSingleNode("./IgnoreMetalogixID");
            if (xmlNodes4 != null && bool.TryParse(xmlNodes4.InnerText, out flag3))
            {
                this.m_bIgnoreMetalogixId = flag3;
            }

            XmlNode xmlNodes5 = xmlNodes.SelectSingleNode("./SourceFilename");
            if (xmlNodes5 != null)
            {
                this.m_sSourceFilename = xmlNodes5.InnerText;
            }

            XmlNode xmlNodes6 = xmlNodes.SelectSingleNode("./ErrorLogging");
            if (xmlNodes6 != null && bool.TryParse(xmlNodes6.InnerText, out flag4))
            {
                this.FailureLogging = flag4;
            }

            XmlNode xmlNodes7 = xmlNodes.SelectSingleNode("./ShowPreview");
            if (xmlNodes7 != null && bool.TryParse(xmlNodes7.InnerText, out flag5))
            {
                this.Preview = flag5;
            }
        }

        public override void ToXML(XmlWriter xmlTextWriter)
        {
            xmlTextWriter.WriteStartElement("ImportFromCSVOptions");
            xmlTextWriter.WriteElementString("OverwriteFullRows", this.m_bOverwriteFullRows.ToString());
            xmlTextWriter.WriteElementString("CreateNewColumns", this.m_bCreateNewColumns.ToString());
            xmlTextWriter.WriteElementString("AddNewRows", this.m_bAddNewRows.ToString());
            xmlTextWriter.WriteElementString("IgnoreMetalogixID", this.m_bIgnoreMetalogixId.ToString());
            xmlTextWriter.WriteElementString("SourceFilename", this.m_sSourceFilename);
            xmlTextWriter.WriteElementString("ErrorLogging", this.FailureLogging.ToString());
            xmlTextWriter.WriteElementString("ShowPreview", this.Preview.ToString());
            xmlTextWriter.WriteEndElement();
        }

        private class XmlNames
        {
            public const string OPTIONS = "ImportFromCSVOptions";

            public const string OVERWRITE_FULL_ROWS = "OverwriteFullRows";

            public const string CREATE_NEW_COLUMNS = "CreateNewColumns";

            public const string ADD_NEW_ROWS = "AddNewRows";

            public const string IGNORE_METALOGIX_ID = "IgnoreMetalogixID";

            public const string SOURCE_FILENAME = "SourceFilename";

            public const string ERROR_LOGGING = "ErrorLogging";

            public const string SHOW_PREVIEW = "ShowPreview";

            private XmlNames()
            {
            }
        }
    }
}