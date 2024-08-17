using System;

namespace Metalogix.Actions
{
    public class CmdletEnabledParameterAttribute : Attribute
    {
        private bool m_bCmdletEnabledParameter = true;

        private string m_propName;

        private object m_expectedValue;

        public bool CmdletEnabledParameter
        {
            get { return this.m_bCmdletEnabledParameter; }
        }

        public string ConditionalPropertyName
        {
            get { return this.m_propName; }
        }

        public object ExpectedValue
        {
            get { return this.m_expectedValue; }
        }

        public CmdletEnabledParameterAttribute(bool bCmdletEnabledParameter)
        {
            this.m_bCmdletEnabledParameter = bCmdletEnabledParameter;
        }

        public CmdletEnabledParameterAttribute(string conditionalPropertyName, object expectedValue)
        {
            this.m_propName = conditionalPropertyName;
            this.m_expectedValue = expectedValue;
            this.m_bCmdletEnabledParameter = false;
        }

        public override string ToString()
        {
            if (this.CmdletEnabledParameter)
            {
                return "CmdletEnabled";
            }

            if (!string.IsNullOrEmpty(this.ConditionalPropertyName))
            {
                return "Not CmdletEnabled";
            }

            return string.Format("CmdletEnabled when {0} equals {1}", this.ConditionalPropertyName, this.ExpectedValue);
        }
    }
}