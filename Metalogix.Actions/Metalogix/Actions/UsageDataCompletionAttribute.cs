using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Metalogix.Actions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class UsageDataCompletionAttribute : Attribute
    {
        private List<string> m_lstUsageCompletionValues;

        public List<string> UsageDataCompletions
        {
            get { return this.m_lstUsageCompletionValues; }
        }

        public UsageDataCompletionAttribute(string sCompletionText) : this(new string[] { sCompletionText })
        {
        }

        public UsageDataCompletionAttribute(string[] sCompletionValues)
        {
            this.m_lstUsageCompletionValues = new List<string>(sCompletionValues);
        }

        public override string ToString()
        {
            return this.UsageDataCompletions.Aggregate<string>((string string_0, string string_1) =>
                string.Concat(string_0, ", ", string_1));
        }
    }
}