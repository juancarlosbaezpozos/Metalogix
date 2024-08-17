using Metalogix.Actions;
using Metalogix.DataStructures.Generic;
using System;
using System.Runtime.CompilerServices;

namespace Metalogix.Jobs.Reporting.Options
{
	public class ExportJobsToExcelOption : ActionOptions
	{
		public string ConnectionString
		{
			get;
			set;
		}

		public string Directory
		{
			get;
			set;
		}

		public string FileName
		{
			get;
			set;
		}

		public bool IsSQLServer
		{
			get;
			set;
		}

		public CommonSerializableList<string> JobIds
		{
			get;
			set;
		}

		public ExportJobsToExcelOption()
		{
		}
	}
}