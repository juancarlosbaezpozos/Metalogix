using Metalogix.Core;
using Metalogix.DataStructures.Generic;
using Metalogix.Jobs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml;

namespace Metalogix.Actions
{
    public class LogItemCollection : SerializableList<LogItem>
    {
        private object m_oLockCollection = new object();

        private Job m_parentJob;

        private int m_iCompletions;

        private Dictionary<string, long> m_dictCompletionDetails;

        private int m_iOtherCompletions;

        private int? m_iFailures = null;

        private int? m_iSkipped = null;

        private int? m_iWarnings = null;

        private int? m_iDifferent = null;

        private int? m_iMissingOnSource = null;

        private int? m_iMissingOnTarget = null;

        public long? m_lLicenseDataUsed = null;

        public Dictionary<string, long> CompletionDetails
        {
            get { return this.m_dictCompletionDetails; }
        }

        public int Completions
        {
            get { return this.m_iCompletions; }
        }

        public int Differences
        {
            get
            {
                int? nullable;
                if (!this.m_iDifferent.HasValue)
                {
                    this.m_iDifferent = new int?(0);
                    for (int i = 0; i < this.m_collection.Count; i++)
                    {
                        if (this.m_collection[i].Status == ActionOperationStatus.Different)
                        {
                            LogItemCollection logItemCollection = this;
                            int? mIDifferent = logItemCollection.m_iDifferent;
                            if (mIDifferent.HasValue)
                            {
                                nullable = new int?(mIDifferent.GetValueOrDefault() + 1);
                            }
                            else
                            {
                                nullable = null;
                            }

                            logItemCollection.m_iDifferent = nullable;
                        }
                    }
                }

                return this.m_iDifferent.Value;
            }
        }

        public Exception[] Exceptions
        {
            get
            {
                if (this.Failures == 0)
                {
                    return new Exception[0];
                }

                List<Exception> exceptions = new List<Exception>();
                for (int i = 0; i < this.m_collection.Count; i++)
                {
                    if (this.m_collection[i].Status == ActionOperationStatus.Failed)
                    {
                        exceptions.Add(this.m_collection[i].Exception);
                    }
                }

                return exceptions.ToArray();
            }
        }

        public int Failures
        {
            get
            {
                int? nullable;
                if (!this.m_iFailures.HasValue)
                {
                    this.m_iFailures = new int?(0);
                    for (int i = 0; i < this.m_collection.Count; i++)
                    {
                        if (this.m_collection[i].Status == ActionOperationStatus.Failed)
                        {
                            LogItemCollection logItemCollection = this;
                            int? mIFailures = logItemCollection.m_iFailures;
                            if (mIFailures.HasValue)
                            {
                                nullable = new int?(mIFailures.GetValueOrDefault() + 1);
                            }
                            else
                            {
                                nullable = null;
                            }

                            logItemCollection.m_iFailures = nullable;
                        }
                    }
                }

                return this.m_iFailures.Value;
            }
        }

        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override bool IsSet
        {
            get { return false; }
        }

        public override LogItem this[LogItem logItem_0]
        {
            get { throw new NotImplementedException(); }
        }

        internal Metalogix.Jobs.JobHistoryDb JobHistoryDb
        {
            get
            {
                if (this.ParentJob == null)
                {
                    return null;
                }

                return this.ParentJob.JobHistoryDb;
            }
        }

        public long LicenseDataUsed
        {
            get
            {
                long? nullable;
                if (!this.m_lLicenseDataUsed.HasValue)
                {
                    this.m_lLicenseDataUsed = new long?(0L);
                    foreach (LogItem logItem in this)
                    {
                        LogItemCollection logItemCollection = this;
                        long? mLLicenseDataUsed = logItemCollection.m_lLicenseDataUsed;
                        long licenseDataUsed = logItem.LicenseDataUsed;
                        if (mLLicenseDataUsed.HasValue)
                        {
                            nullable = new long?(mLLicenseDataUsed.GetValueOrDefault() + licenseDataUsed);
                        }
                        else
                        {
                            nullable = null;
                        }

                        logItemCollection.m_lLicenseDataUsed = nullable;
                    }
                }

                return this.m_lLicenseDataUsed.Value;
            }
        }

        public int MissingOnSource
        {
            get
            {
                int? nullable;
                if (!this.m_iMissingOnSource.HasValue)
                {
                    this.m_iMissingOnSource = new int?(0);
                    for (int i = 0; i < this.m_collection.Count; i++)
                    {
                        if (this.m_collection[i].Status == ActionOperationStatus.MissingOnSource)
                        {
                            LogItemCollection logItemCollection = this;
                            int? mIMissingOnSource = logItemCollection.m_iMissingOnSource;
                            if (mIMissingOnSource.HasValue)
                            {
                                nullable = new int?(mIMissingOnSource.GetValueOrDefault() + 1);
                            }
                            else
                            {
                                nullable = null;
                            }

                            logItemCollection.m_iMissingOnSource = nullable;
                        }
                    }
                }

                return this.m_iMissingOnSource.Value;
            }
        }

        public int MissingOnTarget
        {
            get
            {
                int? nullable;
                if (!this.m_iMissingOnTarget.HasValue)
                {
                    this.m_iMissingOnTarget = new int?(0);
                    for (int i = 0; i < this.m_collection.Count; i++)
                    {
                        if (this.m_collection[i].Status == ActionOperationStatus.MissingOnTarget)
                        {
                            LogItemCollection logItemCollection = this;
                            int? mIMissingOnTarget = logItemCollection.m_iMissingOnTarget;
                            if (mIMissingOnTarget.HasValue)
                            {
                                nullable = new int?(mIMissingOnTarget.GetValueOrDefault() + 1);
                            }
                            else
                            {
                                nullable = null;
                            }

                            logItemCollection.m_iMissingOnTarget = nullable;
                        }
                    }
                }

                return this.m_iMissingOnTarget.Value;
            }
        }

        public int OtherCompletions
        {
            get { return this.m_iOtherCompletions; }
        }

        public Job ParentJob
        {
            get { return this.m_parentJob; }
        }

        public int Skipped
        {
            get
            {
                int? nullable;
                if (!this.m_iSkipped.HasValue)
                {
                    this.m_iSkipped = new int?(0);
                    for (int i = 0; i < this.m_collection.Count; i++)
                    {
                        if (this.m_collection[i].Status == ActionOperationStatus.Skipped)
                        {
                            LogItemCollection logItemCollection = this;
                            int? mISkipped = logItemCollection.m_iSkipped;
                            if (mISkipped.HasValue)
                            {
                                nullable = new int?(mISkipped.GetValueOrDefault() + 1);
                            }
                            else
                            {
                                nullable = null;
                            }

                            logItemCollection.m_iSkipped = nullable;
                        }
                    }
                }

                return this.m_iSkipped.Value;
            }
        }

        public int Warnings
        {
            get
            {
                int? nullable;
                if (!this.m_iWarnings.HasValue)
                {
                    this.m_iWarnings = new int?(0);
                    for (int i = 0; i < this.m_collection.Count; i++)
                    {
                        if (this.m_collection[i].Status == ActionOperationStatus.Warning)
                        {
                            LogItemCollection logItemCollection = this;
                            int? mIWarnings = logItemCollection.m_iWarnings;
                            if (mIWarnings.HasValue)
                            {
                                nullable = new int?(mIWarnings.GetValueOrDefault() + 1);
                            }
                            else
                            {
                                nullable = null;
                            }

                            logItemCollection.m_iWarnings = nullable;
                        }
                    }
                }

                return this.m_iWarnings.Value;
            }
        }

        private LogItemCollection()
        {
            this.InitialiseCommonVariables();
        }

        public LogItemCollection(string fileName) : this()
        {
            if (!string.IsNullOrEmpty(fileName) && File.Exists(fileName))
            {
                this.FromXML(XmlReader.Create(new FileStream(fileName, FileMode.Open)));
                this.PopulateCompletionDetailsWithLock();
            }
        }

        public LogItemCollection(XmlNode xmlNode) : this()
        {
            if (xmlNode != null)
            {
                this.FromXML(xmlNode);
                this.PopulateCompletionDetailsWithLock();
            }
        }

        public LogItemCollection(List<LogItem> logItems) : this()
        {
            this.m_collection = logItems;
            this.PopulateCompletionDetailsWithLock();
        }

        public LogItemCollection(Job job_0) : this()
        {
            this.m_parentJob = job_0;
            this.PopulateCompletionDetailsWithLock();
        }

        public void Add(LogItem newItem)
        {
            lock (this.m_oLockCollection)
            {
                this.m_collection.Add(newItem);
            }

            this.FireListChanged(ChangeType.ItemAdded, newItem);
            if (this.JobHistoryDb != null)
            {
                if (newItem.Adapter == null)
                {
                    newItem.Adapter = this.JobHistoryDb;
                }

                newItem.AddToAdapter(this.ParentJob);
            }
        }

        public new void Clear()
        {
            if (this.JobHistoryDb != null)
            {
                this.JobHistoryDb.DeleteLogItems(this.ParentJob.JobID);
            }

            lock (this.m_oLockCollection)
            {
                this.m_collection.Clear();
            }

            this.FireListChanged(ChangeType.Reset, null);
        }

        private void ClearSummaries()
        {
            this.m_iCompletions = 0;
            this.m_iOtherCompletions = 0;
            this.m_dictCompletionDetails.Clear();
            this.m_iFailures = null;
            this.m_iWarnings = null;
            this.m_iSkipped = null;
            this.m_iDifferent = null;
            this.m_iMissingOnSource = null;
            this.m_iMissingOnTarget = null;
            this.m_lLicenseDataUsed = null;
            this.PopulateCompletionDetailsWithLock();
        }

        public void FetchData()
        {
            lock (this.m_oLockCollection)
            {
                this.m_collection.Clear();
                if (this.JobHistoryDb != null)
                {
                    this.m_collection = this.JobHistoryDb.GetLogItems(this.ParentJob);
                }

                this.PopulateCompletionDetailsNoLock();
            }
        }

        internal void Finish(LogItem item)
        {
            this.FireListChanged(ChangeType.ItemFinished, item);
            if (this.JobHistoryDb != null)
            {
                item.UpdateAdapter(this.ParentJob);
            }
        }

        internal void FireListChanged(ChangeType changeType, LogItem item)
        {
            if (changeType == ChangeType.Reset)
            {
                this.ClearSummaries();
            }
            else if (changeType == ChangeType.ItemFinished)
            {
                this.IncrementSummaries(item);
            }

            if (this.ListChanged != null)
            {
                this.ListChanged(changeType, item);
            }
        }

        public override void FromXML(XmlNode xmlNode)
        {
            this.m_collection.Clear();
            foreach (XmlNode xmlNodes in xmlNode.SelectNodes("//LogItem"))
            {
                LogItem logItem = new LogItem(xmlNodes, this.JobHistoryDb);
                this.m_collection.Add(logItem);
            }
        }

        public void FromXML(XmlReader reader)
        {
            this.m_collection.Clear();
            while (reader.ReadToFollowing("LogItem"))
            {
                LogItem logItem = new LogItem(reader);
                this.m_collection.Add(logItem);
            }
        }

        public LogItem[] GetCurrentLogItems()
        {
            LogItem[] array = null;
            lock (this.m_oLockCollection)
            {
                array = this.m_collection.ToArray();
            }

            return array;
        }

        private void IncrementSummaries(LogItem item)
        {
            long? nullable;
            int? nullable1;
            int? nullable2;
            int? nullable3;
            int? nullable4;
            int? nullable5;
            int? nullable6;
            try
            {
                if (item.Status != ActionOperationStatus.Running && item.Status != ActionOperationStatus.Idle)
                {
                    switch (item.Status)
                    {
                        case ActionOperationStatus.Completed:
                        {
                            this.m_iCompletions++;
                            this.UpdateCompletionDetails(item);
                            goto case ActionOperationStatus.Same;
                        }
                        case ActionOperationStatus.Warning:
                        {
                            LogItemCollection logItemCollection = this;
                            int? mIWarnings = logItemCollection.m_iWarnings;
                            if (mIWarnings.HasValue)
                            {
                                nullable1 = new int?(mIWarnings.GetValueOrDefault() + 1);
                            }
                            else
                            {
                                nullable1 = null;
                            }

                            logItemCollection.m_iWarnings = nullable1;
                            goto case ActionOperationStatus.Same;
                        }
                        case ActionOperationStatus.Failed:
                        {
                            LogItemCollection logItemCollection1 = this;
                            int? mIFailures = logItemCollection1.m_iFailures;
                            if (mIFailures.HasValue)
                            {
                                nullable2 = new int?(mIFailures.GetValueOrDefault() + 1);
                            }
                            else
                            {
                                nullable2 = null;
                            }

                            logItemCollection1.m_iFailures = nullable2;
                            goto case ActionOperationStatus.Same;
                        }
                        case ActionOperationStatus.Same:
                        {
                            LogItemCollection logItemCollection2 = this;
                            long? mLLicenseDataUsed = logItemCollection2.m_lLicenseDataUsed;
                            long licenseDataUsed = item.LicenseDataUsed;
                            if (mLLicenseDataUsed.HasValue)
                            {
                                nullable = new long?(mLLicenseDataUsed.GetValueOrDefault() + licenseDataUsed);
                            }
                            else
                            {
                                nullable = null;
                            }

                            logItemCollection2.m_lLicenseDataUsed = nullable;
                            break;
                        }
                        case ActionOperationStatus.Different:
                        {
                            LogItemCollection logItemCollection3 = this;
                            int? mIDifferent = logItemCollection3.m_iDifferent;
                            if (mIDifferent.HasValue)
                            {
                                nullable3 = new int?(mIDifferent.GetValueOrDefault() + 1);
                            }
                            else
                            {
                                nullable3 = null;
                            }

                            logItemCollection3.m_iDifferent = nullable3;
                            goto case ActionOperationStatus.Same;
                        }
                        case ActionOperationStatus.MissingOnSource:
                        {
                            LogItemCollection logItemCollection4 = this;
                            int? mIMissingOnSource = logItemCollection4.m_iMissingOnSource;
                            if (mIMissingOnSource.HasValue)
                            {
                                nullable4 = new int?(mIMissingOnSource.GetValueOrDefault() + 1);
                            }
                            else
                            {
                                nullable4 = null;
                            }

                            logItemCollection4.m_iMissingOnSource = nullable4;
                            goto case ActionOperationStatus.Same;
                        }
                        case ActionOperationStatus.MissingOnTarget:
                        {
                            LogItemCollection logItemCollection5 = this;
                            int? mIMissingOnTarget = logItemCollection5.m_iMissingOnTarget;
                            if (mIMissingOnTarget.HasValue)
                            {
                                nullable5 = new int?(mIMissingOnTarget.GetValueOrDefault() + 1);
                            }
                            else
                            {
                                nullable5 = null;
                            }

                            logItemCollection5.m_iMissingOnTarget = nullable5;
                            goto case ActionOperationStatus.Same;
                        }
                        case ActionOperationStatus.Skipped:
                        {
                            LogItemCollection logItemCollection6 = this;
                            int? mISkipped = logItemCollection6.m_iSkipped;
                            if (mISkipped.HasValue)
                            {
                                nullable6 = new int?(mISkipped.GetValueOrDefault() + 1);
                            }
                            else
                            {
                                nullable6 = null;
                            }

                            logItemCollection6.m_iSkipped = nullable6;
                            goto case ActionOperationStatus.Same;
                        }
                        default:
                        {
                            goto case ActionOperationStatus.Same;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Logging.LogExceptionToTextFileWithEventLogBackup(exception, "IncrementSummaries", true);
            }
        }

        private void InitialiseCommonVariables()
        {
            this.m_iCompletions = 0;
            this.m_iOtherCompletions = 0;
            this.m_dictCompletionDetails = new Dictionary<string, long>();
        }

        private void PopulateCompletionDetailsNoLock()
        {
            foreach (LogItem mCollection in this.m_collection)
            {
                this.m_iCompletions++;
                if (mCollection.Status != ActionOperationStatus.Completed)
                {
                    continue;
                }

                this.UpdateCompletionDetails(mCollection);
            }
        }

        private void PopulateCompletionDetailsWithLock()
        {
            lock (this.m_oLockCollection)
            {
                this.PopulateCompletionDetailsNoLock();
            }
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            int warnings = this.Warnings;
            int failures = this.Failures;
            int differences = this.Differences;
            int missingOnTarget = this.MissingOnTarget;
            int missingOnSource = this.MissingOnSource;
            int skipped = this.Skipped;
            if (this.Completions > 0)
            {
                if (this.CompletionDetails.Count <= 0)
                {
                    stringBuilder.Append((stringBuilder.Length > 0 ? ", " : ""));
                    stringBuilder.Append(string.Concat("Completions: ", this.Completions));
                }
                else
                {
                    foreach (KeyValuePair<string, long> completionDetail in this.CompletionDetails)
                    {
                        stringBuilder.Append((stringBuilder.Length > 0 ? ", " : ""));
                        stringBuilder.Append(string.Format("{0}: {1}", completionDetail.Key, completionDetail.Value));
                    }

                    if (this.OtherCompletions > 0)
                    {
                        stringBuilder.Append((stringBuilder.Length > 0 ? ", " : ""));
                        stringBuilder.Append(string.Concat("Other Completions: ", this.OtherCompletions));
                    }
                }
            }

            if (warnings > 0)
            {
                stringBuilder.Append((stringBuilder.Length > 0 ? ", " : ""));
                stringBuilder.Append(string.Concat("Warnings: ", warnings));
            }

            if (failures > 0)
            {
                stringBuilder.Append((stringBuilder.Length > 0 ? ", " : ""));
                stringBuilder.Append(string.Concat("Failures: ", failures));
            }

            if (differences > 0)
            {
                stringBuilder.Append((stringBuilder.Length > 0 ? ", " : ""));
                stringBuilder.Append(string.Concat("Differences: ", differences));
            }

            if (missingOnSource > 0)
            {
                stringBuilder.Append((stringBuilder.Length > 0 ? ", " : ""));
                stringBuilder.Append(string.Concat("Missing On Source: ", missingOnSource));
            }

            if (missingOnTarget > 0)
            {
                stringBuilder.Append((stringBuilder.Length > 0 ? ", " : ""));
                stringBuilder.Append(string.Concat("Missing On Target: ", missingOnTarget));
            }

            if (skipped > 0)
            {
                stringBuilder.Append((stringBuilder.Length > 0 ? ", " : ""));
                stringBuilder.Append(string.Concat("Skipped: ", skipped));
            }

            return stringBuilder.ToString();
        }

        public override void ToXML(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("LogItemCollection");
            foreach (LogItem logItem in this)
            {
                logItem.ToXML(xmlWriter);
            }

            xmlWriter.WriteEndElement();
        }

        internal void Update(LogItem item)
        {
            if (item.IsDirty)
            {
                this.FireListChanged(ChangeType.ItemUpdated, item);
                if (this.JobHistoryDb != null)
                {
                    item.UpdateAdapter(this.ParentJob);
                }
            }
        }

        private void UpdateCompletionDetails(LogItem logItem)
        {
            if (logItem.CompletionDetails.Count <= 0)
            {
                this.m_iOtherCompletions++;
            }
            else
            {
                foreach (KeyValuePair<string, long> completionDetail in logItem.CompletionDetails)
                {
                    if (!this.CompletionDetails.ContainsKey(completionDetail.Key))
                    {
                        this.CompletionDetails.Add(completionDetail.Key, completionDetail.Value);
                    }
                    else
                    {
                        Dictionary<string, long> completionDetails = this.CompletionDetails;
                        Dictionary<string, long> strs = completionDetails;
                        string key = completionDetail.Key;
                        string str = key;
                        completionDetails[key] = strs[str] + completionDetail.Value;
                    }
                }
            }
        }

        public event LogItemCollection.ListChangedHandler ListChanged;

        public delegate void ListChangedHandler(ChangeType changeType, LogItem itemChanged);
    }
}