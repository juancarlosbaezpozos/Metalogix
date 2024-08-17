using Metalogix.Actions;
using System;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.Options.Migration
{
	public class StoragePointOptions : ActionOptions
	{
		public bool SideLoadDocuments
		{
			get;
			set;
		}

		public StoragePointOptions()
		{
			this.SideLoadDocuments = true;
		}
	}
}