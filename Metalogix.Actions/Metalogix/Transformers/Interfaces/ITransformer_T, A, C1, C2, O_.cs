using Metalogix.Actions;
using Metalogix.Data;
using Metalogix.Transformers;
using System;
using System.Collections;

namespace Metalogix.Transformers.Interfaces
{
    public interface ITransformer<T, A, C1, C2, O> : IXmlable, ITransformer
        where A : Metalogix.Actions.Action
        where C1 : IEnumerable
        where C2 : IEnumerable
        where O : Metalogix.Transformers.TransformerOptions
    {
        O Options { get; }

        void BeginTransformation(A action, C1 sources, C2 targets);

        void EndTransformation(A action, C1 sources, C2 targets);

        T Transform(T dataObject, A action, C1 sources, C2 targets);
    }
}