using System;

namespace Metalogix.Actions
{
    public class CmdletEnabledAttribute : Attribute
    {
        private readonly string[] m_requiredSnapins;

        private readonly string m_sCmdletName;

        private readonly bool m_bCmdletEnabled;

        public bool CmdletEnabled
        {
            get { return this.m_bCmdletEnabled; }
        }

        public string CmdletName
        {
            get { return this.m_sCmdletName; }
        }

        public string[] RequiredSnapins
        {
            get { return this.m_requiredSnapins; }
        }

        public CmdletEnabledAttribute(bool cmdletEnabled, string cmdletName, params string[] snapinName)
        {
            this.m_requiredSnapins = snapinName;
            this.m_sCmdletName = cmdletName;
            this.m_bCmdletEnabled = cmdletEnabled;
        }

        public override string ToString()
        {
            if (!this.CmdletEnabled)
            {
                return "No Cmdlet Available";
            }

            return string.Concat("Has Cmdlet Named ", this.CmdletName);
        }
    }
}