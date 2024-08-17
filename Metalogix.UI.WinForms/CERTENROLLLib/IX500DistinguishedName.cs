using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CERTENROLLLib
{
	[System.Runtime.CompilerServices.CompilerGenerated, System.Runtime.InteropServices.Guid("728AB303-217D-11DA-B2A4-000E7BBB2B09"), TypeIdentifier]
	[System.Runtime.InteropServices.ComImport]
	public interface IX500DistinguishedName
	{
		void _VtblGap1_1();

		[System.Runtime.InteropServices.DispId(1610743809)]
		void Encode([System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.BStr)] [System.Runtime.InteropServices.In] string strName, [System.Runtime.InteropServices.In] X500NameFlags NameFlags = X500NameFlags.XCN_CERT_NAME_STR_NONE);
	}
}
