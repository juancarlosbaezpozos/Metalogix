using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Metalogix.Jobs
{
    public class CompletionDetailPropertyDescriptor : PropertyDescriptor
    {
        private string m_sKey;

        public override Type ComponentType
        {
            get { return typeof(JobSummary); }
        }

        public override bool IsReadOnly
        {
            get { return true; }
        }

        public override Type PropertyType
        {
            get { return typeof(int); }
        }

        public CompletionDetailPropertyDescriptor(string sName, string sKey) : base(sName, null)
        {
            this.m_sKey = sKey;
        }

        public CompletionDetailPropertyDescriptor(string sName, string sKey, Attribute[] attributes) : base(sName,
            attributes)
        {
            this.m_sKey = sKey;
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override object GetValue(object component)
        {
            object item;
            if (component is JobSummary)
            {
                try
                {
                    Dictionary<string, long> completionDetails = ((JobSummary)component).CompletionDetails;
                    if (!completionDetails.ContainsKey(this.m_sKey))
                    {
                        return null;
                    }
                    else
                    {
                        item = completionDetails[this.m_sKey];
                    }
                }
                catch
                {
                    return null;
                }

                return item;
            }

            return null;
        }

        public override void ResetValue(object component)
        {
        }

        public override void SetValue(object component, object value)
        {
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
    }
}