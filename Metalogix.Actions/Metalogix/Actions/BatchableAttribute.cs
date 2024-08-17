using System;

namespace Metalogix.Actions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class BatchableAttribute : Attribute
    {
        private bool m_value;

        private string m_batchName;

        public bool Batchable
        {
            get { return this.m_value; }
        }

        public string BatchableName
        {
            get { return this.m_batchName; }
        }

        public BatchableAttribute(bool bool_0, string sBatchName)
        {
            this.m_value = bool_0;
            this.m_batchName = sBatchName;
        }

        public override string ToString()
        {
            if (!this.Batchable)
            {
                return "Is Not Batchable";
            }

            return string.Concat("Is Batchable As ", this.BatchableName);
        }
    }
}