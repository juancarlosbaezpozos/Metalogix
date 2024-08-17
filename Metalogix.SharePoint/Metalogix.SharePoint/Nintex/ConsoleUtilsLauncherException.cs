using System;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.Nintex
{
	public sealed class ConsoleUtilsLauncherException : Exception
	{
		public string Args
		{
			get;
			private set;
		}

		public ConsoleUtilsLauncherException(string args, string message) : base(message)
		{
			this.Args = args;
		}
	}
}