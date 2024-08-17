using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Metalogix.SharePoint
{
	[DataContract]
	public class SPOTenantReporting
	{
		[DataMember]
		public string TenantGuid
		{
			get;
			set;
		}

		[DataMember]
		public string Used
		{
			get;
			set;
		}

		public SPOTenantReporting()
		{
		}
	}
}