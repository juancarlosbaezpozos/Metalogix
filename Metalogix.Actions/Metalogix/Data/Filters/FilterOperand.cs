using Metalogix.Data;
using System;

namespace Metalogix.Data.Filters
{
    public enum FilterOperand
    {
        [IsNegation(false)] Equals,
        [IsNegation(true)] NotEquals,
        [IsNegation(false)] StartsWith,
        [IsNegation(true)] NotStartsWith,
        [IsNegation(false)] EndsWith,
        [IsNegation(true)] NotEndsWith,
        [IsNegation(false)] Contains,
        [IsNegation(true)] NotContains,
        [IsNegation(false)] IsNull,
        [IsNegation(true)] NotNull,
        [IsNegation(false)] IsNullOrBlank,
        [IsNegation(true)] NotNullAndNotBlank,
        [IsNegation(false)] GreaterThan,
        [IsNegation(false)] GreaterThanOrEqualTo,
        [IsNegation(false)] LessThan,
        [IsNegation(false)] LessThanOrEqualTo,
        [IsNegation(false)] Regex,
        [IsNegation(true)] NotRegex
    }
}