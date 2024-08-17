using Metalogix.Actions;
using System;
using System.Collections;

namespace Metalogix.Transformers
{
    public abstract class
        SimpleTransformer<T, O> : Transformer<T, Metalogix.Actions.Action, IEnumerable, IEnumerable, O>
        where O : Metalogix.Transformers.TransformerOptions
    {
        protected SimpleTransformer()
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