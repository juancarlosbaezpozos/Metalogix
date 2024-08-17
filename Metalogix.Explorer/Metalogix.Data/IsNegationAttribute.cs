using System;

namespace Metalogix.Data
{
    public class IsNegationAttribute : Attribute
    {
        private bool m_value;

        public bool IsNegation
        {
            get { return this.m_value; }
        }

        public IsNegationAttribute(bool val)
        {
            this.m_value = val;
        }
    }
}