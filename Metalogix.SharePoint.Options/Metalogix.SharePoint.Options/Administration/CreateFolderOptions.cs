using Metalogix.Actions;
using System;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.Options.Administration
{
	public class CreateFolderOptions : ActionOptions
	{
		public string ContentType
		{
			get;
			set;
		}

		public string ContentTypeId
		{
			get;
			set;
		}

		public string FolderName
		{
			get;
			set;
		}

		public bool Overwrite
		{
			get;
			set;
		}

		public CreateFolderOptions()
		{
		}
	}
}