using Metalogix.SharePoint;
using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Commands
{
	public abstract class GetSharePointItem : GetSharePointFolder
	{
		private string m_sFolderPath;

		[Parameter(Mandatory=false, Position=2, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="The list-relative path to the desired folder, separated by \"/\".")]
		public override string FolderPath
		{
			get
			{
				return this.m_sFolderPath;
			}
			set
			{
				this.m_sFolderPath = value;
			}
		}

		protected GetSharePointItem()
		{
		}

		protected abstract SPListItem GetItemFromParameters();
	}
}