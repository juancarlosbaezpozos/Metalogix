using System;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.Nintex.NintexWorkflowArgs
{
	public class PublishNintexWorkflowArgs : Metalogix.SharePoint.Nintex.NintexWorkflowArgs.NintexWorkflowArgs
	{
		public Guid WorkflowId
		{
			get;
			set;
		}

		public PublishNintexWorkflowArgs()
		{
		}
	}
}