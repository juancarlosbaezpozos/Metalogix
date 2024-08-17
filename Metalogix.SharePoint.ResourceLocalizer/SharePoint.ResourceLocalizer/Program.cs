using Microsoft.SharePoint.Utilities;
using System;
using System.Diagnostics;
using System.IO;

namespace Metalogix.SharePoint.ResourceLocalizer
{
	internal class Program
	{
		public Program()
		{
		}

		private static void Main(string[] args)
		{
			uint num = 0;
			string empty = string.Empty;
			try
			{
				if ((int)args.Length != 2)
				{
					throw new ArgumentNullException("Valid arguments are not provided");
				}
				num = uint.Parse(args[0]);
				empty = args[1];
				Console.Write(SPUtility.GetLocalizedString(empty, null, num));
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				string str = string.Format("An error occurred while retrieving localized name for resource '{0}' having language '{1}'. Error '{2}'", empty, num, exception.Message);
				Console.Error.Write(str);
				Trace.WriteLine(exception.ToString());
				Console.Error.Flush();
			}
		}
	}
}