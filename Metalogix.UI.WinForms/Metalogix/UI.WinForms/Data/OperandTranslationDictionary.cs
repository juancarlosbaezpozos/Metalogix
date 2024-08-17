using Metalogix.Data.Filters;
using System;
using System.Collections.Generic;

namespace Metalogix.UI.WinForms.Data
{
	public class OperandTranslationDictionary : Dictionary<FilterOperand, string>
	{
		public OperandTranslationDictionary()
		{
			base.Add(FilterOperand.Equals, "equal");
			base.Add(FilterOperand.Contains, "contain");
			base.Add(FilterOperand.GreaterThan, "be greater than");
			base.Add(FilterOperand.GreaterThanOrEqualTo, "be greater than or equal to");
			base.Add(FilterOperand.IsNull, "be empty");
			base.Add(FilterOperand.LessThan, "be less than");
			base.Add(FilterOperand.LessThanOrEqualTo, "be less than or equal to");
			base.Add(FilterOperand.NotContains, "not contain");
			base.Add(FilterOperand.NotEquals, "not equal");
			base.Add(FilterOperand.NotNull, "not be empty");
			base.Add(FilterOperand.NotStartsWith, "not start with");
			base.Add(FilterOperand.StartsWith, "start with");
			base.Add(FilterOperand.NotEndsWith, "not end with");
			base.Add(FilterOperand.EndsWith, "end with");
			base.Add(FilterOperand.Regex, "match regular expression");
			base.Add(FilterOperand.NotRegex, "does not match regular expression");
			base.Add(FilterOperand.IsNullOrBlank, "be empty or blank");
			base.Add(FilterOperand.NotNullAndNotBlank, "not be empty or blank");
		}
	}
}