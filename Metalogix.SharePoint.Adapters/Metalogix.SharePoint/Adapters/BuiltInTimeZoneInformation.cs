using System;
using System.Globalization;

namespace Metalogix.SharePoint.Adapters
{
    public class BuiltInTimeZoneInformation : TimeZoneInformation
    {
        private TimeZone m_timeZone;

        private int m_iBias;

        private int m_iStandardBias;

        private int m_iDaylightBias;

        public override int Bias
        {
            get { return this.m_iBias; }
        }

        public override int DaylightBias
        {
            get { return this.m_iDaylightBias; }
        }

        public override int ID
        {
            get { return -1; }
        }

        public override string Name
        {
            get { return this.m_timeZone.StandardName; }
        }

        public override int StandardBias
        {
            get { return this.m_iStandardBias; }
        }

        public BuiltInTimeZoneInformation(TimeZone timeZone)
        {
            this.m_timeZone = timeZone;
            DaylightTime daylightChanges = timeZone.GetDaylightChanges(DateTime.Now.Year);
            if (daylightChanges.Start == DateTime.MinValue && daylightChanges.End == DateTime.MinValue)
            {
                TimeSpan utcOffset = timeZone.GetUtcOffset(DateTime.Now);
                this.m_iBias = (int)utcOffset.TotalMinutes;
                this.m_iStandardBias = this.m_iBias;
                this.m_iDaylightBias = this.m_iStandardBias;
                return;
            }

            DateTime dateTime = daylightChanges.Start.AddDays(-1);
            DateTime dateTime1 = daylightChanges.Start.AddDays(1);
            this.m_iBias = (int)timeZone.GetUtcOffset(dateTime).TotalMinutes;
            this.m_iStandardBias = this.m_iBias;
            this.m_iDaylightBias = (int)timeZone.GetUtcOffset(dateTime1).TotalMinutes;
        }

        public override DateTime LocalTimeToUtc(DateTime local)
        {
            return this.m_timeZone.ToUniversalTime(local);
        }

        public override DateTime UtcToLocalTime(DateTime utc)
        {
            return this.m_timeZone.ToLocalTime(utc);
        }
    }
}