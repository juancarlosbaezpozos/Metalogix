using Metalogix.Actions;
using System;
using System.Runtime.CompilerServices;

namespace Metalogix.Jobs.Options
{
    public class JobRunnerOptions : ActionOptions
    {
        public bool RunAsUserName { get; set; }

        public string UserName { get; set; }

        public JobRunnerOptions()
        {
        }
    }
}