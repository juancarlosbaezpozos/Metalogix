using System;

namespace Metalogix.SharePoint.Administration.LinkManagement
{
	public class ErrorEncounteredEventArgs : EventArgs
	{
		private readonly string m_sMessage;

		private readonly string m_sURL;

		public string Message
		{
			get
			{
				return this.m_sMessage;
			}
		}

		public string URL
		{
			get
			{
				return this.m_sURL;
			}
		}

		public ErrorEncounteredEventArgs(string sMessage, string sUrl)
		{
			this.m_sMessage = sMessage;
			this.m_sURL = sUrl;
		}
	}
}