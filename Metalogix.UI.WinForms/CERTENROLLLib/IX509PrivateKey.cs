using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CERTENROLLLib
{
	[System.Runtime.CompilerServices.CompilerGenerated, System.Runtime.InteropServices.Guid("728AB30C-217D-11DA-B2A4-000E7BBB2B09"), TypeIdentifier]
	[System.Runtime.InteropServices.ComImport]
	public interface IX509PrivateKey
	{
		string ProviderName
		{
			[System.Runtime.InteropServices.DispId(1610743826)]
			[return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.BStr)]
			get;
			[System.Runtime.InteropServices.DispId(1610743826)]
			[param: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.BStr)]
			set;
		}

		X509KeySpec KeySpec
		{
			[System.Runtime.InteropServices.DispId(1610743834)]
			get;
			[System.Runtime.InteropServices.DispId(1610743834)]
			set;
		}

		int Length
		{
			[System.Runtime.InteropServices.DispId(1610743836)]
			get;
			[System.Runtime.InteropServices.DispId(1610743836)]
			set;
		}

		X509PrivateKeyExportFlags ExportPolicy
		{
			[System.Runtime.InteropServices.DispId(1610743838)]
			get;
			[System.Runtime.InteropServices.DispId(1610743838)]
			set;
		}

		bool MachineContext
		{
			[System.Runtime.InteropServices.DispId(1610743844)]
			get;
			[System.Runtime.InteropServices.DispId(1610743844)]
			set;
		}

		void _VtblGap1_1();

		[System.Runtime.InteropServices.DispId(1610743809)]
		void Create();

		void _VtblGap2_16();

		void _VtblGap3_6();

		void _VtblGap4_4();
	}
}
