using CommandLine;
using System;
using System.Diagnostics;
using System.IO;

namespace Metalogix.SharePoint.BlobUnshredder
{
	internal class Program
	{
		public Program()
		{
		}

		private static void Main(string[] args)
		{
			string empty = string.Empty;
			SharePointUnshredderArgs sharePointUnshredderArg = new SharePointUnshredderArgs();
			try
			{
				if (Parser.Default.ParseArguments(args, sharePointUnshredderArg))
				{
					Guid guid = new Guid(sharePointUnshredderArg.DocumentId);
					string outputFile = sharePointUnshredderArg.OutputFile;
					int uIVersion = sharePointUnshredderArg.UIVersion;
					string connectionString = sharePointUnshredderArg.ConnectionString;
					empty = guid.ToString();
					SharePointUnshredder sharePointUnshredder = new SharePointUnshredder(connectionString, sharePointUnshredderArg.ChunkSize);
					sharePointUnshredder.GetBlobUsingCobaltStream(guid, uIVersion, (byte)sharePointUnshredderArg.Level, outputFile);
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				string str = string.Format("An error occurred while retrieving binary contents of document '{0}' having version '{1}'. Error : '{2}'", empty, sharePointUnshredderArg.UIVersion, exception.Message);
				Console.Error.Write(str);
				Trace.WriteLine(exception.ToString());
				Console.Error.Flush();
			}
		}
	}
}