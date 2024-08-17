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

namespace Metalogix.SharePoint.Adapters.NWS.SocialData
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.0.30319.17929")]
    [WebServiceBinding(Name = "SocialDataServiceSoap",
        Namespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService")]
    [XmlInclude(typeof(DeletedSocialDataDetail))]
    [XmlInclude(typeof(SocialDataDetail))]
    public class SocialDataService : SoapHttpClientProtocol
    {
        private SendOrPostCallback CountCommentsOfUserOperationCompleted;

        private SendOrPostCallback CountCommentsOnUrlOperationCompleted;

        private SendOrPostCallback CountCommentsOfUserOnUrlOperationCompleted;

        private SendOrPostCallback GetCommentsOfUserOperationCompleted;

        private SendOrPostCallback GetCommentsOnUrlOperationCompleted;

        private SendOrPostCallback GetCommentsOfUserOnUrlOperationCompleted;

        private SendOrPostCallback AddCommentOperationCompleted;

        private SendOrPostCallback UpdateCommentOperationCompleted;

        private SendOrPostCallback DeleteCommentOperationCompleted;

        private SendOrPostCallback CountRatingsOnUrlOperationCompleted;

        private SendOrPostCallback GetRatingsOfUserOperationCompleted;

        private SendOrPostCallback GetRatingsOnUrlOperationCompleted;

        private SendOrPostCallback GetRatingOnUrlOperationCompleted;

        private SendOrPostCallback GetRatingOfUserOnUrlOperationCompleted;

        private SendOrPostCallback SetRatingOperationCompleted;

        private SendOrPostCallback DeleteRatingOperationCompleted;

        private SendOrPostCallback GetRatingAverageOnUrlOperationCompleted;

        private SendOrPostCallback PropagateRatingOperationCompleted;

        private SendOrPostCallback CountTagsOfUserOperationCompleted;

        private SendOrPostCallback GetTagTermsOperationCompleted;

        private SendOrPostCallback GetTagTermsOfUserOperationCompleted;

        private SendOrPostCallback GetTagTermsOnUrlOperationCompleted;

        private SendOrPostCallback GetAllTagTermsOperationCompleted;

        private SendOrPostCallback GetAllTagTermsForUrlFolderOperationCompleted;

        private SendOrPostCallback GetTagsOperationCompleted;

        private SendOrPostCallback GetTagsOfUserOperationCompleted;

        private SendOrPostCallback GetTagUrlsOperationCompleted;

        private SendOrPostCallback GetTagUrlsByKeywordOperationCompleted;

        private SendOrPostCallback GetTagUrlsOfUserOperationCompleted;

        private SendOrPostCallback GetTagUrlsOfUserByKeywordOperationCompleted;

        private SendOrPostCallback GetAllTagUrlsOperationCompleted;

        private SendOrPostCallback GetAllTagUrlsByKeywordOperationCompleted;

        private SendOrPostCallback AddTagOperationCompleted;

        private SendOrPostCallback AddTagByKeywordOperationCompleted;

        private SendOrPostCallback DeleteTagOperationCompleted;

        private SendOrPostCallback DeleteTagByKeywordOperationCompleted;

        private SendOrPostCallback DeleteTagsOperationCompleted;

        private SendOrPostCallback GetSocialDataForIncrementalReplicationOperationCompleted;

        private SendOrPostCallback GetSocialDataForFullReplicationOperationCompleted;

        private SendOrPostCallback ReplicateIncrementalSocialDataOperationCompleted;

        private SendOrPostCallback ReplicateFullSocialDataOperationCompleted;

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

        public SocialDataService()
        {
            this.Url = Settings.Default.Metalogix_SharePoint_Adapters_NWS_SocialData_SocialDataService;
            if (!this.IsLocalFileSystemWebService(this.Url))
            {
                this.useDefaultCredentialsSetExplicitly = true;
                return;
            }

            this.UseDefaultCredentials = true;
            this.useDefaultCredentialsSetExplicitly = false;
        }

        [SoapDocumentMethod("http://microsoft.com/webservices/SharePointPortalServer/SocialDataService/AddComment",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public SocialCommentDetail AddComment(string url, string comment,
            [XmlElement(IsNullable = true)] bool? isHighPriority, string title)
        {
            object[] objArray = new object[] { url, comment, isHighPriority, title };
            return (SocialCommentDetail)base.Invoke("AddComment", objArray)[0];
        }

        public void AddCommentAsync(string url, string comment, bool? isHighPriority, string title)
        {
            this.AddCommentAsync(url, comment, isHighPriority, title, null);
        }

        public void AddCommentAsync(string url, string comment, bool? isHighPriority, string title, object userState)
        {
            if (this.AddCommentOperationCompleted == null)
            {
                this.AddCommentOperationCompleted = new SendOrPostCallback(this.OnAddCommentOperationCompleted);
            }

            object[] objArray = new object[] { url, comment, isHighPriority, title };
            base.InvokeAsync("AddComment", objArray, this.AddCommentOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://microsoft.com/webservices/SharePointPortalServer/SocialDataService/AddTag",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public SocialTagDetail AddTag(string url, Guid termID, string title,
            [XmlElement(IsNullable = true)] bool? isPrivate)
        {
            object[] objArray = new object[] { url, termID, title, isPrivate };
            return (SocialTagDetail)base.Invoke("AddTag", objArray)[0];
        }

        public void AddTagAsync(string url, Guid termID, string title, bool? isPrivate)
        {
            this.AddTagAsync(url, termID, title, isPrivate, null);
        }

        public void AddTagAsync(string url, Guid termID, string title, bool? isPrivate, object userState)
        {
            if (this.AddTagOperationCompleted == null)
            {
                this.AddTagOperationCompleted = new SendOrPostCallback(this.OnAddTagOperationCompleted);
            }

            object[] objArray = new object[] { url, termID, title, isPrivate };
            base.InvokeAsync("AddTag", objArray, this.AddTagOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://microsoft.com/webservices/SharePointPortalServer/SocialDataService/AddTagByKeyword",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public SocialTagDetail AddTagByKeyword(string url, string keyword, string title,
            [XmlElement(IsNullable = true)] bool? isPrivate)
        {
            object[] objArray = new object[] { url, keyword, title, isPrivate };
            return (SocialTagDetail)base.Invoke("AddTagByKeyword", objArray)[0];
        }

        public void AddTagByKeywordAsync(string url, string keyword, string title, bool? isPrivate)
        {
            this.AddTagByKeywordAsync(url, keyword, title, isPrivate, null);
        }

        public void AddTagByKeywordAsync(string url, string keyword, string title, bool? isPrivate, object userState)
        {
            if (this.AddTagByKeywordOperationCompleted == null)
            {
                this.AddTagByKeywordOperationCompleted =
                    new SendOrPostCallback(this.OnAddTagByKeywordOperationCompleted);
            }

            object[] objArray = new object[] { url, keyword, title, isPrivate };
            base.InvokeAsync("AddTagByKeyword", objArray, this.AddTagByKeywordOperationCompleted, userState);
        }

        public new void CancelAsync(object userState)
        {
            base.CancelAsync(userState);
        }

        [SoapDocumentMethod(
            "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService/CountCommentsOfUser",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public int CountCommentsOfUser(string userAccountName)
        {
            object[] objArray = new object[] { userAccountName };
            return (int)base.Invoke("CountCommentsOfUser", objArray)[0];
        }

        public void CountCommentsOfUserAsync(string userAccountName)
        {
            this.CountCommentsOfUserAsync(userAccountName, null);
        }

        public void CountCommentsOfUserAsync(string userAccountName, object userState)
        {
            if (this.CountCommentsOfUserOperationCompleted == null)
            {
                this.CountCommentsOfUserOperationCompleted =
                    new SendOrPostCallback(this.OnCountCommentsOfUserOperationCompleted);
            }

            object[] objArray = new object[] { userAccountName };
            base.InvokeAsync("CountCommentsOfUser", objArray, this.CountCommentsOfUserOperationCompleted, userState);
        }

        [SoapDocumentMethod(
            "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService/CountCommentsOfUserOnUrl",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public int CountCommentsOfUserOnUrl(string userAccountName, string url)
        {
            object[] objArray = new object[] { userAccountName, url };
            return (int)base.Invoke("CountCommentsOfUserOnUrl", objArray)[0];
        }

        public void CountCommentsOfUserOnUrlAsync(string userAccountName, string url)
        {
            this.CountCommentsOfUserOnUrlAsync(userAccountName, url, null);
        }

        public void CountCommentsOfUserOnUrlAsync(string userAccountName, string url, object userState)
        {
            if (this.CountCommentsOfUserOnUrlOperationCompleted == null)
            {
                this.CountCommentsOfUserOnUrlOperationCompleted =
                    new SendOrPostCallback(this.OnCountCommentsOfUserOnUrlOperationCompleted);
            }

            object[] objArray = new object[] { userAccountName, url };
            base.InvokeAsync("CountCommentsOfUserOnUrl", objArray, this.CountCommentsOfUserOnUrlOperationCompleted,
                userState);
        }

        [SoapDocumentMethod(
            "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService/CountCommentsOnUrl",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public int CountCommentsOnUrl(string url)
        {
            object[] objArray = new object[] { url };
            return (int)base.Invoke("CountCommentsOnUrl", objArray)[0];
        }

        public void CountCommentsOnUrlAsync(string url)
        {
            this.CountCommentsOnUrlAsync(url, null);
        }

        public void CountCommentsOnUrlAsync(string url, object userState)
        {
            if (this.CountCommentsOnUrlOperationCompleted == null)
            {
                this.CountCommentsOnUrlOperationCompleted =
                    new SendOrPostCallback(this.OnCountCommentsOnUrlOperationCompleted);
            }

            object[] objArray = new object[] { url };
            base.InvokeAsync("CountCommentsOnUrl", objArray, this.CountCommentsOnUrlOperationCompleted, userState);
        }

        [SoapDocumentMethod(
            "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService/CountRatingsOnUrl",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public int CountRatingsOnUrl(string url)
        {
            object[] objArray = new object[] { url };
            return (int)base.Invoke("CountRatingsOnUrl", objArray)[0];
        }

        public void CountRatingsOnUrlAsync(string url)
        {
            this.CountRatingsOnUrlAsync(url, null);
        }

        public void CountRatingsOnUrlAsync(string url, object userState)
        {
            if (this.CountRatingsOnUrlOperationCompleted == null)
            {
                this.CountRatingsOnUrlOperationCompleted =
                    new SendOrPostCallback(this.OnCountRatingsOnUrlOperationCompleted);
            }

            object[] objArray = new object[] { url };
            base.InvokeAsync("CountRatingsOnUrl", objArray, this.CountRatingsOnUrlOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://microsoft.com/webservices/SharePointPortalServer/SocialDataService/CountTagsOfUser",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public int CountTagsOfUser(string userAccountName)
        {
            object[] objArray = new object[] { userAccountName };
            return (int)base.Invoke("CountTagsOfUser", objArray)[0];
        }

        public void CountTagsOfUserAsync(string userAccountName)
        {
            this.CountTagsOfUserAsync(userAccountName, null);
        }

        public void CountTagsOfUserAsync(string userAccountName, object userState)
        {
            if (this.CountTagsOfUserOperationCompleted == null)
            {
                this.CountTagsOfUserOperationCompleted =
                    new SendOrPostCallback(this.OnCountTagsOfUserOperationCompleted);
            }

            object[] objArray = new object[] { userAccountName };
            base.InvokeAsync("CountTagsOfUser", objArray, this.CountTagsOfUserOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://microsoft.com/webservices/SharePointPortalServer/SocialDataService/DeleteComment",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public void DeleteComment(string url, DateTime lastModifiedTime)
        {
            object[] objArray = new object[] { url, lastModifiedTime };
            base.Invoke("DeleteComment", objArray);
        }

        public void DeleteCommentAsync(string url, DateTime lastModifiedTime)
        {
            this.DeleteCommentAsync(url, lastModifiedTime, null);
        }

        public void DeleteCommentAsync(string url, DateTime lastModifiedTime, object userState)
        {
            if (this.DeleteCommentOperationCompleted == null)
            {
                this.DeleteCommentOperationCompleted = new SendOrPostCallback(this.OnDeleteCommentOperationCompleted);
            }

            object[] objArray = new object[] { url, lastModifiedTime };
            base.InvokeAsync("DeleteComment", objArray, this.DeleteCommentOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://microsoft.com/webservices/SharePointPortalServer/SocialDataService/DeleteRating",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public void DeleteRating(string url)
        {
            object[] objArray = new object[] { url };
            base.Invoke("DeleteRating", objArray);
        }

        public void DeleteRatingAsync(string url)
        {
            this.DeleteRatingAsync(url, null);
        }

        public void DeleteRatingAsync(string url, object userState)
        {
            if (this.DeleteRatingOperationCompleted == null)
            {
                this.DeleteRatingOperationCompleted = new SendOrPostCallback(this.OnDeleteRatingOperationCompleted);
            }

            object[] objArray = new object[] { url };
            base.InvokeAsync("DeleteRating", objArray, this.DeleteRatingOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://microsoft.com/webservices/SharePointPortalServer/SocialDataService/DeleteTag",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public void DeleteTag(string url, Guid termID)
        {
            object[] objArray = new object[] { url, termID };
            base.Invoke("DeleteTag", objArray);
        }

        public void DeleteTagAsync(string url, Guid termID)
        {
            this.DeleteTagAsync(url, termID, null);
        }

        public void DeleteTagAsync(string url, Guid termID, object userState)
        {
            if (this.DeleteTagOperationCompleted == null)
            {
                this.DeleteTagOperationCompleted = new SendOrPostCallback(this.OnDeleteTagOperationCompleted);
            }

            object[] objArray = new object[] { url, termID };
            base.InvokeAsync("DeleteTag", objArray, this.DeleteTagOperationCompleted, userState);
        }

        [SoapDocumentMethod(
            "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService/DeleteTagByKeyword",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public void DeleteTagByKeyword(string url, string keyword)
        {
            object[] objArray = new object[] { url, keyword };
            base.Invoke("DeleteTagByKeyword", objArray);
        }

        public void DeleteTagByKeywordAsync(string url, string keyword)
        {
            this.DeleteTagByKeywordAsync(url, keyword, null);
        }

        public void DeleteTagByKeywordAsync(string url, string keyword, object userState)
        {
            if (this.DeleteTagByKeywordOperationCompleted == null)
            {
                this.DeleteTagByKeywordOperationCompleted =
                    new SendOrPostCallback(this.OnDeleteTagByKeywordOperationCompleted);
            }

            object[] objArray = new object[] { url, keyword };
            base.InvokeAsync("DeleteTagByKeyword", objArray, this.DeleteTagByKeywordOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://microsoft.com/webservices/SharePointPortalServer/SocialDataService/DeleteTags",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public void DeleteTags(string url)
        {
            object[] objArray = new object[] { url };
            base.Invoke("DeleteTags", objArray);
        }

        public void DeleteTagsAsync(string url)
        {
            this.DeleteTagsAsync(url, null);
        }

        public void DeleteTagsAsync(string url, object userState)
        {
            if (this.DeleteTagsOperationCompleted == null)
            {
                this.DeleteTagsOperationCompleted = new SendOrPostCallback(this.OnDeleteTagsOperationCompleted);
            }

            object[] objArray = new object[] { url };
            base.InvokeAsync("DeleteTags", objArray, this.DeleteTagsOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://microsoft.com/webservices/SharePointPortalServer/SocialDataService/GetAllTagTerms",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public SocialTermDetail[] GetAllTagTerms([XmlElement(IsNullable = true)] int? maximumItemsToReturn)
        {
            object[] objArray = new object[] { maximumItemsToReturn };
            return (SocialTermDetail[])base.Invoke("GetAllTagTerms", objArray)[0];
        }

        public void GetAllTagTermsAsync(int? maximumItemsToReturn)
        {
            this.GetAllTagTermsAsync(maximumItemsToReturn, null);
        }

        public void GetAllTagTermsAsync(int? maximumItemsToReturn, object userState)
        {
            if (this.GetAllTagTermsOperationCompleted == null)
            {
                this.GetAllTagTermsOperationCompleted = new SendOrPostCallback(this.OnGetAllTagTermsOperationCompleted);
            }

            object[] objArray = new object[] { maximumItemsToReturn };
            base.InvokeAsync("GetAllTagTerms", objArray, this.GetAllTagTermsOperationCompleted, userState);
        }

        [SoapDocumentMethod(
            "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService/GetAllTagTermsForUrlFolder",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public SocialTermDetail[] GetAllTagTermsForUrlFolder(string urlFolder, int maximumItemsToReturn)
        {
            object[] objArray = new object[] { urlFolder, maximumItemsToReturn };
            return (SocialTermDetail[])base.Invoke("GetAllTagTermsForUrlFolder", objArray)[0];
        }

        public void GetAllTagTermsForUrlFolderAsync(string urlFolder, int maximumItemsToReturn)
        {
            this.GetAllTagTermsForUrlFolderAsync(urlFolder, maximumItemsToReturn, null);
        }

        public void GetAllTagTermsForUrlFolderAsync(string urlFolder, int maximumItemsToReturn, object userState)
        {
            if (this.GetAllTagTermsForUrlFolderOperationCompleted == null)
            {
                this.GetAllTagTermsForUrlFolderOperationCompleted =
                    new SendOrPostCallback(this.OnGetAllTagTermsForUrlFolderOperationCompleted);
            }

            object[] objArray = new object[] { urlFolder, maximumItemsToReturn };
            base.InvokeAsync("GetAllTagTermsForUrlFolder", objArray, this.GetAllTagTermsForUrlFolderOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://microsoft.com/webservices/SharePointPortalServer/SocialDataService/GetAllTagUrls",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public SocialUrlDetail[] GetAllTagUrls(Guid termID)
        {
            object[] objArray = new object[] { termID };
            return (SocialUrlDetail[])base.Invoke("GetAllTagUrls", objArray)[0];
        }

        public void GetAllTagUrlsAsync(Guid termID)
        {
            this.GetAllTagUrlsAsync(termID, null);
        }

        public void GetAllTagUrlsAsync(Guid termID, object userState)
        {
            if (this.GetAllTagUrlsOperationCompleted == null)
            {
                this.GetAllTagUrlsOperationCompleted = new SendOrPostCallback(this.OnGetAllTagUrlsOperationCompleted);
            }

            object[] objArray = new object[] { termID };
            base.InvokeAsync("GetAllTagUrls", objArray, this.GetAllTagUrlsOperationCompleted, userState);
        }

        [SoapDocumentMethod(
            "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService/GetAllTagUrlsByKeyword",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public SocialUrlDetail[] GetAllTagUrlsByKeyword(string keyword)
        {
            object[] objArray = new object[] { keyword };
            return (SocialUrlDetail[])base.Invoke("GetAllTagUrlsByKeyword", objArray)[0];
        }

        public void GetAllTagUrlsByKeywordAsync(string keyword)
        {
            this.GetAllTagUrlsByKeywordAsync(keyword, null);
        }

        public void GetAllTagUrlsByKeywordAsync(string keyword, object userState)
        {
            if (this.GetAllTagUrlsByKeywordOperationCompleted == null)
            {
                this.GetAllTagUrlsByKeywordOperationCompleted =
                    new SendOrPostCallback(this.OnGetAllTagUrlsByKeywordOperationCompleted);
            }

            object[] objArray = new object[] { keyword };
            base.InvokeAsync("GetAllTagUrlsByKeyword", objArray, this.GetAllTagUrlsByKeywordOperationCompleted,
                userState);
        }

        [SoapDocumentMethod(
            "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService/GetCommentsOfUser",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public SocialCommentDetail[] GetCommentsOfUser(string userAccountName,
            [XmlElement(IsNullable = true)] int? maximumItemsToReturn, [XmlElement(IsNullable = true)] int? startIndex)
        {
            object[] objArray = new object[] { userAccountName, maximumItemsToReturn, startIndex };
            return (SocialCommentDetail[])base.Invoke("GetCommentsOfUser", objArray)[0];
        }

        public void GetCommentsOfUserAsync(string userAccountName, int? maximumItemsToReturn, int? startIndex)
        {
            this.GetCommentsOfUserAsync(userAccountName, maximumItemsToReturn, startIndex, null);
        }

        public void GetCommentsOfUserAsync(string userAccountName, int? maximumItemsToReturn, int? startIndex,
            object userState)
        {
            if (this.GetCommentsOfUserOperationCompleted == null)
            {
                this.GetCommentsOfUserOperationCompleted =
                    new SendOrPostCallback(this.OnGetCommentsOfUserOperationCompleted);
            }

            object[] objArray = new object[] { userAccountName, maximumItemsToReturn, startIndex };
            base.InvokeAsync("GetCommentsOfUser", objArray, this.GetCommentsOfUserOperationCompleted, userState);
        }

        [SoapDocumentMethod(
            "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService/GetCommentsOfUserOnUrl",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public SocialCommentDetail[] GetCommentsOfUserOnUrl(string userAccountName, string url)
        {
            object[] objArray = new object[] { userAccountName, url };
            return (SocialCommentDetail[])base.Invoke("GetCommentsOfUserOnUrl", objArray)[0];
        }

        public void GetCommentsOfUserOnUrlAsync(string userAccountName, string url)
        {
            this.GetCommentsOfUserOnUrlAsync(userAccountName, url, null);
        }

        public void GetCommentsOfUserOnUrlAsync(string userAccountName, string url, object userState)
        {
            if (this.GetCommentsOfUserOnUrlOperationCompleted == null)
            {
                this.GetCommentsOfUserOnUrlOperationCompleted =
                    new SendOrPostCallback(this.OnGetCommentsOfUserOnUrlOperationCompleted);
            }

            object[] objArray = new object[] { userAccountName, url };
            base.InvokeAsync("GetCommentsOfUserOnUrl", objArray, this.GetCommentsOfUserOnUrlOperationCompleted,
                userState);
        }

        [SoapDocumentMethod(
            "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService/GetCommentsOnUrl",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public SocialCommentDetail[] GetCommentsOnUrl(string url,
            [XmlElement(IsNullable = true)] int? maximumItemsToReturn, [XmlElement(IsNullable = true)] int? startIndex,
            [XmlElement(IsNullable = true)] DateTime? excludeItemsTime)
        {
            object[] objArray = new object[] { url, maximumItemsToReturn, startIndex, excludeItemsTime };
            return (SocialCommentDetail[])base.Invoke("GetCommentsOnUrl", objArray)[0];
        }

        public void GetCommentsOnUrlAsync(string url, int? maximumItemsToReturn, int? startIndex,
            DateTime? excludeItemsTime)
        {
            this.GetCommentsOnUrlAsync(url, maximumItemsToReturn, startIndex, excludeItemsTime, null);
        }

        public void GetCommentsOnUrlAsync(string url, int? maximumItemsToReturn, int? startIndex,
            DateTime? excludeItemsTime, object userState)
        {
            if (this.GetCommentsOnUrlOperationCompleted == null)
            {
                this.GetCommentsOnUrlOperationCompleted =
                    new SendOrPostCallback(this.OnGetCommentsOnUrlOperationCompleted);
            }

            object[] objArray = new object[] { url, maximumItemsToReturn, startIndex, excludeItemsTime };
            base.InvokeAsync("GetCommentsOnUrl", objArray, this.GetCommentsOnUrlOperationCompleted, userState);
        }

        [SoapDocumentMethod(
            "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService/GetRatingAverageOnUrl",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public SocialRatingAverageDetail GetRatingAverageOnUrl(string url)
        {
            object[] objArray = new object[] { url };
            return (SocialRatingAverageDetail)base.Invoke("GetRatingAverageOnUrl", objArray)[0];
        }

        public void GetRatingAverageOnUrlAsync(string url)
        {
            this.GetRatingAverageOnUrlAsync(url, null);
        }

        public void GetRatingAverageOnUrlAsync(string url, object userState)
        {
            if (this.GetRatingAverageOnUrlOperationCompleted == null)
            {
                this.GetRatingAverageOnUrlOperationCompleted =
                    new SendOrPostCallback(this.OnGetRatingAverageOnUrlOperationCompleted);
            }

            object[] objArray = new object[] { url };
            base.InvokeAsync("GetRatingAverageOnUrl", objArray, this.GetRatingAverageOnUrlOperationCompleted,
                userState);
        }

        [SoapDocumentMethod(
            "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService/GetRatingOfUserOnUrl",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public SocialRatingDetail GetRatingOfUserOnUrl(string userAccountName, string url)
        {
            object[] objArray = new object[] { userAccountName, url };
            return (SocialRatingDetail)base.Invoke("GetRatingOfUserOnUrl", objArray)[0];
        }

        public void GetRatingOfUserOnUrlAsync(string userAccountName, string url)
        {
            this.GetRatingOfUserOnUrlAsync(userAccountName, url, null);
        }

        public void GetRatingOfUserOnUrlAsync(string userAccountName, string url, object userState)
        {
            if (this.GetRatingOfUserOnUrlOperationCompleted == null)
            {
                this.GetRatingOfUserOnUrlOperationCompleted =
                    new SendOrPostCallback(this.OnGetRatingOfUserOnUrlOperationCompleted);
            }

            object[] objArray = new object[] { userAccountName, url };
            base.InvokeAsync("GetRatingOfUserOnUrl", objArray, this.GetRatingOfUserOnUrlOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://microsoft.com/webservices/SharePointPortalServer/SocialDataService/GetRatingOnUrl",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public SocialRatingDetail GetRatingOnUrl(string url)
        {
            object[] objArray = new object[] { url };
            return (SocialRatingDetail)base.Invoke("GetRatingOnUrl", objArray)[0];
        }

        public void GetRatingOnUrlAsync(string url)
        {
            this.GetRatingOnUrlAsync(url, null);
        }

        public void GetRatingOnUrlAsync(string url, object userState)
        {
            if (this.GetRatingOnUrlOperationCompleted == null)
            {
                this.GetRatingOnUrlOperationCompleted = new SendOrPostCallback(this.OnGetRatingOnUrlOperationCompleted);
            }

            object[] objArray = new object[] { url };
            base.InvokeAsync("GetRatingOnUrl", objArray, this.GetRatingOnUrlOperationCompleted, userState);
        }

        [SoapDocumentMethod(
            "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService/GetRatingsOfUser",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public SocialRatingDetail[] GetRatingsOfUser(string userAccountName)
        {
            object[] objArray = new object[] { userAccountName };
            return (SocialRatingDetail[])base.Invoke("GetRatingsOfUser", objArray)[0];
        }

        public void GetRatingsOfUserAsync(string userAccountName)
        {
            this.GetRatingsOfUserAsync(userAccountName, null);
        }

        public void GetRatingsOfUserAsync(string userAccountName, object userState)
        {
            if (this.GetRatingsOfUserOperationCompleted == null)
            {
                this.GetRatingsOfUserOperationCompleted =
                    new SendOrPostCallback(this.OnGetRatingsOfUserOperationCompleted);
            }

            object[] objArray = new object[] { userAccountName };
            base.InvokeAsync("GetRatingsOfUser", objArray, this.GetRatingsOfUserOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://microsoft.com/webservices/SharePointPortalServer/SocialDataService/GetRatingsOnUrl",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public SocialRatingDetail[] GetRatingsOnUrl(string url)
        {
            object[] objArray = new object[] { url };
            return (SocialRatingDetail[])base.Invoke("GetRatingsOnUrl", objArray)[0];
        }

        public void GetRatingsOnUrlAsync(string url)
        {
            this.GetRatingsOnUrlAsync(url, null);
        }

        public void GetRatingsOnUrlAsync(string url, object userState)
        {
            if (this.GetRatingsOnUrlOperationCompleted == null)
            {
                this.GetRatingsOnUrlOperationCompleted =
                    new SendOrPostCallback(this.OnGetRatingsOnUrlOperationCompleted);
            }

            object[] objArray = new object[] { url };
            base.InvokeAsync("GetRatingsOnUrl", objArray, this.GetRatingsOnUrlOperationCompleted, userState);
        }

        [SoapDocumentMethod(
            "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService/GetSocialDataForFullReplication",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public SocialReplicationData GetSocialDataForFullReplication(string userAccountName)
        {
            object[] objArray = new object[] { userAccountName };
            return (SocialReplicationData)base.Invoke("GetSocialDataForFullReplication", objArray)[0];
        }

        public void GetSocialDataForFullReplicationAsync(string userAccountName)
        {
            this.GetSocialDataForFullReplicationAsync(userAccountName, null);
        }

        public void GetSocialDataForFullReplicationAsync(string userAccountName, object userState)
        {
            if (this.GetSocialDataForFullReplicationOperationCompleted == null)
            {
                this.GetSocialDataForFullReplicationOperationCompleted =
                    new SendOrPostCallback(this.OnGetSocialDataForFullReplicationOperationCompleted);
            }

            object[] objArray = new object[] { userAccountName };
            base.InvokeAsync("GetSocialDataForFullReplication", objArray,
                this.GetSocialDataForFullReplicationOperationCompleted, userState);
        }

        [SoapDocumentMethod(
            "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService/GetSocialDataForIncrementalReplication",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public SocialReplicationData GetSocialDataForIncrementalReplication(DateTime startTime, DateTime endTime)
        {
            object[] objArray = new object[] { startTime, endTime };
            return (SocialReplicationData)base.Invoke("GetSocialDataForIncrementalReplication", objArray)[0];
        }

        public void GetSocialDataForIncrementalReplicationAsync(DateTime startTime, DateTime endTime)
        {
            this.GetSocialDataForIncrementalReplicationAsync(startTime, endTime, null);
        }

        public void GetSocialDataForIncrementalReplicationAsync(DateTime startTime, DateTime endTime, object userState)
        {
            if (this.GetSocialDataForIncrementalReplicationOperationCompleted == null)
            {
                this.GetSocialDataForIncrementalReplicationOperationCompleted =
                    new SendOrPostCallback(this.OnGetSocialDataForIncrementalReplicationOperationCompleted);
            }

            object[] objArray = new object[] { startTime, endTime };
            base.InvokeAsync("GetSocialDataForIncrementalReplication", objArray,
                this.GetSocialDataForIncrementalReplicationOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://microsoft.com/webservices/SharePointPortalServer/SocialDataService/GetTags",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public SocialTagDetail[] GetTags(string url)
        {
            object[] objArray = new object[] { url };
            return (SocialTagDetail[])base.Invoke("GetTags", objArray)[0];
        }

        public void GetTagsAsync(string url)
        {
            this.GetTagsAsync(url, null);
        }

        public void GetTagsAsync(string url, object userState)
        {
            if (this.GetTagsOperationCompleted == null)
            {
                this.GetTagsOperationCompleted = new SendOrPostCallback(this.OnGetTagsOperationCompleted);
            }

            object[] objArray = new object[] { url };
            base.InvokeAsync("GetTags", objArray, this.GetTagsOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://microsoft.com/webservices/SharePointPortalServer/SocialDataService/GetTagsOfUser",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public SocialTagDetail[] GetTagsOfUser(string userAccountName,
            [XmlElement(IsNullable = true)] int? maximumItemsToReturn, [XmlElement(IsNullable = true)] int? startIndex)
        {
            object[] objArray = new object[] { userAccountName, maximumItemsToReturn, startIndex };
            return (SocialTagDetail[])base.Invoke("GetTagsOfUser", objArray)[0];
        }

        public void GetTagsOfUserAsync(string userAccountName, int? maximumItemsToReturn, int? startIndex)
        {
            this.GetTagsOfUserAsync(userAccountName, maximumItemsToReturn, startIndex, null);
        }

        public void GetTagsOfUserAsync(string userAccountName, int? maximumItemsToReturn, int? startIndex,
            object userState)
        {
            if (this.GetTagsOfUserOperationCompleted == null)
            {
                this.GetTagsOfUserOperationCompleted = new SendOrPostCallback(this.OnGetTagsOfUserOperationCompleted);
            }

            object[] objArray = new object[] { userAccountName, maximumItemsToReturn, startIndex };
            base.InvokeAsync("GetTagsOfUser", objArray, this.GetTagsOfUserOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://microsoft.com/webservices/SharePointPortalServer/SocialDataService/GetTagTerms",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public SocialTermDetail[] GetTagTerms([XmlElement(IsNullable = true)] int? maximumItemsToReturn)
        {
            object[] objArray = new object[] { maximumItemsToReturn };
            return (SocialTermDetail[])base.Invoke("GetTagTerms", objArray)[0];
        }

        public void GetTagTermsAsync(int? maximumItemsToReturn)
        {
            this.GetTagTermsAsync(maximumItemsToReturn, null);
        }

        public void GetTagTermsAsync(int? maximumItemsToReturn, object userState)
        {
            if (this.GetTagTermsOperationCompleted == null)
            {
                this.GetTagTermsOperationCompleted = new SendOrPostCallback(this.OnGetTagTermsOperationCompleted);
            }

            object[] objArray = new object[] { maximumItemsToReturn };
            base.InvokeAsync("GetTagTerms", objArray, this.GetTagTermsOperationCompleted, userState);
        }

        [SoapDocumentMethod(
            "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService/GetTagTermsOfUser",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public SocialTermDetail[] GetTagTermsOfUser(string userAccountName,
            [XmlElement(IsNullable = true)] int? maximumItemsToReturn)
        {
            object[] objArray = new object[] { userAccountName, maximumItemsToReturn };
            return (SocialTermDetail[])base.Invoke("GetTagTermsOfUser", objArray)[0];
        }

        public void GetTagTermsOfUserAsync(string userAccountName, int? maximumItemsToReturn)
        {
            this.GetTagTermsOfUserAsync(userAccountName, maximumItemsToReturn, null);
        }

        public void GetTagTermsOfUserAsync(string userAccountName, int? maximumItemsToReturn, object userState)
        {
            if (this.GetTagTermsOfUserOperationCompleted == null)
            {
                this.GetTagTermsOfUserOperationCompleted =
                    new SendOrPostCallback(this.OnGetTagTermsOfUserOperationCompleted);
            }

            object[] objArray = new object[] { userAccountName, maximumItemsToReturn };
            base.InvokeAsync("GetTagTermsOfUser", objArray, this.GetTagTermsOfUserOperationCompleted, userState);
        }

        [SoapDocumentMethod(
            "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService/GetTagTermsOnUrl",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public SocialTermDetail[] GetTagTermsOnUrl(string url,
            [XmlElement(IsNullable = true)] int? maximumItemsToReturn)
        {
            object[] objArray = new object[] { url, maximumItemsToReturn };
            return (SocialTermDetail[])base.Invoke("GetTagTermsOnUrl", objArray)[0];
        }

        public void GetTagTermsOnUrlAsync(string url, int? maximumItemsToReturn)
        {
            this.GetTagTermsOnUrlAsync(url, maximumItemsToReturn, null);
        }

        public void GetTagTermsOnUrlAsync(string url, int? maximumItemsToReturn, object userState)
        {
            if (this.GetTagTermsOnUrlOperationCompleted == null)
            {
                this.GetTagTermsOnUrlOperationCompleted =
                    new SendOrPostCallback(this.OnGetTagTermsOnUrlOperationCompleted);
            }

            object[] objArray = new object[] { url, maximumItemsToReturn };
            base.InvokeAsync("GetTagTermsOnUrl", objArray, this.GetTagTermsOnUrlOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://microsoft.com/webservices/SharePointPortalServer/SocialDataService/GetTagUrls",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string[] GetTagUrls(Guid termID)
        {
            object[] objArray = new object[] { termID };
            return (string[])base.Invoke("GetTagUrls", objArray)[0];
        }

        public void GetTagUrlsAsync(Guid termID)
        {
            this.GetTagUrlsAsync(termID, null);
        }

        public void GetTagUrlsAsync(Guid termID, object userState)
        {
            if (this.GetTagUrlsOperationCompleted == null)
            {
                this.GetTagUrlsOperationCompleted = new SendOrPostCallback(this.OnGetTagUrlsOperationCompleted);
            }

            object[] objArray = new object[] { termID };
            base.InvokeAsync("GetTagUrls", objArray, this.GetTagUrlsOperationCompleted, userState);
        }

        [SoapDocumentMethod(
            "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService/GetTagUrlsByKeyword",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string[] GetTagUrlsByKeyword(string keyword)
        {
            object[] objArray = new object[] { keyword };
            return (string[])base.Invoke("GetTagUrlsByKeyword", objArray)[0];
        }

        public void GetTagUrlsByKeywordAsync(string keyword)
        {
            this.GetTagUrlsByKeywordAsync(keyword, null);
        }

        public void GetTagUrlsByKeywordAsync(string keyword, object userState)
        {
            if (this.GetTagUrlsByKeywordOperationCompleted == null)
            {
                this.GetTagUrlsByKeywordOperationCompleted =
                    new SendOrPostCallback(this.OnGetTagUrlsByKeywordOperationCompleted);
            }

            object[] objArray = new object[] { keyword };
            base.InvokeAsync("GetTagUrlsByKeyword", objArray, this.GetTagUrlsByKeywordOperationCompleted, userState);
        }

        [SoapDocumentMethod(
            "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService/GetTagUrlsOfUser",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string[] GetTagUrlsOfUser(Guid termID, string userAccountName)
        {
            object[] objArray = new object[] { termID, userAccountName };
            return (string[])base.Invoke("GetTagUrlsOfUser", objArray)[0];
        }

        public void GetTagUrlsOfUserAsync(Guid termID, string userAccountName)
        {
            this.GetTagUrlsOfUserAsync(termID, userAccountName, null);
        }

        public void GetTagUrlsOfUserAsync(Guid termID, string userAccountName, object userState)
        {
            if (this.GetTagUrlsOfUserOperationCompleted == null)
            {
                this.GetTagUrlsOfUserOperationCompleted =
                    new SendOrPostCallback(this.OnGetTagUrlsOfUserOperationCompleted);
            }

            object[] objArray = new object[] { termID, userAccountName };
            base.InvokeAsync("GetTagUrlsOfUser", objArray, this.GetTagUrlsOfUserOperationCompleted, userState);
        }

        [SoapDocumentMethod(
            "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService/GetTagUrlsOfUserByKeyword",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string[] GetTagUrlsOfUserByKeyword(string keyword, string userAccountName)
        {
            object[] objArray = new object[] { keyword, userAccountName };
            return (string[])base.Invoke("GetTagUrlsOfUserByKeyword", objArray)[0];
        }

        public void GetTagUrlsOfUserByKeywordAsync(string keyword, string userAccountName)
        {
            this.GetTagUrlsOfUserByKeywordAsync(keyword, userAccountName, null);
        }

        public void GetTagUrlsOfUserByKeywordAsync(string keyword, string userAccountName, object userState)
        {
            if (this.GetTagUrlsOfUserByKeywordOperationCompleted == null)
            {
                this.GetTagUrlsOfUserByKeywordOperationCompleted =
                    new SendOrPostCallback(this.OnGetTagUrlsOfUserByKeywordOperationCompleted);
            }

            object[] objArray = new object[] { keyword, userAccountName };
            base.InvokeAsync("GetTagUrlsOfUserByKeyword", objArray, this.GetTagUrlsOfUserByKeywordOperationCompleted,
                userState);
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

        private void OnAddCommentOperationCompleted(object arg)
        {
            if (this.AddCommentCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddCommentCompleted(this,
                    new AddCommentCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnAddTagByKeywordOperationCompleted(object arg)
        {
            if (this.AddTagByKeywordCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddTagByKeywordCompleted(this,
                    new AddTagByKeywordCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnAddTagOperationCompleted(object arg)
        {
            if (this.AddTagCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddTagCompleted(this,
                    new AddTagCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnCountCommentsOfUserOnUrlOperationCompleted(object arg)
        {
            if (this.CountCommentsOfUserOnUrlCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.CountCommentsOfUserOnUrlCompleted(this,
                    new CountCommentsOfUserOnUrlCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnCountCommentsOfUserOperationCompleted(object arg)
        {
            if (this.CountCommentsOfUserCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.CountCommentsOfUserCompleted(this,
                    new CountCommentsOfUserCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnCountCommentsOnUrlOperationCompleted(object arg)
        {
            if (this.CountCommentsOnUrlCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.CountCommentsOnUrlCompleted(this,
                    new CountCommentsOnUrlCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnCountRatingsOnUrlOperationCompleted(object arg)
        {
            if (this.CountRatingsOnUrlCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.CountRatingsOnUrlCompleted(this,
                    new CountRatingsOnUrlCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnCountTagsOfUserOperationCompleted(object arg)
        {
            if (this.CountTagsOfUserCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.CountTagsOfUserCompleted(this,
                    new CountTagsOfUserCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnDeleteCommentOperationCompleted(object arg)
        {
            if (this.DeleteCommentCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.DeleteCommentCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnDeleteRatingOperationCompleted(object arg)
        {
            if (this.DeleteRatingCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.DeleteRatingCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnDeleteTagByKeywordOperationCompleted(object arg)
        {
            if (this.DeleteTagByKeywordCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.DeleteTagByKeywordCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnDeleteTagOperationCompleted(object arg)
        {
            if (this.DeleteTagCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.DeleteTagCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnDeleteTagsOperationCompleted(object arg)
        {
            if (this.DeleteTagsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.DeleteTagsCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetAllTagTermsForUrlFolderOperationCompleted(object arg)
        {
            if (this.GetAllTagTermsForUrlFolderCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetAllTagTermsForUrlFolderCompleted(this,
                    new GetAllTagTermsForUrlFolderCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetAllTagTermsOperationCompleted(object arg)
        {
            if (this.GetAllTagTermsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetAllTagTermsCompleted(this,
                    new GetAllTagTermsCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetAllTagUrlsByKeywordOperationCompleted(object arg)
        {
            if (this.GetAllTagUrlsByKeywordCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetAllTagUrlsByKeywordCompleted(this,
                    new GetAllTagUrlsByKeywordCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetAllTagUrlsOperationCompleted(object arg)
        {
            if (this.GetAllTagUrlsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetAllTagUrlsCompleted(this,
                    new GetAllTagUrlsCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetCommentsOfUserOnUrlOperationCompleted(object arg)
        {
            if (this.GetCommentsOfUserOnUrlCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetCommentsOfUserOnUrlCompleted(this,
                    new GetCommentsOfUserOnUrlCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetCommentsOfUserOperationCompleted(object arg)
        {
            if (this.GetCommentsOfUserCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetCommentsOfUserCompleted(this,
                    new GetCommentsOfUserCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetCommentsOnUrlOperationCompleted(object arg)
        {
            if (this.GetCommentsOnUrlCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetCommentsOnUrlCompleted(this,
                    new GetCommentsOnUrlCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetRatingAverageOnUrlOperationCompleted(object arg)
        {
            if (this.GetRatingAverageOnUrlCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetRatingAverageOnUrlCompleted(this,
                    new GetRatingAverageOnUrlCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetRatingOfUserOnUrlOperationCompleted(object arg)
        {
            if (this.GetRatingOfUserOnUrlCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetRatingOfUserOnUrlCompleted(this,
                    new GetRatingOfUserOnUrlCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetRatingOnUrlOperationCompleted(object arg)
        {
            if (this.GetRatingOnUrlCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetRatingOnUrlCompleted(this,
                    new GetRatingOnUrlCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetRatingsOfUserOperationCompleted(object arg)
        {
            if (this.GetRatingsOfUserCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetRatingsOfUserCompleted(this,
                    new GetRatingsOfUserCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetRatingsOnUrlOperationCompleted(object arg)
        {
            if (this.GetRatingsOnUrlCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetRatingsOnUrlCompleted(this,
                    new GetRatingsOnUrlCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetSocialDataForFullReplicationOperationCompleted(object arg)
        {
            if (this.GetSocialDataForFullReplicationCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetSocialDataForFullReplicationCompleted(this,
                    new GetSocialDataForFullReplicationCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetSocialDataForIncrementalReplicationOperationCompleted(object arg)
        {
            if (this.GetSocialDataForIncrementalReplicationCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetSocialDataForIncrementalReplicationCompleted(this,
                    new GetSocialDataForIncrementalReplicationCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetTagsOfUserOperationCompleted(object arg)
        {
            if (this.GetTagsOfUserCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetTagsOfUserCompleted(this,
                    new GetTagsOfUserCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetTagsOperationCompleted(object arg)
        {
            if (this.GetTagsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetTagsCompleted(this,
                    new GetTagsCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetTagTermsOfUserOperationCompleted(object arg)
        {
            if (this.GetTagTermsOfUserCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetTagTermsOfUserCompleted(this,
                    new GetTagTermsOfUserCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetTagTermsOnUrlOperationCompleted(object arg)
        {
            if (this.GetTagTermsOnUrlCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetTagTermsOnUrlCompleted(this,
                    new GetTagTermsOnUrlCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetTagTermsOperationCompleted(object arg)
        {
            if (this.GetTagTermsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetTagTermsCompleted(this,
                    new GetTagTermsCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetTagUrlsByKeywordOperationCompleted(object arg)
        {
            if (this.GetTagUrlsByKeywordCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetTagUrlsByKeywordCompleted(this,
                    new GetTagUrlsByKeywordCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetTagUrlsOfUserByKeywordOperationCompleted(object arg)
        {
            if (this.GetTagUrlsOfUserByKeywordCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetTagUrlsOfUserByKeywordCompleted(this,
                    new GetTagUrlsOfUserByKeywordCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetTagUrlsOfUserOperationCompleted(object arg)
        {
            if (this.GetTagUrlsOfUserCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetTagUrlsOfUserCompleted(this,
                    new GetTagUrlsOfUserCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetTagUrlsOperationCompleted(object arg)
        {
            if (this.GetTagUrlsCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetTagUrlsCompleted(this,
                    new GetTagUrlsCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnPropagateRatingOperationCompleted(object arg)
        {
            if (this.PropagateRatingCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.PropagateRatingCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnReplicateFullSocialDataOperationCompleted(object arg)
        {
            if (this.ReplicateFullSocialDataCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.ReplicateFullSocialDataCompleted(this,
                    new ReplicateFullSocialDataCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnReplicateIncrementalSocialDataOperationCompleted(object arg)
        {
            if (this.ReplicateIncrementalSocialDataCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.ReplicateIncrementalSocialDataCompleted(this,
                    new ReplicateIncrementalSocialDataCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnSetRatingOperationCompleted(object arg)
        {
            if (this.SetRatingCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.SetRatingCompleted(this,
                    new SetRatingCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnUpdateCommentOperationCompleted(object arg)
        {
            if (this.UpdateCommentCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.UpdateCommentCompleted(this,
                    new UpdateCommentCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        [SoapDocumentMethod("http://microsoft.com/webservices/SharePointPortalServer/SocialDataService/PropagateRating",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public void PropagateRating(string url)
        {
            object[] objArray = new object[] { url };
            base.Invoke("PropagateRating", objArray);
        }

        public void PropagateRatingAsync(string url)
        {
            this.PropagateRatingAsync(url, null);
        }

        public void PropagateRatingAsync(string url, object userState)
        {
            if (this.PropagateRatingOperationCompleted == null)
            {
                this.PropagateRatingOperationCompleted =
                    new SendOrPostCallback(this.OnPropagateRatingOperationCompleted);
            }

            object[] objArray = new object[] { url };
            base.InvokeAsync("PropagateRating", objArray, this.PropagateRatingOperationCompleted, userState);
        }

        [SoapDocumentMethod(
            "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService/ReplicateFullSocialData",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public bool ReplicateFullSocialData(string userAccountName, SocialReplicationData changes)
        {
            object[] objArray = new object[] { userAccountName, changes };
            return (bool)base.Invoke("ReplicateFullSocialData", objArray)[0];
        }

        public void ReplicateFullSocialDataAsync(string userAccountName, SocialReplicationData changes)
        {
            this.ReplicateFullSocialDataAsync(userAccountName, changes, null);
        }

        public void ReplicateFullSocialDataAsync(string userAccountName, SocialReplicationData changes,
            object userState)
        {
            if (this.ReplicateFullSocialDataOperationCompleted == null)
            {
                this.ReplicateFullSocialDataOperationCompleted =
                    new SendOrPostCallback(this.OnReplicateFullSocialDataOperationCompleted);
            }

            object[] objArray = new object[] { userAccountName, changes };
            base.InvokeAsync("ReplicateFullSocialData", objArray, this.ReplicateFullSocialDataOperationCompleted,
                userState);
        }

        [SoapDocumentMethod(
            "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService/ReplicateIncrementalSocialData",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public bool ReplicateIncrementalSocialData(SocialReplicationData changes)
        {
            object[] objArray = new object[] { changes };
            return (bool)base.Invoke("ReplicateIncrementalSocialData", objArray)[0];
        }

        public void ReplicateIncrementalSocialDataAsync(SocialReplicationData changes)
        {
            this.ReplicateIncrementalSocialDataAsync(changes, null);
        }

        public void ReplicateIncrementalSocialDataAsync(SocialReplicationData changes, object userState)
        {
            if (this.ReplicateIncrementalSocialDataOperationCompleted == null)
            {
                this.ReplicateIncrementalSocialDataOperationCompleted =
                    new SendOrPostCallback(this.OnReplicateIncrementalSocialDataOperationCompleted);
            }

            object[] objArray = new object[] { changes };
            base.InvokeAsync("ReplicateIncrementalSocialData", objArray,
                this.ReplicateIncrementalSocialDataOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://microsoft.com/webservices/SharePointPortalServer/SocialDataService/SetRating",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public DateTime SetRating(string url, int rating, string title, FeedbackData analysisDataEntry)
        {
            object[] objArray = new object[] { url, rating, title, analysisDataEntry };
            return (DateTime)base.Invoke("SetRating", objArray)[0];
        }

        public void SetRatingAsync(string url, int rating, string title, FeedbackData analysisDataEntry)
        {
            this.SetRatingAsync(url, rating, title, analysisDataEntry, null);
        }

        public void SetRatingAsync(string url, int rating, string title, FeedbackData analysisDataEntry,
            object userState)
        {
            if (this.SetRatingOperationCompleted == null)
            {
                this.SetRatingOperationCompleted = new SendOrPostCallback(this.OnSetRatingOperationCompleted);
            }

            object[] objArray = new object[] { url, rating, title, analysisDataEntry };
            base.InvokeAsync("SetRating", objArray, this.SetRatingOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://microsoft.com/webservices/SharePointPortalServer/SocialDataService/UpdateComment",
            RequestNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            ResponseNamespace = "http://microsoft.com/webservices/SharePointPortalServer/SocialDataService",
            Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public DateTime UpdateComment(string url, DateTime lastModifiedTime, string comment, bool isHighPriority)
        {
            object[] objArray = new object[] { url, lastModifiedTime, comment, isHighPriority };
            return (DateTime)base.Invoke("UpdateComment", objArray)[0];
        }

        public void UpdateCommentAsync(string url, DateTime lastModifiedTime, string comment, bool isHighPriority)
        {
            this.UpdateCommentAsync(url, lastModifiedTime, comment, isHighPriority, null);
        }

        public void UpdateCommentAsync(string url, DateTime lastModifiedTime, string comment, bool isHighPriority,
            object userState)
        {
            if (this.UpdateCommentOperationCompleted == null)
            {
                this.UpdateCommentOperationCompleted = new SendOrPostCallback(this.OnUpdateCommentOperationCompleted);
            }

            object[] objArray = new object[] { url, lastModifiedTime, comment, isHighPriority };
            base.InvokeAsync("UpdateComment", objArray, this.UpdateCommentOperationCompleted, userState);
        }

        public event AddCommentCompletedEventHandler AddCommentCompleted;

        public event AddTagByKeywordCompletedEventHandler AddTagByKeywordCompleted;

        public event AddTagCompletedEventHandler AddTagCompleted;

        public event CountCommentsOfUserCompletedEventHandler CountCommentsOfUserCompleted;

        public event CountCommentsOfUserOnUrlCompletedEventHandler CountCommentsOfUserOnUrlCompleted;

        public event CountCommentsOnUrlCompletedEventHandler CountCommentsOnUrlCompleted;

        public event CountRatingsOnUrlCompletedEventHandler CountRatingsOnUrlCompleted;

        public event CountTagsOfUserCompletedEventHandler CountTagsOfUserCompleted;

        public event DeleteCommentCompletedEventHandler DeleteCommentCompleted;

        public event DeleteRatingCompletedEventHandler DeleteRatingCompleted;

        public event DeleteTagByKeywordCompletedEventHandler DeleteTagByKeywordCompleted;

        public event DeleteTagCompletedEventHandler DeleteTagCompleted;

        public event DeleteTagsCompletedEventHandler DeleteTagsCompleted;

        public event GetAllTagTermsCompletedEventHandler GetAllTagTermsCompleted;

        public event GetAllTagTermsForUrlFolderCompletedEventHandler GetAllTagTermsForUrlFolderCompleted;

        public event GetAllTagUrlsByKeywordCompletedEventHandler GetAllTagUrlsByKeywordCompleted;

        public event GetAllTagUrlsCompletedEventHandler GetAllTagUrlsCompleted;

        public event GetCommentsOfUserCompletedEventHandler GetCommentsOfUserCompleted;

        public event GetCommentsOfUserOnUrlCompletedEventHandler GetCommentsOfUserOnUrlCompleted;

        public event GetCommentsOnUrlCompletedEventHandler GetCommentsOnUrlCompleted;

        public event GetRatingAverageOnUrlCompletedEventHandler GetRatingAverageOnUrlCompleted;

        public event GetRatingOfUserOnUrlCompletedEventHandler GetRatingOfUserOnUrlCompleted;

        public event GetRatingOnUrlCompletedEventHandler GetRatingOnUrlCompleted;

        public event GetRatingsOfUserCompletedEventHandler GetRatingsOfUserCompleted;

        public event GetRatingsOnUrlCompletedEventHandler GetRatingsOnUrlCompleted;

        public event GetSocialDataForFullReplicationCompletedEventHandler GetSocialDataForFullReplicationCompleted;

        public event GetSocialDataForIncrementalReplicationCompletedEventHandler
            GetSocialDataForIncrementalReplicationCompleted;

        public event GetTagsCompletedEventHandler GetTagsCompleted;

        public event GetTagsOfUserCompletedEventHandler GetTagsOfUserCompleted;

        public event GetTagTermsCompletedEventHandler GetTagTermsCompleted;

        public event GetTagTermsOfUserCompletedEventHandler GetTagTermsOfUserCompleted;

        public event GetTagTermsOnUrlCompletedEventHandler GetTagTermsOnUrlCompleted;

        public event GetTagUrlsByKeywordCompletedEventHandler GetTagUrlsByKeywordCompleted;

        public event GetTagUrlsCompletedEventHandler GetTagUrlsCompleted;

        public event GetTagUrlsOfUserByKeywordCompletedEventHandler GetTagUrlsOfUserByKeywordCompleted;

        public event GetTagUrlsOfUserCompletedEventHandler GetTagUrlsOfUserCompleted;

        public event PropagateRatingCompletedEventHandler PropagateRatingCompleted;

        public event ReplicateFullSocialDataCompletedEventHandler ReplicateFullSocialDataCompleted;

        public event ReplicateIncrementalSocialDataCompletedEventHandler ReplicateIncrementalSocialDataCompleted;

        public event SetRatingCompletedEventHandler SetRatingCompleted;

        public event UpdateCommentCompletedEventHandler UpdateCommentCompleted;
    }
}