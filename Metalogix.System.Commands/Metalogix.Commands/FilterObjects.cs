using Metalogix.Data.Filters;
using System;
using System.Management.Automation;

namespace Metalogix.Commands
{
	[Cmdlet("Invoke", "Filter")]
	public class FilterObjects : Cmdlet
	{
		private object[] m_objects;

		private IFilterExpression m_filter;

		[Parameter(Mandatory=true, Position=0)]
		public IFilterExpression Filter
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

		[Parameter(Mandatory=true, ValueFromPipeline=true, Position=1)]
		public object[] Objects
		{
			get
			{
				return this.m_objects;
			}
			set
			{
				this.m_objects = value;
			}
		}

		public FilterObjects()
		{
		}

		protected override void ProcessRecord()
		{
			object[] objects = this.Objects;
			for (int i = 0; i < (int)objects.Length; i++)
			{
				object obj = objects[i];
				if (this.Filter.Evaluate(obj))
				{
					base.WriteObject(obj);
				}
			}
		}
	}
}