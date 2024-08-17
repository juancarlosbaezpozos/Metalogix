using Metalogix.Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;

namespace Metalogix.Jobs
{
    public class JobSummary : ICustomTypeDescriptor
    {
        private string m_sJobFile;

        private string m_sJobID;

        private string m_sResultsSummary;

        private string m_statusMessage;

        private string m_sTitle;

        private string m_sSource;

        private string m_sTarget;

        private DateTime m_created = DateTime.Now;

        private DateTime? m_started = null;

        private DateTime? m_finished = null;

        private Dictionary<string, long> m_dictCompletionDetails = new Dictionary<string, long>();

        private int m_iCompletions;

        private int m_iOtherCompletions;

        private int m_iFailures;

        private int m_iWarnings;

        private int m_iSkipped;

        private int m_iDifferent;

        private int m_iMissingOnSource;

        private int m_iMissingOnTarget;

        private string m_sLicenseDataUsed;

        public Dictionary<string, long> CompletionDetails
        {
            get { return this.m_dictCompletionDetails; }
        }

        public DateTime Created
        {
            get { return this.m_created; }
        }

        public int Different
        {
            get { return this.m_iDifferent; }
        }

        public int Failures
        {
            get { return this.m_iFailures; }
        }

        public DateTime? Finished
        {
            get { return this.m_finished; }
        }

        public string JobFileName
        {
            get { return this.m_sJobFile; }
        }

        public string JobID
        {
            get { return this.m_sJobID; }
        }

        public string LicenseDataUsed
        {
            get { return this.m_sLicenseDataUsed; }
        }

        public int MissingOnSource
        {
            get { return this.m_iMissingOnSource; }
        }

        public int MissingOnTarget
        {
            get { return this.m_iMissingOnTarget; }
        }

        public int OtherCompletions
        {
            get { return this.m_iOtherCompletions; }
        }

        public string ResultsSummary
        {
            get { return this.m_sResultsSummary; }
        }

        public int Skipped
        {
            get { return this.m_iSkipped; }
        }

        public string Source
        {
            get { return this.m_sSource; }
        }

        public DateTime? Started
        {
            get { return this.m_started; }
        }

        public string StatusMessage
        {
            get { return this.m_statusMessage; }
        }

        public string Target
        {
            get { return this.m_sTarget; }
        }

        public string Title
        {
            get { return this.m_sTitle; }
        }

        public int TotalCompletions
        {
            get { return this.m_iCompletions; }
        }

        public int Warnings
        {
            get { return this.m_iWarnings; }
        }

        public JobSummary(Job jobToSummarize)
        {
            if (jobToSummarize.Parent != null)
            {
                string adapterContext = jobToSummarize.Parent.JobHistoryDb.AdapterContext;
                if (!jobToSummarize.Parent.JobHistoryDb.AdapterType.Equals(JobHistoryAdapterType.SqlCe.ToString(),
                        StringComparison.InvariantCultureIgnoreCase))
                {
                    SqlConnectionStringBuilder sqlConnectionStringBuilder =
                        new SqlConnectionStringBuilder(adapterContext);
                    if (!sqlConnectionStringBuilder.IntegratedSecurity)
                    {
                        adapterContext = adapterContext.Replace(sqlConnectionStringBuilder.Password, "*****");
                    }
                }

                this.m_sJobFile = adapterContext;
            }

            this.m_sJobID = jobToSummarize.JobID;
            this.m_sTitle = jobToSummarize.Title;
            this.m_sSource = jobToSummarize.Source;
            this.m_sTarget = jobToSummarize.Target;
            this.m_created = jobToSummarize.Created;
            this.m_finished = jobToSummarize.Finished;
            this.m_started = jobToSummarize.Started;
            this.m_statusMessage = jobToSummarize.StatusMessage;
            this.m_sResultsSummary = jobToSummarize.ResultsSummary;
            this.m_iCompletions = jobToSummarize.Log.Completions;
            this.m_iDifferent = jobToSummarize.Log.Differences;
            this.m_iFailures = jobToSummarize.Log.Failures;
            this.m_iMissingOnSource = jobToSummarize.Log.MissingOnSource;
            this.m_iMissingOnTarget = jobToSummarize.Log.MissingOnTarget;
            this.m_iOtherCompletions = jobToSummarize.Log.OtherCompletions;
            this.m_iSkipped = jobToSummarize.Log.Skipped;
            this.m_iWarnings = jobToSummarize.Log.Warnings;
            this.m_dictCompletionDetails = jobToSummarize.Log.CompletionDetails;
            this.m_sLicenseDataUsed = jobToSummarize.GetFormattedLicensedData(true);
        }

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this.GetType());
        }

        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this.GetType());
        }

        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this.GetType());
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this.GetType());
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this.GetType());
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this.GetType());
        }

        public object GetEditor(Type editorBaseType)
        {
            return null;
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this.GetType(), attributes);
        }

        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this.GetType());
        }

        public Job GetFullJob()
        {
            Job job;
            if (this.JobFileName == null)
            {
                throw new Exception("The full job and log are not available, as no job history file was specified.");
            }

            using (JobHistoryDb jobHistoryDb =
                   JobFactory.CreateJobHistoryDb(JobHistoryAdapterType.SqlCe, this.JobFileName))
            {
                job = jobHistoryDb.GetJob(this.JobID);
            }

            return job;
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(this.GetType(), attributes);
            PropertyDescriptor[] completionDetailPropertyDescriptor =
                new PropertyDescriptor[properties.Count + this.m_dictCompletionDetails.Count];
            int num = 0;
            foreach (PropertyDescriptor property in properties)
            {
                completionDetailPropertyDescriptor[num] = property;
                num++;
            }

            foreach (string key in this.CompletionDetails.Keys)
            {
                completionDetailPropertyDescriptor[num] =
                    new CompletionDetailPropertyDescriptor(key.Replace(" ", ""), key);
                num++;
            }

            return new PropertyDescriptorCollection(completionDetailPropertyDescriptor);
        }

        public PropertyDescriptorCollection GetProperties()
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(this.GetType());
            PropertyDescriptor[] completionDetailPropertyDescriptor =
                new PropertyDescriptor[properties.Count + this.m_dictCompletionDetails.Count];
            int num = 0;
            foreach (PropertyDescriptor property in properties)
            {
                completionDetailPropertyDescriptor[num] = property;
                num++;
            }

            foreach (string key in this.CompletionDetails.Keys)
            {
                completionDetailPropertyDescriptor[num] =
                    new CompletionDetailPropertyDescriptor(key.Replace(" ", ""), key);
                num++;
            }

            return new PropertyDescriptorCollection(completionDetailPropertyDescriptor);
        }

        public object GetPropertyOwner(PropertyDescriptor propertyDescriptor_0)
        {
            if (this.GetProperties().Contains(propertyDescriptor_0))
            {
                return this;
            }

            return null;
        }
    }
}