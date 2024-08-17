using Metalogix.Actions;
using System;
using System.Collections;

namespace Metalogix.Transformers
{
    public abstract class
        SimplePreconfiguredTransformer<T> : PreconfiguredTransformer<T, Metalogix.Actions.Action, IEnumerable,
        IEnumerable>
    {
        protected SimplePreconfiguredTransformer()
        {
        }

        public override void BeginTransformation(Metalogix.Actions.Action action, IEnumerable sources,
            IEnumerable targets)
        {
        }

        public override void EndTransformation(Metalogix.Actions.Action action, IEnumerable sources,
            IEnumerable targets)
        {
        }

        public override T Transform(T dataObject, Metalogix.Actions.Action action, IEnumerable sources,
            IEnumerable targets)
        {
            return this.Transform(dataObject);
        }

        protected abstract T Transform(T dataObject);
    }
}