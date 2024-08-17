using Metalogix.Explorer;
using Metalogix.SharePoint;
using System;
using System.Management.Automation;

namespace Metalogix.SharePoint.Commands.Database
{
	[Cmdlet("Get", "MLSharePointFolderFromDatabase")]
	public class GetSharePointFolderFromDatabase : GetSharePointListFromDatabase
	{
		private string m_sFolderPath;

		[Parameter(Mandatory=true, Position=4, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="The list-relative path to the desired folder, separated by \"/\".")]
		public string FolderPath
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

		public GetSharePointFolderFromDatabase()
		{
		}

		protected SPFolder GetFolderFromParameters()
		{
			SPList listFromParameters = base.GetListFromParameters();
			string[] strArrays = this.FolderPath.Replace('\\', '/').Split(new char[] { '/' });
			SPFolder item = listFromParameters;
			string[] strArrays1 = strArrays;
			for (int i = 0; i < (int)strArrays1.Length; i++)
			{
				string str = strArrays1[i];
				item = item.SubFolders[str] as SPFolder;
				if (item == null)
				{
					base.ThrowTerminatingError(new ErrorRecord(new NullReferenceException(), "Could not find a folder at the specified path", ErrorCategory.InvalidArgument, this.FolderPath));
				}
			}
			return item;
		}

		protected override void ProcessRecord()
		{
			base.WriteObject(this.GetFolderFromParameters());
		}
	}
}