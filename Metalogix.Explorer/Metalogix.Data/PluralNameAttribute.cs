using System;

namespace Metalogix.Data
{
    public class PluralNameAttribute : Attribute
    {
        private string m_value;

        public string PluralName
        {
            get { return this.m_value; }
        }

        public PluralNameAttribute(string val)
        {
            this.m_value = val;
        }
    }
}