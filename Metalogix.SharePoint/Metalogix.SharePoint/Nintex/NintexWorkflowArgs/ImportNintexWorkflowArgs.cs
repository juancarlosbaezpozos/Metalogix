using System;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.Nintex.NintexWorkflowArgs
{
	public class ImportNintexWorkflowArgs : Metalogix.SharePoint.Nintex.NintexWorkflowArgs.NintexWorkflowArgs
	{
		public string ListTitle
		{
			get;
			set;
		}

		public string NwpFile
		{
			get;
			set;
		}

		public string WorkflowType
		{
			get;
			set;
		}

		public ImportNintexWorkflowArgs()
		{
		}
	}
}