using Metalogix.Actions.Incremental;
using System;
using System.Runtime.CompilerServices;

namespace Metalogix.Actions.Incremental.Checks
{
    public class ModifiedDateCheck : IIncrementalCheck
    {
        IIncrementableNode Metalogix.Actions.Incremental.IIncrementalCheck.Source { get; set; }

        IIncrementableNode Metalogix.Actions.Incremental.IIncrementalCheck.Target { get; set; }

        public IncrementByModifiedDateNode Source
        {
            get { return ((IIncrementalCheck)this).Source as IncrementByModifiedDateNode; }
            set { ((IIncrementalCheck)this).Source = value; }
        }

        public IncrementByModifiedDateNode Target
        {
            get { return ((IIncrementalCheck)this).Target as IncrementByModifiedDateNode; }
            set { ((IIncrementalCheck)this).Target = value; }
        }

        public ModifiedDateCheck()
        {
        }

        public bool CanIncrement()
        {
            return this.IsSourceNewer(this.Source.ModifiedDate, this.Target.ModifiedDate);
        }

        private bool IsSourceNewer(DateTime sourceTimestamp, DateTime targetTimestamp)
        {
            TimeSpan universalTime = sourceTimestamp.ToUniversalTime() - targetTimestamp.ToUniversalTime();
            return universalTime.TotalSeconds > 1;
        }
    }
}