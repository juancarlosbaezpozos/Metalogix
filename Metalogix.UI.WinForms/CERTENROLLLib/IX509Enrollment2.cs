using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CERTENROLLLib
{
	[System.Runtime.CompilerServices.CompilerGenerated, System.Runtime.InteropServices.Guid("728AB350-217D-11DA-B2A4-000E7BBB2B09"), TypeIdentifier]
	[System.Runtime.InteropServices.ComImport]
	public interface IX509Enrollment2 : IX509Enrollment
	{
		string CertificateFriendlyName
		{
			[System.Runtime.InteropServices.DispId(1610743825)]
			[return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.BStr)]
			get;
			[System.Runtime.InteropServices.DispId(1610743825)]
			[param: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.BStr)]
			set;
		}

		void _VtblGap1_2();

		[System.Runtime.InteropServices.DispId(1610743810)]
		void InitializeFromRequest([System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Interface)] [System.Runtime.InteropServices.In] IX509CertificateRequest pRequest);

		[System.Runtime.InteropServices.DispId(1610743811)]
		[return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.BStr)]
		string CreateRequest([System.Runtime.InteropServices.In] EncodingType Encoding = EncodingType.XCN_CRYPT_STRING_BASE64);

		void _VtblGap2_1();

		[System.Runtime.InteropServices.DispId(1610743813)]
		void InstallResponse([System.Runtime.InteropServices.In] InstallResponseRestrictionFlags Restrictions, [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.BStr)] [System.Runtime.InteropServices.In] string strResponse, [System.Runtime.InteropServices.In] EncodingType Encoding, [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.BStr)] [System.Runtime.InteropServices.In] string strPassword);

		[System.Runtime.InteropServices.DispId(1610743814)]
		[return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.BStr)]
		string CreatePFX([System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.BStr)] [System.Runtime.InteropServices.In] string strPassword, [System.Runtime.InteropServices.In] PFXExportOptions ExportOptions, [System.Runtime.InteropServices.In] EncodingType Encoding = EncodingType.XCN_CRYPT_STRING_BASE64);

		void _VtblGap3_10();
	}
}
