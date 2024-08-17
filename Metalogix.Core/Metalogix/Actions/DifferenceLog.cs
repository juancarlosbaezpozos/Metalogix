using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Metalogix.Actions
{
    public class DifferenceLog : IEnumerable
    {
        private bool m_bContainsExpected;

        private bool m_bContainsUnexpected;

        private List<DifferenceLogItem> m_list;

        public DifferenceLog()
        {
            this.m_list = new List<DifferenceLogItem>();
            this.m_bContainsExpected = false;
            this.m_bContainsUnexpected = false;
        }

        private void Add(string sMessage, string sAttr, DifferenceStatus status, bool bEx)
        {
            this.m_list.Add(new DifferenceLogItem(sMessage, sAttr, status, bEx));
            if (bEx)
            {
                this.m_bContainsExpected = true;
                return;
            }

            this.m_bContainsUnexpected = true;
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)this.m_list).GetEnumerator();
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (DifferenceLogItem differenceLogItem in this)
            {
                stringBuilder.AppendLine(differenceLogItem.ToString());
            }

            return stringBuilder.ToString().TrimEnd(new char[0]);
        }

        public string ToStringFormat()
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (this.m_bContainsUnexpected)
            {
                foreach (DifferenceLogItem differenceLogItem in this)
                {
                    if (differenceLogItem.Expected)
                    {
                        continue;
                    }

                    stringBuilder.AppendLine(differenceLogItem.ToString());
                }

                stringBuilder.AppendLine("");
            }

            if (this.m_bContainsExpected)
            {
                foreach (DifferenceLogItem differenceLogItem1 in this)
                {
                    if (!differenceLogItem1.Expected)
                    {
                        continue;
                    }

                    stringBuilder.AppendLine(differenceLogItem1.ToString());
                }
            }

            return stringBuilder.ToString().TrimEnd(new char[0]);
        }

        public void Write(string sMessage)
        {
            this.Add(sMessage, "", DifferenceStatus.Unclassified, false);
        }

        public void Write(string sMessage, string sAttr)
        {
            this.Add(sMessage, sAttr, DifferenceStatus.Difference, false);
        }

        public void Write(string sMessage, string sAttr, DifferenceStatus status)
        {
            this.Add(sMessage, sAttr, status, false);
        }

        public void Write(string sMessage, string sAttr, DifferenceStatus status, bool bEx)
        {
            this.Add(sMessage, sAttr, status, bEx);
        }
    }
}