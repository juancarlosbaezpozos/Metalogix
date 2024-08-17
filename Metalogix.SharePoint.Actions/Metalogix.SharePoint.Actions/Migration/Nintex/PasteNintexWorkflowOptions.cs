using Metalogix.SharePoint.Options;
using System;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.Actions.Migration.Nintex
{
	public class PasteNintexWorkflowOptions : SharePointActionOptions
	{
		public bool CopyContentTypeSharePointDesignerNintexWorkflowAssociations
		{
			get;
			set;
		}

		public bool CopyListSharePointDesignerNintexWorkflowAssociations
		{
			get;
			set;
		}

		public bool CopyWebSharePointDesignerNintexWorkflowAssociations
		{
			get;
			set;
		}

		public PasteNintexWorkflowOptions()
		{
		}
	}
}