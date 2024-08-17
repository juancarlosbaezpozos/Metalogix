using Metalogix;
using System;

namespace Metalogix.Actions
{
    public class SendEmailOptions : OptionsBase
    {
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

        public string BCCEmailAddress
        {
            get { return this.m_sBCCEmailAddress; }
            set { this.m_sBCCEmailAddress = value; }
        }

        public string CCEmailAddress
        {
            get { return this.m_sCCEmailAddress; }
            set { this.m_sCCEmailAddress = value; }
        }

        public string EmailFailureTemplateFilePath
        {
            get { return this.m_sEmailFailureTemplateFilePath; }
            set { this.m_sEmailFailureTemplateFilePath = value; }
        }

        public string EmailPassword
        {
            get { return this.m_sEmailPassword; }
            set { this.m_sEmailPassword = value; }
        }

        public string EmailServer
        {
            get { return this.m_sEmailServer; }
            set { this.m_sEmailServer = value; }
        }

        public string EmailSubject
        {
            get { return this.m_sSubject; }
            set { this.m_sSubject = value; }
        }

        public string EmailSuccessTemplateFilePath
        {
            get { return this.m_sEmailSuccessTemplateFilePath; }
            set { this.m_sEmailSuccessTemplateFilePath = value; }
        }

        public string EmailUserName
        {
            get { return this.m_sEmailUserName; }
            set { this.m_sEmailUserName = value; }
        }

        public string FromEmailAddress
        {
            get { return this.m_sFromEmailAddress; }
            set { this.m_sFromEmailAddress = value; }
        }

        public string ToEmailAddress
        {
            get { return this.m_sToEmailAddress; }
            set { this.m_sToEmailAddress = value; }
        }

        public SendEmailOptions()
        {
        }
    }
}