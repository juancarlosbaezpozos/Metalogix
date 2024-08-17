using System;
using System.IO;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint.Adapters
{
    public class SPSearchParameters
    {
        private string m_sContentType;

        private string m_sCreatedBy;

        private string m_sModifiedBy;

        private DateTime m_dtCreatedBefore = DateTime.MinValue;

        private DateTime m_dtCreatedAfter = DateTime.MinValue;

        private DateTime m_dtModifiedBefore = DateTime.MinValue;

        private DateTime m_dtModifiedAfter = DateTime.MinValue;

        private bool m_bRecursive = true;

        private bool m_bIncludeItems;

        private bool m_bIncludeDocuments = true;

        private bool m_bIncludeFolders;

        private bool m_bIncludeLists;

        private bool m_bIncludeSites;

        private bool m_bMatchExactly;

        private string m_sListID;

        public string ContentType
        {
            get
            {
                if (this.m_sContentType == null)
                {
                    return null;
                }

                string str = this.m_sContentType.Replace("\\", "\\\\");
                str = str.Replace("%", "\\%");
                str = str.Replace("_", "\\_");
                if (this.MatchExactly)
                {
                    return str;
                }

                return string.Concat("%", str, "%");
            }
            set { this.m_sContentType = value; }
        }

        public DateTime CreatedAfter
        {
            get { return this.m_dtCreatedAfter; }
            set { this.m_dtCreatedAfter = value; }
        }

        public DateTime CreatedBefore
        {
            get { return this.m_dtCreatedBefore; }
            set { this.m_dtCreatedBefore = value; }
        }

        public string CreatedBy
        {
            get
            {
                if (this.m_sCreatedBy == null)
                {
                    return null;
                }

                string str = this.m_sCreatedBy.Replace("\\", "\\\\");
                str = str.Replace("%", "\\%");
                str = str.Replace("_", "\\_");
                if (this.MatchExactly)
                {
                    return str;
                }

                return string.Concat("%", str, "%");
            }
            set { this.m_sCreatedBy = value; }
        }

        public bool IncludeDocuments
        {
            get { return this.m_bIncludeDocuments; }
            set { this.m_bIncludeDocuments = value; }
        }

        public bool IncludeFolders
        {
            get { return this.m_bIncludeFolders; }
            set { this.m_bIncludeFolders = value; }
        }

        public bool IncludeItems
        {
            get { return this.m_bIncludeItems; }
            set { this.m_bIncludeItems = value; }
        }

        public bool IncludeLists
        {
            get { return this.m_bIncludeLists; }
            set { this.m_bIncludeLists = value; }
        }

        public bool IncludeSites
        {
            get { return this.m_bIncludeSites; }
            set { this.m_bIncludeSites = value; }
        }

        public string ListID
        {
            get { return this.m_sListID; }
            set { this.m_sListID = value; }
        }

        public bool MatchExactly
        {
            get { return this.m_bMatchExactly; }
            set { this.m_bMatchExactly = value; }
        }

        public DateTime ModifiedAfter
        {
            get { return this.m_dtModifiedAfter; }
            set { this.m_dtModifiedAfter = value; }
        }

        public DateTime ModifiedBefore
        {
            get { return this.m_dtModifiedBefore; }
            set { this.m_dtModifiedBefore = value; }
        }

        public string ModifiedBy
        {
            get
            {
                if (this.m_sModifiedBy == null)
                {
                    return null;
                }

                string str = this.m_sModifiedBy.Replace("\\", "\\\\");
                str = str.Replace("%", "\\%");
                str = str.Replace("_", "\\_");
                if (this.MatchExactly)
                {
                    return str;
                }

                return string.Concat("%", str, "%");
            }
            set { this.m_sModifiedBy = value; }
        }

        public bool Recursive
        {
            get { return this.m_bRecursive; }
            set { this.m_bRecursive = value; }
        }

        public SPSearchParameters()
        {
        }

        public SPSearchParameters(XmlNode node)
        {
            this.FromXML(node);
        }

        public void FromXML(string sXml)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(sXml);
            this.FromXML(xmlDocument.DocumentElement);
        }

        public void FromXML(XmlNode xmlNode)
        {
            XmlNode xmlNodes = xmlNode.SelectSingleNode("//SearchOptions");
            XmlAttribute itemOf = xmlNodes.Attributes["ListID"];
            if (itemOf != null)
            {
                this.ListID = itemOf.Value;
            }

            itemOf = xmlNodes.Attributes["ContentType"];
            if (itemOf != null)
            {
                this.ContentType = itemOf.Value;
            }

            itemOf = xmlNodes.Attributes["CreatedBy"];
            if (itemOf != null)
            {
                this.CreatedBy = itemOf.Value;
            }

            itemOf = xmlNodes.Attributes["ModifiedBy"];
            if (itemOf != null)
            {
                this.ModifiedBy = itemOf.Value;
            }

            itemOf = xmlNodes.Attributes["CreatedBefore"];
            if (itemOf != null)
            {
                this.CreatedBefore = Utils.ParseDateAsUtc(itemOf.Value);
            }

            itemOf = xmlNodes.Attributes["CreatedAfter"];
            if (itemOf != null)
            {
                this.CreatedAfter = Utils.ParseDateAsUtc(itemOf.Value);
            }

            itemOf = xmlNodes.Attributes["ModifiedBefore"];
            if (itemOf != null)
            {
                this.ModifiedBefore = Utils.ParseDateAsUtc(itemOf.Value);
            }

            itemOf = xmlNodes.Attributes["ModifiedAfter"];
            if (itemOf != null)
            {
                this.ModifiedAfter = Utils.ParseDateAsUtc(itemOf.Value);
            }

            itemOf = xmlNodes.Attributes["MatchExactly"];
            if (itemOf != null)
            {
                bool.TryParse(itemOf.Value, out this.m_bMatchExactly);
            }

            itemOf = xmlNodes.Attributes["Recursive"];
            if (itemOf != null)
            {
                bool.TryParse(itemOf.Value, out this.m_bRecursive);
            }

            itemOf = xmlNodes.Attributes["IncludeItems"];
            if (itemOf != null)
            {
                bool.TryParse(itemOf.Value, out this.m_bIncludeItems);
            }

            itemOf = xmlNodes.Attributes["IncludeDocuments"];
            if (itemOf != null)
            {
                bool.TryParse(itemOf.Value, out this.m_bIncludeDocuments);
            }

            itemOf = xmlNodes.Attributes["IncludeFolders"];
            if (itemOf != null)
            {
                bool.TryParse(itemOf.Value, out this.m_bIncludeFolders);
            }

            itemOf = xmlNodes.Attributes["IncludeLists"];
            if (itemOf != null)
            {
                bool.TryParse(itemOf.Value, out this.m_bIncludeLists);
            }

            itemOf = xmlNodes.Attributes["IncludeSites"];
            if (itemOf != null)
            {
                bool.TryParse(itemOf.Value, out this.m_bIncludeSites);
            }
        }

        public string ToXML()
        {
            StringBuilder stringBuilder = new StringBuilder();
            XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
            xmlTextWriter.WriteStartElement("ActionOptions");
            this.ToXML(xmlTextWriter);
            xmlTextWriter.WriteEndElement();
            return stringBuilder.ToString();
        }

        public void ToXML(XmlWriter xmlTextWriter)
        {
            xmlTextWriter.WriteStartElement("SearchOptions");
            xmlTextWriter.WriteAttributeString("MatchExactly", this.MatchExactly.ToString());
            xmlTextWriter.WriteAttributeString("Recursive", this.Recursive.ToString());
            xmlTextWriter.WriteAttributeString("IncludeItems", this.IncludeItems.ToString());
            xmlTextWriter.WriteAttributeString("IncludeDocuments", this.IncludeDocuments.ToString());
            xmlTextWriter.WriteAttributeString("IncludeFolders", this.IncludeFolders.ToString());
            xmlTextWriter.WriteAttributeString("IncludeLists", this.IncludeLists.ToString());
            xmlTextWriter.WriteAttributeString("IncludeSites", this.IncludeSites.ToString());
            if (this.ListID != null)
            {
                xmlTextWriter.WriteAttributeString("ListID", this.ListID);
            }

            if (this.ContentType != null)
            {
                xmlTextWriter.WriteAttributeString("ContentType", this.m_sContentType);
            }

            if (this.CreatedBy != null)
            {
                xmlTextWriter.WriteAttributeString("CreatedBy", this.m_sCreatedBy);
            }

            if (this.ModifiedBy != null)
            {
                xmlTextWriter.WriteAttributeString("ModifiedBy", this.m_sModifiedBy);
            }

            if (this.CreatedBefore != DateTime.MinValue)
            {
                xmlTextWriter.WriteAttributeString("CreatedBefore", Utils.FormatDate(this.CreatedBefore));
            }

            if (this.CreatedAfter != DateTime.MinValue)
            {
                xmlTextWriter.WriteAttributeString("CreatedAfter", Utils.FormatDate(this.CreatedAfter));
            }

            if (this.ModifiedBefore != DateTime.MinValue)
            {
                xmlTextWriter.WriteAttributeString("ModifiedBefore", Utils.FormatDate(this.ModifiedBefore));
            }

            if (this.ModifiedAfter != DateTime.MinValue)
            {
                xmlTextWriter.WriteAttributeString("ModifiedAfter", Utils.FormatDate(this.ModifiedAfter));
            }

            xmlTextWriter.WriteEndElement();
        }
    }
}