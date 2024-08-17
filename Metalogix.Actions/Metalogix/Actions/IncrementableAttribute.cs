using System;

namespace Metalogix.Actions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class IncrementableAttribute : Attribute
    {
        private bool m_value;

        private string m_IncrementalName;

        public bool Incrementable
        {
            get { return this.m_value; }
        }

        public string IncrementalName
        {
            get { return this.m_IncrementalName; }
        }

        public IncrementableAttribute(bool bool_0, string sIncrementalName)
        {
            this.m_value = bool_0;
            this.m_IncrementalName = sIncrementalName;
        }

        public override string ToString()
        {
            if (!this.Incrementable)
            {
                return "Is Not Incrementable";
            }

            return string.Concat("Is Incrementable As ", this.IncrementalName);
        }
    }
}