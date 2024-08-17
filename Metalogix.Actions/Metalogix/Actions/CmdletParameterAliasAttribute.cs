using System;

namespace Metalogix.Actions
{
    public class CmdletParameterAliasAttribute : Attribute
    {
        private string m_sAlias;

        public string Alias
        {
            get { return this.m_sAlias; }
        }

        public CmdletParameterAliasAttribute(string sAlias)
        {
            this.m_sAlias = sAlias;
        }

        public override string ToString()
        {
            return this.Alias;
        }
    }
}