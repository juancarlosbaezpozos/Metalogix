using Metalogix;
using Metalogix.Transformers;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Metalogix.Actions
{
    public class ActionOptions : ActionOptionsBase
    {
        private Dictionary<string, string> _telemetryLogs = new Dictionary<string, string>();

        private string m_sEmailServer;

        private string m_sEmailUserName;

        private string m_sEmailPassword;

        private string m_sToEmailAddress;

        private string m_sFromEmailAddress;

        private string m_sCCEmailAddress;

        private string m_sBCCEmailAddress;

        private string m_sEmailSuccessTemplateFilePath;

        private string m_sEmailFailureTemplateFilePath;

        private string m_sSubject;

        protected bool m_bSendEmail;

        private TransformerCollection m_transformers;

        [CmdletEnabledParameter("SendEmail", true)]
        public string BCCEmailAddress
        {
            get { return this.m_sBCCEmailAddress; }
            set { this.m_sBCCEmailAddress = value; }
        }

        [CmdletEnabledParameter("SendEmail", true)]
        public string CCEmailAddress
        {
            get { return this.m_sCCEmailAddress; }
            set { this.m_sCCEmailAddress = value; }
        }

        [CmdletEnabledParameter("SendEmail", true)]
        public string EmailFailureTemplateFilePath
        {
            get { return this.m_sEmailFailureTemplateFilePath; }
            set { this.m_sEmailFailureTemplateFilePath = value; }
        }

        [CmdletEnabledParameter("SendEmail", true)]
        [EncryptedValueParameter(true)]
        public string EmailPassword
        {
            get { return this.m_sEmailPassword; }
            set { this.m_sEmailPassword = value; }
        }

        [CmdletEnabledParameter("SendEmail", true)]
        public string EmailServer
        {
            get { return this.m_sEmailServer; }
            set { this.m_sEmailServer = value; }
        }

        [CmdletEnabledParameter("SendEmail", true)]
        public string EmailSubject
        {
            get { return this.m_sSubject; }
            set { this.m_sSubject = value; }
        }

        [CmdletEnabledParameter("SendEmail", true)]
        public string EmailSuccessTemplateFilePath
        {
            get { return this.m_sEmailSuccessTemplateFilePath; }
            set { this.m_sEmailSuccessTemplateFilePath = value; }
        }

        [CmdletEnabledParameter("SendEmail", true)]
        public string EmailUserName
        {
            get { return this.m_sEmailUserName; }
            set { this.m_sEmailUserName = value; }
        }

        [CmdletEnabledParameter("SendEmail", true)]
        public bool EnableSslForEmail { get; set; }

        [CmdletEnabledParameter("SendEmail", true)]
        public string FromEmailAddress
        {
            get { return this.m_sFromEmailAddress; }
            set { this.m_sFromEmailAddress = value; }
        }

        [CmdletEnabledParameter(false)]
        public virtual bool SendEmail
        {
            get { return this.m_bSendEmail; }
            set { this.m_bSendEmail = value; }
        }

        [CmdletEnabledParameter("TelemetryLogs", true)]
        public Dictionary<string, string> TelemetryLogs
        {
            get { return this._telemetryLogs; }
            set { this._telemetryLogs = value; }
        }

        [CmdletEnabledParameter("SendEmail", true)]
        public string ToEmailAddress
        {
            get { return this.m_sToEmailAddress; }
            set { this.m_sToEmailAddress = value; }
        }

        [CmdletParameterEnumerate(true)]
        [UsesStickySettings(false)]
        public virtual TransformerCollection Transformers
        {
            get
            {
                if (this.m_transformers == null)
                {
                    this.m_transformers = new TransformerCollection();
                }

                return this.m_transformers;
            }
            set { this.m_transformers = (value != null ? value.Clone() : new TransformerCollection()); }
        }

        public ActionOptions()
        {
        }

        public new ActionOptions Clone()
        {
            ActionOptions actionOption = (ActionOptions)Activator.CreateInstance(base.GetType());
            actionOption.FromXML(base.ToXML());
            return actionOption;
        }
    }
}