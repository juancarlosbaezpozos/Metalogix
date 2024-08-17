using System;
using System.Collections.Generic;

namespace Metalogix.UI.CommandLine
{
	internal class JobRunnerSandboxOptions
	{
		public IEnumerable<int> JobNums;

		public IEnumerable<string> JobIDs;

		public IEnumerable<string> JobNames;

		public bool RunAllJobs;

		public bool RunAsync;

		public JobRunnerSandboxOptions()
		{
		}
	}
}