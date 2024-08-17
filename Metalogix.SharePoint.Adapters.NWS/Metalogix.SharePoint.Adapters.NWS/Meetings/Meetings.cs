using Metalogix.SharePoint.Adapters.NWS.Properties;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.Services.Protocols;
using System.Xml;

namespace Metalogix.SharePoint.Adapters.NWS.Meetings
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [GeneratedCode("System.Web.Services", "4.0.30319.17929")]
    [WebServiceBinding(Name = "MeetingsSoap", Namespace = "http://schemas.microsoft.com/sharepoint/soap/meetings/")]
    public class Meetings : SoapHttpClientProtocol
    {
        private SendOrPostCallback CreateWorkspaceOperationCompleted;

        private SendOrPostCallback DeleteWorkspaceOperationCompleted;

        private SendOrPostCallback GetMeetingWorkspacesOperationCompleted;

        private SendOrPostCallback SetWorkspaceTitleOperationCompleted;

        private SendOrPostCallback AddMeetingFromICalOperationCompleted;

        private SendOrPostCallback AddMeetingOperationCompleted;

        private SendOrPostCallback UpdateMeetingFromICalOperationCompleted;

        private SendOrPostCallback UpdateMeetingOperationCompleted;

        private SendOrPostCallback RemoveMeetingOperationCompleted;

        private SendOrPostCallback SetAttendeeResponseOperationCompleted;

        private SendOrPostCallback GetMeetingsInformationOperationCompleted;

        private SendOrPostCallback RestoreMeetingOperationCompleted;

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

        public Meetings()
        {
            this.Url = Settings.Default.Metalogix_SharePoint_Adapters_NWS_Meetings_Meetings;
            if (!this.IsLocalFileSystemWebService(this.Url))
            {
                this.useDefaultCredentialsSetExplicitly = true;
                return;
            }

            this.UseDefaultCredentials = true;
            this.useDefaultCredentialsSetExplicitly = false;
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/meetings/AddMeeting",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/meetings/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/meetings/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode AddMeeting(string organizerEmail, string uid, uint sequence, DateTime utcDateStamp, string title,
            string location, DateTime utcDateStart, DateTime utcDateEnd, bool nonGregorian)
        {
            object[] objArray = new object[]
            {
                organizerEmail, uid, sequence, utcDateStamp, title, location, utcDateStart, utcDateEnd, nonGregorian
            };
            return (XmlNode)base.Invoke("AddMeeting", objArray)[0];
        }

        public void AddMeetingAsync(string organizerEmail, string uid, uint sequence, DateTime utcDateStamp,
            string title, string location, DateTime utcDateStart, DateTime utcDateEnd, bool nonGregorian)
        {
            this.AddMeetingAsync(organizerEmail, uid, sequence, utcDateStamp, title, location, utcDateStart, utcDateEnd,
                nonGregorian, null);
        }

        public void AddMeetingAsync(string organizerEmail, string uid, uint sequence, DateTime utcDateStamp,
            string title, string location, DateTime utcDateStart, DateTime utcDateEnd, bool nonGregorian,
            object userState)
        {
            if (this.AddMeetingOperationCompleted == null)
            {
                this.AddMeetingOperationCompleted = new SendOrPostCallback(this.OnAddMeetingOperationCompleted);
            }

            object[] objArray = new object[]
            {
                organizerEmail, uid, sequence, utcDateStamp, title, location, utcDateStart, utcDateEnd, nonGregorian
            };
            base.InvokeAsync("AddMeeting", objArray, this.AddMeetingOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/meetings/AddMeetingFromICal",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/meetings/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/meetings/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode AddMeetingFromICal(string organizerEmail, string icalText)
        {
            object[] objArray = new object[] { organizerEmail, icalText };
            return (XmlNode)base.Invoke("AddMeetingFromICal", objArray)[0];
        }

        public void AddMeetingFromICalAsync(string organizerEmail, string icalText)
        {
            this.AddMeetingFromICalAsync(organizerEmail, icalText, null);
        }

        public void AddMeetingFromICalAsync(string organizerEmail, string icalText, object userState)
        {
            if (this.AddMeetingFromICalOperationCompleted == null)
            {
                this.AddMeetingFromICalOperationCompleted =
                    new SendOrPostCallback(this.OnAddMeetingFromICalOperationCompleted);
            }

            object[] objArray = new object[] { organizerEmail, icalText };
            base.InvokeAsync("AddMeetingFromICal", objArray, this.AddMeetingFromICalOperationCompleted, userState);
        }

        public new void CancelAsync(object userState)
        {
            base.CancelAsync(userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/meetings/CreateWorkspace",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/meetings/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/meetings/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode CreateWorkspace(string title, string templateName, uint lcid, TimeZoneInf timeZoneInformation)
        {
            object[] objArray = new object[] { title, templateName, lcid, timeZoneInformation };
            return (XmlNode)base.Invoke("CreateWorkspace", objArray)[0];
        }

        public void CreateWorkspaceAsync(string title, string templateName, uint lcid, TimeZoneInf timeZoneInformation)
        {
            this.CreateWorkspaceAsync(title, templateName, lcid, timeZoneInformation, null);
        }

        public void CreateWorkspaceAsync(string title, string templateName, uint lcid, TimeZoneInf timeZoneInformation,
            object userState)
        {
            if (this.CreateWorkspaceOperationCompleted == null)
            {
                this.CreateWorkspaceOperationCompleted =
                    new SendOrPostCallback(this.OnCreateWorkspaceOperationCompleted);
            }

            object[] objArray = new object[] { title, templateName, lcid, timeZoneInformation };
            base.InvokeAsync("CreateWorkspace", objArray, this.CreateWorkspaceOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/meetings/DeleteWorkspace",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/meetings/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/meetings/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public void DeleteWorkspace()
        {
            base.Invoke("DeleteWorkspace", new object[0]);
        }

        public void DeleteWorkspaceAsync()
        {
            this.DeleteWorkspaceAsync(null);
        }

        public void DeleteWorkspaceAsync(object userState)
        {
            if (this.DeleteWorkspaceOperationCompleted == null)
            {
                this.DeleteWorkspaceOperationCompleted =
                    new SendOrPostCallback(this.OnDeleteWorkspaceOperationCompleted);
            }

            base.InvokeAsync("DeleteWorkspace", new object[0], this.DeleteWorkspaceOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/meetings/GetMeetingsInformation",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/meetings/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/meetings/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode GetMeetingsInformation(uint requestFlags, uint lcid)
        {
            object[] objArray = new object[] { requestFlags, lcid };
            return (XmlNode)base.Invoke("GetMeetingsInformation", objArray)[0];
        }

        public void GetMeetingsInformationAsync(uint requestFlags, uint lcid)
        {
            this.GetMeetingsInformationAsync(requestFlags, lcid, null);
        }

        public void GetMeetingsInformationAsync(uint requestFlags, uint lcid, object userState)
        {
            if (this.GetMeetingsInformationOperationCompleted == null)
            {
                this.GetMeetingsInformationOperationCompleted =
                    new SendOrPostCallback(this.OnGetMeetingsInformationOperationCompleted);
            }

            object[] objArray = new object[] { requestFlags, lcid };
            base.InvokeAsync("GetMeetingsInformation", objArray, this.GetMeetingsInformationOperationCompleted,
                userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/meetings/GetMeetingWorkspaces",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/meetings/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/meetings/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode GetMeetingWorkspaces(bool recurring)
        {
            object[] objArray = new object[] { recurring };
            return (XmlNode)base.Invoke("GetMeetingWorkspaces", objArray)[0];
        }

        public void GetMeetingWorkspacesAsync(bool recurring)
        {
            this.GetMeetingWorkspacesAsync(recurring, null);
        }

        public void GetMeetingWorkspacesAsync(bool recurring, object userState)
        {
            if (this.GetMeetingWorkspacesOperationCompleted == null)
            {
                this.GetMeetingWorkspacesOperationCompleted =
                    new SendOrPostCallback(this.OnGetMeetingWorkspacesOperationCompleted);
            }

            object[] objArray = new object[] { recurring };
            base.InvokeAsync("GetMeetingWorkspaces", objArray, this.GetMeetingWorkspacesOperationCompleted, userState);
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

        private void OnAddMeetingFromICalOperationCompleted(object arg)
        {
            if (this.AddMeetingFromICalCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddMeetingFromICalCompleted(this,
                    new AddMeetingFromICalCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnAddMeetingOperationCompleted(object arg)
        {
            if (this.AddMeetingCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.AddMeetingCompleted(this,
                    new AddMeetingCompletedEventArgs(invokeCompletedEventArg.Results, invokeCompletedEventArg.Error,
                        invokeCompletedEventArg.Cancelled, invokeCompletedEventArg.UserState));
            }
        }

        private void OnCreateWorkspaceOperationCompleted(object arg)
        {
            if (this.CreateWorkspaceCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.CreateWorkspaceCompleted(this,
                    new CreateWorkspaceCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnDeleteWorkspaceOperationCompleted(object arg)
        {
            if (this.DeleteWorkspaceCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.DeleteWorkspaceCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetMeetingsInformationOperationCompleted(object arg)
        {
            if (this.GetMeetingsInformationCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetMeetingsInformationCompleted(this,
                    new GetMeetingsInformationCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnGetMeetingWorkspacesOperationCompleted(object arg)
        {
            if (this.GetMeetingWorkspacesCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.GetMeetingWorkspacesCompleted(this,
                    new GetMeetingWorkspacesCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnRemoveMeetingOperationCompleted(object arg)
        {
            if (this.RemoveMeetingCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.RemoveMeetingCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnRestoreMeetingOperationCompleted(object arg)
        {
            if (this.RestoreMeetingCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.RestoreMeetingCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnSetAttendeeResponseOperationCompleted(object arg)
        {
            if (this.SetAttendeeResponseCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.SetAttendeeResponseCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnSetWorkspaceTitleOperationCompleted(object arg)
        {
            if (this.SetWorkspaceTitleCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.SetWorkspaceTitleCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnUpdateMeetingFromICalOperationCompleted(object arg)
        {
            if (this.UpdateMeetingFromICalCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.UpdateMeetingFromICalCompleted(this,
                    new UpdateMeetingFromICalCompletedEventArgs(invokeCompletedEventArg.Results,
                        invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        private void OnUpdateMeetingOperationCompleted(object arg)
        {
            if (this.UpdateMeetingCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArg = (InvokeCompletedEventArgs)arg;
                this.UpdateMeetingCompleted(this,
                    new AsyncCompletedEventArgs(invokeCompletedEventArg.Error, invokeCompletedEventArg.Cancelled,
                        invokeCompletedEventArg.UserState));
            }
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/meetings/RemoveMeeting",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/meetings/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/meetings/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public void RemoveMeeting(uint recurrenceId, string uid, uint sequence, DateTime utcDateStamp,
            bool cancelMeeting)
        {
            object[] objArray = new object[] { recurrenceId, uid, sequence, utcDateStamp, cancelMeeting };
            base.Invoke("RemoveMeeting", objArray);
        }

        public void RemoveMeetingAsync(uint recurrenceId, string uid, uint sequence, DateTime utcDateStamp,
            bool cancelMeeting)
        {
            this.RemoveMeetingAsync(recurrenceId, uid, sequence, utcDateStamp, cancelMeeting, null);
        }

        public void RemoveMeetingAsync(uint recurrenceId, string uid, uint sequence, DateTime utcDateStamp,
            bool cancelMeeting, object userState)
        {
            if (this.RemoveMeetingOperationCompleted == null)
            {
                this.RemoveMeetingOperationCompleted = new SendOrPostCallback(this.OnRemoveMeetingOperationCompleted);
            }

            object[] objArray = new object[] { recurrenceId, uid, sequence, utcDateStamp, cancelMeeting };
            base.InvokeAsync("RemoveMeeting", objArray, this.RemoveMeetingOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/meetings/RestoreMeeting",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/meetings/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/meetings/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public void RestoreMeeting(string uid)
        {
            object[] objArray = new object[] { uid };
            base.Invoke("RestoreMeeting", objArray);
        }

        public void RestoreMeetingAsync(string uid)
        {
            this.RestoreMeetingAsync(uid, null);
        }

        public void RestoreMeetingAsync(string uid, object userState)
        {
            if (this.RestoreMeetingOperationCompleted == null)
            {
                this.RestoreMeetingOperationCompleted = new SendOrPostCallback(this.OnRestoreMeetingOperationCompleted);
            }

            object[] objArray = new object[] { uid };
            base.InvokeAsync("RestoreMeeting", objArray, this.RestoreMeetingOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/meetings/SetAttendeeResponse",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/meetings/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/meetings/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public void SetAttendeeResponse(string attendeeEmail, uint recurrenceId, string uid, uint sequence,
            DateTime utcDateTimeOrganizerCriticalChange, DateTime utcDateTimeAttendeeCriticalChange,
            AttendeeResponse response)
        {
            object[] objArray = new object[]
            {
                attendeeEmail, recurrenceId, uid, sequence, utcDateTimeOrganizerCriticalChange,
                utcDateTimeAttendeeCriticalChange, response
            };
            base.Invoke("SetAttendeeResponse", objArray);
        }

        public void SetAttendeeResponseAsync(string attendeeEmail, uint recurrenceId, string uid, uint sequence,
            DateTime utcDateTimeOrganizerCriticalChange, DateTime utcDateTimeAttendeeCriticalChange,
            AttendeeResponse response)
        {
            this.SetAttendeeResponseAsync(attendeeEmail, recurrenceId, uid, sequence,
                utcDateTimeOrganizerCriticalChange, utcDateTimeAttendeeCriticalChange, response, null);
        }

        public void SetAttendeeResponseAsync(string attendeeEmail, uint recurrenceId, string uid, uint sequence,
            DateTime utcDateTimeOrganizerCriticalChange, DateTime utcDateTimeAttendeeCriticalChange,
            AttendeeResponse response, object userState)
        {
            if (this.SetAttendeeResponseOperationCompleted == null)
            {
                this.SetAttendeeResponseOperationCompleted =
                    new SendOrPostCallback(this.OnSetAttendeeResponseOperationCompleted);
            }

            object[] objArray = new object[]
            {
                attendeeEmail, recurrenceId, uid, sequence, utcDateTimeOrganizerCriticalChange,
                utcDateTimeAttendeeCriticalChange, response
            };
            base.InvokeAsync("SetAttendeeResponse", objArray, this.SetAttendeeResponseOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/meetings/SetWorkspaceTitle",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/meetings/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/meetings/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public void SetWorkspaceTitle(string title)
        {
            object[] objArray = new object[] { title };
            base.Invoke("SetWorkspaceTitle", objArray);
        }

        public void SetWorkspaceTitleAsync(string title)
        {
            this.SetWorkspaceTitleAsync(title, null);
        }

        public void SetWorkspaceTitleAsync(string title, object userState)
        {
            if (this.SetWorkspaceTitleOperationCompleted == null)
            {
                this.SetWorkspaceTitleOperationCompleted =
                    new SendOrPostCallback(this.OnSetWorkspaceTitleOperationCompleted);
            }

            object[] objArray = new object[] { title };
            base.InvokeAsync("SetWorkspaceTitle", objArray, this.SetWorkspaceTitleOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/meetings/UpdateMeeting",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/meetings/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/meetings/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public void UpdateMeeting(string uid, uint sequence, DateTime utcDateStamp, string title, string location,
            DateTime utcDateStart, DateTime utcDateEnd, bool nonGregorian)
        {
            object[] objArray = new object[]
                { uid, sequence, utcDateStamp, title, location, utcDateStart, utcDateEnd, nonGregorian };
            base.Invoke("UpdateMeeting", objArray);
        }

        public void UpdateMeetingAsync(string uid, uint sequence, DateTime utcDateStamp, string title, string location,
            DateTime utcDateStart, DateTime utcDateEnd, bool nonGregorian)
        {
            this.UpdateMeetingAsync(uid, sequence, utcDateStamp, title, location, utcDateStart, utcDateEnd,
                nonGregorian, null);
        }

        public void UpdateMeetingAsync(string uid, uint sequence, DateTime utcDateStamp, string title, string location,
            DateTime utcDateStart, DateTime utcDateEnd, bool nonGregorian, object userState)
        {
            if (this.UpdateMeetingOperationCompleted == null)
            {
                this.UpdateMeetingOperationCompleted = new SendOrPostCallback(this.OnUpdateMeetingOperationCompleted);
            }

            object[] objArray = new object[]
                { uid, sequence, utcDateStamp, title, location, utcDateStart, utcDateEnd, nonGregorian };
            base.InvokeAsync("UpdateMeeting", objArray, this.UpdateMeetingOperationCompleted, userState);
        }

        [SoapDocumentMethod("http://schemas.microsoft.com/sharepoint/soap/meetings/UpdateMeetingFromICal",
            RequestNamespace = "http://schemas.microsoft.com/sharepoint/soap/meetings/",
            ResponseNamespace = "http://schemas.microsoft.com/sharepoint/soap/meetings/", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public XmlNode UpdateMeetingFromICal(string icalText, bool ignoreAttendees)
        {
            object[] objArray = new object[] { icalText, ignoreAttendees };
            return (XmlNode)base.Invoke("UpdateMeetingFromICal", objArray)[0];
        }

        public void UpdateMeetingFromICalAsync(string icalText, bool ignoreAttendees)
        {
            this.UpdateMeetingFromICalAsync(icalText, ignoreAttendees, null);
        }

        public void UpdateMeetingFromICalAsync(string icalText, bool ignoreAttendees, object userState)
        {
            if (this.UpdateMeetingFromICalOperationCompleted == null)
            {
                this.UpdateMeetingFromICalOperationCompleted =
                    new SendOrPostCallback(this.OnUpdateMeetingFromICalOperationCompleted);
            }

            object[] objArray = new object[] { icalText, ignoreAttendees };
            base.InvokeAsync("UpdateMeetingFromICal", objArray, this.UpdateMeetingFromICalOperationCompleted,
                userState);
        }

        public event AddMeetingCompletedEventHandler AddMeetingCompleted;

        public event AddMeetingFromICalCompletedEventHandler AddMeetingFromICalCompleted;

        public event CreateWorkspaceCompletedEventHandler CreateWorkspaceCompleted;

        public event DeleteWorkspaceCompletedEventHandler DeleteWorkspaceCompleted;

        public event GetMeetingsInformationCompletedEventHandler GetMeetingsInformationCompleted;

        public event GetMeetingWorkspacesCompletedEventHandler GetMeetingWorkspacesCompleted;

        public event RemoveMeetingCompletedEventHandler RemoveMeetingCompleted;

        public event RestoreMeetingCompletedEventHandler RestoreMeetingCompleted;

        public event SetAttendeeResponseCompletedEventHandler SetAttendeeResponseCompleted;

        public event SetWorkspaceTitleCompletedEventHandler SetWorkspaceTitleCompleted;

        public event UpdateMeetingCompletedEventHandler UpdateMeetingCompleted;

        public event UpdateMeetingFromICalCompletedEventHandler UpdateMeetingFromICalCompleted;
    }
}