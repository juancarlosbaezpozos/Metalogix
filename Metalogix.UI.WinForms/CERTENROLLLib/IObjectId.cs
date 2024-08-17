using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CERTENROLLLib
{
	[System.Runtime.CompilerServices.CompilerGenerated, System.Runtime.InteropServices.Guid("728AB300-217D-11DA-B2A4-000E7BBB2B09"), TypeIdentifier]
	[System.Runtime.InteropServices.ComImport]
	public interface IObjectId
	{
		void _VtblGap1_2();

		[System.Runtime.InteropServices.DispId(1610743810)]
		void InitializeFromAlgorithmName([System.Runtime.InteropServices.In] ObjectIdGroupId GroupId, [System.Runtime.InteropServices.In] ObjectIdPublicKeyFlags KeyFlags, [System.Runtime.InteropServices.In] AlgorithmFlags AlgFlags, [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.BStr)] [System.Runtime.InteropServices.In] string strAlgorithmName);
	}
}
