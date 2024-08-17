using System;

namespace Metalogix.UI.WinForms.Attributes
{
	public class LicenseWarningMessagesAttribute : UIAttribute
	{
		private readonly string m_trialWarningMessage;

		public virtual string TrialWarningMessage
		{
			get
			{
				return this.m_trialWarningMessage;
			}
		}

		public LicenseWarningMessagesAttribute(string trialWarningMessage)
		{
			this.m_trialWarningMessage = trialWarningMessage;
		}
	}
}