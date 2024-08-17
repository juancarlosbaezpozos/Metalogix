using Metalogix.SharePoint.Options;
using System;

namespace Metalogix.SharePoint.Options.Administration
{
	public class AnalyzeChurnOptions : SharePointActionOptions
	{
		private DateTime m_dtPivot = DateTime.Now;

		private bool m_bAnalyzeRecursively = true;

		public bool AnalyzeRecursively
		{
			get
			{
				return this.m_bAnalyzeRecursively;
			}
			set
			{
				this.m_bAnalyzeRecursively = value;
			}
		}

		public DateTime PivotDate
		{
			get
			{
				return this.m_dtPivot;
			}
			set
			{
				this.m_dtPivot = value;
			}
		}

		public AnalyzeChurnOptions()
		{
		}
	}
}