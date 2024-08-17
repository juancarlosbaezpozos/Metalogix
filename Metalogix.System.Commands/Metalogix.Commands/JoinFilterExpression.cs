using Metalogix.Data.Filters;
using Metalogix.DataStructures.Generic;
using System;
using System.Management.Automation;

namespace Metalogix.Commands
{
	[Cmdlet("Join", "Filter")]
	public class JoinFilterExpression : Cmdlet
	{
		private string m_sLogic;

		public IFilterExpression[] m_filter;

		private FilterExpressionList m_list;

		[Parameter(Mandatory=true, Position=1, HelpMessage="The filter object to join together. Can be a set of filters).")]
		public IFilterExpression[] FilterExpression
		{
			get
			{
				return this.m_filter;
			}
			set
			{
				this.m_filter = value;
			}
		}

		public FilterExpressionList List
		{
			get
			{
				return this.m_list;
			}
			set
			{
				this.m_list = value;
			}
		}

		[Parameter(Mandatory=true, Position=0, HelpMessage="The logic to use in the join (and or or).")]
		public string Logic
		{
			get
			{
				return this.m_sLogic;
			}
			set
			{
				this.m_sLogic = value;
			}
		}

		public JoinFilterExpression()
		{
		}

		protected override void BeginProcessing()
		{
			base.BeginProcessing();
			ExpressionLogic expressionLogic = (ExpressionLogic)Enum.Parse(typeof(ExpressionLogic), this.Logic);
			this.m_list = new FilterExpressionList(expressionLogic);
		}

		protected override void EndProcessing()
		{
			base.WriteObject(this.List);
		}

		protected override void ProcessRecord()
		{
			this.List.AddRange(this.FilterExpression);
		}
	}
}