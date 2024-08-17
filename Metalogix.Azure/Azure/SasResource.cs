using System;
using System.Runtime.CompilerServices;

namespace Metalogix.Azure
{
	public class SasResource
	{
		public string FullAccessUri
		{
			get;
			private set;
		}

		public string MigrationUri
		{
			get;
			private set;
		}

		public SasResource(string fullAccessUri, string migrationUri)
		{
			this.FullAccessUri = fullAccessUri;
			this.MigrationUri = migrationUri;
		}
	}
}