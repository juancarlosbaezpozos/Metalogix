using CommandLine;
using CommandLine.Text;
using System;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.BlobUnshredder
{
	public class SharePointUnshredderArgs
	{
		[Option('s', "chunkSize", HelpText="Chunk Size for reading data from a file at a time (Default: 512 Kb)")]
		public int ChunkSize
		{
			get;
			set;
		}

		[Option('c', "connnectionString", HelpText="Connection string to the database.", Required=true)]
		public string ConnectionString
		{
			get;
			set;
		}

		[Option('d', "documentId", HelpText="The unique document id.", Required=true)]
		public string DocumentId
		{
			get;
			set;
		}

		[Option('l', "level", HelpText="File level of the document (1, 2 or 255)")]
		public int Level
		{
			get;
			set;
		}

		[Option('o', "outputFile", HelpText="Save as file name.", Required=true)]
		public string OutputFile
		{
			get;
			set;
		}

		[Option('v', "uiVersion", HelpText="UI version of the document")]
		public int UIVersion
		{
			get;
			set;
		}

		public SharePointUnshredderArgs()
		{
			this.UIVersion = 0;
			this.Level = 0;
		}

		[HelpOption]
		public string GetUsage()
		{
			return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current), false);
		}
	}
}