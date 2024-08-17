using System;

namespace Metalogix.Actions
{
    [Flags]
    public enum Cardinality
    {
        Zero = 1,
        One = 2,
        ZeroOrOne = 3,
        MoreThanOne = 4,
        OneOrMore = 6,
        ZeroOrMore = 7
    }
}