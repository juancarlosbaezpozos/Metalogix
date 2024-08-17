using Metalogix.SharePoint.Adapters.NWS.Properties;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.Services.Protocols;
using System.Xml.Serialization;

namespace Metalogix.SharePoint.Adapters.NWS.UserProfile
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.0.30319.17929")]
    [WebServiceBinding(Name = "UserProfileServiceSoap",
        Namespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService")]
    [XmlInclude(typeof(PropertyData[]))]
    [XmlInclude(typeof(SPTimeZone))]
    [XmlInclude(typeof(ValueData[]))]
    public class UserProfileService : SoapHttpClientProtocol
    {
        private SendOrPostCallback GetUserProfileByIndexOperationCompleted;

        private SendOrPostCallback CreateUserProfileByAccountNameOperationCompleted;

        private SendOrPostCallback GetUserProfileByNameOperationCompleted;

        private SendOrPostCallback GetUserProfileByGuidOperationCompleted;

        private SendOrPostCallback GetUserProfileSchemaOperationCompleted;

        private SendOrPostCallback GetProfileSchemaNameByAccountNameOperationCompleted;

        private SendOrPostCallback GetPropertyChoiceListOperationCompleted;

        private SendOrPostCallback ModifyUserPropertyByAccountNameOperationCompleted;

        private SendOrPostCallback GetUserPropertyByAccountNameOperationCompleted;

        private SendOrPostCallback CreateMemberGroupOperationCompleted;

        private SendOrPostCallback AddMembershipOperationCompleted;

        private SendOrPostCallback RemoveMembershipOperationCompleted;

        private SendOrPostCallback UpdateMembershipPrivacyOperationCompleted;

        private SendOrPostCallback GetUserMembershipsOperationCompleted;

        private SendOrPostCallback GetUserOrganizationsOperationCompleted;

        private SendOrPostCallback GetUserColleaguesOperationCompleted;

        private SendOrPostCallback GetUserLinksOperationCompleted;

        private SendOrPostCallback GetUserPinnedLinksOperationCompleted;

        private SendOrPostCallback GetInCommonOperationCompleted;

        private SendOrPostCallback GetCommonManagerOperationCompleted;

        private SendOrPostCallback GetCommonColleaguesOperationCompleted;

        private SendOrPostCallback GetCommonMembershipsOperationCompleted;

        private SendOrPostCallback AddColleagueOperationCompleted;

        private SendOrPostCallback AddColleagueWithoutEmailNotificationOperationCompleted;

        private SendOrPostCallback RemoveColleagueOperationCompleted;

        private SendOrPostCallback UpdateColleaguePrivacyOperationCompleted;

        private SendOrPostCallback AddPinnedLinkOperationCompleted;

        private SendOrPostCallback UpdatePinnedLinkOperationCompleted;

        private SendOrPostCallback RemovePinnedLinkOperationCompleted;

        private SendOrPostCallback AddLinkOperationCompleted;

        private SendOrPostCallback UpdateLinkOperationCompleted;

        private SendOrPostCallback RemoveLinkOperationCompleted;

        private SendOrPostCallback RemoveAllLinksOperationCompleted;

        private SendOrPostCallback RemoveAllPinnedLinksOperationCompleted;

        private SendOrPostCallback RemoveAllColleaguesOperationCompleted;

        private SendOrPostCallback RemoveAllMembershipsOperationCompleted;

        private SendOrPostCallback GetUserProfileCountOperationCompleted;

        private SendOrPostCallback AddSuggestionsOperationCompleted;

        private SendOrPostCallback GetProfileSchemaNamesOperationCompleted;

        private SendOrPostCallback GetProfileSchemaOperationCompleted;

        private SendOrPostCallback GetLeadersOperationCompleted;

        private SendOrPostCallback AddLeaderOperationCompleted;

        private SendOrPostCallback RemoveLeaderOperationCompleted;

        private bool useDefaultCredentialsSetExplicitly;

        public new string Url
        {
            get { return base.Url; }
            set
            {
                if (this.IsLocalFileSystemWebService(base.Url) && !this.useDefaultCredentialsSetExplicitly &&
                    !this.IsLocalFileSystemWebService(value))
                {
                    base.UseDefaultCredentials = false;
                }

                base.Url = value;
            }
        }

        public new bool UseDefaultCredentials
        {
            get { return base.UseDefaultCredentials; }
            set
            {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }

        public UserProfileService()
        {
            this.Url = Settings.Default.Metalogix_SharePoint_Adapters_NWS_UserProfileService_UserProfileService;
            if (!this.IsLocalFileSystemWebService(this.Url))
            {
                this.useDefaultCredentialsSetExplicitly = true;
                return;
            }

            this.UseDefaultCredentials = true;
            this.useDefaultCredentialsSetExplicitly = false;
        }

        [SoapDocumentMethod("http://microsoft.com/webservices/SharePointPortalServer/UserProfileService/AddColleague",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public ContactData AddColleague(string accountName, string colleagueAccountName, string group, Privacy privacy,
            bool isInWorkGroup)
        {
            object[] objArray = new object[] { accountName, colleagueAccountName, group, privacy, isInWorkGroup };
            return (ContactData)base.Invoke("AddColleague", objArray)[0];
        }

        public void AddColleagueAsync(string accountName, string colleagueAccountName, string group, Privacy privacy,
            bool isInWorkGroup)
        {
            this.AddColleagueAsync(accountName, colleagueAccountName, group, privacy, isInWorkGroup, null);
        }

        public void AddColleagueAsync(string accountName, string colleagueAccountName, string group, Privacy privacy,
            bool isInWorkGroup, object userState)
        {
            if (this.AddColleagueOperationCompleted == null)
            {
                this.AddColleagueOperationCompleted = new SendOrPostCallback(this.OnAddColleagueOperationCompleted);
            }

            object[] objArray = new object[] { accountName, colleagueAccountName, group, privacy, isInWorkGroup };
            base.InvokeAsync("AddColleague", objArray, this.AddColleagueOperationCompleted, userState);
        }

        [SoapDocumentMethod(
            "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService/AddColleagueWithoutEmailNotification",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public ContactData AddColleagueWithoutEmailNotification(string accountName, string colleagueAccountName,
            string group, Privacy privacy, bool isInWorkGroup)
        {
            object[] objArray = new object[] { accountName, colleagueAccountName, group, privacy, isInWorkGroup };
            return (ContactData)base.Invoke("AddColleagueWithoutEmailNotification", objArray)[0];
        }

        public void AddColleagueWithoutEmailNotificationAsync(string accountName, string colleagueAccountName,
            string group, Privacy privacy, bool isInWorkGroup)
        {
            this.AddColleagueWithoutEmailNotificationAsync(accountName, colleagueAccountName, group, privacy,
                isInWorkGroup, null);
        }

        public void AddColleagueWithoutEmailNotificationAsync(string accountName, string colleagueAccountName,
            string group, Privacy privacy, bool isInWorkGroup, object userState)
        {
            if (this.AddColleagueWithoutEmailNotificationOperationCompleted == null)
            {
                this.AddColleagueWithoutEmailNotificationOperationCompleted =
                    new SendOrPostCallback(this.OnAddColleagueWithoutEmailNotificationOperationCompleted);
            }

            object[] objArray = new object[] { accountName, colleagueAccountName, group, privacy, isInWorkGroup };
            base.InvokeAsync("AddColleagueWithoutEmailNotification", objArray,
                this.AddColleagueWithoutEmailNotificationOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://microsoft.com/webservices/SharePointPortalServer/UserProfileService/AddLeader",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public void AddLeader(string accountName)
        {
            object[] objArray = new object[] { accountName };
            base.Invoke("AddLeader", objArray);
        }

        public void AddLeaderAsync(string accountName)
        {
            this.AddLeaderAsync(accountName, null);
        }

        public void AddLeaderAsync(string accountName, object userState)
        {
            if (this.AddLeaderOperationCompleted == null)
            {
                this.AddLeaderOperationCompleted = new SendOrPostCallback(this.OnAddLeaderOperationCompleted);
            }

            object[] objArray = new object[] { accountName };
            base.InvokeAsync("AddLeader", objArray, this.AddLeaderOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://microsoft.com/webservices/SharePointPortalServer/UserProfileService/AddLink",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public QuickLinkData AddLink(string accountName, string name, string url, string group, Privacy privacy)
        {
            object[] objArray = new object[] { accountName, name, url, group, privacy };
            return (QuickLinkData)base.Invoke("AddLink", objArray)[0];
        }

        public void AddLinkAsync(string accountName, string name, string url, string group, Privacy privacy)
        {
            this.AddLinkAsync(accountName, name, url, group, privacy, null);
        }

        public void AddLinkAsync(string accountName, string name, string url, string group, Privacy privacy,
            object userState)
        {
            if (this.AddLinkOperationCompleted == null)
            {
                this.AddLinkOperationCompleted = new SendOrPostCallback(this.OnAddLinkOperationCompleted);
            }

            object[] objArray = new object[] { accountName, name, url, group, privacy };
            base.InvokeAsync("AddLink", objArray, this.AddLinkOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://microsoft.com/webservices/SharePointPortalServer/UserProfileService/AddMembership",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public MembershipData AddMembership(string accountName, MembershipData membershipInfo, string group,
            Privacy privacy)
        {
            object[] objArray = new object[] { accountName, membershipInfo, group, privacy };
            return (MembershipData)base.Invoke("AddMembership", objArray)[0];
        }

        public void AddMembershipAsync(string accountName, MembershipData membershipInfo, string group, Privacy privacy)
        {
            this.AddMembershipAsync(accountName, membershipInfo, group, privacy, null);
        }

        public void AddMembershipAsync(string accountName, MembershipData membershipInfo, string group, Privacy privacy,
            object userState)
        {
            if (this.AddMembershipOperationCompleted == null)
            {
                this.AddMembershipOperationCompleted = new SendOrPostCallback(this.OnAddMembershipOperationCompleted);
            }

            object[] objArray = new object[] { accountName, membershipInfo, group, privacy };
            base.InvokeAsync("AddMembership", objArray, this.AddMembershipOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://microsoft.com/webservices/SharePointPortalServer/UserProfileService/AddPinnedLink",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public PinnedLinkData AddPinnedLink(string accountName, string name, string url)
        {
            object[] objArray = new object[] { accountName, name, url };
            return (PinnedLinkData)base.Invoke("AddPinnedLink", objArray)[0];
        }

        public void AddPinnedLinkAsync(string accountName, string name, string url)
        {
            this.AddPinnedLinkAsync(accountName, name, url, null);
        }

        public void AddPinnedLinkAsync(string accountName, string name, string url, object userState)
        {
            if (this.AddPinnedLinkOperationCompleted == null)
            {
                this.AddPinnedLinkOperationCompleted = new SendOrPostCallback(this.OnAddPinnedLinkOperationCompleted);
            }

            object[] objArray = new object[] { accountName, name, url };
            base.InvokeAsync("AddPinnedLink", objArray, this.AddPinnedLinkOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://microsoft.com/webservices/SharePointPortalServer/UserProfileService/AddSuggestions",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public void AddSuggestions(SuggestionType type, string[] suggestions, double[] weights)
        {
            object[] objArray = new object[] { type, suggestions, weights };
            base.Invoke("AddSuggestions", objArray);
        }

        public void AddSuggestionsAsync(SuggestionType type, string[] suggestions, double[] weights)
        {
            this.AddSuggestionsAsync(type, suggestions, weights, null);
        }

        public void AddSuggestionsAsync(SuggestionType type, string[] suggestions, double[] weights, object userState)
        {
            if (this.AddSuggestionsOperationCompleted == null)
            {
                this.AddSuggestionsOperationCompleted = new SendOrPostCallback(this.OnAddSuggestionsOperationCompleted);
            }

            object[] objArray = new object[] { type, suggestions, weights };
            base.InvokeAsync("AddSuggestions", objArray, this.AddSuggestionsOperationCompleted, userState);
        }

        public new void CancelAsync(object userState)
        {
            base.CancelAsync(userState);
        }

        [SoapDocumentMethod(
            "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService/CreateMemberGroup",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public void CreateMemberGroup(MembershipData membershipInfo)
        {
            object[] objArray = new object[] { membershipInfo };
            base.Invoke("CreateMemberGroup", objArray);
        }

        public void CreateMemberGroupAsync(MembershipData membershipInfo)
        {
            this.CreateMemberGroupAsync(membershipInfo, null);
        }

        public void CreateMemberGroupAsync(MembershipData membershipInfo, object userState)
        {
            if (this.CreateMemberGroupOperationCompleted == null)
            {
                this.CreateMemberGroupOperationCompleted =
                    new SendOrPostCallback(this.OnCreateMemberGroupOperationCompleted);
            }

            object[] objArray = new object[] { membershipInfo };
            base.InvokeAsync("CreateMemberGroup", objArray, this.CreateMemberGroupOperationCompleted, userState);
        }

        [SoapDocumentMethod(
            "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService/CreateUserProfileByAccountName",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public PropertyData[] CreateUserProfileByAccountName(string accountName)
        {
            object[] objArray = new object[] { accountName };
            return (PropertyData[])base.Invoke("CreateUserProfileByAccountName", objArray)[0];
        }

        public void CreateUserProfileByAccountNameAsync(string accountName)
        {
            this.CreateUserProfileByAccountNameAsync(accountName, null);
        }

        public void CreateUserProfileByAccountNameAsync(string accountName, object userState)
        {
            if (this.CreateUserProfileByAccountNameOperationCompleted == null)
            {
                this.CreateUserProfileByAccountNameOperationCompleted =
                    new SendOrPostCallback(this.OnCreateUserProfileByAccountNameOperationCompleted);
            }

            object[] objArray = new object[] { accountName };
            base.InvokeAsync("CreateUserProfileByAccountName", objArray,
                this.CreateUserProfileByAccountNameOperationCompleted, userState);
        }

        [SoapDocumentMethod(
            "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService/GetCommonColleagues",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public ContactData[] GetCommonColleagues(string accountName)
        {
            object[] objArray = new object[] { accountName };
            return (ContactData[])base.Invoke("GetCommonColleagues", objArray)[0];
        }

        public void GetCommonColleaguesAsync(string accountName)
        {
            this.GetCommonColleaguesAsync(accountName, null);
        }

        public void GetCommonColleaguesAsync(string accountName, object userState)
        {
            if (this.GetCommonColleaguesOperationCompleted == null)
            {
                this.GetCommonColleaguesOperationCompleted =
                    new SendOrPostCallback(this.OnGetCommonColleaguesOperationCompleted);
            }

            object[] objArray = new object[] { accountName };
            base.InvokeAsync("GetCommonColleagues", objArray, this.GetCommonColleaguesOperationCompleted, userState);
        }

        [SoapDocumentMethod(
            "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService/GetCommonManager",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public ContactData GetCommonManager(string accountName)
        {
            object[] objArray = new object[] { accountName };
            return (ContactData)base.Invoke("GetCommonManager", objArray)[0];
        }

        public void GetCommonManagerAsync(string accountName)
        {
            this.GetCommonManagerAsync(accountName, null);
        }

        public void GetCommonManagerAsync(string accountName, object userState)
        {
            if (this.GetCommonManagerOperationCompleted == null)
            {
                this.GetCommonManagerOperationCompleted =
                    new SendOrPostCallback(this.OnGetCommonManagerOperationCompleted);
            }

            object[] objArray = new object[] { accountName };
            base.InvokeAsync("GetCommonManager", objArray, this.GetCommonManagerOperationCompleted, userState);
        }

        [SoapDocumentMethod(
            "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService/GetCommonMemberships",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public MembershipData[] GetCommonMemberships(string accountName)
        {
            object[] objArray = new object[] { accountName };
            return (MembershipData[])base.Invoke("GetCommonMemberships", objArray)[0];
        }

        public void GetCommonMembershipsAsync(string accountName)
        {
            this.GetCommonMembershipsAsync(accountName, null);
        }

        public void GetCommonMembershipsAsync(string accountName, object userState)
        {
            if (this.GetCommonMembershipsOperationCompleted == null)
            {
                this.GetCommonMembershipsOperationCompleted =
                    new SendOrPostCallback(this.OnGetCommonMembershipsOperationCompleted);
            }

            object[] objArray = new object[] { accountName };
            base.InvokeAsync("GetCommonMemberships", objArray, this.GetCommonMembershipsOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://microsoft.com/webservices/SharePointPortalServer/UserProfileService/GetInCommon",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public InCommonData GetInCommon(string accountName)
        {
            object[] objArray = new object[] { accountName };
            return (InCommonData)base.Invoke("GetInCommon", objArray)[0];
        }

        public void GetInCommonAsync(string accountName)
        {
            this.GetInCommonAsync(accountName, null);
        }

        public void GetInCommonAsync(string accountName, object userState)
        {
            if (this.GetInCommonOperationCompleted == null)
            {
                this.GetInCommonOperationCompleted = new SendOrPostCallback(this.OnGetInCommonOperationCompleted);
            }

            object[] objArray = new object[] { accountName };
            base.InvokeAsync("GetInCommon", objArray, this.GetInCommonOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://microsoft.com/webservices/SharePointPortalServer/UserProfileService/GetLeaders",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public Leader[] GetLeaders()
        {
            object[] objArray = base.Invoke("GetLeaders", new object[0]);
            return (Leader[])objArray[0];
        }

        public void GetLeadersAsync()
        {
            this.GetLeadersAsync(null);
        }

        public void GetLeadersAsync(object userState)
        {
            if (this.GetLeadersOperationCompleted == null)
            {
                this.GetLeadersOperationCompleted = new SendOrPostCallback(this.OnGetLeadersOperationCompleted);
            }

            base.InvokeAsync("GetLeaders", new object[0], this.GetLeadersOperationCompleted, userState);
        }

        [SoapDocumentMethod(
            "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService/GetProfileSchema",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public PropertyInfo[] GetProfileSchema(string schemaName)
        {
            object[] objArray = new object[] { schemaName };
            return (PropertyInfo[])base.Invoke("GetProfileSchema", objArray)[0];
        }

        public void GetProfileSchemaAsync(string schemaName)
        {
            this.GetProfileSchemaAsync(schemaName, null);
        }

        public void GetProfileSchemaAsync(string schemaName, object userState)
        {
            if (this.GetProfileSchemaOperationCompleted == null)
            {
                this.GetProfileSchemaOperationCompleted =
                    new SendOrPostCallback(this.OnGetProfileSchemaOperationCompleted);
            }

            object[] objArray = new object[] { schemaName };
            base.InvokeAsync("GetProfileSchema", objArray, this.GetProfileSchemaOperationCompleted, userState);
        }

        [SoapDocumentMethod(
            "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService/GetProfileSchemaNameByAccountName",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetProfileSchemaNameByAccountName(string accountName)
        {
            object[] objArray = new object[] { accountName };
            return (string)base.Invoke("GetProfileSchemaNameByAccountName", objArray)[0];
        }

        public void GetProfileSchemaNameByAccountNameAsync(string accountName)
        {
            this.GetProfileSchemaNameByAccountNameAsync(accountName, null);
        }

        public void GetProfileSchemaNameByAccountNameAsync(string accountName, object userState)
        {
            if (this.GetProfileSchemaNameByAccountNameOperationCompleted == null)
            {
                this.GetProfileSchemaNameByAccountNameOperationCompleted =
                    new SendOrPostCallback(this.OnGetProfileSchemaNameByAccountNameOperationCompleted);
            }

            object[] objArray = new object[] { accountName };
            base.InvokeAsync("GetProfileSchemaNameByAccountName", objArray,
                this.GetProfileSchemaNameByAccountNameOperationCompleted, userState);
        }

        [SoapDocumentMethod(
            "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService/GetProfileSchemaNames",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string[] GetProfileSchemaNames()
        {
            object[] objArray = base.Invoke("GetProfileSchemaNames", new object[0]);
            return (string[])objArray[0];
        }

        public void GetProfileSchemaNamesAsync()
        {
            this.GetProfileSchemaNamesAsync(null);
        }

        public void GetProfileSchemaNamesAsync(object userState)
        {
            if (this.GetProfileSchemaNamesOperationCompleted == null)
            {
                this.GetProfileSchemaNamesOperationCompleted =
                    new SendOrPostCallback(this.OnGetProfileSchemaNamesOperationCompleted);
            }

            base.InvokeAsync("GetProfileSchemaNames", new object[0], this.GetProfileSchemaNamesOperationCompleted,
                userState);
        }

        [SoapDocumentMethod(
            "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService/GetPropertyChoiceList",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string[] GetPropertyChoiceList(string propertyName)
        {
            object[] objArray = new object[] { propertyName };
            return (string[])base.Invoke("GetPropertyChoiceList", objArray)[0];
        }

        public void GetPropertyChoiceListAsync(string propertyName)
        {
            this.GetPropertyChoiceListAsync(propertyName, null);
        }

        public void GetPropertyChoiceListAsync(string propertyName, object userState)
        {
            if (this.GetPropertyChoiceListOperationCompleted == null)
            {
                this.GetPropertyChoiceListOperationCompleted =
                    new SendOrPostCallback(this.OnGetPropertyChoiceListOperationCompleted);
            }

            object[] objArray = new object[] { propertyName };
            base.InvokeAsync("GetPropertyChoiceList", objArray, this.GetPropertyChoiceListOperationCompleted,
                userState);
        }

        [SoapDocumentMethod(
            "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService/GetUserColleagues",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public ContactData[] GetUserColleagues(string accountName)
        {
            object[] objArray = new object[] { accountName };
            return (ContactData[])base.Invoke("GetUserColleagues", objArray)[0];
        }

        public void GetUserColleaguesAsync(string accountName)
        {
            this.GetUserColleaguesAsync(accountName, null);
        }

        public void GetUserColleaguesAsync(string accountName, object userState)
        {
            if (this.GetUserColleaguesOperationCompleted == null)
            {
                this.GetUserColleaguesOperationCompleted =
                    new SendOrPostCallback(this.OnGetUserColleaguesOperationCompleted);
            }

            object[] objArray = new object[] { accountName };
            base.InvokeAsync("GetUserColleagues", objArray, this.GetUserColleaguesOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://microsoft.com/webservices/SharePointPortalServer/UserProfileService/GetUserLinks",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public QuickLinkData[] GetUserLinks(string accountName)
        {
            object[] objArray = new object[] { accountName };
            return (QuickLinkData[])base.Invoke("GetUserLinks", objArray)[0];
        }

        public void GetUserLinksAsync(string accountName)
        {
            this.GetUserLinksAsync(accountName, null);
        }

        public void GetUserLinksAsync(string accountName, object userState)
        {
            if (this.GetUserLinksOperationCompleted == null)
            {
                this.GetUserLinksOperationCompleted = new SendOrPostCallback(this.OnGetUserLinksOperationCompleted);
            }

            object[] objArray = new object[] { accountName };
            base.InvokeAsync("GetUserLinks", objArray, this.GetUserLinksOperationCompleted, userState);
        }

        [SoapDocumentMethod(
            "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService/GetUserMemberships",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public MembershipData[] GetUserMemberships(string accountName)
        {
            object[] objArray = new object[] { accountName };
            return (MembershipData[])base.Invoke("GetUserMemberships", objArray)[0];
        }

        public void GetUserMembershipsAsync(string accountName)
        {
            this.GetUserMembershipsAsync(accountName, null);
        }

        public void GetUserMembershipsAsync(string accountName, object userState)
        {
            if (this.GetUserMembershipsOperationCompleted == null)
            {
                this.GetUserMembershipsOperationCompleted =
                    new SendOrPostCallback(this.OnGetUserMembershipsOperationCompleted);
            }

            object[] objArray = new object[] { accountName };
            base.InvokeAsync("GetUserMemberships", objArray, this.GetUserMembershipsOperationCompleted, userState);
        }

        [SoapDocumentMethod(
            "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService/GetUserOrganizations",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public OrganizationProfileData[] GetUserOrganizations(string accountName)
        {
            object[] objArray = new object[] { accountName };
            return (OrganizationProfileData[])base.Invoke("GetUserOrganizations", objArray)[0];
        }

        public void GetUserOrganizationsAsync(string accountName)
        {
            this.GetUserOrganizationsAsync(accountName, null);
        }

        public void GetUserOrganizationsAsync(string accountName, object userState)
        {
            if (this.GetUserOrganizationsOperationCompleted == null)
            {
                this.GetUserOrganizationsOperationCompleted =
                    new SendOrPostCallback(this.OnGetUserOrganizationsOperationCompleted);
            }

            object[] objArray = new object[] { accountName };
            base.InvokeAsync("GetUserOrganizations", objArray, this.GetUserOrganizationsOperationCompleted, userState);
        }

        [SoapDocumentMethod(
            "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService/GetUserPinnedLinks",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public PinnedLinkData[] GetUserPinnedLinks(string accountName)
        {
            object[] objArray = new object[] { accountName };
            return (PinnedLinkData[])base.Invoke("GetUserPinnedLinks", objArray)[0];
        }

        public void GetUserPinnedLinksAsync(string accountName)
        {
            this.GetUserPinnedLinksAsync(accountName, null);
        }

        public void GetUserPinnedLinksAsync(string accountName, object userState)
        {
            if (this.GetUserPinnedLinksOperationCompleted == null)
            {
                this.GetUserPinnedLinksOperationCompleted =
                    new SendOrPostCallback(this.OnGetUserPinnedLinksOperationCompleted);
            }

            object[] objArray = new object[] { accountName };
            base.InvokeAsync("GetUserPinnedLinks", objArray, this.GetUserPinnedLinksOperationCompleted, userState);
        }

        [SoapDocumentMethod(
            "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService/GetUserProfileByGuid",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public PropertyData[] GetUserProfileByGuid(Guid guid)
        {
            object[] objArray = new object[] { guid };
            return (PropertyData[])base.Invoke("GetUserProfileByGuid", objArray)[0];
        }

        public void GetUserProfileByGuidAsync(Guid guid)
        {
            this.GetUserProfileByGuidAsync(guid, null);
        }

        public void GetUserProfileByGuidAsync(Guid guid, object userState)
        {
            if (this.GetUserProfileByGuidOperationCompleted == null)
            {
                this.GetUserProfileByGuidOperationCompleted =
                    new SendOrPostCallback(this.OnGetUserProfileByGuidOperationCompleted);
            }

            object[] objArray = new object[] { guid };
            base.InvokeAsync("GetUserProfileByGuid", objArray, this.GetUserProfileByGuidOperationCompleted, userState);
        }

        [SoapDocumentMethod(
            "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService/GetUserProfileByIndex",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public GetUserProfileByIndexResult GetUserProfileByIndex(int index)
        {
            object[] objArray = new object[] { index };
            return (GetUserProfileByIndexResult)base.Invoke("GetUserProfileByIndex", objArray)[0];
        }

        public void GetUserProfileByIndexAsync(int index)
        {
            this.GetUserProfileByIndexAsync(index, null);
        }

        public void GetUserProfileByIndexAsync(int index, object userState)
        {
            if (this.GetUserProfileByIndexOperationCompleted == null)
            {
                this.GetUserProfileByIndexOperationCompleted =
                    new SendOrPostCallback(this.OnGetUserProfileByIndexOperationCompleted);
            }

            object[] objArray = new object[] { index };
            base.InvokeAsync("GetUserProfileByIndex", objArray, this.GetUserProfileByIndexOperationCompleted,
                userState);
        }

        [SoapDocumentMethod(
            "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService/GetUserProfileByName",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public PropertyData[] GetUserProfileByName(string AccountName)
        {
            object[] accountName = new object[] { AccountName };
            return (PropertyData[])base.Invoke("GetUserProfileByName", accountName)[0];
        }

        public void GetUserProfileByNameAsync(string AccountName)
        {
            this.GetUserProfileByNameAsync(AccountName, null);
        }

        public void GetUserProfileByNameAsync(string AccountName, object userState)
        {
            if (this.GetUserProfileByNameOperationCompleted == null)
            {
                this.GetUserProfileByNameOperationCompleted =
                    new SendOrPostCallback(this.OnGetUserProfileByNameOperationCompleted);
            }

            object[] accountName = new object[] { AccountName };
            base.InvokeAsync("GetUserProfileByName", accountName, this.GetUserProfileByNameOperationCompleted,
                userState);
        }

        [SoapDocumentMethod(
            "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService/GetUserProfileCount",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public long GetUserProfileCount()
        {
            object[] objArray = base.Invoke("GetUserProfileCount", new object[0]);
            return (long)objArray[0];
        }

        public void GetUserProfileCountAsync()
        {
            this.GetUserProfileCountAsync(null);
        }

        public void GetUserProfileCountAsync(object userState)
        {
            if (this.GetUserProfileCountOperationCompleted == null)
            {
                this.GetUserProfileCountOperationCompleted =
                    new SendOrPostCallback(this.OnGetUserProfileCountOperationCompleted);
            }

            base.InvokeAsync("GetUserProfileCount", new object[0], this.GetUserProfileCountOperationCompleted,
                userState);
        }

        [SoapDocumentMethod(
            "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService/GetUserProfileSchema",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public PropertyInfo[] GetUserProfileSchema()
        {
            object[] objArray = base.Invoke("GetUserProfileSchema", new object[0]);
            return (PropertyInfo[])objArray[0];
        }

        public void GetUserProfileSchemaAsync()
        {
            this.GetUserProfileSchemaAsync(null);
        }

        public void GetUserProfileSchemaAsync(object userState)
        {
            if (this.GetUserProfileSchemaOperationCompleted == null)
            {
                this.GetUserProfileSchemaOperationCompleted =
                    new SendOrPostCallback(this.OnGetUserProfileSchemaOperationCompleted);
            }

            base.InvokeAsync("GetUserProfileSchema", new object[0], this.GetUserProfileSchemaOperationCompleted,
                userState);
        }

        [SoapDocumentMethod(
            "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService/GetUserPropertyByAccountName",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public PropertyData GetUserPropertyByAccountName(string accountName, string propertyName)
        {
            object[] objArray = new object[] { accountName, propertyName };
            return (PropertyData)base.Invoke("GetUserPropertyByAccountName", objArray)[0];
        }

        public void GetUserPropertyByAccountNameAsync(string accountName, string propertyName)
        {
            this.GetUserPropertyByAccountNameAsync(accountName, propertyName, null);
        }

        public void GetUserPropertyByAccountNameAsync(string accountName, string propertyName, object userState)
        {
            if (this.GetUserPropertyByAccountNameOperationCompleted == null)
            {
                this.GetUserPropertyByAccountNameOperationCompleted =
                    new SendOrPostCallback(this.OnGetUserPropertyByAccountNameOperationCompleted);
            }

            object[] objArray = new object[] { accountName, propertyName };
            base.InvokeAsync("GetUserPropertyByAccountName", objArray,
                this.GetUserPropertyByAccountNameOperationCompleted, userState);
        }

        private bool IsLocalFileSystemWebService(string url)
        {
            if (url == null || url == string.Empty)
            {
                return false;
            }

            System.Uri uri = new System.Uri(url);
            if (uri.Port >= 1024 && string.Compare(uri.Host, "localHost", StringComparison.OrdinalIgnoreCase) == 0)
            {
                return true;
            }

            return false;
        }

        [SoapDocumentMethod(
            "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService/ModifyUserPropertyByAccountName",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public void ModifyUserPropertyByAccountName(string accountName, PropertyData[] newData)
        {
            object[] objArray = new object[] { accountName, newData };
            base.Invoke("ModifyUserPropertyByAccountName", objArray);
        }

        public void ModifyUserPropertyByAccountNameAsync(string accountName, PropertyData[] newData)
        {
            this.ModifyUserPropertyByAccountNameAsync(accountName, newData, null);
        }

        public void ModifyUserPropertyByAccountNameAsync(string accountName, PropertyData[] newData, object userState)
        {
            if (this.ModifyUserPropertyByAccountNameOperationCompleted == null)
            {
                this.ModifyUserPropertyByAccountNameOperationCompleted =
                    new SendOrPostCallback(this.OnModifyUserPropertyByAccountNameOperationCompleted);
            }

            object[] objArray = new object[] { accountName, newData };
            base.InvokeAsync("ModifyUserPropertyByAccountName", objArray,
                this.ModifyUserPropertyByAccountNameOperationCompleted, userState);
        }

        private void OnAddColleagueOperationCompleted(object arg)
        {
            if (this.AddColleagueCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddColleagueCompleted(this,
                    new AddColleagueCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnAddColleagueWithoutEmailNotificationOperationCompleted(object arg)
        {
            if (this.AddColleagueWithoutEmailNotificationCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddColleagueWithoutEmailNotificationCompleted(this,
                    new AddColleagueWithoutEmailNotificationCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnAddLeaderOperationCompleted(object arg)
        {
            if (this.AddLeaderCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddLeaderCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnAddLinkOperationCompleted(object arg)
        {
            if (this.AddLinkCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddLinkCompleted(this,
                    new AddLinkCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnAddMembershipOperationCompleted(object arg)
        {
            if (this.AddMembershipCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddMembershipCompleted(this,
                    new AddMembershipCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnAddPinnedLinkOperationCompleted(object arg)
        {
            if (this.AddPinnedLinkCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddPinnedLinkCompleted(this,
                    new AddPinnedLinkCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnAddSuggestionsOperationCompleted(object arg)
        {
            if (this.AddSuggestionsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddSuggestionsCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnCreateMemberGroupOperationCompleted(object arg)
        {
            if (this.CreateMemberGroupCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.CreateMemberGroupCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnCreateUserProfileByAccountNameOperationCompleted(object arg)
        {
            if (this.CreateUserProfileByAccountNameCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.CreateUserProfileByAccountNameCompleted(this,
                    new CreateUserProfileByAccountNameCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetCommonColleaguesOperationCompleted(object arg)
        {
            if (this.GetCommonColleaguesCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetCommonColleaguesCompleted(this,
                    new GetCommonColleaguesCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetCommonManagerOperationCompleted(object arg)
        {
            if (this.GetCommonManagerCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetCommonManagerCompleted(this,
                    new GetCommonManagerCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetCommonMembershipsOperationCompleted(object arg)
        {
            if (this.GetCommonMembershipsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetCommonMembershipsCompleted(this,
                    new GetCommonMembershipsCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetInCommonOperationCompleted(object arg)
        {
            if (this.GetInCommonCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetInCommonCompleted(this,
                    new GetInCommonCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetLeadersOperationCompleted(object arg)
        {
            if (this.GetLeadersCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetLeadersCompleted(this,
                    new GetLeadersCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetProfileSchemaNameByAccountNameOperationCompleted(object arg)
        {
            if (this.GetProfileSchemaNameByAccountNameCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetProfileSchemaNameByAccountNameCompleted(this,
                    new GetProfileSchemaNameByAccountNameCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetProfileSchemaNamesOperationCompleted(object arg)
        {
            if (this.GetProfileSchemaNamesCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetProfileSchemaNamesCompleted(this,
                    new GetProfileSchemaNamesCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetProfileSchemaOperationCompleted(object arg)
        {
            if (this.GetProfileSchemaCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetProfileSchemaCompleted(this,
                    new GetProfileSchemaCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetPropertyChoiceListOperationCompleted(object arg)
        {
            if (this.GetPropertyChoiceListCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetPropertyChoiceListCompleted(this,
                    new GetPropertyChoiceListCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetUserColleaguesOperationCompleted(object arg)
        {
            if (this.GetUserColleaguesCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetUserColleaguesCompleted(this,
                    new GetUserColleaguesCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetUserLinksOperationCompleted(object arg)
        {
            if (this.GetUserLinksCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetUserLinksCompleted(this,
                    new GetUserLinksCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetUserMembershipsOperationCompleted(object arg)
        {
            if (this.GetUserMembershipsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetUserMembershipsCompleted(this,
                    new GetUserMembershipsCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetUserOrganizationsOperationCompleted(object arg)
        {
            if (this.GetUserOrganizationsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetUserOrganizationsCompleted(this,
                    new GetUserOrganizationsCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetUserPinnedLinksOperationCompleted(object arg)
        {
            if (this.GetUserPinnedLinksCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetUserPinnedLinksCompleted(this,
                    new GetUserPinnedLinksCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetUserProfileByGuidOperationCompleted(object arg)
        {
            if (this.GetUserProfileByGuidCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetUserProfileByGuidCompleted(this,
                    new GetUserProfileByGuidCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetUserProfileByIndexOperationCompleted(object arg)
        {
            if (this.GetUserProfileByIndexCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetUserProfileByIndexCompleted(this,
                    new GetUserProfileByIndexCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetUserProfileByNameOperationCompleted(object arg)
        {
            if (this.GetUserProfileByNameCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetUserProfileByNameCompleted(this,
                    new GetUserProfileByNameCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetUserProfileCountOperationCompleted(object arg)
        {
            if (this.GetUserProfileCountCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetUserProfileCountCompleted(this,
                    new GetUserProfileCountCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetUserProfileSchemaOperationCompleted(object arg)
        {
            if (this.GetUserProfileSchemaCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetUserProfileSchemaCompleted(this,
                    new GetUserProfileSchemaCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetUserPropertyByAccountNameOperationCompleted(object arg)
        {
            if (this.GetUserPropertyByAccountNameCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetUserPropertyByAccountNameCompleted(this,
                    new GetUserPropertyByAccountNameCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnModifyUserPropertyByAccountNameOperationCompleted(object arg)
        {
            if (this.ModifyUserPropertyByAccountNameCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.ModifyUserPropertyByAccountNameCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnRemoveAllColleaguesOperationCompleted(object arg)
        {
            if (this.RemoveAllColleaguesCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.RemoveAllColleaguesCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnRemoveAllLinksOperationCompleted(object arg)
        {
            if (this.RemoveAllLinksCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.RemoveAllLinksCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnRemoveAllMembershipsOperationCompleted(object arg)
        {
            if (this.RemoveAllMembershipsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.RemoveAllMembershipsCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnRemoveAllPinnedLinksOperationCompleted(object arg)
        {
            if (this.RemoveAllPinnedLinksCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.RemoveAllPinnedLinksCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnRemoveColleagueOperationCompleted(object arg)
        {
            if (this.RemoveColleagueCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.RemoveColleagueCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnRemoveLeaderOperationCompleted(object arg)
        {
            if (this.RemoveLeaderCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.RemoveLeaderCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnRemoveLinkOperationCompleted(object arg)
        {
            if (this.RemoveLinkCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.RemoveLinkCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnRemoveMembershipOperationCompleted(object arg)
        {
            if (this.RemoveMembershipCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.RemoveMembershipCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnRemovePinnedLinkOperationCompleted(object arg)
        {
            if (this.RemovePinnedLinkCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.RemovePinnedLinkCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnUpdateColleaguePrivacyOperationCompleted(object arg)
        {
            if (this.UpdateColleaguePrivacyCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.UpdateColleaguePrivacyCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnUpdateLinkOperationCompleted(object arg)
        {
            if (this.UpdateLinkCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.UpdateLinkCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnUpdateMembershipPrivacyOperationCompleted(object arg)
        {
            if (this.UpdateMembershipPrivacyCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.UpdateMembershipPrivacyCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnUpdatePinnedLinkOperationCompleted(object arg)
        {
            if (this.UpdatePinnedLinkCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.UpdatePinnedLinkCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        [SoapDocumentMethod(
            "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService/RemoveAllColleagues",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public void RemoveAllColleagues(string accountName)
        {
            object[] objArray = new object[] { accountName };
            base.Invoke("RemoveAllColleagues", objArray);
        }

        public void RemoveAllColleaguesAsync(string accountName)
        {
            this.RemoveAllColleaguesAsync(accountName, null);
        }

        public void RemoveAllColleaguesAsync(string accountName, object userState)
        {
            if (this.RemoveAllColleaguesOperationCompleted == null)
            {
                this.RemoveAllColleaguesOperationCompleted =
                    new SendOrPostCallback(this.OnRemoveAllColleaguesOperationCompleted);
            }

            object[] objArray = new object[] { accountName };
            base.InvokeAsync("RemoveAllColleagues", objArray, this.RemoveAllColleaguesOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://microsoft.com/webservices/SharePointPortalServer/UserProfileService/RemoveAllLinks",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public void RemoveAllLinks(string accountName)
        {
            object[] objArray = new object[] { accountName };
            base.Invoke("RemoveAllLinks", objArray);
        }

        public void RemoveAllLinksAsync(string accountName)
        {
            this.RemoveAllLinksAsync(accountName, null);
        }

        public void RemoveAllLinksAsync(string accountName, object userState)
        {
            if (this.RemoveAllLinksOperationCompleted == null)
            {
                this.RemoveAllLinksOperationCompleted = new SendOrPostCallback(this.OnRemoveAllLinksOperationCompleted);
            }

            object[] objArray = new object[] { accountName };
            base.InvokeAsync("RemoveAllLinks", objArray, this.RemoveAllLinksOperationCompleted, userState);
        }

        [SoapDocumentMethod(
            "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService/RemoveAllMemberships",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public void RemoveAllMemberships(string accountName)
        {
            object[] objArray = new object[] { accountName };
            base.Invoke("RemoveAllMemberships", objArray);
        }

        public void RemoveAllMembershipsAsync(string accountName)
        {
            this.RemoveAllMembershipsAsync(accountName, null);
        }

        public void RemoveAllMembershipsAsync(string accountName, object userState)
        {
            if (this.RemoveAllMembershipsOperationCompleted == null)
            {
                this.RemoveAllMembershipsOperationCompleted =
                    new SendOrPostCallback(this.OnRemoveAllMembershipsOperationCompleted);
            }

            object[] objArray = new object[] { accountName };
            base.InvokeAsync("RemoveAllMemberships", objArray, this.RemoveAllMembershipsOperationCompleted, userState);
        }

        [SoapDocumentMethod(
            "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService/RemoveAllPinnedLinks",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public void RemoveAllPinnedLinks(string accountName)
        {
            object[] objArray = new object[] { accountName };
            base.Invoke("RemoveAllPinnedLinks", objArray);
        }

        public void RemoveAllPinnedLinksAsync(string accountName)
        {
            this.RemoveAllPinnedLinksAsync(accountName, null);
        }

        public void RemoveAllPinnedLinksAsync(string accountName, object userState)
        {
            if (this.RemoveAllPinnedLinksOperationCompleted == null)
            {
                this.RemoveAllPinnedLinksOperationCompleted =
                    new SendOrPostCallback(this.OnRemoveAllPinnedLinksOperationCompleted);
            }

            object[] objArray = new object[] { accountName };
            base.InvokeAsync("RemoveAllPinnedLinks", objArray, this.RemoveAllPinnedLinksOperationCompleted, userState);
        }

        [SoapDocumentMethod(
            "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService/RemoveColleague",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public void RemoveColleague(string accountName, string colleagueAccountName)
        {
            object[] objArray = new object[] { accountName, colleagueAccountName };
            base.Invoke("RemoveColleague", objArray);
        }

        public void RemoveColleagueAsync(string accountName, string colleagueAccountName)
        {
            this.RemoveColleagueAsync(accountName, colleagueAccountName, null);
        }

        public void RemoveColleagueAsync(string accountName, string colleagueAccountName, object userState)
        {
            if (this.RemoveColleagueOperationCompleted == null)
            {
                this.RemoveColleagueOperationCompleted =
                    new SendOrPostCallback(this.OnRemoveColleagueOperationCompleted);
            }

            object[] objArray = new object[] { accountName, colleagueAccountName };
            base.InvokeAsync("RemoveColleague", objArray, this.RemoveColleagueOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://microsoft.com/webservices/SharePointPortalServer/UserProfileService/RemoveLeader",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public void RemoveLeader(string accountName)
        {
            object[] objArray = new object[] { accountName };
            base.Invoke("RemoveLeader", objArray);
        }

        public void RemoveLeaderAsync(string accountName)
        {
            this.RemoveLeaderAsync(accountName, null);
        }

        public void RemoveLeaderAsync(string accountName, object userState)
        {
            if (this.RemoveLeaderOperationCompleted == null)
            {
                this.RemoveLeaderOperationCompleted = new SendOrPostCallback(this.OnRemoveLeaderOperationCompleted);
            }

            object[] objArray = new object[] { accountName };
            base.InvokeAsync("RemoveLeader", objArray, this.RemoveLeaderOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://microsoft.com/webservices/SharePointPortalServer/UserProfileService/RemoveLink",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public void RemoveLink(string accountName, int id)
        {
            object[] objArray = new object[] { accountName, id };
            base.Invoke("RemoveLink", objArray);
        }

        public void RemoveLinkAsync(string accountName, int id)
        {
            this.RemoveLinkAsync(accountName, id, null);
        }

        public void RemoveLinkAsync(string accountName, int id, object userState)
        {
            if (this.RemoveLinkOperationCompleted == null)
            {
                this.RemoveLinkOperationCompleted = new SendOrPostCallback(this.OnRemoveLinkOperationCompleted);
            }

            object[] objArray = new object[] { accountName, id };
            base.InvokeAsync("RemoveLink", objArray, this.RemoveLinkOperationCompleted, userState);
        }

        [SoapDocumentMethod(
            "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService/RemoveMembership",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public void RemoveMembership(string accountName, Guid sourceInternal, string sourceReference)
        {
            object[] objArray = new object[] { accountName, sourceInternal, sourceReference };
            base.Invoke("RemoveMembership", objArray);
        }

        public void RemoveMembershipAsync(string accountName, Guid sourceInternal, string sourceReference)
        {
            this.RemoveMembershipAsync(accountName, sourceInternal, sourceReference, null);
        }

        public void RemoveMembershipAsync(string accountName, Guid sourceInternal, string sourceReference,
            object userState)
        {
            if (this.RemoveMembershipOperationCompleted == null)
            {
                this.RemoveMembershipOperationCompleted =
                    new SendOrPostCallback(this.OnRemoveMembershipOperationCompleted);
            }

            object[] objArray = new object[] { accountName, sourceInternal, sourceReference };
            base.InvokeAsync("RemoveMembership", objArray, this.RemoveMembershipOperationCompleted, userState);
        }

        [SoapDocumentMethod(
            "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService/RemovePinnedLink",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public void RemovePinnedLink(string accountName, int id)
        {
            object[] objArray = new object[] { accountName, id };
            base.Invoke("RemovePinnedLink", objArray);
        }

        public void RemovePinnedLinkAsync(string accountName, int id)
        {
            this.RemovePinnedLinkAsync(accountName, id, null);
        }

        public void RemovePinnedLinkAsync(string accountName, int id, object userState)
        {
            if (this.RemovePinnedLinkOperationCompleted == null)
            {
                this.RemovePinnedLinkOperationCompleted =
                    new SendOrPostCallback(this.OnRemovePinnedLinkOperationCompleted);
            }

            object[] objArray = new object[] { accountName, id };
            base.InvokeAsync("RemovePinnedLink", objArray, this.RemovePinnedLinkOperationCompleted, userState);
        }

        [SoapDocumentMethod(
            "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService/UpdateColleaguePrivacy",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public void UpdateColleaguePrivacy(string accountName, string colleagueAccountName, Privacy newPrivacy)
        {
            object[] objArray = new object[] { accountName, colleagueAccountName, newPrivacy };
            base.Invoke("UpdateColleaguePrivacy", objArray);
        }

        public void UpdateColleaguePrivacyAsync(string accountName, string colleagueAccountName, Privacy newPrivacy)
        {
            this.UpdateColleaguePrivacyAsync(accountName, colleagueAccountName, newPrivacy, null);
        }

        public void UpdateColleaguePrivacyAsync(string accountName, string colleagueAccountName, Privacy newPrivacy,
            object userState)
        {
            if (this.UpdateColleaguePrivacyOperationCompleted == null)
            {
                this.UpdateColleaguePrivacyOperationCompleted =
                    new SendOrPostCallback(this.OnUpdateColleaguePrivacyOperationCompleted);
            }

            object[] objArray = new object[] { accountName, colleagueAccountName, newPrivacy };
            base.InvokeAsync("UpdateColleaguePrivacy", objArray, this.UpdateColleaguePrivacyOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://microsoft.com/webservices/SharePointPortalServer/UserProfileService/UpdateLink",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public void UpdateLink(string accountName, QuickLinkData data)
        {
            object[] objArray = new object[] { accountName, data };
            base.Invoke("UpdateLink", objArray);
        }

        public void UpdateLinkAsync(string accountName, QuickLinkData data)
        {
            this.UpdateLinkAsync(accountName, data, null);
        }

        public void UpdateLinkAsync(string accountName, QuickLinkData data, object userState)
        {
            if (this.UpdateLinkOperationCompleted == null)
            {
                this.UpdateLinkOperationCompleted = new SendOrPostCallback(this.OnUpdateLinkOperationCompleted);
            }

            object[] objArray = new object[] { accountName, data };
            base.InvokeAsync("UpdateLink", objArray, this.UpdateLinkOperationCompleted, userState);
        }

        [SoapDocumentMethod(
            "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService/UpdateMembershipPrivacy",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public void UpdateMembershipPrivacy(string accountName, Guid sourceInternal, string sourceReference,
            Privacy newPrivacy)
        {
            object[] objArray = new object[] { accountName, sourceInternal, sourceReference, newPrivacy };
            base.Invoke("UpdateMembershipPrivacy", objArray);
        }

        public void UpdateMembershipPrivacyAsync(string accountName, Guid sourceInternal, string sourceReference,
            Privacy newPrivacy)
        {
            this.UpdateMembershipPrivacyAsync(accountName, sourceInternal, sourceReference, newPrivacy, null);
        }

        public void UpdateMembershipPrivacyAsync(string accountName, Guid sourceInternal, string sourceReference,
            Privacy newPrivacy, object userState)
        {
            if (this.UpdateMembershipPrivacyOperationCompleted == null)
            {
                this.UpdateMembershipPrivacyOperationCompleted =
                    new SendOrPostCallback(this.OnUpdateMembershipPrivacyOperationCompleted);
            }

            object[] objArray = new object[] { accountName, sourceInternal, sourceReference, newPrivacy };
            base.InvokeAsync("UpdateMembershipPrivacy", objArray, this.UpdateMembershipPrivacyOperationCompleted,
                userState);
        }

        [SoapDocumentMethod(
            "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService/UpdatePinnedLink",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/UserProfileService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public void UpdatePinnedLink(string accountName, PinnedLinkData data)
        {
            object[] objArray = new object[] { accountName, data };
            base.Invoke("UpdatePinnedLink", objArray);
        }

        public void UpdatePinnedLinkAsync(string accountName, PinnedLinkData data)
        {
            this.UpdatePinnedLinkAsync(accountName, data, null);
        }

        public void UpdatePinnedLinkAsync(string accountName, PinnedLinkData data, object userState)
        {
            if (this.UpdatePinnedLinkOperationCompleted == null)
            {
                this.UpdatePinnedLinkOperationCompleted =
                    new SendOrPostCallback(this.OnUpdatePinnedLinkOperationCompleted);
            }

            object[] objArray = new object[] { accountName, data };
            base.InvokeAsync("UpdatePinnedLink", objArray, this.UpdatePinnedLinkOperationCompleted, userState);
        }

        public event AddColleagueCompletedEventHandler AddColleagueCompleted;

        public event AddColleagueWithoutEmailNotificationCompletedEventHandler
            AddColleagueWithoutEmailNotificationCompleted;

        public event AddLeaderCompletedEventHandler AddLeaderCompleted;

        public event AddLinkCompletedEventHandler AddLinkCompleted;

        public event AddMembershipCompletedEventHandler AddMembershipCompleted;

        public event AddPinnedLinkCompletedEventHandler AddPinnedLinkCompleted;

        public event AddSuggestionsCompletedEventHandler AddSuggestionsCompleted;

        public event CreateMemberGroupCompletedEventHandler CreateMemberGroupCompleted;

        public event CreateUserProfileByAccountNameCompletedEventHandler CreateUserProfileByAccountNameCompleted;

        public event GetCommonColleaguesCompletedEventHandler GetCommonColleaguesCompleted;

        public event GetCommonManagerCompletedEventHandler GetCommonManagerCompleted;

        public event GetCommonMembershipsCompletedEventHandler GetCommonMembershipsCompleted;

        public event GetInCommonCompletedEventHandler GetInCommonCompleted;

        public event GetLeadersCompletedEventHandler GetLeadersCompleted;

        public event GetProfileSchemaCompletedEventHandler GetProfileSchemaCompleted;

        public event GetProfileSchemaNameByAccountNameCompletedEventHandler GetProfileSchemaNameByAccountNameCompleted;

        public event GetProfileSchemaNamesCompletedEventHandler GetProfileSchemaNamesCompleted;

        public event GetPropertyChoiceListCompletedEventHandler GetPropertyChoiceListCompleted;

        public event GetUserColleaguesCompletedEventHandler GetUserColleaguesCompleted;

        public event GetUserLinksCompletedEventHandler GetUserLinksCompleted;

        public event GetUserMembershipsCompletedEventHandler GetUserMembershipsCompleted;

        public event GetUserOrganizationsCompletedEventHandler GetUserOrganizationsCompleted;

        public event GetUserPinnedLinksCompletedEventHandler GetUserPinnedLinksCompleted;

        public event GetUserProfileByGuidCompletedEventHandler GetUserProfileByGuidCompleted;

        public event GetUserProfileByIndexCompletedEventHandler GetUserProfileByIndexCompleted;

        public event GetUserProfileByNameCompletedEventHandler GetUserProfileByNameCompleted;

        public event GetUserProfileCountCompletedEventHandler GetUserProfileCountCompleted;

        public event GetUserProfileSchemaCompletedEventHandler GetUserProfileSchemaCompleted;

        public event GetUserPropertyByAccountNameCompletedEventHandler GetUserPropertyByAccountNameCompleted;

        public event ModifyUserPropertyByAccountNameCompletedEventHandler ModifyUserPropertyByAccountNameCompleted;

        public event RemoveAllColleaguesCompletedEventHandler RemoveAllColleaguesCompleted;

        public event RemoveAllLinksCompletedEventHandler RemoveAllLinksCompleted;

        public event RemoveAllMembershipsCompletedEventHandler RemoveAllMembershipsCompleted;

        public event RemoveAllPinnedLinksCompletedEventHandler RemoveAllPinnedLinksCompleted;

        public event RemoveColleagueCompletedEventHandler RemoveColleagueCompleted;

        public event RemoveLeaderCompletedEventHandler RemoveLeaderCompleted;

        public event RemoveLinkCompletedEventHandler RemoveLinkCompleted;

        public event RemoveMembershipCompletedEventHandler RemoveMembershipCompleted;

        public event RemovePinnedLinkCompletedEventHandler RemovePinnedLinkCompleted;

        public event UpdateColleaguePrivacyCompletedEventHandler UpdateColleaguePrivacyCompleted;

        public event UpdateLinkCompletedEventHandler UpdateLinkCompleted;

        public event UpdateMembershipPrivacyCompletedEventHandler UpdateMembershipPrivacyCompleted;

        public event UpdatePinnedLinkCompletedEventHandler UpdatePinnedLinkCompleted;
    }
}