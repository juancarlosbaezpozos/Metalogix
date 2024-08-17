using Metalogix;
using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.Interfaces;
using Metalogix.Jobs;
using Metalogix.Jobs.Options;
using Metalogix.Licensing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml;

namespace Metalogix.Jobs.Actions
{
    [LicensedProducts(ProductFlags.UnifiedContentMatrixKey)]
    [Name("Run multiple actions")]
    [SourceCardinality(Cardinality.ZeroOrMore)]
    [TargetCardinality(Cardinality.ZeroOrMore)]
    public class JobRunner : Metalogix.Actions.Action<JobRunnerOptions>
    {
        private readonly Job[] m_jobs;

        private ActionStatus _jobsRunState;

        private string m_sLicensingDescriptor;

        private string m_sLicensingUnit;

        protected Job CurrentJob { get; private set; }

        public Job[] Jobs
        {
            get { return this.m_jobs; }
        }

        public override string LicensingDescriptor
        {
            get
            {
                if (base.SubActions.First == null)
                {
                    if (this.m_sLicensingDescriptor != null)
                    {
                        return this.m_sLicensingDescriptor;
                    }

                    return base.LicensingDescriptor;
                }

                this.m_sLicensingDescriptor = base.SubActions.First.LicensingDescriptor;
                return this.m_sLicensingDescriptor;
            }
        }

        public override string LicensingUnit
        {
            get
            {
                if (base.SubActions.First == null)
                {
                    if (this.m_sLicensingUnit != null)
                    {
                        return this.m_sLicensingUnit;
                    }

                    return base.LicensingUnit;
                }

                this.m_sLicensingUnit = base.SubActions.First.LicensingUnit;
                return this.m_sLicensingUnit;
            }
        }

        public override string Name
        {
            get
            {
                if (this.Jobs == null || (int)this.Jobs.Length != 1 || this.Jobs[0].Action == null)
                {
                    return base.Name;
                }

                return this.Jobs[0].Action.Name;
            }
        }

        public override bool ThreadingEnabled
        {
            get
            {
                if (base.SubActions.First == null)
                {
                    return false;
                }

                return base.SubActions.First.ThreadingEnabled;
            }
        }

        public override bool UpdateLicensing
        {
            get { return false; }
        }

        public JobRunner(Job[] jobs)
        {
            this.m_jobs = jobs;
        }

        public override CompletionDetailsOrderProvider GetCompletionDetailsOrderProvider()
        {
            Job jobs;
            Job currentJob = this.CurrentJob;
            if (currentJob != null || (int)this.Jobs.Length <= 0)
            {
                jobs = null;
            }
            else
            {
                jobs = this.Jobs[0];
            }

            currentJob = jobs;
            if (currentJob == null)
            {
                return CompletionDetailsOrderProvider.GetOrderProvider(this);
            }

            return currentJob.Action.GetCompletionDetailsOrderProvider();
        }

        public override ActionStatusLabelProvider GetStatusLabelProvider()
        {
            Job jobs;
            Job currentJob = this.CurrentJob;
            if (currentJob != null || (int)this.Jobs.Length <= 0)
            {
                jobs = null;
            }
            else
            {
                jobs = this.Jobs[0];
            }

            currentJob = jobs;
            if (currentJob == null)
            {
                return ActionStatusLabelProvider.GetStatusLabelProvider(this);
            }

            return currentJob.Action.GetStatusLabelProvider();
        }

        public override ActionStatusSummaryProvider GetStatusSummaryProvider()
        {
            Job jobs;
            Job currentJob = this.CurrentJob;
            if (currentJob != null || (int)this.Jobs.Length <= 0)
            {
                jobs = null;
            }
            else
            {
                jobs = this.Jobs[0];
            }

            currentJob = jobs;
            if (currentJob == null)
            {
                return ActionStatusSummaryProvider.GetSummaryProvider(this);
            }

            return currentJob.Action.GetStatusSummaryProvider();
        }

        public override void Run(IXMLAbleList source, IXMLAbleList target)
        {
            string str;
            string str1;
            base.SetStatus(ActionStatus.Running);
            if (source != null)
            {
                str = source.ToString();
            }
            else
            {
                str = null;
            }

            if (target != null)
            {
                str1 = target.ToString();
            }
            else
            {
                str1 = null;
            }

            base.FireActionStarted(str, str1);
            try
            {
                this.RunAction(source, target);
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                if (Assembly.GetEntryAssembly() == null)
                {
                    throw exception;
                }

                string message = exception.Message;
                if (!(exception is ObjectDisposedException))
                {
                    message = string.Concat("Error running action: ", message);
                    GlobalServices.ErrorHandler.HandleException(message, exception);
                    LogItem logItem = new LogItem("Error", null, null, null, ActionOperationStatus.Failed)
                    {
                        Exception = exception,
                        Information = exception.Message,
                        Details = exception.StackTrace
                    };
                    base.FireOperationStarted(logItem);
                    base.FireOperationFinished(logItem);
                    base.SetStatus(ActionStatus.Failed);
                }
            }

            if (base.Status == ActionStatus.Aborting)
            {
                base.SetStatus(ActionStatus.Aborted);
            }
            else if (base.Status != ActionStatus.Aborted && base.Status != ActionStatus.Failed)
            {
                base.SetStatus(this._jobsRunState);
            }

            base.FireActionFinished();
        }

        protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
        {
            IXMLAbleList empty;
            if (this.ActionOptions.RunAsUserName)
            {
                for (int i = 0; i < (int)this.Jobs.Length; i++)
                {
                    this.Jobs[i] = this.TransformJobToRunAsUserName(this.Jobs[i], this.ActionOptions.UserName);
                }
            }

            bool flag = false;
            bool flag1 = false;
            Job[] jobs = this.Jobs;
            int num = 0;
            while (true)
            {
                if (num >= (int)jobs.Length)
                {
                    this.CurrentJob = null;
                    if (flag1)
                    {
                        this._jobsRunState = (flag ? ActionStatus.Warning : ActionStatus.Failed);
                        return;
                    }

                    this._jobsRunState = ActionStatus.Done;
                    break;
                }
                else
                {
                    Job job = jobs[num];
                    this.CurrentJob = job;
                    LogItem logItem = null;
                    IXMLAbleList xMLAbleLists = null;
                    IXMLAbleList targetList = null;
                    try
                    {
                        try
                        {
                            if (!base.CheckForAbort())
                            {
                                logItem = new LogItem("Loading source and target of job", job.Title, job.Source,
                                    job.Target, ActionOperationStatus.Running);
                                base.SubActions.Add(job.Action);
                                job.Action.ActionStarted += new ActionStartedEventHandler(this.SubAction_ActionStarted);
                                job.Action.FireOperationStarted(logItem);
                                if (job.Action.SourceCardinality == Cardinality.Zero)
                                {
                                    empty = NodeCollection.Empty;
                                }
                                else
                                {
                                    empty = job.SourceList;
                                }

                                xMLAbleLists = empty;
                                targetList = job.TargetList;
                                if (!job.Action.AppliesTo(xMLAbleLists, targetList))
                                {
                                    logItem.Information =
                                        "The action does not apply in the configured context. The source or target of this job may no longer exist";
                                    logItem.SourceContent = job.SourceXml;
                                    logItem.TargetContent = job.TargetXml;
                                    logItem.Status = ActionOperationStatus.Failed;
                                    goto Label0;
                                }
                                else
                                {
                                    logItem.Status = ActionOperationStatus.Completed;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        catch (Exception exception1)
                        {
                            Exception exception = exception1;
                            if (logItem != null)
                            {
                                logItem.Exception = exception;
                                logItem.Information = string.Concat("Could not load data: ", exception.Message);
                                logItem.Details = exception.StackTrace;
                                logItem.Status = ActionOperationStatus.Failed;
                            }

                            goto Label0;
                        }
                    }
                    finally
                    {
                        if (logItem != null)
                        {
                            job.Action.FireOperationFinished(logItem);
                            if (logItem.Status == ActionOperationStatus.Failed)
                            {
                                job.Action.SetStatus(ActionStatus.Failed);
                                flag1 = true;
                                job.Action.ActionStarted -= new ActionStartedEventHandler(this.SubAction_ActionStarted);
                                base.SubActions.Remove(job.Action);
                            }
                        }
                    }

                    flag = true;
                    job.Action.Run(xMLAbleLists, targetList);
                    job.Action.ActionStarted -= new ActionStartedEventHandler(this.SubAction_ActionStarted);
                    base.SubActions.Remove(job.Action);
                    Label0:
                    num++;
                }
            }
        }

        private Job TransformJobToRunAsUserName(Job job_0, string userName)
        {
            IList<Job> jobs;
            string xML = job_0.XML;
            if (string.IsNullOrEmpty(xML))
            {
                throw new Exception("ApplyRunAsUserNameToJob failed - job.XML was not available.");
            }

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xML);
            string sourceXml = job_0.SourceXml;
            string targetXml = job_0.TargetXml;
            if (!string.IsNullOrEmpty(sourceXml))
            {
                XmlDocument xmlDocument1 = new XmlDocument();
                xmlDocument1.LoadXml(sourceXml);
                foreach (XmlNode xmlNodes in xmlDocument1.SelectNodes("./Connection"))
                {
                    xmlNodes.Attributes["UserName"].Value = userName;
                }

                xmlDocument.DocumentElement.Attributes["SourceXml"].Value = xmlDocument1.OuterXml;
            }

            if (!string.IsNullOrEmpty(targetXml))
            {
                XmlDocument xmlDocument2 = new XmlDocument();
                xmlDocument2.LoadXml(targetXml);
                foreach (XmlNode xmlNodes1 in xmlDocument2.SelectNodes("./Connection"))
                {
                    xmlNodes1.Attributes["UserName"].Value = userName;
                }

                xmlDocument.DocumentElement.Attributes["TargetXml"].Value = xmlDocument2.OuterXml;
            }

            if (!Job.TryCreateJobs(xmlDocument.DocumentElement, out jobs))
            {
                throw new Exception("Failed to create job from xml.");
            }

            Job job = jobs.First<Job>();
            job.SetParent(job_0.Parent);
            return job;
        }
    }
}