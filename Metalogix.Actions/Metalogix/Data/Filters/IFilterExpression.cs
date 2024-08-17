using Metalogix.Data;
using System;
using System.Collections;

namespace Metalogix.Data.Filters
{
    public interface IFilterExpression : IEquatable<IFilterExpression>, IXmlable
    {
        IFilterExpression Clone();

        bool Evaluate(object target);

        bool Evaluate(object target, IComparer comparer);

        bool Evaluate(object target, Comparison<object> comparer);

        string GetExpressionString();

        string GetLogicString();
    }
}