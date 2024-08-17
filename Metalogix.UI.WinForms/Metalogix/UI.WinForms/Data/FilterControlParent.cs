using DevExpress.XtraEditors;
using Metalogix.Data.Filters;
using Metalogix.DataStructures.Generic;
using Metalogix.UI.WinForms.Data.Filters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Data
{
	public class FilterControlParent : XtraUserControl
	{
		protected List<Type> TypeFilters = new List<Type>(new Type[] { typeof(object) });

		protected bool AllowFreeFormFilterEntry;

		private IContainer components;

		public FilterBuilderType FilterType
		{
			get
			{
				return new FilterBuilderType(this.TypeFilters, this.AllowFreeFormFilterEntry);
			}
			set
			{
				this.TypeFilters = value.ObjectTypes;
				this.AllowFreeFormFilterEntry = value.AllowFreeFormEntry;
			}
		}

		public FilterControlParent()
		{
			this.InitializeComponent();
		}

		protected void BuildExclusionString(IFilterExpression iFilter, StringBuilder sb)
		{
			if (!(iFilter is FilterExpressionList))
			{
				FilterExpression filterExpression = (FilterExpression)iFilter;
				sb.AppendFormat("\"{0}\" must {1}", filterExpression.Property, Metalogix.UI.WinForms.Data.Filters.FilterControl.Translator[filterExpression.Operand]);
				if (filterExpression.Pattern != null)
				{
					sb.AppendFormat(" \"{0}\"", filterExpression.Pattern);
				}
			}
			else
			{
				FilterExpressionList filterExpressionList = (FilterExpressionList)iFilter;
				string str = string.Concat(" ", filterExpressionList.Logic.ToString(), " \n");
				int num = 0;
				foreach (IFilterExpression filterExpression1 in filterExpressionList)
				{
					this.BuildExclusionString(filterExpression1, sb);
					num++;
					if (num == filterExpressionList.Count)
					{
						continue;
					}
					sb.Append(str);
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		}
	}
}