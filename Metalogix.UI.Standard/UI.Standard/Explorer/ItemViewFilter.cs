using System;

namespace Metalogix.UI.Standard.Explorer
{
	public class ItemViewFilter
	{
		private string m_sFilterExpression;

		private string m_sColumnName;

		public string ColumnName
		{
			get
			{
				return this.m_sColumnName;
			}
			set
			{
				this.m_sColumnName = value;
			}
		}

		public string FilterExpression
		{
			get
			{
				return this.m_sFilterExpression;
			}
			set
			{
				this.m_sFilterExpression = value;
			}
		}

		public ItemViewFilter(string sFilterExpression, string sColumnName)
		{
			this.m_sFilterExpression = sFilterExpression;
			this.m_sColumnName = sColumnName;
		}
	}
}