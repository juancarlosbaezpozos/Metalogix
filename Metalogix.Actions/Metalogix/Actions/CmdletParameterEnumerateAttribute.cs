using System;

namespace Metalogix.Actions
{
    public class CmdletParameterEnumerateAttribute : Attribute
    {
        private bool m_bEnumerate = true;

        public bool Enumerate
        {
            get { return true; }
        }

        public CmdletParameterEnumerateAttribute(bool enumerate)
        {
            this.m_bEnumerate = this.Enumerate;
        }

        public override string ToString()
        {
            if (!this.Enumerate)
            {
                return "Do Not Enumerate";
            }

            return "Enumerate";
        }
    }
}