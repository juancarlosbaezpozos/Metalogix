using Metalogix.Data.Filters;
using System;
using System.Management.Automation;

namespace Metalogix.Commands
{
	[Cmdlet("New", "Filter")]
	public class NewFilterExpressionCmdlet : AssemblyBindingCmdlet
	{
		private string m_sPropertyName;

		private string m_sOperand;

		private object m_oValue;

		private bool m_bCaseSensitive;

		private string m_sTypeName;

		[Parameter(HelpMessage="Indicates if the comparison should be case-sensitive. This parameter is only meaningful when the property to be compared to is a string.")]
		public SwitchParameter CaseSensitive
		{
			get
			{
				return this.m_bCaseSensitive;
			}
			set
			{
				this.m_bCaseSensitive = value;
			}
		}

		[Parameter(Mandatory=true, Position=1, HelpMessage="The operand to use to compare the specified value to the given property value.")]
		public string Operand
		{
			get
			{
				return this.m_sOperand;
			}
			set
			{
				this.m_sOperand = value;
			}
		}

		[Parameter(Mandatory=true, Position=0, HelpMessage="The name of the parameter to compare the value to on the object being filtered.")]
		public string PropertyName
		{
			get
			{
				return this.m_sPropertyName;
			}
			set
			{
				this.m_sPropertyName = value;
			}
		}

		[Parameter(Mandatory=false, Position=3, HelpMessage="The name of a type to apply the filter to. If not specified, the filter will apply to any object.")]
		public string TypeName
		{
			get
			{
				return this.m_sTypeName;
			}
			set
			{
				this.m_sTypeName = value;
			}
		}

		[Parameter(Mandatory=true, Position=2, HelpMessage="The value to compare the given property to.")]
		public object Value
		{
			get
			{
				return this.m_oValue;
			}
			set
			{
				this.m_oValue = value;
			}
		}

		public NewFilterExpressionCmdlet()
		{
		}

		private static FilterOperand ParseOperand(string sOperand)
		{
			FilterOperand filterOperand;
			try
			{
				filterOperand = (FilterOperand)Enum.Parse(typeof(FilterOperand), sOperand, true);
			}
			catch (ArgumentException argumentException)
			{
				string lower = sOperand.ToLower();
				string str = lower;
				if (lower != null)
				{
					switch (str)
					{
						case "<":
						{
							filterOperand = FilterOperand.LessThan;
							return filterOperand;
						}
						case "lt":
						{
							filterOperand = FilterOperand.LessThan;
							return filterOperand;
						}
						case "before":
						{
							filterOperand = FilterOperand.LessThan;
							return filterOperand;
						}
						case "<=":
						{
							filterOperand = FilterOperand.LessThanOrEqualTo;
							return filterOperand;
						}
						case "le":
						{
							filterOperand = FilterOperand.LessThanOrEqualTo;
							return filterOperand;
						}
						case ">":
						{
							filterOperand = FilterOperand.GreaterThan;
							return filterOperand;
						}
						case "gt":
						{
							filterOperand = FilterOperand.GreaterThan;
							return filterOperand;
						}
						case "after":
						{
							filterOperand = FilterOperand.GreaterThan;
							return filterOperand;
						}
						case ">=":
						{
							filterOperand = FilterOperand.GreaterThanOrEqualTo;
							return filterOperand;
						}
						case "ge":
						{
							filterOperand = FilterOperand.GreaterThanOrEqualTo;
							return filterOperand;
						}
						case "=":
						{
							filterOperand = FilterOperand.Equals;
							return filterOperand;
						}
						case "==":
						{
							filterOperand = FilterOperand.Equals;
							return filterOperand;
						}
						case "eq":
						{
							filterOperand = FilterOperand.Equals;
							return filterOperand;
						}
						case "is":
						{
							filterOperand = FilterOperand.Equals;
							return filterOperand;
						}
						case "!=":
						{
							filterOperand = FilterOperand.NotEquals;
							return filterOperand;
						}
						case "<>":
						{
							filterOperand = FilterOperand.NotEquals;
							return filterOperand;
						}
						case "ne":
						{
							filterOperand = FilterOperand.NotEquals;
							return filterOperand;
						}
						case "is not":
						{
							filterOperand = FilterOperand.NotEquals;
							return filterOperand;
						}
						case "match":
						{
							filterOperand = FilterOperand.Regex;
							return filterOperand;
						}
						case "matches":
						{
							filterOperand = FilterOperand.Regex;
							return filterOperand;
						}
						case "notmatch":
						{
							filterOperand = FilterOperand.NotRegex;
							return filterOperand;
						}
					}
				}
				throw new ArgumentException("The string does not represent a known filter operand");
			}
			return filterOperand;
		}

		protected override void ProcessRecord()
		{
			string str = this.m_oValue.ToString();
			if (this.Value is DateTime)
			{
				str = ((DateTime)this.Value).ToString();
			}
			FilterOperand filterOperand = NewFilterExpressionCmdlet.ParseOperand(this.Operand);
			FilterExpression filterExpression = new FilterExpression(filterOperand, this.TypeName, this.PropertyName, str, this.CaseSensitive, false);
			base.WriteObject(filterExpression);
		}
	}
}