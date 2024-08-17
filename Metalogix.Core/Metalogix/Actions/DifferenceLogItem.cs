using System;

namespace Metalogix.Actions
{
    public class DifferenceLogItem
    {
        private string m_sMessage;

        private string m_sAttribute;

        private DifferenceStatus m_Status;

        private bool m_bExpected;

        public string Attribute
        {
            get { return this.m_sAttribute; }
        }

        public bool Expected
        {
            get { return this.m_bExpected; }
        }

        public string Message
        {
            get { return this.m_sMessage; }
        }

        public DifferenceStatus Status
        {
            get { return this.m_Status; }
        }

        internal DifferenceLogItem(string sMessage, string sAttr, DifferenceStatus status, bool bEx)
        {
            this.m_sMessage = sMessage;
            this.m_sAttribute = sAttr;
            this.m_Status = status;
            this.m_bExpected = bEx;
        }

        public override string ToString()
        {
            return this.m_sMessage;
        }
    }
}