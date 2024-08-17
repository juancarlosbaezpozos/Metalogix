using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Metalogix.SharePoint
{
	[DataContract]
	public class SPOTenantStorageMetricReport
	{
		[DataMember(Name="d")]
		public List<SPOTenantReporting> SPOTenantStorageDetails
		{
			get;
			set;
		}

		public SPOTenantStorageMetricReport()
		{
		}
	}
}