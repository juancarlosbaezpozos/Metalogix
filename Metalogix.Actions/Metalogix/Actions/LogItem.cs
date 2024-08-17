using Metalogix.Data;
using Metalogix.Jobs;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web.Services.Protocols;
using System.Xml;

namespace Metalogix.Actions
{
    public class LogItem : IXmlable
    {
        public bool WriteToJobDatabase = true;

        private JobHistoryDb m_adapter;

        private string m_sID = Guid.NewGuid().ToString();

        private bool m_bDirtyTimeStamp;

        private DateTime m_timeStamp = DateTime.Now;

        private bool m_bDirtyFinishedTime;

        private DateTime m_dtFinished = DateTime.Now;

        private bool m_bDirtyOperation;

        private string m_sOperation;

        private bool m_bDirtyItemName;

        private string m_sItemName;

        private bool m_bDirtySource;

        private string m_sSource;

        private bool m_bDirtyTarget;

        private string m_sTarget;

        private bool m_bDirtyStatus;

        private ActionOperationStatus m_status;

        private bool m_bDirtyInformation;

        private string m_sInformation;

        private DifferenceLog m_DifferenceLog = new DifferenceLog();

        private bool m_bDirtyLicenseDataUsed;

        private long m_lLicenseDataUsed;

        private long m_lLicenseItemsCount;

        private System.Exception _exception;

        private object m_oLockDetails = new object();

        private bool m_bHasDetails;

        private bool m_bDirtyDetails;

        private string m_sDetails;

        private bool m_bDirtySourceContent;

        private string m_sSourceContent;

        private bool m_bDirtyTargetContent;

        private string m_sTargetContent;

        private Dictionary<string, long> m_dictCompletionDetails;

        private object m_oDirtyFieldsLock = new object();

        private string m_sActionLicensingUnit;

        private string m_sActionLicensingDescriptor;

        public string ActionLicensingDescriptor
        {
            get { return this.m_sActionLicensingDescriptor; }
            set { this.m_sActionLicensingDescriptor = value; }
        }

        public string ActionLicensingUnit
        {
            get { return this.m_sActionLicensingUnit; }
            set { this.m_sActionLicensingUnit = value; }
        }

        internal JobHistoryDb Adapter
        {
            get { return this.m_adapter; }
            set { this.m_adapter = value; }
        }

        internal Dictionary<string, long> CompletionDetails
        {
            get
            {
                if (this.m_dictCompletionDetails == null)
                {
                    this.m_dictCompletionDetails = new Dictionary<string, long>();
                }

                return this.m_dictCompletionDetails;
            }
        }

        public string Details
        {
            get
            {
                if (!this.m_bHasDetails)
                {
                    lock (this.m_oLockDetails)
                    {
                        if (!this.m_bHasDetails && this.Adapter != null)
                        {
                            this.Adapter.GetLogItemDetails(this.ID, this);
                            this.m_bHasDetails = true;
                        }
                    }
                }

                return this.m_sDetails;
            }
            set
            {
                lock (this.m_oLockDetails)
                {
                    this.m_sDetails = value;
                    lock (this.m_oDirtyFieldsLock)
                    {
                        this.m_bDirtyDetails = true;
                    }

                    this.m_bHasDetails = true;
                }
            }
        }

        public DifferenceLog Differences
        {
            get { return this.m_DifferenceLog; }
        }

        private string[] DirtyFieldsList
        {
            get
            {
                string[] array;
                lock (this.m_oDirtyFieldsLock)
                {
                    List<string> strs = new List<string>();
                    if (this.m_bDirtyDetails)
                    {
                        strs.Add("Details");
                    }

                    if (this.m_bDirtySource)
                    {
                        strs.Add("Source");
                    }

                    if (this.m_bDirtyTarget)
                    {
                        strs.Add("Target");
                    }

                    if (this.m_bDirtySourceContent)
                    {
                        strs.Add("SourceContent");
                    }

                    if (this.m_bDirtyTargetContent)
                    {
                        strs.Add("TargetContent");
                    }

                    if (this.m_bDirtyInformation)
                    {
                        strs.Add("Information");
                    }

                    if (this.m_bDirtyOperation)
                    {
                        strs.Add("Operation");
                    }

                    if (this.m_bDirtyItemName)
                    {
                        strs.Add("ItemName");
                    }

                    if (this.m_bDirtyStatus)
                    {
                        strs.Add("Status");
                    }

                    if (this.m_bDirtyTimeStamp)
                    {
                        strs.Add("[TimeStamp]");
                    }

                    if (this.m_bDirtyFinishedTime)
                    {
                        strs.Add("[FinishedTime]");
                    }

                    if (this.m_bDirtyLicenseDataUsed)
                    {
                        strs.Add("LicensedDataUsed");
                    }

                    array = strs.ToArray();
                }

                return array;
            }
        }

        public System.Exception Exception
        {
            get { return this._exception; }
            set
            {
                this._exception = value;
                if (value != null)
                {
                    ExceptionDetail exceptionMessageAndDetail = ExceptionUtils.GetExceptionMessageAndDetail(value);
                    System.Exception exception = (value is TargetInvocationException ? value.InnerException : value);
                    this.Status = ActionOperationStatus.Failed;
                    string message = exception.Message;
                    SoapException soapException = value as SoapException;
                    if (soapException != null)
                    {
                        XmlNode detail = soapException.Detail;
                        if (detail != null && detail["errorstring"] != null)
                        {
                            message = string.Concat("Web service error: ", detail["errorstring"].InnerText);
                        }
                    }

                    this.Information = string.Concat("Exception: ", message);
                    this.Details = exceptionMessageAndDetail.Detail;
                }
            }
        }

        public DateTime FinishedTime
        {
            get { return this.m_dtFinished; }
            set
            {
                this.m_dtFinished = value;
                lock (this.m_oDirtyFieldsLock)
                {
                    this.m_bDirtyFinishedTime = true;
                }
            }
        }

        public string ID
        {
            get { return this.m_sID; }
        }

        public string Information
        {
            get { return this.m_sInformation; }
            set
            {
                this.m_sInformation = value;
                lock (this.m_oDirtyFieldsLock)
                {
                    this.m_bDirtyInformation = true;
                }
            }
        }

        public bool IsDirty
        {
            get
            {
                return (this.m_bDirtyDetails || this.m_bDirtyInformation || this.m_bDirtyItemName ||
                        this.m_bDirtyOperation || this.m_bDirtySource || this.m_bDirtySourceContent ||
                        this.m_bDirtyStatus || this.m_bDirtyTarget || this.m_bDirtyTargetContent ||
                        this.m_bDirtyTimeStamp || this.m_bDirtyFinishedTime
                    ? true
                    : this.m_bDirtyLicenseDataUsed);
            }
        }

        public string ItemName
        {
            get { return this.m_sItemName; }
            set
            {
                this.m_sItemName = value;
                lock (this.m_oDirtyFieldsLock)
                {
                    this.m_bDirtyItemName = true;
                }
            }
        }

        public long LicenseDataUsed
        {
            get { return this.m_lLicenseDataUsed; }
            set
            {
                this.m_lLicenseDataUsed = value;
                lock (this.m_oDirtyFieldsLock)
                {
                    this.m_bDirtyLicenseDataUsed = true;
                }
            }
        }

        public long LicenseItemsCount
        {
            get { return this.m_lLicenseItemsCount; }
            set { this.m_lLicenseItemsCount = value; }
        }

        public string Message
        {
            get
            {
                if (this.Status != ActionOperationStatus.Running)
                {
                    if (this.Status != ActionOperationStatus.Completed)
                    {
                        return this.Status.ToString();
                    }
                }

                return string.Concat(this.Operation, ": '", this.ItemName, "' ...");
            }
        }

        public string Operation
        {
            get { return this.m_sOperation; }
            set
            {
                this.m_sOperation = value;
                lock (this.m_oDirtyFieldsLock)
                {
                    this.m_bDirtyOperation = true;
                }
            }
        }

        public string Source
        {
            get { return this.m_sSource; }
            set
            {
                this.m_sSource = value;
                this.m_bDirtySource = true;
            }
        }

        public string SourceContent
        {
            get
            {
                if (!this.m_bHasDetails)
                {
                    lock (this.m_oLockDetails)
                    {
                        if (!this.m_bHasDetails && this.Adapter != null)
                        {
                            this.Adapter.GetLogItemDetails(this.ID, this);
                            this.m_bHasDetails = true;
                        }
                    }
                }

                return this.m_sSourceContent;
            }
            set
            {
                lock (this.m_oLockDetails)
                {
                    this.m_sSourceContent = value;
                    lock (this.m_oDirtyFieldsLock)
                    {
                        this.m_bDirtySourceContent = true;
                    }

                    this.m_bHasDetails = true;
                }
            }
        }

        public ActionOperationStatus Status
        {
            get { return this.m_status; }
            set
            {
                this.m_status = value;
                lock (this.m_oDirtyFieldsLock)
                {
                    this.m_bDirtyStatus = true;
                }
            }
        }

        public string Target
        {
            get { return this.m_sTarget; }
            set
            {
                this.m_sTarget = value;
                lock (this.m_oDirtyFieldsLock)
                {
                    this.m_bDirtyTarget = true;
                }
            }
        }

        public string TargetContent
        {
            get
            {
                if (!this.m_bHasDetails)
                {
                    lock (this.m_oLockDetails)
                    {
                        if (!this.m_bHasDetails && this.Adapter != null)
                        {
                            this.Adapter.GetLogItemDetails(this.ID, this);
                            this.m_bHasDetails = true;
                        }
                    }
                }

                return this.m_sTargetContent;
            }
            set
            {
                lock (this.m_oLockDetails)
                {
                    this.m_sTargetContent = value;
                    lock (this.m_oDirtyFieldsLock)
                    {
                        this.m_bDirtyTargetContent = true;
                    }

                    this.m_bHasDetails = true;
                }
            }
        }

        public DateTime TimeStamp
        {
            get { return this.m_timeStamp; }
        }

        public string XML
        {
            get
            {
                StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
                this.ToXML(new XmlTextWriter(stringWriter));
                return stringWriter.ToString();
            }
        }

        internal LogItem(XmlNode logItemXML, JobHistoryDb adapter)
        {
            this.FromXML(logItemXML);
            this.m_adapter = adapter;
        }

        internal LogItem(DataRow dataRow_0, JobHistoryDb adapter)
        {
            this.FromDataRow(dataRow_0);
            this.m_adapter = adapter;
        }

        internal LogItem(XmlReader reader)
        {
            if (reader != null && (reader.Name == "LogItem" || reader.ReadToFollowing("LogItem")))
            {
                this.FromXML(reader);
            }
        }

        public LogItem(XmlNode node)
        {
            if (node != null)
            {
                this.FromXML(node);
            }
        }

        public LogItem(string sOperation, string sItemName, string sSource, string sTarget,
            ActionOperationStatus status)
        {
            this.m_sOperation = sOperation;
            this.m_status = status;
            this.m_sItemName = sItemName;
            this.m_sSource = sSource;
            this.m_sTarget = sTarget;
            lock (this.m_oDirtyFieldsLock)
            {
                this.m_bDirtyOperation = true;
                this.m_bDirtyStatus = true;
                this.m_bDirtyItemName = true;
                this.m_bDirtySource = true;
                this.m_bDirtyTarget = true;
                this.m_bDirtyTimeStamp = true;
            }
        }

        public void AddCompletionDetail(string sDetailType, long lCompletionNumber)
        {
            if (!this.CompletionDetails.ContainsKey(sDetailType))
            {
                this.CompletionDetails.Add(sDetailType, lCompletionNumber);
                return;
            }

            Dictionary<string, long> completionDetails = this.CompletionDetails;
            Dictionary<string, long> strs = completionDetails;
            string str = sDetailType;
            completionDetails[str] = strs[str] + lCompletionNumber;
        }

        internal void AddToAdapter(Job parentJob)
        {
            lock (this.m_oDirtyFieldsLock)
            {
                this.Adapter.AddLogItem(parentJob.JobID, this, this.DirtyFieldsList);
                this.CleanFlags();
            }
        }

        private void CleanFlags()
        {
            lock (this.m_oDirtyFieldsLock)
            {
                this.m_bDirtyDetails = false;
                this.m_bDirtyInformation = false;
                this.m_bDirtyItemName = false;
                this.m_bDirtyOperation = false;
                this.m_bDirtySource = false;
                this.m_bDirtySourceContent = false;
                this.m_bDirtyStatus = false;
                this.m_bDirtyTarget = false;
                this.m_bDirtyTargetContent = false;
                this.m_bDirtyTimeStamp = false;
                this.m_bDirtyFinishedTime = false;
                this.m_bDirtyLicenseDataUsed = false;
            }
        }

        public void ClearDetails()
        {
            this.m_sDetails = null;
            this.m_sSourceContent = null;
            this.m_sTargetContent = null;
            this.m_bHasDetails = false;
        }

        private void FromDataRow(DataRow dataRow_0)
        {
            this.m_sID = (!(dataRow_0["LogItemID"] is DBNull) ? (string)dataRow_0["LogItemID"] : this.m_sID);
            this.m_timeStamp = (!(dataRow_0["TimeStamp"] is DBNull)
                ? (DateTime)dataRow_0["TimeStamp"]
                : this.m_timeStamp);
            if (!dataRow_0.Table.Columns.Contains("FinishedTime"))
            {
                this.m_dtFinished = this.m_timeStamp;
            }
            else
            {
                this.m_dtFinished = (!(dataRow_0["FinishedTime"] is DBNull)
                    ? (DateTime)dataRow_0["FinishedTime"]
                    : this.m_dtFinished);
            }

            this.m_status = (!(dataRow_0["Status"] is DBNull)
                ? (ActionOperationStatus)Enum.Parse(typeof(ActionOperationStatus), (string)dataRow_0["Status"])
                : this.m_status);
            this.m_sOperation =
                (!(dataRow_0["Operation"] is DBNull) ? (string)dataRow_0["Operation"] : this.m_sOperation);
            this.m_sItemName = (!(dataRow_0["ItemName"] is DBNull) ? (string)dataRow_0["ItemName"] : this.m_sItemName);
            this.m_sSource = (!(dataRow_0["Source"] is DBNull) ? (string)dataRow_0["Source"] : this.m_sSource);
            this.m_sTarget = (!(dataRow_0["Target"] is DBNull) ? (string)dataRow_0["Target"] : this.m_sTarget);
            this.m_sInformation = (!(dataRow_0["Information"] is DBNull)
                ? (string)dataRow_0["Information"]
                : this.m_sInformation);
            if (dataRow_0.Table.Columns.Contains("LicensedDataUsed"))
            {
                this.m_lLicenseDataUsed =
                    (!(dataRow_0["LicensedDataUsed"] is DBNull) ? (long)dataRow_0["LicensedDataUsed"] : 0L);
            }
        }

        private void FromXML(XmlNode logItemXML)
        {
            this.m_sID = (logItemXML.Attributes["LogItemID"] != null
                ? logItemXML.Attributes["LogItemID"].Value
                : this.m_sID);
            this.m_timeStamp = (logItemXML.Attributes["TimeStamp"] != null
                ? DateTime.Parse(logItemXML.Attributes["TimeStamp"].Value, new CultureInfo("en-US", false))
                : this.m_timeStamp);
            this.m_dtFinished = (logItemXML.Attributes["FinishedTime"] != null
                ? DateTime.Parse(logItemXML.Attributes["FinishedTime"].Value, new CultureInfo("en-US", false))
                : this.m_dtFinished);
            this.m_status = (logItemXML.Attributes["Status"] != null
                ? (ActionOperationStatus)Enum.Parse(typeof(ActionOperationStatus),
                    logItemXML.Attributes["Status"].Value)
                : this.m_status);
            this.m_sOperation = (logItemXML.Attributes["Operation"] != null
                ? logItemXML.Attributes["Operation"].Value
                : this.m_sOperation);
            this.m_sItemName = (logItemXML.Attributes["ItemName"] != null
                ? logItemXML.Attributes["ItemName"].Value
                : this.m_sItemName);
            this.m_sSource = (logItemXML.Attributes["Source"] != null
                ? logItemXML.Attributes["Source"].Value
                : this.m_sSource);
            this.m_sTarget = (logItemXML.Attributes["Target"] != null
                ? logItemXML.Attributes["Target"].Value
                : this.m_sTarget);
            this.m_sInformation = (logItemXML.Attributes["Information"] != null
                ? logItemXML.Attributes["Information"].Value
                : this.m_sInformation);
            this.m_lLicenseDataUsed = (logItemXML.Attributes["LicensedDataUsed"] != null
                ? long.Parse(logItemXML.Attributes["LicensedDataUsed"].Value)
                : 0L);
            this.m_sSourceContent = (logItemXML.Attributes["SourceContent"] != null
                ? logItemXML.Attributes["SourceContent"].Value
                : this.m_sSourceContent);
            this.m_sTargetContent = (logItemXML.Attributes["SourceContent"] != null
                ? logItemXML.Attributes["TargetContent"].Value
                : this.m_sTargetContent);
            this.m_sDetails = (logItemXML.Attributes["Details"] != null
                ? logItemXML.Attributes["Details"].Value
                : this.m_sDetails);
        }

        private void FromXML(XmlReader logItemReader)
        {
            this.m_sID = (logItemReader.MoveToAttribute("LogItemID") ? logItemReader.Value : this.m_sID);
            this.m_timeStamp = (logItemReader.MoveToAttribute("TimeStamp")
                ? DateTime.Parse(logItemReader.Value, new CultureInfo("en-US", false))
                : this.m_timeStamp);
            this.m_dtFinished = (logItemReader.MoveToAttribute("FinishedTime")
                ? DateTime.Parse(logItemReader.Value, new CultureInfo("en-US", false))
                : this.m_dtFinished);
            this.m_status = (logItemReader.MoveToAttribute("Status")
                ? (ActionOperationStatus)Enum.Parse(typeof(ActionOperationStatus), logItemReader.Value)
                : this.m_status);
            this.m_sOperation = (logItemReader.MoveToAttribute("Operation") ? logItemReader.Value : this.m_sOperation);
            this.m_sItemName = (logItemReader.MoveToAttribute("ItemName") ? logItemReader.Value : this.m_sItemName);
            this.m_sSource = (logItemReader.MoveToAttribute("Source") ? logItemReader.Value : this.m_sSource);
            this.m_sTarget = (logItemReader.MoveToAttribute("Target") ? logItemReader.Value : this.m_sTarget);
            this.m_sInformation =
                (logItemReader.MoveToAttribute("Information") ? logItemReader.Value : this.m_sInformation);
            this.m_lLicenseDataUsed =
                (logItemReader.MoveToAttribute("LicensedDataUsed") ? long.Parse(logItemReader.Value) : 0L);
            this.m_sSourceContent = (logItemReader.MoveToAttribute("SourceContent")
                ? logItemReader.Value
                : this.m_sSourceContent);
            this.m_sTargetContent = (logItemReader.MoveToAttribute("SourceContent")
                ? logItemReader.Value
                : this.m_sTargetContent);
            this.m_sDetails = (logItemReader.MoveToAttribute("Details") ? logItemReader.Value : this.m_sDetails);
        }

        internal string GetCompletionsXML()
        {
            StringBuilder stringBuilder = new StringBuilder();
            this.GetCompletionsXML(new XmlTextWriter(new StringWriter(stringBuilder)));
            return stringBuilder.ToString();
        }

        internal void GetCompletionsXML(XmlWriter writer)
        {
            writer.WriteStartElement("CompletionDetails");
            foreach (KeyValuePair<string, long> completionDetail in this.CompletionDetails)
            {
                writer.WriteAttributeString(XmlConvert.EncodeName(completionDetail.Key),
                    completionDetail.Value.ToString());
            }

            writer.WriteEndElement();
        }

        public string GetFormattedLicensedData()
        {
            if (this.ActionLicensingUnit != null && (this.ActionLicensingUnit == "GB" ||
                                                     this.ActionLicensingUnit == "MB" ||
                                                     this.ActionLicensingUnit == "kB" ||
                                                     this.ActionLicensingUnit == "B" ||
                                                     this.ActionLicensingUnit == "Bytes"))
            {
                return Format.FormatSize(new long?(this.LicenseDataUsed));
            }

            long licenseDataUsed = this.LicenseDataUsed;
            return string.Format("{0} {1}", licenseDataUsed.ToString(), this.ActionLicensingUnit);
        }

        internal void LoadCompletionsXML(string sCompletionsXML)
        {
            this.LoadCompletionsXML(XmlUtility.StringToXmlNode(sCompletionsXML));
        }

        internal void LoadCompletionsXML(XmlNode node)
        {
            int num;
            this.m_dictCompletionDetails = new Dictionary<string, long>();
            XmlNode xmlNodes = node.SelectSingleNode(".//CompletionDetails");
            if (xmlNodes != null)
            {
                foreach (XmlAttribute attribute in xmlNodes.Attributes)
                {
                    string str = XmlConvert.DecodeName(attribute.Name);
                    if (!int.TryParse(attribute.Value, out num))
                    {
                        continue;
                    }

                    this.m_dictCompletionDetails.Add(str, (long)num);
                }
            }
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder(1024);
            stringBuilder.Append(this.Operation);
            stringBuilder.Append((this.ItemName == null || this.ItemName.Length <= 0
                ? ""
                : string.Concat(": '", this.ItemName, "' ...")));
            stringBuilder.Append(string.Concat(", [", this.Status, "] "));
            if (this.Status == ActionOperationStatus.Different)
            {
                stringBuilder.Append(this.Information);
            }

            if (this.Status == ActionOperationStatus.Failed)
            {
                stringBuilder.Append(string.Concat(": ", this.Information));
                stringBuilder.Append(string.Concat("\n", this.Details));
            }

            return stringBuilder.ToString();
        }

        public string ToXML()
        {
            string str;
            using (StringWriter stringWriter = new StringWriter())
            {
                using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
                {
                    this.ToXML(xmlTextWriter);
                }

                str = stringWriter.ToString();
            }

            return str;
        }

        public void ToXML(XmlWriter xmlWriter)
        {
            CultureInfo cultureInfo = new CultureInfo("en-US", false);
            xmlWriter.WriteStartElement("LogItem");
            xmlWriter.WriteAttributeString("LogItemID", this.ID);
            xmlWriter.WriteAttributeString("TimeStamp", this.TimeStamp.ToString(cultureInfo));
            xmlWriter.WriteAttributeString("Status", this.Status.ToString());
            xmlWriter.WriteAttributeString("Operation", this.Operation);
            xmlWriter.WriteAttributeString("ItemName", this.ItemName);
            xmlWriter.WriteAttributeString("Source", this.Source);
            xmlWriter.WriteAttributeString("Target", this.Target);
            xmlWriter.WriteAttributeString("Information", this.Information);
            xmlWriter.WriteAttributeString("Details", this.Details);
            xmlWriter.WriteAttributeString("SourceContent", this.SourceContent);
            xmlWriter.WriteAttributeString("TargetContent", this.TargetContent);
            xmlWriter.WriteAttributeString("FinishedTime", this.FinishedTime.ToString(cultureInfo));
            xmlWriter.WriteAttributeString("LicensedDataUsed", this.LicenseDataUsed.ToString());
            xmlWriter.WriteEndElement();
        }

        internal void UpdateAdapter(Job parentJob)
        {
            lock (this.m_oDirtyFieldsLock)
            {
                this.Adapter.UpdateLogItem(parentJob.JobID, this.ID, this, this.DirtyFieldsList);
                this.CleanFlags();
            }
        }
    }
}