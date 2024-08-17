using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CERTENROLLLib
{
	[System.Runtime.CompilerServices.CompilerGenerated, System.Runtime.InteropServices.Guid("728AB35A-217D-11DA-B2A4-000E7BBB2B09"), TypeIdentifier]
	[System.Runtime.InteropServices.ComImport]
	public interface IX509CertificateRequestCertificate2 : IX509CertificateRequestCertificate, IX509CertificateRequestPkcs10, IX509CertificateRequest
	{
		CObjectId HashAlgorithm
		{
			[System.Runtime.InteropServices.DispId(1610743828)]
			[return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Interface)]
			get;
			[System.Runtime.InteropServices.DispId(1610743828)]
			[param: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Interface)]
			set;
		}

		CX500DistinguishedName Subject
		{
			[System.Runtime.InteropServices.DispId(1610809357)]
			[return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Interface)]
			get;
			[System.Runtime.InteropServices.DispId(1610809357)]
			[param: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Interface)]
			set;
		}

		CX500DistinguishedName Issuer
		{
			[System.Runtime.InteropServices.DispId(1610874881)]
			[return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Interface)]
			get;
			[System.Runtime.InteropServices.DispId(1610874881)]
			[param: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Interface)]
			set;
		}

		System.DateTime NotBefore
		{
			[System.Runtime.InteropServices.DispId(1610874883)]
			get;
			[System.Runtime.InteropServices.DispId(1610874883)]
			set;
		}

		System.DateTime NotAfter
		{
			[System.Runtime.InteropServices.DispId(1610874885)]
			get;
			[System.Runtime.InteropServices.DispId(1610874885)]
			set;
		}

		void _VtblGap1_1();

		[System.Runtime.InteropServices.DispId(1610743809)]
		void Encode();

		void _VtblGap2_18();

		void _VtblGap3_4();

		[System.Runtime.InteropServices.DispId(1610809345)]
		void InitializeFromPrivateKey([System.Runtime.InteropServices.In] X509CertificateEnrollmentContext Context, [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Interface)] [System.Runtime.InteropServices.In] CX509PrivateKey pPrivateKey, [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.BStr)] [System.Runtime.InteropServices.In] string strTemplateName);

		void _VtblGap4_11();

		void _VtblGap5_14();
	}
}
