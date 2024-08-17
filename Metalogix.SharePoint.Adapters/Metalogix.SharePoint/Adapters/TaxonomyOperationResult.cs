using System;
using System.Xml;

namespace Metalogix.SharePoint.Adapters
{
    public class TaxonomyOperationResult
    {
        private readonly string adapterResult;

        private readonly bool errorsOccured;

        private readonly bool warningsOccured;

        private readonly string objectXml;

        private readonly TaxonomyStatistics statistics;

        public string AdapterResult
        {
            get { return this.adapterResult; }
        }

        public bool ErrorOccured
        {
            get { return this.errorsOccured; }
        }

        public string ObjectXml
        {
            get { return this.objectXml; }
        }

        public TaxonomyStatistics Statistics
        {
            get { return this.statistics; }
        }

        public bool WarningOccured
        {
            get { return this.warningsOccured; }
        }

        public TaxonomyOperationResult(TaxonomyClassType classType, string adapterResultXml) : this(
            classType.ToString(), adapterResultXml)
        {
        }

        public TaxonomyOperationResult(string adapterResultXml) : this(null, adapterResultXml)
        {
        }

        private TaxonomyOperationResult(string classType, string adapterResultXml)
        {
            if (string.IsNullOrEmpty(adapterResultXml))
            {
                this.adapterResult = "adapterResultXml empty or null. Unable to determine result.";
                this.errorsOccured = true;
                return;
            }

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(adapterResultXml);
            this.statistics = new TaxonomyStatistics();
            XmlNode xmlNodes = xmlDocument.SelectSingleNode(string.Format("{0}/{1}",
                TaxonomyReportingElements.Result.ToString(), TaxonomyReportingElements.Statistics.ToString()));
            if (xmlNodes != null)
            {
                this.statistics.Populate(xmlNodes);
                xmlDocument.DocumentElement.RemoveChild(xmlNodes);
            }

            this.objectXml = string.Empty;
            if (!string.IsNullOrEmpty(classType))
            {
                XmlNode xmlNodes1 = xmlDocument.SelectSingleNode(string.Format("{0}/{1}",
                    TaxonomyReportingElements.Result.ToString(), classType));
                if (xmlNodes1 != null)
                {
                    this.objectXml = xmlNodes1.OuterXml;
                    xmlDocument.DocumentElement.RemoveChild(xmlNodes1);
                }
            }

            XmlNodeList elementsByTagName =
                xmlDocument.GetElementsByTagName(TaxonomyReportingElements.Error.ToString());
            this.errorsOccured = (elementsByTagName == null ? false : elementsByTagName.Count > 0);
            XmlNodeList xmlNodeLists = xmlDocument.GetElementsByTagName(TaxonomyReportingElements.Warning.ToString());
            this.warningsOccured = (xmlNodeLists == null ? false : xmlNodeLists.Count > 0);
            this.adapterResult = (xmlDocument.DocumentElement.HasAttributes || xmlDocument.DocumentElement.HasChildNodes
                ? xmlDocument.OuterXml
                : string.Empty);
        }
    }
}