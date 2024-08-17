using Metalogix.SharePoint.Workflow2013;
using System;

namespace Metalogix.SharePoint.Interfaces
{
	public interface ISupportSP2013Workflows
	{
		Metalogix.SharePoint.Workflow2013.SP2013WorkflowCollection SP2013WorkflowCollection
		{
			get;
			set;
		}
	}
}