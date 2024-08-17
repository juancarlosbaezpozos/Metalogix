using System;
using System.Runtime.CompilerServices;

namespace Metalogix.Azure
{
	public class ContainerCreateResponse : Response
	{
		public string PrimaryUri
		{
			get;
			set;
		}

		public string SecondaryUri
		{
			get;
			set;
		}

		public string SharedAccessSignature
		{
			get;
			set;
		}

		public ContainerCreateResponse()
		{
			this.SharedAccessSignature = string.Empty;
			this.PrimaryUri = string.Empty;
			this.SecondaryUri = string.Empty;
		}
	}
}