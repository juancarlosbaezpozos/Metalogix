using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Metalogix.SharePoint.Adapters.NWS.SocialData
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Xml", "4.0.30319.17929")]
    [Serializable]
    [XmlType(Namespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService")]
    public class FeedbackData
    {
        private string browserSessionIdField;

        private object customDataField;

        private Guid feedbackAnswerIdField;

        private int feedbackAnswerNumericEquivalentField;

        private string feedbackAnswerTextField;

        private string feedbackControlCultureField;

        private Guid feedbackIdentifierField;

        private Guid feedbackQuestionIdField;

        private string feedbackQuestionTextField;

        private bool isUserSatisfactionAnswerField;

        private Guid queryIdField;

        private string ratedAssetIdField;

        private string ratedAssetTitleField;

        private byte sampleRateField;

        private Guid ratedAssetWebIdField;

        private Guid siteIdField;

        private string userIdField;

        private string userDepartmentField;

        private string userTitleField;

        private string userVerbatimField;

        public string BrowserSessionId
        {
            get { return this.browserSessionIdField; }
            set { this.browserSessionIdField = value; }
        }

        public object CustomData
        {
            get { return this.customDataField; }
            set { this.customDataField = value; }
        }

        public Guid FeedbackAnswerId
        {
            get { return this.feedbackAnswerIdField; }
            set { this.feedbackAnswerIdField = value; }
        }

        public int FeedbackAnswerNumericEquivalent
        {
            get { return this.feedbackAnswerNumericEquivalentField; }
            set { this.feedbackAnswerNumericEquivalentField = value; }
        }

        public string FeedbackAnswerText
        {
            get { return this.feedbackAnswerTextField; }
            set { this.feedbackAnswerTextField = value; }
        }

        public string FeedbackControlCulture
        {
            get { return this.feedbackControlCultureField; }
            set { this.feedbackControlCultureField = value; }
        }

        public Guid FeedbackIdentifier
        {
            get { return this.feedbackIdentifierField; }
            set { this.feedbackIdentifierField = value; }
        }

        public Guid FeedbackQuestionId
        {
            get { return this.feedbackQuestionIdField; }
            set { this.feedbackQuestionIdField = value; }
        }

        public string FeedbackQuestionText
        {
            get { return this.feedbackQuestionTextField; }
            set { this.feedbackQuestionTextField = value; }
        }

        public bool IsUserSatisfactionAnswer
        {
            get { return this.isUserSatisfactionAnswerField; }
            set { this.isUserSatisfactionAnswerField = value; }
        }

        public Guid QueryId
        {
            get { return this.queryIdField; }
            set { this.queryIdField = value; }
        }

        public string RatedAssetId
        {
            get { return this.ratedAssetIdField; }
            set { this.ratedAssetIdField = value; }
        }

        public string RatedAssetTitle
        {
            get { return this.ratedAssetTitleField; }
            set { this.ratedAssetTitleField = value; }
        }

        public Guid RatedAssetWebId
        {
            get { return this.ratedAssetWebIdField; }
            set { this.ratedAssetWebIdField = value; }
        }

        public byte SampleRate
        {
            get { return this.sampleRateField; }
            set { this.sampleRateField = value; }
        }

        public Guid SiteId
        {
            get { return this.siteIdField; }
            set { this.siteIdField = value; }
        }

        public string UserDepartment
        {
            get { return this.userDepartmentField; }
            set { this.userDepartmentField = value; }
        }

        public string UserId
        {
            get { return this.userIdField; }
            set { this.userIdField = value; }
        }

        public string UserTitle
        {
            get { return this.userTitleField; }
            set { this.userTitleField = value; }
        }

        public string UserVerbatim
        {
            get { return this.userVerbatimField; }
            set { this.userVerbatimField = value; }
        }

        public FeedbackData()
        {
        }
    }
}