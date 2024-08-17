using Metalogix.Actions;
using Metalogix.Actions.Blocker;
using Metalogix.Data;
using Metalogix.Threading;
using Metalogix.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Xml;

namespace Metalogix.Jobs
{
    public class Job : IXmlable
    {
        private object m_oLockUpdater = new object();

        private WorkerThread m_updaterThread;

        private bool _isRemoteJob;

        private static DataTable s_tempDT;

        private JobCollection m_parent;

        private bool m_bLogAsync = true;

        private string m_sActionDefinitionXML;

        private bool m_bDirtyActionOptions;

        private Metalogix.Actions.Action m_action;

        private LogItemCollection m_logItems;

        private object m_oLockLog = new object();

        private string m_sJobID = Guid.NewGuid().ToString();

        private bool m_bDirtyResultsSummary;

        private string m_sResultsSummary;

        private bool m_bDirtyLicenseDataUsed;

        private long m_lLicenseDataUsed;

        private bool m_bDirtyStatusMessage;

        private string m_statusMessage;

        private bool m_bDirtyTitle;

        private string m_sTitle;

        private bool m_bDirtySource;

        private string m_sSource;

        private bool m_bDirtyTarget;

        private string m_sTarget;

        protected string m_sSourceXML;

        protected string m_sTargetXML;

        private bool m_bDirtyStatus;

        private ActionStatus m_status;

        private bool m_bDirtyCreated;

        private DateTime m_created = DateTime.Now;

        private bool m_bDirtyStarted;

        private DateTime? m_started = null;

        private bool m_bDirtyFinished;

        private DateTime? m_finished = null;

        private static string _loggedInUser;

        private bool m_bDirtyUserName;

        private string m_sUserName = Job._loggedInUser;

        private bool m_bDirtyMachineName;

        private string m_sMachineName = Environment.MachineName;

        private bool m_bDirtyCreatedBy;

        private string m_sCreatedBy = Job._loggedInUser;

        public Metalogix.Actions.Action Action
        {
            get { return this.m_action; }
        }

        public DateTime Created
        {
            get { return this.m_created; }
        }

        public string CreatedBy
        {
            get { return this.m_sCreatedBy; }
        }

        private string[] DirtyFieldsList
        {
            get
            {
                List<string> strs = new List<string>();
                if (this.m_bDirtyActionOptions)
                {
                    strs.Add("[Action]");
                }

                if (this.m_bDirtyResultsSummary)
                {
                    strs.Add("ResultsSummary");
                }

                if (this.m_bDirtyStatus)
                {
                    strs.Add("Status");
                }

                if (this.m_bDirtyStatusMessage)
                {
                    strs.Add("StatusMessage");
                }

                if (this.m_bDirtyTitle)
                {
                    strs.Add("Title");
                }

                if (this.m_bDirtySource)
                {
                    strs.Add("Source");
                    if (!this._isRemoteJob)
                    {
                        strs.Add("SourceXml");
                    }
                }

                if (this.m_bDirtyTarget)
                {
                    strs.Add("Target");
                    if (!this._isRemoteJob)
                    {
                        strs.Add("TargetXml");
                    }
                }

                if (this.m_bDirtyStarted)
                {
                    strs.Add("Started");
                }

                if (this.m_bDirtyFinished)
                {
                    strs.Add("Finished");
                }

                if (this.m_bDirtyCreated)
                {
                    strs.Add("Created");
                }

                if (this.m_bDirtyLicenseDataUsed)
                {
                    strs.Add("LicensedDataUsed");
                }

                if (this.m_bDirtyUserName)
                {
                    strs.Add("UserName");
                }

                if (this.m_bDirtyMachineName)
                {
                    strs.Add("MachineName");
                }

                if (this.m_bDirtyCreatedBy)
                {
                    strs.Add("CreatedBy");
                }

                return strs.ToArray();
            }
        }

        public DateTime? Finished
        {
            get { return this.m_finished; }
        }

        internal Metalogix.Jobs.JobHistoryDb JobHistoryDb
        {
            get
            {
                if (this.Parent == null)
                {
                    return null;
                }

                return this.Parent.JobHistoryDb;
            }
        }

        public string JobID
        {
            get { return this.m_sJobID; }
        }

        public long LicenseDataUsed
        {
            get { return this.m_lLicenseDataUsed; }
        }

        public LogItemCollection Log
        {
            get
            {
                LogItemCollection mLogItems;
                lock (this.m_oLockLog)
                {
                    if (this.m_logItems == null)
                    {
                        this.m_logItems = new LogItemCollection(this);
                        this.m_logItems.FetchData();
                    }

                    mLogItems = this.m_logItems;
                }

                return mLogItems;
            }
        }

        public bool LogAsynchronously
        {
            get { return this.m_bLogAsync; }
            set { this.m_bLogAsync = value; }
        }

        public string MachineName
        {
            get { return this.m_sMachineName; }
            set { this.m_sMachineName = value; }
        }

        public JobCollection Parent
        {
            get { return this.m_parent; }
        }

        public string ResultsSummary
        {
            get { return this.m_sResultsSummary; }
        }

        public string Source
        {
            get { return this.m_sSource; }
        }

        public IXMLAbleList SourceList
        {
            get
            {
                if (this.SourceXml == null)
                {
                    return new XMLAbleList();
                }

                return XMLAbleList.CreateIXMLAbleList(this.SourceXml);
            }
            set
            {
                if (value != null)
                {
                    this.m_sSourceXML = XMLAbleList.SerializeXMLAbleList(value);
                    this.m_sSource = value.ToString();
                    this.m_bDirtySource = true;
                    this.FireItemChanged();
                }
            }
        }

        public string SourceXml
        {
            get { return this.m_sSourceXML; }
        }

        public DateTime? Started
        {
            get { return this.m_started; }
        }

        public ActionStatus Status
        {
            get { return this.m_status; }
            set { this.m_status = value; }
        }

        public string StatusMessage
        {
            get { return this.m_statusMessage; }
            set { this.m_statusMessage = value; }
        }

        public string Target
        {
            get { return this.m_sTarget; }
        }

        public IXMLAbleList TargetList
        {
            get
            {
                if (this.TargetXml == null)
                {
                    return new XMLAbleList();
                }

                return XMLAbleList.CreateIXMLAbleList(this.TargetXml);
            }
            set
            {
                if (value != null)
                {
                    this.m_sTargetXML = XMLAbleList.SerializeXMLAbleList(value);
                    this.m_sTarget = value.ToString();
                    this.m_bDirtyTarget = true;
                    this.FireItemChanged();
                }
            }
        }

        public string TargetXml
        {
            get { return this.m_sTargetXML; }
        }

        public string Title
        {
            get { return this.m_sTitle; }
            set
            {
                if (value == null)
                {
                    return;
                }

                if (value.Trim().Length <= 0)
                {
                    throw new ConditionalDetailException("Invalid job name specified.");
                }

                this.m_sTitle = value;
                if (!this.LogAsynchronously || this.m_updaterThread == null)
                {
                    this.JobTitle_Changed(null);
                    return;
                }

                this.m_updaterThread.Enqueue(new TaskDefinition(new ThreadedOperationDelegate(this.JobTitle_Changed),
                    null));
            }
        }

        public string UserName
        {
            get { return this.m_sUserName; }
        }

        public string XML
        {
            get
            {
                StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
                this.ToXML(new XmlTextWriter(stringWriter), false);
                return stringWriter.ToString();
            }
        }

        static Job()
        {
            Job.s_tempDT = new DataTable();
            Job._loggedInUser = string.Concat(Environment.UserDomainName, "\\", Environment.UserName);
        }

        public Job(XmlNode jobXML)
        {
            this.FromXML(jobXML);
        }

        internal Job(DataRow dataRow_0)
        {
            this.FromDataRow(dataRow_0);
        }

        public Job(Metalogix.Actions.Action action_0, IXMLAbleList source, IXMLAbleList target)
        {
            this.SetAction(action_0);
            this.m_sActionDefinitionXML = action_0.ToXML();
            this.m_sTitle = action_0.DisplayName;
            this.m_created = DateTime.Now;
            this.m_sSource = (source != null ? source.ToString() : this.m_sSource);
            this.m_sTarget = (target != null ? target.ToString() : this.m_sTarget);
            this.m_sSourceXML = (source != null ? XMLAbleList.SerializeXMLAbleList(source) : this.m_sSourceXML);
            this.m_sTargetXML = (target != null ? XMLAbleList.SerializeXMLAbleList(target) : this.m_sTargetXML);
            this.m_bDirtyCreated = true;
            this.m_bDirtyTitle = true;
            this.m_bDirtySource = true;
            this.m_bDirtyTarget = true;
            this.m_bDirtyUserName = true;
            this.m_bDirtyMachineName = true;
            this.m_bDirtyCreatedBy = true;
            lock (this.m_oLockLog)
            {
                this.m_logItems = new LogItemCollection(this);
            }
        }

        public Job(Metalogix.Actions.Action action_0, IXMLAbleList source, IXMLAbleList target, string jobId) : this(
            action_0, source, target)
        {
            this.m_sJobID = jobId;
            this.m_logItems = null;
            this.m_finished = null;
            this.m_bDirtyFinished = true;
            this.m_bDirtyTitle = false;
            this._isRemoteJob = true;
        }

        protected void Action_OperationFinished(object[] oParams)
        {
            this.Action_OperationFinished(oParams[0] as LogItem);
        }

        protected void Action_OperationFinished(LogItem operation)
        {
            if (operation.WriteToJobDatabase)
            {
                this.Log.Finish(operation);
                this.m_sResultsSummary = this.Log.ToString();
                this.m_bDirtyResultsSummary = true;
                this.m_lLicenseDataUsed = this.Log.LicenseDataUsed;
                this.m_bDirtyLicenseDataUsed = true;
                this.m_statusMessage = operation.Message;
                this.m_bDirtyStatusMessage = true;
            }

            this.FireActionOperationChanged(operation);
            this.FireActionOperationFinished(operation);
            this.FireItemChanged();
        }

        protected void Action_OperationStarted(object[] oParams)
        {
            this.Action_OperationStarted(oParams[0] as LogItem);
        }

        protected void Action_OperationStarted(LogItem operation)
        {
            if (operation.WriteToJobDatabase)
            {
                this.Log.Add(operation);
                this.m_statusMessage = operation.Message;
                this.m_bDirtyStatusMessage = true;
            }

            this.FireActionOperationChanged(operation);
            this.FireItemChanged();
        }

        protected void Action_OperationUpdated(object[] oParams)
        {
            this.Action_OperationUpdated(oParams[0] as LogItem);
        }

        protected void Action_OperationUpdated(LogItem operation)
        {
            if (operation.WriteToJobDatabase)
            {
                this.Log.Update(operation);
                this.m_sResultsSummary = this.Log.ToString();
                this.m_lLicenseDataUsed = this.Log.LicenseDataUsed;
                this.m_bDirtyResultsSummary = true;
                this.m_bDirtyLicenseDataUsed = true;
            }

            this.FireActionOperationChanged(operation);
            this.FireItemChanged();
        }

        private void Action_Status_Changed(object[] oParams)
        {
            this.Action_Status_Changed((ActionStatus)oParams[0]);
        }

        private void Action_Status_Changed(ActionStatus status)
        {
            this.m_status = status;
            this.m_bDirtyStatus = true;
            this.FireItemChanged();
            this.FireActionStatusChanged();
        }

        protected void ActionFinished(object[] oParams)
        {
            this.ActionFinished();
        }

        protected void ActionFinished()
        {
            this.m_finished = new DateTime?(DateTime.Now);
            this.m_sResultsSummary = this.Log.ToString();
            this.m_status = this.Action.Status;
            if (this.Log.Completions == 0)
            {
                if (this.Log.Failures > 0)
                {
                    this.m_status = ActionStatus.Failed;
                }
                else if (this.Log.Warnings > 0)
                {
                    this.m_status = ActionStatus.Warning;
                }
            }

            this.m_statusMessage = EnumExtensions.GetDescription(this.Status);
            if (this.Action.Options != null && this.Action.Options.SendEmail)
            {
                this.SendJobCompletionEmail();
                this.m_statusMessage = EnumExtensions.GetDescription(this.Status);
            }

            this.m_bDirtyStatus = true;
            this.m_bDirtyStatusMessage = true;
            this.m_bDirtyFinished = true;
            this.m_bDirtyResultsSummary = true;
            this.FireItemChanged();
        }

        protected void ActionOptions_Changed(object[] oParams)
        {
            this.m_bDirtyActionOptions = true;
            this.FireActionChanged();
        }

        protected void ActionStarted(object[] oParams)
        {
            Metalogix.Actions.Action action = oParams[0] as Metalogix.Actions.Action;
            string str = (string)oParams[1];
            this.ActionStarted(action, str, (string)oParams[2]);
        }

        protected void ActionStarted(Metalogix.Actions.Action sender, string sSourceString, string sTargetString)
        {
            if (this.Action == sender)
            {
                this.m_started = new DateTime?(DateTime.Now);
                this.m_status = this.m_action.Status;
                this.m_bDirtyStarted = true;
                this.m_bDirtyStatus = true;
                this.m_sUserName = Job._loggedInUser;
                this.m_sMachineName = Environment.MachineName;
                this.m_bDirtyUserName = true;
                this.m_bDirtyMachineName = true;
            }

            this.FireActionStarting(sender, sSourceString, sTargetString);
            if (this.Action == sender)
            {
                this.FireItemChanged();
            }
        }

        internal void Add()
        {
            this.JobHistoryDb.AddJob(this, this.DirtyFieldsList);
            this.CleanFlags();
        }

        public Dictionary<string, string> Analyze(DateTime? pivotDate)
        {
            DateTime value;
            Metalogix.Actions.Action action = this.Action;
            if (pivotDate.HasValue)
            {
                value = pivotDate.Value;
            }
            else
            {
                value = (this.Finished.HasValue ? this.Finished.Value : new DateTime(1800, 1, 1));
            }

            Dictionary<string, string> strs = action.AnalyzeAction(this, value);
            string title = this.Title;
            int num = title.IndexOf("::");
            if (num > 0)
            {
                title = title.Substring(0, num);
            }

            title = string.Concat(title, "::");
            if (strs == null || strs.Count == 0)
            {
                title = string.Concat(title, " Analysis is unavailable");
            }
            else
            {
                foreach (KeyValuePair<string, string> keyValuePair in strs)
                {
                    string str = string.Concat(keyValuePair.Key, " - ", keyValuePair.Value);
                    title = (title.EndsWith("::") ? string.Concat(title, " ", str) : string.Concat(title, " ; ", str));
                }
            }

            this.Title = title;
            return strs;
        }

        private void CleanFlags()
        {
            this.m_bDirtyResultsSummary = false;
            this.m_bDirtyLicenseDataUsed = false;
            this.m_bDirtyStatus = false;
            this.m_bDirtyStatusMessage = false;
            this.m_bDirtyTitle = false;
            this.m_bDirtySource = false;
            this.m_bDirtyTarget = false;
            this.m_bDirtyStarted = false;
            this.m_bDirtyFinished = false;
            this.m_bDirtyCreated = false;
            this.m_bDirtyActionOptions = false;
            this.m_bDirtyUserName = false;
            this.m_bDirtyMachineName = false;
            this.m_bDirtyCreatedBy = false;
        }

        public void Clear()
        {
            this.m_started = null;
            this.m_finished = null;
            this.m_status = ActionStatus.NotRunning;
            this.m_statusMessage = null;
            this.m_sResultsSummary = null;
            this.m_lLicenseDataUsed = 0L;
            this.m_sUserName = null;
            this.m_sMachineName = null;
            this.ClearLog();
            this.m_bDirtyStarted = true;
            this.m_bDirtyFinished = true;
            this.m_bDirtyLicenseDataUsed = true;
            this.m_bDirtyStatus = true;
            this.m_bDirtyStatusMessage = true;
            this.m_bDirtyResultsSummary = true;
            this.m_bDirtyUserName = true;
            this.m_bDirtyMachineName = true;
            this.FireItemChanged();
        }

        private void ClearLog()
        {
            lock (this.m_oLockLog)
            {
                if (this.m_logItems == null)
                {
                    this.m_logItems = new LogItemCollection(this);
                }

                this.m_logItems.Clear();
            }
        }

        private void FireActionBlocked(object sender, ActionBlockerEventArgs e)
        {
            if (this.ActionBlocked != null)
            {
                this.ActionBlocked(sender, e);
            }
        }

        private void FireActionChanged()
        {
            if (this.Parent != null)
            {
                this.Parent.FireListChanged(ChangeType.ItemComponentUpdated, this);
                this.Parent.Update();
            }
        }

        private void FireActionOperationChanged(LogItem logItem)
        {
            if (this.ActionOperationChanged != null)
            {
                this.ActionOperationChanged(logItem);
            }
        }

        private void FireActionOperationFinished(LogItem logItem)
        {
            if (this.ActionOperationFinishedEvent != null)
            {
                this.ActionOperationFinishedEvent(logItem);
            }
        }

        private void FireActionStarting(Metalogix.Actions.Action sender, string sSourceString, string sTargetString)
        {
            if (this.ActionStarting != null)
            {
                this.ActionStarting(sender, sSourceString, sTargetString);
            }
        }

        private void FireActionStatusChanged()
        {
            if (this.ActionStatusChanged != null)
            {
                this.ActionStatusChanged(this.Status);
            }
        }

        private void FireItemChanged()
        {
            if (this.Parent != null)
            {
                this.Parent.FireListChanged(ChangeType.ItemUpdated, this);
                this.Parent.Update();
            }
        }

        private void FromDataRow(DataRow dataRow_0)
        {
            string item;
            try
            {
                this.m_sJobID = (!(dataRow_0["JobID"] is DBNull) ? (string)dataRow_0["JobID"] : this.m_sJobID);
                this.m_sResultsSummary = (!(dataRow_0["ResultsSummary"] is DBNull)
                    ? (string)dataRow_0["ResultsSummary"]
                    : this.m_sResultsSummary);
                this.m_status = (!(dataRow_0["Status"] is DBNull)
                    ? (ActionStatus)Enum.Parse(typeof(ActionStatus), (string)dataRow_0["Status"])
                    : this.m_status);
                this.m_statusMessage = (!(dataRow_0["StatusMessage"] is DBNull)
                    ? (string)dataRow_0["StatusMessage"]
                    : this.m_statusMessage);
                this.m_sTitle = (!(dataRow_0["Title"] is DBNull) ? (string)dataRow_0["Title"] : this.m_sTitle);
                this.m_sSource = (!(dataRow_0["Source"] is DBNull) ? (string)dataRow_0["Source"] : this.m_sSource);
                this.m_sTarget = (!(dataRow_0["Target"] is DBNull) ? (string)dataRow_0["Target"] : this.m_sTarget);
                this.m_sSourceXML = (!(dataRow_0["SourceXml"] is DBNull)
                    ? (string)dataRow_0["SourceXml"]
                    : this.m_sSourceXML);
                this.m_sTargetXML = (!(dataRow_0["TargetXml"] is DBNull)
                    ? (string)dataRow_0["TargetXml"]
                    : this.m_sTargetXML);
                this.m_started = (!(dataRow_0["Started"] is DBNull)
                    ? new DateTime?((DateTime)dataRow_0["Started"])
                    : this.m_started);
                this.m_finished = (!(dataRow_0["Finished"] is DBNull)
                    ? new DateTime?((DateTime)dataRow_0["Finished"])
                    : this.m_finished);
                this.m_created = (!(dataRow_0["Created"] is DBNull) ? (DateTime)dataRow_0["Created"] : this.m_created);
                if (dataRow_0.Table.Columns.Contains("LicensedDataUsed"))
                {
                    this.m_lLicenseDataUsed = (!(dataRow_0["LicensedDataUsed"] is DBNull)
                        ? (long)dataRow_0["LicensedDataUsed"]
                        : 0L);
                }

                if (!(dataRow_0["Action"] is DBNull))
                {
                    item = (string)dataRow_0["Action"];
                }
                else
                {
                    item = null;
                }

                this.m_sActionDefinitionXML = item;
                if (dataRow_0.Table.Columns.Contains("UserName"))
                {
                    this.m_sUserName = (!(dataRow_0["UserName"] is DBNull)
                        ? (string)dataRow_0["UserName"]
                        : string.Empty);
                }

                if (dataRow_0.Table.Columns.Contains("MachineName"))
                {
                    this.m_sMachineName = (!(dataRow_0["MachineName"] is DBNull)
                        ? (string)dataRow_0["MachineName"]
                        : string.Empty);
                }

                if (dataRow_0.Table.Columns.Contains("CreatedBy"))
                {
                    this.m_sCreatedBy = (!(dataRow_0["CreatedBy"] is DBNull)
                        ? (string)dataRow_0["CreatedBy"]
                        : string.Empty);
                }

                this.SetAction((this.m_sActionDefinitionXML != null
                    ? Metalogix.Actions.Action.CreateAction(this.m_sActionDefinitionXML)
                    : this.m_action));
            }
            catch (Exception exception)
            {
            }
        }

        private void FromXML(XmlNode xmlNode_0)
        {
            string outerXml;
            try
            {
                this.m_sJobID = (xmlNode_0.Attributes["JobID"] != null
                    ? xmlNode_0.Attributes["JobID"].Value
                    : this.m_sJobID);
                this.m_sResultsSummary = (xmlNode_0.Attributes["ResultsSummary"] != null
                    ? xmlNode_0.Attributes["ResultsSummary"].Value
                    : this.m_sResultsSummary);
                this.m_status = (xmlNode_0.Attributes["Status"] != null
                    ? (ActionStatus)Enum.Parse(typeof(ActionStatus), xmlNode_0.Attributes["Status"].Value)
                    : this.m_status);
                this.m_statusMessage = (xmlNode_0.Attributes["StatusMessage"] != null
                    ? xmlNode_0.Attributes["StatusMessage"].Value
                    : this.m_statusMessage);
                this.m_sTitle = (xmlNode_0.Attributes["Title"] != null
                    ? xmlNode_0.Attributes["Title"].Value
                    : this.m_sTitle);
                this.m_sSource = (xmlNode_0.Attributes["Source"] != null
                    ? xmlNode_0.Attributes["Source"].Value
                    : this.m_sSource);
                this.m_sTarget = (xmlNode_0.Attributes["Target"] != null
                    ? xmlNode_0.Attributes["Target"].Value
                    : this.m_sTarget);
                this.m_sSourceXML = (xmlNode_0.Attributes["SourceXml"] != null
                    ? xmlNode_0.Attributes["SourceXml"].Value
                    : this.m_sSourceXML);
                this.m_sTargetXML = (xmlNode_0.Attributes["TargetXml"] != null
                    ? xmlNode_0.Attributes["TargetXml"].Value
                    : this.m_sTargetXML);
                this.m_started = (xmlNode_0.Attributes["Started"] != null
                    ? new DateTime?(DateTime.Parse(xmlNode_0.Attributes["Started"].Value,
                        new CultureInfo("en-US", false)))
                    : this.m_started);
                this.m_finished = (xmlNode_0.Attributes["Finished"] != null
                    ? new DateTime?(DateTime.Parse(xmlNode_0.Attributes["Finished"].Value,
                        new CultureInfo("en-US", false)))
                    : this.m_finished);
                this.m_created = (xmlNode_0.Attributes["Created"] != null
                    ? DateTime.Parse(xmlNode_0.Attributes["Created"].Value, new CultureInfo("en-US", false))
                    : this.m_created);
                this.m_lLicenseDataUsed = (xmlNode_0.Attributes["LicenseDataUsed"] != null
                    ? long.Parse(xmlNode_0.Attributes["LicenseDataUsed"].Value)
                    : 0L);
                this.m_sUserName = (xmlNode_0.Attributes["UserName"] != null
                    ? xmlNode_0.Attributes["UserName"].Value
                    : string.Empty);
                this.m_sMachineName = (xmlNode_0.Attributes["MachineName"] != null
                    ? xmlNode_0.Attributes["MachineName"].Value
                    : string.Empty);
                this.m_sCreatedBy = (xmlNode_0.Attributes["CreatedBy"] != null
                    ? xmlNode_0.Attributes["CreatedBy"].Value
                    : string.Empty);
                if (xmlNode_0["Action"] != null)
                {
                    outerXml = xmlNode_0["Action"].OuterXml;
                }
                else
                {
                    outerXml = null;
                }

                this.m_sActionDefinitionXML = outerXml;
                this.SetAction((this.m_sActionDefinitionXML != null
                    ? Metalogix.Actions.Action.CreateAction(this.m_sActionDefinitionXML)
                    : this.m_action));
                XmlNode xmlNodes = xmlNode_0.SelectSingleNode("LogItems");
                if (xmlNodes != null)
                {
                    this.m_logItems = new LogItemCollection(xmlNodes);
                }

                this.m_bDirtyResultsSummary = true;
                this.m_bDirtyLicenseDataUsed = true;
                this.m_bDirtyStatus = true;
                this.m_bDirtyStatusMessage = true;
                this.m_bDirtyTitle = true;
                this.m_bDirtySource = true;
                this.m_bDirtyTarget = true;
                this.m_bDirtyStarted = true;
                this.m_bDirtyFinished = true;
                this.m_bDirtyCreated = true;
                this.m_bDirtyActionOptions = true;
                this.m_bDirtyUserName = true;
                this.m_bDirtyMachineName = true;
                this.m_bDirtyCreatedBy = true;
            }
            catch (Exception exception)
            {
            }
        }

        internal string GetActionXml()
        {
            if (this.m_action == null)
            {
                return this.m_sActionDefinitionXML;
            }

            return this.m_action.ToXML();
        }

        public string GetFormattedLicensedData(bool includeDataName = true)
        {
            string licensingUnit = null;
            string licensingDescriptor = null;
            if (this.Action == null)
            {
                licensingUnit = "Units";
                licensingDescriptor = "Data Used";
            }
            else
            {
                licensingUnit = this.Action.LicensingUnit;
                licensingDescriptor = this.Action.LicensingDescriptor;
                if (licensingUnit == "GB" || licensingUnit == "MB" || licensingUnit == "kB" || licensingUnit == "B" ||
                    licensingUnit == "Bytes")
                {
                    return string.Concat((includeDataName ? string.Concat(licensingDescriptor, ": ") : ""),
                        Format.FormatSize(new long?(this.LicenseDataUsed)));
                }
            }

            long licenseDataUsed = this.LicenseDataUsed;
            string str = string.Format("{0} {1}", licenseDataUsed.ToString(), licensingUnit);
            if (!includeDataName)
            {
                return str;
            }

            return string.Format("{0}: {1}", licensingDescriptor, str);
        }

        protected void JobTitle_Changed(object[] oParams)
        {
            this.m_bDirtyTitle = true;
            this.FireItemChanged();
        }

        private void On_Action_OperationFinished(LogItem operation)
        {
            if (!this.LogAsynchronously || this.m_updaterThread == null)
            {
                this.Action_OperationFinished(operation);
                return;
            }

            WorkerThread mUpdaterThread = this.m_updaterThread;
            ThreadedOperationDelegate threadedOperationDelegate =
                new ThreadedOperationDelegate(this.Action_OperationFinished);
            object[] objArray = new object[] { operation };
            mUpdaterThread.Enqueue(new TaskDefinition(threadedOperationDelegate, objArray));
        }

        private void On_Action_OperationStarted(LogItem operation)
        {
            if (!this.LogAsynchronously || this.m_updaterThread == null)
            {
                this.Action_OperationStarted(operation);
                return;
            }

            WorkerThread mUpdaterThread = this.m_updaterThread;
            ThreadedOperationDelegate threadedOperationDelegate =
                new ThreadedOperationDelegate(this.Action_OperationStarted);
            object[] objArray = new object[] { operation };
            mUpdaterThread.Enqueue(new TaskDefinition(threadedOperationDelegate, objArray));
        }

        private void On_Action_OperationUpdated(LogItem operation)
        {
            if (!this.LogAsynchronously || this.m_updaterThread == null)
            {
                this.Action_OperationUpdated(operation);
                return;
            }

            WorkerThread mUpdaterThread = this.m_updaterThread;
            ThreadedOperationDelegate threadedOperationDelegate =
                new ThreadedOperationDelegate(this.Action_OperationUpdated);
            object[] objArray = new object[] { operation };
            mUpdaterThread.Enqueue(new TaskDefinition(threadedOperationDelegate, objArray));
        }

        private void On_Action_StatusChanged(ActionStatus status)
        {
            if (!this.LogAsynchronously || this.m_updaterThread == null)
            {
                this.Action_Status_Changed(status);
                return;
            }

            WorkerThread mUpdaterThread = this.m_updaterThread;
            ThreadedOperationDelegate threadedOperationDelegate =
                new ThreadedOperationDelegate(this.Action_Status_Changed);
            object[] objArray = new object[] { status };
            mUpdaterThread.Enqueue(new TaskDefinition(threadedOperationDelegate, objArray));
        }

        private void On_ActionBlocked(object sender, ActionBlockerEventArgs e)
        {
            this.FireActionBlocked(sender, e);
        }

        private void On_ActionFinished()
        {
            if (!this.LogAsynchronously || this.m_updaterThread == null)
            {
                this.ActionFinished();
            }
            else
            {
                lock (this.m_oLockUpdater)
                {
                    this.m_updaterThread.Enqueue(new TaskDefinition(new ThreadedOperationDelegate(this.ActionFinished),
                        null));
                    this.m_updaterThread.Dispose();
                    WorkerThread.Count++;
                    this.m_updaterThread = null;
                }
            }
        }

        private void On_ActionOptions_Changed()
        {
            if (!this.LogAsynchronously || this.m_updaterThread == null)
            {
                this.ActionOptions_Changed(null);
                return;
            }

            this.m_updaterThread.Enqueue(new TaskDefinition(new ThreadedOperationDelegate(this.ActionOptions_Changed),
                null));
        }

        private void On_ActionStarted(Metalogix.Actions.Action sender, string sSourceString, string sTargetString)
        {
            if (!this.LogAsynchronously)
            {
                this.ActionStarted(sender, sSourceString, sTargetString);
            }
            else
            {
                lock (this.m_oLockUpdater)
                {
                    if (this.m_updaterThread == null)
                    {
                        this.m_updaterThread = new WorkerThread(ThreadPriority.AboveNormal);
                        WorkerThread.Count--;
                    }

                    WorkerThread mUpdaterThread = this.m_updaterThread;
                    ThreadedOperationDelegate threadedOperationDelegate =
                        new ThreadedOperationDelegate(this.ActionStarted);
                    object[] objArray = new object[] { sender, sSourceString, sTargetString };
                    mUpdaterThread.Enqueue(new TaskDefinition(threadedOperationDelegate, objArray));
                }
            }
        }

        private void On_ActionValidated(Metalogix.Actions.Action action, IXMLAbleList source, IXMLAbleList target)
        {
            if (this.JobHistoryDb == null)
            {
                return;
            }

            try
            {
                this.SourceList = source;
                this.TargetList = target;
                this.Update();
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception);
            }
        }

        private string ReadFile()
        {
            TextReader textReader = null;
            textReader = (this.Status != ActionStatus.Failed
                ? new StreamReader(this.Action.Options.EmailSuccessTemplateFilePath)
                : new StreamReader(this.Action.Options.EmailFailureTemplateFilePath));
            string end = textReader.ReadToEnd();
            textReader.Close();
            return end;
        }

        private string ReplaceTags(string input, Dictionary<string, object> tagDictionary)
        {
            string str = input;
            foreach (KeyValuePair<string, object> keyValuePair in tagDictionary)
            {
                str = str.Replace(string.Concat("<", keyValuePair.Key, ">"),
                    string.Format("<{0}>{1}</{0}>", keyValuePair.Key, keyValuePair.Value));
            }

            return str;
        }

        public void RevalidateActionLicense()
        {
            if (this.m_action == null)
            {
                Metalogix.Actions.Action action = null;
                try
                {
                    action = (!string.IsNullOrEmpty(this.m_sActionDefinitionXML)
                        ? Metalogix.Actions.Action.CreateAction(this.m_sActionDefinitionXML)
                        : this.m_action);
                }
                catch
                {
                }

                if (action != null)
                {
                    this.SetAction(action);
                    this.FireItemChanged();
                    return;
                }
            }
            else if (!ActionLicenseProvider.Instance.IsValid(this.m_action.GetType()))
            {
                this.SetAction(null);
                this.FireItemChanged();
            }
        }

        private void SendJobCompletionEmail()
        {
            TimeSpan? nullable;
            Dictionary<string, object> strs = new Dictionary<string, object>()
            {
                { "MLJobName", this.Title },
                { "MLSource", this.Source },
                { "MLTarget", this.Target },
                { "MLStartTime", this.Started },
                { "MLStatus", this.StatusMessage },
                { "MLFinishedTime", this.Finished }
            };
            Dictionary<string, object> strs1 = strs;
            DateTime? finished = this.Finished;
            DateTime? started = this.Started;
            if (finished.HasValue & started.HasValue)
            {
                nullable = new TimeSpan?(finished.GetValueOrDefault() - started.GetValueOrDefault());
            }
            else
            {
                nullable = null;
            }

            strs1.Add("MLDuration", nullable);
            strs.Add("MLLogSummary", this.ResultsSummary);
            LogItem logItem = new LogItem("Sending Job Completion Email", "Job Completion Email",
                this.Action.Options.FromEmailAddress, this.Action.Options.ToEmailAddress,
                ActionOperationStatus.Running);
            this.Action.FireOperationStarted(logItem);
            try
            {
                try
                {
                    MailAddress mailAddress = new MailAddress(this.Action.Options.FromEmailAddress);
                    MailAddress mailAddress1 = null;
                    string[] strArrays = this.Action.Options.ToEmailAddress.Split(new char[] { ';' });
                    int length = strArrays.GetLength(0);
                    if (length >= 1)
                    {
                        mailAddress1 = new MailAddress(strArrays[0]);
                    }

                    MailMessage mailMessage = new MailMessage(mailAddress, mailAddress1);
                    for (int i = 1; i < length; i++)
                    {
                        mailMessage.To.Add(strArrays[i]);
                    }

                    mailMessage.Subject = (!string.IsNullOrEmpty(this.Action.Options.EmailSubject)
                        ? this.ReplaceTags(this.Action.Options.EmailSubject, strs)
                        : string.Concat("Job ID: ", this.JobID));
                    mailMessage.IsBodyHtml = true;
                    if (string.IsNullOrEmpty(this.Action.Options.EmailSuccessTemplateFilePath) ||
                        string.IsNullOrEmpty(this.Action.Options.EmailFailureTemplateFilePath))
                    {
                        MailMessage mailMessage1 = mailMessage;
                        string[] jobID = new string[]
                            { "<b>Job ID:</b>", this.JobID, " completed at ", null, null, null };
                        jobID[3] = (this.Finished.HasValue ? this.Finished.ToString() : "");
                        jobID[4] = ".<br>";
                        jobID[5] = this.ResultsSummary;
                        mailMessage1.Body = string.Concat(jobID);
                    }
                    else
                    {
                        mailMessage.Body = this.ReadFile();
                    }

                    mailMessage.Body = this.ReplaceTags(mailMessage.Body, strs);
                    if (!string.IsNullOrEmpty(this.Action.Options.CCEmailAddress))
                    {
                        string[] strArrays1 = this.Action.Options.CCEmailAddress.Split(new char[] { ';' });
                        for (int j = 0; j < (int)strArrays1.Length; j++)
                        {
                            string str = strArrays1[j];
                            mailMessage.CC.Add(str);
                        }
                    }

                    if (!string.IsNullOrEmpty(this.Action.Options.BCCEmailAddress))
                    {
                        string[] strArrays2 = this.Action.Options.BCCEmailAddress.Split(new char[] { ';' });
                        for (int k = 0; k < (int)strArrays2.Length; k++)
                        {
                            string str1 = strArrays2[k];
                            mailMessage.Bcc.Add(str1);
                        }
                    }

                    SmtpClient smtpClient = new SmtpClient(this.Action.Options.EmailServer)
                    {
                        EnableSsl = this.Action.Options.EnableSslForEmail
                    };
                    if (!string.IsNullOrEmpty(this.Action.Options.EmailUserName))
                    {
                        smtpClient.Credentials = new NetworkCredential(this.Action.Options.EmailUserName,
                            this.Action.Options.EmailPassword);
                    }
                    else
                    {
                        smtpClient.Credentials = CredentialCache.DefaultNetworkCredentials;
                    }

                    smtpClient.Send(mailMessage);
                    logItem.Status = ActionOperationStatus.Completed;
                }
                catch (Exception exception)
                {
                    logItem.Exception = exception;
                }
            }
            finally
            {
                this.Action.FireOperationFinished(logItem);
            }
        }

        private void SetAction(Metalogix.Actions.Action action_0)
        {
            this.m_action = action_0;
            if (this.m_action != null)
            {
                this.m_action.ActionStarted += new ActionStartedEventHandler(this.On_ActionStarted);
                this.m_action.ActionFinished += new ActionFinishedEventHandler(this.On_ActionFinished);
                this.m_action.OperationStarted += new ActionEventHandler(this.On_Action_OperationStarted);
                this.m_action.OperationUpdated += new ActionEventHandler(this.On_Action_OperationUpdated);
                this.m_action.OperationFinished += new ActionEventHandler(this.On_Action_OperationFinished);
                this.m_action.StatusChanged += new ActionStatusChangedHandler(this.On_Action_StatusChanged);
                this.m_action.OptionsChanged += new ActionOptionsChangedHandler(this.On_ActionOptions_Changed);
                this.m_action.ActionValidated += new ActionValidatedEventHandler(this.On_ActionValidated);
                this.m_action.ActionBlocked += new ActionBlockerHandler(this.On_ActionBlocked);
            }

            this.m_bDirtyActionOptions = true;
        }

        internal void SetActionXml(string actionXml)
        {
            this.m_sActionDefinitionXML = actionXml;
        }

        internal void SetParent(JobCollection parent)
        {
            this.m_parent = parent;
        }

        public void ToXML(XmlWriter xmlWriter, bool bIncludeItems)
        {
            xmlWriter.WriteStartElement("Job");
            xmlWriter.WriteAttributeString("JobID", this.JobID);
            xmlWriter.WriteAttributeString("Status", this.Status.ToString());
            if (this.ResultsSummary != null)
            {
                xmlWriter.WriteAttributeString("ResultsSummary", this.ResultsSummary);
            }

            if (this.StatusMessage != null)
            {
                xmlWriter.WriteAttributeString("StatusMessage", this.StatusMessage);
            }

            if (this.Title != null)
            {
                xmlWriter.WriteAttributeString("Title", this.Title);
            }

            if (this.Source != null)
            {
                xmlWriter.WriteAttributeString("Source", this.Source);
            }

            if (this.Target != null)
            {
                xmlWriter.WriteAttributeString("Target", this.Target);
            }

            if (this.SourceXml != null)
            {
                xmlWriter.WriteAttributeString("SourceXml", this.SourceXml);
            }

            if (this.TargetXml != null)
            {
                xmlWriter.WriteAttributeString("TargetXml", this.TargetXml);
            }

            if (this.Started.HasValue)
            {
                DateTime value = this.Started.Value;
                xmlWriter.WriteAttributeString("Started", value.ToString(new CultureInfo("en-US", false)));
            }

            if (this.Finished.HasValue)
            {
                DateTime dateTime = this.Finished.Value;
                xmlWriter.WriteAttributeString("Finished", dateTime.ToString(new CultureInfo("en-US", false)));
            }

            DateTime created = this.Created;
            xmlWriter.WriteAttributeString("Created", created.ToString(new CultureInfo("en-US", false)));
            xmlWriter.WriteAttributeString("UserName", this.UserName);
            xmlWriter.WriteAttributeString("MachineName", this.MachineName);
            xmlWriter.WriteAttributeString("CreatedBy", this.CreatedBy);
            xmlWriter.WriteAttributeString("LicenseDataUsed", this.LicenseDataUsed.ToString());
            if (this.m_action != null)
            {
                this.m_action.ToXML(xmlWriter);
            }
            else if (this.m_sActionDefinitionXML != null)
            {
                xmlWriter.WriteRaw(this.m_sActionDefinitionXML);
            }

            if (bIncludeItems)
            {
                xmlWriter.WriteStartElement("LogItems");
                foreach (LogItem log in this.Log)
                {
                    log.ToXML(xmlWriter);
                    log.ClearDetails();
                }

                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
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
            this.ToXML(xmlWriter, true);
        }

        public static bool TryCreateJobs(XmlNode jobsXML, out IList<Job> output)
        {
            XmlNodeList xmlNodeLists = jobsXML.SelectNodes("//Job");
            if (xmlNodeLists == null || xmlNodeLists.Count == 0)
            {
                output = new List<Job>();
                return false;
            }

            List<Job> list = (
                from XmlNode jobXml in xmlNodeLists
                select new Job(jobXml)).ToList<Job>();
            output = list;
            return true;
        }

        internal void Update()
        {
            this.JobHistoryDb.UpdateJob(this, this.DirtyFieldsList);
            this.CleanFlags();
        }

        public event ActionBlockerHandler ActionBlocked;

        public event ActionEventHandler ActionOperationChanged;

        public event ActionEventHandler ActionOperationFinishedEvent;

        public event ActionStartedEventHandler ActionStarting;

        public event ActionStatusChangedHandler ActionStatusChanged;
    }
}