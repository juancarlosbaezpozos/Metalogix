using Metalogix.Data.Filters;
using System;
using System.Collections.Generic;

namespace Metalogix.UI.WinForms.Data
{
	public class PluralOperandTranslationDictionary : Dictionary<FilterOperand, string>
	{
		public PluralOperandTranslationDictionary()
		{
			base.Add(FilterOperand.Equals, "equals");
			base.Add(FilterOperand.Contains, "contains");
			base.Add(FilterOperand.GreaterThan, "is greater than");
			base.Add(FilterOperand.GreaterThanOrEqualTo, "is greater than or equal to");
			base.Add(FilterOperand.IsNull, "is empty");
			base.Add(FilterOperand.LessThan, "is less than");
			base.Add(FilterOperand.LessThanOrEqualTo, "is less than or equal to");
			base.Add(FilterOperand.NotContains, "does not contain");
			base.Add(FilterOperand.NotEquals, "does not equal");
			base.Add(FilterOperand.NotNull, "is not empty");
			base.Add(FilterOperand.NotStartsWith, "does not start with");
			base.Add(FilterOperand.StartsWith, "starts with");
			base.Add(FilterOperand.NotEndsWith, "does not end with");
			base.Add(FilterOperand.EndsWith, "ends with");
			base.Add(FilterOperand.Regex, "matches regular expression");
			base.Add(FilterOperand.NotRegex, "does not match regular expression");
			base.Add(FilterOperand.IsNullOrBlank, "is empty or blank");
			base.Add(FilterOperand.NotNullAndNotBlank, "is not empty or blank");
		}
	}
}