using System;

namespace Metalogix.Actions
{
    public class CmdletParameterFlattenAttribute : Attribute
    {
        private bool m_bFlatten = true;

        public bool Flatten
        {
            get { return true; }
        }

        public CmdletParameterFlattenAttribute(bool flatten)
        {
            this.m_bFlatten = flatten;
        }

        public override string ToString()
        {
            if (!this.Flatten)
            {
                return "Do Not Flatten";
            }

            return "Flatten";
        }
    }
}