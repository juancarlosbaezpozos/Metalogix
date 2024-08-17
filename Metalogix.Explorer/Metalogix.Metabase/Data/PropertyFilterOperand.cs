using System;

namespace Metalogix.Metabase.Data
{
    public enum PropertyFilterOperand
    {
        Equals,
        StartsWith,
        NotContains,
        Contains,
        ContainedBy,
        RegularExpression,
        EndsWith
    }
}