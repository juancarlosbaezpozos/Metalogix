using System;
using System.Collections.Generic;

namespace Metalogix.Actions.Incremental
{
    public class IncrementCheckerBuilder
    {
        private readonly List<IIncrementalCheck> _incrementalChecks = new List<IIncrementalCheck>();

        public IncrementCheckerBuilder()
        {
        }

        public IncrementCheckerBuilder AddCheck(IIncrementalCheck incrementalCheck)
        {
            this._incrementalChecks.Add(incrementalCheck);
            return this;
        }

        public IncrementalChecker Build()
        {
            return new IncrementalChecker()
            {
                IncrementalChecks = this._incrementalChecks.ToArray()
            };
        }
    }
}