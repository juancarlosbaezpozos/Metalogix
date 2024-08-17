using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Adapters.NWS.Meetings;
using System;
using System.Reflection;
using System.Xml;

namespace Metalogix.SharePoint.Adapters.NWS.WrappedServices
{
    public class MeetingsService : BaseServiceWrapper
    {
        public MeetingsService(SharePointAdapter parent)
        {
            this.m_Parent = parent;
            this.m_wrappedService = new Metalogix.SharePoint.Adapters.NWS.Meetings.Meetings();
            base.InitializeWrappedWebService("Meetings");
        }

        public XmlNode AddMeeting(string organizerEmail, string uid, uint sequence, DateTime utcDateStamp, string title,
            string location, DateTime utcDateStart, DateTime utcDateEnd, bool nonGregorian)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[]
            {
                organizerEmail, uid, sequence, utcDateStamp, title, location, utcDateStart, utcDateEnd, nonGregorian
            };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode AddMeetingFromICal(string organizerEmail, string icalText)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { organizerEmail, icalText };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode CreateWorkspace(string title, string templateName, uint lcid, TimeZoneInf timeZoneInformation)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { title, templateName, lcid, timeZoneInformation };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public void DeleteWorkspace()
        {
            WebServiceWrapperUtilities.ExecuteMethod(this, MethodBase.GetCurrentMethod().Name, null);
        }

        public XmlNode GetMeetingsInformation(uint requestFlags, uint lcid)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { requestFlags, lcid };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode GetMeetingWorkspaces(bool recurring)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { recurring };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public void RemoveMeeting(uint recurranceID, string uid, uint sequence, DateTime utcDateStamp,
            bool cancelMeeting)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { recurranceID, uid, sequence, utcDateStamp, cancelMeeting };
            WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public void RestoreMeeting(string uid)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { uid };
            WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public void SetAttendeeResponse(string attendeeEmail, uint recurranceID, string uid, uint sequence,
            DateTime utcDateTimeOrganizerCriticalChange, DateTime utcDateTimeAttendeeCriticalChange,
            AttendeeResponse response)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[]
            {
                attendeeEmail, recurranceID, uid, sequence, utcDateTimeOrganizerCriticalChange,
                utcDateTimeAttendeeCriticalChange, response
            };
            WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public void SetWorkspaceTitle(string title)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { title };
            WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public void UpdateMeeting(string uid, uint sequence, DateTime utcDateStamp, string title, string location,
            DateTime utcDateStart, DateTime utcDateEnd, bool nonGregorian)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[]
                { uid, utcDateStamp, title, location, utcDateStart, utcDateEnd, nonGregorian };
            WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }

        public XmlNode UpdateMeetingFromICal(string iCalTest, bool ignoreAttendees)
        {
            string name = MethodBase.GetCurrentMethod().Name;
            object[] objArray = new object[] { iCalTest, ignoreAttendees };
            return (XmlNode)WebServiceWrapperUtilities.ExecuteMethod(this, name, objArray);
        }
    }
}