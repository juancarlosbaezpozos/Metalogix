using System;

namespace Metalogix.Actions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class AnalyzableAttribute : Attribute
    {
        private bool m_value = true;

        public bool Analyzable
        {
            get { return this.m_value; }
        }

        public AnalyzableAttribute(bool bAnalyzable)
        {
            this.m_value = bAnalyzable;
        }

        public override string ToString()
        {
            if (!this.Analyzable)
            {
                return "Not Analyzable";
            }

            return "Analyzable";
        }
    }
}