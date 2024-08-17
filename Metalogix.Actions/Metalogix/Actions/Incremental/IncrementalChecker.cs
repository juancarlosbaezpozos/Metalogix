using System;
using System.Runtime.CompilerServices;

namespace Metalogix.Actions.Incremental
{
    public sealed class IncrementalChecker
    {
        public IIncrementalCheck[] IncrementalChecks { get; set; }

        public IncrementalChecker()
        {
        }

        public bool Check(IIncrementableNode source, IIncrementableNode target)
        {
            if (this.IncrementalChecks == null)
            {
                return false;
            }

            IIncrementalCheck[] incrementalChecks = this.IncrementalChecks;
            for (int i = 0; i < (int)incrementalChecks.Length; i++)
            {
                IIncrementalCheck incrementalCheck = incrementalChecks[i];
                incrementalCheck.Source = source;
                incrementalCheck.Target = target;
                if (incrementalCheck.CanIncrement())
                {
                    return true;
                }
            }

            return false;
        }
    }
}