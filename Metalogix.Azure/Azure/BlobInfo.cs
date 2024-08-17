using System;
using System.Runtime.CompilerServices;

namespace Metalogix.Azure
{
	public class BlobInfo
	{
		public string BlobName
		{
			get;
			set;
		}

		public string BlobType
		{
			get;
			set;
		}

		public string ContentType
		{
			get;
			set;
		}

		public long Filesize
		{
			get;
			set;
		}

		public BlobInfo()
		{
		}
	}
}