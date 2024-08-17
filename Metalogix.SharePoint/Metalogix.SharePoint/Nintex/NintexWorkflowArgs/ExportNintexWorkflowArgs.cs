using System;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.Nintex.NintexWorkflowArgs
{
	public class ExportNintexWorkflowArgs : Metalogix.SharePoint.Nintex.NintexWorkflowArgs.NintexWorkflowArgs
	{
		public string EndpointUrl
		{
			get;
			set;
		}

		public string ListTitle
		{
			get;
			set;
		}

		public string NwfFile
		{
			get;
			set;
		}

		public string NwpFile
		{
			get;
			set;
		}

		public string WorkflowName
		{
			get;
			set;
		}

		public string WorkflowType
		{
			get;
			set;
		}

		public ExportNintexWorkflowArgs()
		{
		}
	}
}