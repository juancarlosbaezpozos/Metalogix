using Metalogix.SharePoint.Adapters.Properties;
using System;

namespace Metalogix.SharePoint.Adapters.Authentication
{
    public class InsufficentIEVersionException : Exception
    {
        private int m_iDetectedVersion;

        private int m_iExpectedVersion;

        public int DetectedVersion
        {
            get { return this.m_iDetectedVersion; }
        }

        public int ExpectedVersion
        {
            get { return this.m_iExpectedVersion; }
        }

        public InsufficentIEVersionException(int iDetectedVersion, int iExpectedVersion) : base(
            string.Format(Resources.InsufficientIEVersionMessage, iDetectedVersion, iExpectedVersion))
        {
            this.m_iDetectedVersion = iDetectedVersion;
            this.m_iExpectedVersion = iExpectedVersion;
        }
    }
}