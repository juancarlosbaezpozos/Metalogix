using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;

namespace Metalogix.Core.OperationLog
{
    public class OperationReportingResult
    {
        private readonly bool _errorsOccured;

        private readonly bool _warningsOccured;

        private readonly bool _hasInformation;

        private readonly string _objectXml;

        private readonly IList<ReportingElement> _reportingElements;

        public string AllInformationAsString
        {
            get
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (ReportingElement information in this.Information)
                {
                    if (stringBuilder.Length != 0)
                    {
                        stringBuilder.AppendLine();
                    }

                    stringBuilder.AppendLine(information.ToString());
                }

                return stringBuilder.ToString();
            }
        }

        public string AllReportElementsAsString
        {
            get
            {
                StringBuilder stringBuilder = new StringBuilder();
                if (this.ErrorOccured)
                {
                    stringBuilder.AppendLine("ERRORS:");
                    stringBuilder.AppendLine(this.GetAllErrorsAsString);
                }

                if (this.WarningOccured)
                {
                    stringBuilder.AppendLine("WARNINGS:");
                    stringBuilder.AppendLine(this.GetAllWarningsAsString);
                }

                if (this.HasInformation)
                {
                    stringBuilder.AppendLine("INFORMATION:");
                    stringBuilder.AppendLine(this.AllInformationAsString);
                }

                return stringBuilder.ToString();
            }
        }

        public bool ErrorOccured
        {
            get { return this._errorsOccured; }
        }

        public IEnumerable<ReportingElement> Errors
        {
            get
            {
                return
                    from e in this._reportingElements
                    where e.EntryType == OperationReportingElements.Error
                    select e;
            }
        }

        public string GetAllErrorsAsString
        {
            get
            {
                StringBuilder stringBuilder = new StringBuilder(1024);
                foreach (ReportingElement error in this.Errors)
                {
                    if (stringBuilder.Length != 0)
                    {
                        stringBuilder.AppendLine();
                    }

                    stringBuilder.AppendLine(error.ToString());
                }

                return stringBuilder.ToString();
            }
        }

        public string GetAllWarningsAsString
        {
            get
            {
                StringBuilder stringBuilder = new StringBuilder(1024);
                foreach (ReportingElement warning in this.Warnings)
                {
                    if (stringBuilder.Length != 0)
                    {
                        stringBuilder.AppendLine();
                    }

                    stringBuilder.AppendLine(warning.ToString());
                }

                return stringBuilder.ToString();
            }
        }

        public string GetMessageOfFirstErrorElement
        {
            get
            {
                ReportingElement reportingElement =
                    this._reportingElements.FirstOrDefault<ReportingElement>((ReportingElement e) =>
                        e.EntryType == OperationReportingElements.Error);
                if (reportingElement == null)
                {
                    return string.Empty;
                }

                return reportingElement.Message;
            }
        }

        public bool HasInformation
        {
            get { return this._hasInformation; }
        }

        public IEnumerable<ReportingElement> Information
        {
            get
            {
                return
                    from e in this._reportingElements
                    where e.EntryType == OperationReportingElements.Information
                    select e;
            }
        }

        public string ObjectXml
        {
            get { return this._objectXml; }
        }

        public bool WarningOccured
        {
            get { return this._warningsOccured; }
        }

        public IEnumerable<ReportingElement> Warnings
        {
            get
            {
                return
                    from e in this._reportingElements
                    where e.EntryType == OperationReportingElements.Warning
                    select e;
            }
        }

        public OperationReportingResult(string adapterResultXml)
        {
            this._reportingElements = new List<ReportingElement>();
            if (string.IsNullOrEmpty(adapterResultXml))
            {
                this._errorsOccured = true;
                ReportingElement reportingElement = new ReportingElement(OperationReportingElements.Error,
                    "Operation reporting result is empty.", string.Empty, null, 0, 0);
                this._reportingElements.Add(reportingElement);
                return;
            }

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(adapterResultXml);
            XmlNodeList elementsByTagName =
                xmlDocument.GetElementsByTagName(OperationReportingElements.Error.ToString());
            this._errorsOccured = (elementsByTagName == null ? false : elementsByTagName.Count > 0);
            XmlNodeList xmlNodeLists =
                xmlDocument.GetElementsByTagName(OperationReportingElements.Information.ToString());
            this._hasInformation = (xmlNodeLists == null ? false : xmlNodeLists.Count > 0);
            XmlNodeList elementsByTagName1 =
                xmlDocument.GetElementsByTagName(OperationReportingElements.Warning.ToString());
            this._warningsOccured = (elementsByTagName1 == null ? false : elementsByTagName1.Count > 0);
            if (this._errorsOccured)
            {
                this.PopulateElements(elementsByTagName, OperationReportingElements.Error);
            }

            if (this._hasInformation)
            {
                this.PopulateElements(xmlNodeLists, OperationReportingElements.Information);
            }

            if (this._warningsOccured)
            {
                this.PopulateElements(elementsByTagName1, OperationReportingElements.Warning);
            }

            this._objectXml = string.Empty;
            XmlNode xmlNodes = xmlDocument.SelectSingleNode(string.Format("{0}/{1}",
                OperationReportingElements.Result.ToString(), OperationReportingElements.ObjectXml.ToString()));
            if (xmlNodes != null)
            {
                this._objectXml = xmlNodes.InnerXml;
                return;
            }

            if (!this._errorsOccured)
            {
                this._objectXml = adapterResultXml;
            }
        }

        private void PopulateElements(XmlNodeList nodes, OperationReportingElements entryType)
        {
            foreach (XmlNode node in nodes)
            {
                ReportingElement reportingElement = new ReportingElement(entryType,
                    node.GetAttributeValueAsString(OperationReportingAttributes.Message.ToString()),
                    node.GetAttributeValueAsString(OperationReportingAttributes.Detail.ToString()),
                    node.GetAttributeValueAsString(OperationReportingAttributes.Stack.ToString()),
                    node.GetAttributeValueAsInt(OperationReportingAttributes.HResult.ToString()), 0);
                this._reportingElements.Add(reportingElement);
            }
        }
    }
}