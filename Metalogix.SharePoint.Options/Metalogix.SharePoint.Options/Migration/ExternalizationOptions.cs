using Metalogix;
using System;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.Options.Migration
{
	public class ExternalizationOptions : OptionsBase
	{
		public bool SideLoadDocuments
		{
			get;
			set;
		}

		public ExternalizationOptions()
		{
			this.SideLoadDocuments = true;
		}
	}
}