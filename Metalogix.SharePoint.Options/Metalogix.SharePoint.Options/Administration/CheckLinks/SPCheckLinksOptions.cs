using Metalogix.Actions;
using System;

namespace Metalogix.SharePoint.Options.Administration.CheckLinks
{
	public class SPCheckLinksOptions : ActionOptions
	{
		private bool m_bCheckSubsites = true;

		private bool m_bCheckWebparts;

		private bool m_bCheckTextFields;

		private bool m_bShowSuccesses;

		private int m_iPageResponseTimeout = 5000;

		public bool CheckSubsites
		{
			get
			{
				return this.m_bCheckSubsites;
			}
			set
			{
				this.m_bCheckSubsites = value;
			}
		}

		public bool CheckTextFields
		{
			get
			{
				return this.m_bCheckTextFields;
			}
			set
			{
				this.m_bCheckTextFields = value;
			}
		}

		public bool CheckWebparts
		{
			get
			{
				return this.m_bCheckWebparts;
			}
			set
			{
				this.m_bCheckWebparts = value;
			}
		}

		public int PageResponseTimeout
		{
			get
			{
				return this.m_iPageResponseTimeout;
			}
			set
			{
				this.m_iPageResponseTimeout = value;
			}
		}

		public bool ShowSuccesses
		{
			get
			{
				return this.m_bShowSuccesses;
			}
			set
			{
				this.m_bShowSuccesses = value;
			}
		}

		public SPCheckLinksOptions()
		{
		}
	}
}