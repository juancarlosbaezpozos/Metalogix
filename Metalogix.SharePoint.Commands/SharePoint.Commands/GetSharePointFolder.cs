using Metalogix.Explorer;
using Metalogix.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.Commands
{
	[Cmdlet("Get", "MLSharePointFolder")]
	public class GetSharePointFolder : GetSharePointList
	{
		private string m_sFolderPath;

		[Parameter(Mandatory=true, Position=2, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="The list-relative path to the desired folder, seperated by \"/\".")]
		public virtual string FolderPath
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

		public GetSharePointFolder()
		{
		}

		protected SPFolder GetFolderFromParameters()
		{
			SPList listFromParameters = base.GetListFromParameters();
			if (this.FolderPath == null)
			{
				return listFromParameters;
			}
			SPFolder item = listFromParameters;
			string folderPath = this.FolderPath;
			char[] chrArray = new char[] { '/', '\\' };
			List<string> list = folderPath.Split(chrArray).ToList<string>();
			if (string.IsNullOrEmpty(list.First<string>()))
			{
				list.RemoveAt(0);
			}
			list.ForEach((string sfolderChunk) => {
				item = item.SubFolders[sfolderChunk] as SPFolder;
				if (item == null)
				{
					base.ThrowTerminatingError(new ErrorRecord(new NullReferenceException(), "Could not find a folder at the specified path", ErrorCategory.InvalidArgument, this.FolderPath));
				}
			});
			return item;
		}

		protected override void ProcessRecord()
		{
			base.WriteObject(this.GetFolderFromParameters());
		}
	}
}