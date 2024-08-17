using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace Metalogix.SharePoint.Adapters
{
    public class SharePointTimeZoneInformation : TimeZoneInformation
    {
        private string m_sName;

        private int m_iID;

        private SharePointTimeZoneInformation.TimeShiftInfo m_baseTimeShift;

        private Dictionary<int, SharePointTimeZoneInformation.TimeShiftInfo> m_history;

        public override int Bias
        {
            get { return this.m_baseTimeShift.Bias; }
        }

        public override int DaylightBias
        {
            get { return this.m_baseTimeShift.DaylightBias; }
        }

        public override int ID
        {
            get { return this.m_iID; }
        }

        public override string Name
        {
            get { return this.m_sName; }
        }

        public override int StandardBias
        {
            get { return this.m_baseTimeShift.StandardBias; }
        }

        public SharePointTimeZoneInformation(XmlNode tziNode)
        {
            this.m_iID = Convert.ToInt32(tziNode.Attributes["ID"].Value, NumberFormatInfo.InvariantInfo);
            this.m_sName = tziNode.Attributes["Name"].Value;
            this.m_baseTimeShift = new SharePointTimeZoneInformation.TimeShiftInfo(tziNode);
            XmlNodeList xmlNodeLists = tziNode.SelectNodes("./History");
            this.m_history = new Dictionary<int, SharePointTimeZoneInformation.TimeShiftInfo>(xmlNodeLists.Count);
            foreach (XmlNode xmlNodes in xmlNodeLists)
            {
                int num = Convert.ToInt32(xmlNodes.Attributes["Year"].Value, NumberFormatInfo.InvariantInfo);
                this.m_history.Add(num, new SharePointTimeZoneInformation.TimeShiftInfo(xmlNodes));
            }
        }

        private SharePointTimeZoneInformation.TimeShiftInfo GetTimeShiftInfo(DateTime time)
        {
            return this.GetTimeShiftInfo(time.Year);
        }

        private SharePointTimeZoneInformation.TimeShiftInfo GetTimeShiftInfo(int iYear)
        {
            SharePointTimeZoneInformation.TimeShiftInfo mBaseTimeShift = this.m_baseTimeShift;
            foreach (KeyValuePair<int, SharePointTimeZoneInformation.TimeShiftInfo> mHistory in this.m_history)
            {
                if (iYear > mHistory.Key)
                {
                    continue;
                }

                mBaseTimeShift = mHistory.Value;
            }

            return mBaseTimeShift;
        }

        public override DateTime LocalTimeToUtc(DateTime local)
        {
            return this.GetTimeShiftInfo(local).ConvertLocalTimeToUTC(local);
        }

        public override DateTime UtcToLocalTime(DateTime utc)
        {
            return this.GetTimeShiftInfo(utc).ConvertUTCToLocalTime(utc);
        }

        private class TimeChangeInfo
        {
            private int m_iMonth;

            private int m_iWeek;

            private int m_iHour;

            private int m_iMinute;

            private int m_iSecond;

            private int m_iMillisecond;

            private int m_iDayOfWeek;

            public int DayOfWeek
            {
                get { return this.m_iDayOfWeek; }
                set { this.m_iDayOfWeek = value; }
            }

            public int Hour
            {
                get { return this.m_iHour; }
                set { this.m_iHour = value; }
            }

            public int Millisecond
            {
                get { return this.m_iMillisecond; }
                set { this.m_iMillisecond = value; }
            }

            public int Minute
            {
                get { return this.m_iMinute; }
                set { this.m_iMinute = value; }
            }

            public int Month
            {
                get { return this.m_iMonth; }
                set { this.m_iMonth = value; }
            }

            public int Second
            {
                get { return this.m_iSecond; }
                set { this.m_iSecond = value; }
            }

            public int Week
            {
                get { return this.m_iWeek; }
                set { this.m_iWeek = value; }
            }

            public TimeChangeInfo()
            {
            }

            public static int ConvertDayOfWeekToInt(DayOfWeek day)
            {
                if (day == System.DayOfWeek.Sunday)
                {
                    return 0;
                }

                if (day == System.DayOfWeek.Monday)
                {
                    return 1;
                }

                if (day == System.DayOfWeek.Tuesday)
                {
                    return 2;
                }

                if (day == System.DayOfWeek.Wednesday)
                {
                    return 3;
                }

                if (day == System.DayOfWeek.Thursday)
                {
                    return 4;
                }

                if (day == System.DayOfWeek.Friday)
                {
                    return 5;
                }

                return 6;
            }

            public DateTime GetDate(int iYear)
            {
                DateTime dateTime = new DateTime(iYear, this.Month, 1, this.Hour, this.Minute, this.Second,
                    this.Millisecond);
                int num = SharePointTimeZoneInformation.TimeChangeInfo.ConvertDayOfWeekToInt(dateTime.DayOfWeek);
                num = this.DayOfWeek - num;
                if (num < 0)
                {
                    num += 7;
                }

                dateTime = dateTime.AddDays((double)num);
                for (int i = 0; i < this.Week - 1; i++)
                {
                    DateTime dateTime1 = dateTime.AddDays(7);
                    if (dateTime.Month != dateTime1.Month)
                    {
                        break;
                    }

                    dateTime = dateTime1;
                }

                return dateTime;
            }
        }

        private class TimeShiftInfo
        {
            private int m_iBias;

            private int m_iStandardBias;

            private int m_iDaylightBias;

            private SharePointTimeZoneInformation.TimeChangeInfo m_daylightTimeStart;

            private SharePointTimeZoneInformation.TimeChangeInfo m_daylightTimeEnd;

            public int Bias
            {
                get { return this.m_iBias; }
            }

            public int DaylightBias
            {
                get { return this.m_iDaylightBias; }
            }

            public SharePointTimeZoneInformation.TimeChangeInfo DaylightTimeEnd
            {
                get { return this.m_daylightTimeEnd; }
            }

            public SharePointTimeZoneInformation.TimeChangeInfo DaylightTimeStart
            {
                get { return this.m_daylightTimeStart; }
            }

            public int StandardBias
            {
                get { return this.m_iStandardBias; }
            }

            public TimeShiftInfo(XmlNode timeShiftNode)
            {
                XmlNode xmlNodes = timeShiftNode.SelectSingleNode("./Bias");
                this.m_iBias = -1 * Convert.ToInt32(xmlNodes.InnerText, NumberFormatInfo.InvariantInfo);
                XmlNode xmlNodes1 = timeShiftNode.SelectSingleNode("./StandardTime/Bias");
                XmlNode xmlNodes2 = timeShiftNode.SelectSingleNode("./StandardTime/Date");
                if (xmlNodes1 == null || xmlNodes2 == null)
                {
                    this.m_iStandardBias = this.m_iBias;
                    this.m_daylightTimeEnd = null;
                }
                else
                {
                    this.m_iStandardBias =
                        this.m_iBias - Convert.ToInt32(xmlNodes1.InnerText, NumberFormatInfo.InvariantInfo);
                    this.m_daylightTimeEnd = this.GetTimeChangeInfoFromDateNode(xmlNodes2);
                }

                XmlNode xmlNodes3 = timeShiftNode.SelectSingleNode("./DaylightTime/Bias");
                XmlNode xmlNodes4 = timeShiftNode.SelectSingleNode("./DaylightTime/Date");
                if (xmlNodes3 == null || xmlNodes4 == null)
                {
                    this.m_iDaylightBias = this.m_iBias;
                    this.m_daylightTimeStart = null;
                    return;
                }

                this.m_iDaylightBias =
                    this.m_iBias - Convert.ToInt32(xmlNodes3.InnerText, NumberFormatInfo.InvariantInfo);
                this.m_daylightTimeStart = this.GetTimeChangeInfoFromDateNode(xmlNodes4);
            }

            public DateTime ConvertLocalTimeToUTC(DateTime time)
            {
                if (!this.IsInDaylightSavingsTime(time))
                {
                    time = time.AddMinutes(-1 * (double)this.StandardBias);
                }
                else
                {
                    time = time.AddMinutes(-1 * (double)this.DaylightBias);
                }

                return new DateTime(time.Ticks, DateTimeKind.Utc);
            }

            public DateTime ConvertUTCToLocalTime(DateTime time)
            {
                DateTime dateTime;
                DateTime dateTime1 = time.AddMinutes((double)this.StandardBias);
                DateTime dateTime2 = time.AddMinutes((double)this.DaylightBias);
                dateTime = (this.IsInDaylightSavingsTime(dateTime1) || this.IsInDaylightSavingsTime(dateTime2)
                    ? dateTime2
                    : dateTime1);
                return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute,
                    dateTime.Second, dateTime.Millisecond, DateTimeKind.Local);
            }

            private SharePointTimeZoneInformation.TimeChangeInfo GetTimeChangeInfoFromDateNode(XmlNode dateNode)
            {
                SharePointTimeZoneInformation.TimeChangeInfo timeChangeInfo =
                    new SharePointTimeZoneInformation.TimeChangeInfo();
                XmlNode xmlNodes = dateNode.SelectSingleNode("./Month");
                if (xmlNodes != null)
                {
                    timeChangeInfo.Month = Convert.ToInt32(xmlNodes.InnerText, NumberFormatInfo.InvariantInfo);
                }

                XmlNode xmlNodes1 = dateNode.SelectSingleNode("./Day");
                if (xmlNodes1 != null)
                {
                    timeChangeInfo.Week = Convert.ToInt32(xmlNodes1.InnerText, NumberFormatInfo.InvariantInfo);
                }

                XmlNode xmlNodes2 = dateNode.SelectSingleNode("./Hour");
                if (xmlNodes2 != null)
                {
                    timeChangeInfo.Hour = Convert.ToInt32(xmlNodes2.InnerText, NumberFormatInfo.InvariantInfo);
                }

                XmlNode xmlNodes3 = dateNode.SelectSingleNode("./Minute");
                if (xmlNodes3 != null)
                {
                    timeChangeInfo.Minute = Convert.ToInt32(xmlNodes3.InnerText, NumberFormatInfo.InvariantInfo);
                }

                XmlNode xmlNodes4 = dateNode.SelectSingleNode("./Second");
                if (xmlNodes4 != null)
                {
                    timeChangeInfo.Second = Convert.ToInt32(xmlNodes4.InnerText, NumberFormatInfo.InvariantInfo);
                }

                XmlNode xmlNodes5 = dateNode.SelectSingleNode("./Millisecond");
                if (xmlNodes5 != null)
                {
                    timeChangeInfo.Millisecond = Convert.ToInt32(xmlNodes5.InnerText, NumberFormatInfo.InvariantInfo);
                }

                XmlNode xmlNodes6 = dateNode.SelectSingleNode("./DayOfWeek");
                if (xmlNodes6 != null)
                {
                    timeChangeInfo.DayOfWeek = Convert.ToInt32(xmlNodes6.InnerText, NumberFormatInfo.InvariantInfo);
                }

                return timeChangeInfo;
            }

            private bool IsInDaylightSavingsTime(DateTime time)
            {
                if (this.DaylightTimeStart == null || this.DaylightTimeEnd == null)
                {
                    return false;
                }

                if (this.DaylightTimeStart.Month == time.Month)
                {
                    DateTime date = this.DaylightTimeStart.GetDate(time.Year);
                    date = date.AddMinutes((double)(this.DaylightBias - this.StandardBias));
                    return time >= date;
                }

                if (this.DaylightTimeEnd.Month == time.Month)
                {
                    DateTime dateTime = this.DaylightTimeEnd.GetDate(time.Year);
                    dateTime = dateTime.AddMinutes((double)(this.StandardBias - this.DaylightBias));
                    return time < dateTime;
                }

                if (this.DaylightTimeEnd.Month < this.DaylightTimeStart.Month)
                {
                    if (time.Month < this.DaylightTimeEnd.Month)
                    {
                        return true;
                    }

                    return time.Month > this.DaylightTimeStart.Month;
                }

                if (time.Month >= this.DaylightTimeEnd.Month)
                {
                    return false;
                }

                return time.Month > this.DaylightTimeStart.Month;
            }
        }
    }
}