using Metalogix.DataStructures;
using System;
using System.Collections.Generic;

namespace Metalogix.UI.WinForms.Components
{
	public interface ICertificateInclusionOptionsContainer
	{
		IEnumerable<X509CertificateWrapper> Certificates
		{
			get;
			set;
		}
	}
}