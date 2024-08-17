using Metalogix.SharePoint.Options;
using System;

namespace Metalogix.SharePoint.Options.Administration
{
	public class PublishDocumentsAndPagesOptions : SharePointActionOptions
	{
		private bool m_bPublish;

		private bool m_bCheckin;

		private bool m_bApprove;

		private string m_sPublishComment = string.Empty;

		private string m_sCheckinComment = string.Empty;

		private string m_sApproveComment = string.Empty;

		public bool Approve
		{
			get
			{
				return this.m_bApprove;
			}
			set
			{
				this.m_bApprove = value;
			}
		}

		public string ApproveComment
		{
			get
			{
				return this.m_sApproveComment;
			}
			set
			{
				this.m_sApproveComment = value;
			}
		}

		public bool Checkin
		{
			get
			{
				return this.m_bCheckin;
			}
			set
			{
				this.m_bCheckin = value;
			}
		}

		public string CheckinComment
		{
			get
			{
				return this.m_sCheckinComment;
			}
			set
			{
				this.m_sCheckinComment = value;
			}
		}

		public bool Publish
		{
			get
			{
				return this.m_bPublish;
			}
			set
			{
				this.m_bPublish = value;
			}
		}

		public string PublishComment
		{
			get
			{
				return this.m_sPublishComment;
			}
			set
			{
				this.m_sPublishComment = value;
			}
		}

		public PublishDocumentsAndPagesOptions()
		{
		}
	}
}