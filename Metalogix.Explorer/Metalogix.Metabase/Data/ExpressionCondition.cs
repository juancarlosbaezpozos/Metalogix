using System;
using System.ComponentModel;

namespace Metalogix.Metabase.Data
{
    public enum ExpressionCondition
    {
        [Description("Must contain")] MustContainAll,
        [Description("Must not contain")] MustNotContainAny,
        [Description("Must contain one of")] MustContainAny
    }
}