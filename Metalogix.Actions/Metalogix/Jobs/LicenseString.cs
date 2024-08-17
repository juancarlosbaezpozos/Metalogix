using Metalogix.Utilities;
using System;

namespace Metalogix.Jobs
{
    public class LicenseString
    {
        private string m_sLicenseString;

        private string m_sLabel;

        private long m_lValue;

        public long DataUsedValue
        {
            get { return this.m_lValue; }
        }

        public LicenseString(string sData)
        {
            this.m_sLicenseString = sData;
            string[] strArrays = sData.Split(new char[] { ':' });
            if ((int)strArrays.Length != 2)
            {
                this.m_sLabel = sData;
                this.m_lValue = 0L;
            }

            this.m_sLabel = strArrays[0].Trim();
            string str = strArrays[1].Trim();
            this.m_lValue = Format.ParseFormattedSize(str);
        }
    }
}