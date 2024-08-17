using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Metalogix.Azure
{
	public class ListResponse : Response
	{
		public List<BlobInfo> Blobs
		{
			get;
			set;
		}

		public ListResponse()
		{
			this.Blobs = new List<BlobInfo>();
		}
	}
}