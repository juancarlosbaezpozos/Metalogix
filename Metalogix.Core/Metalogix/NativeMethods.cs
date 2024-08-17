using System;
using System.Runtime.InteropServices;

namespace Metalogix
{
    public class NativeMethods
    {
        public const int MAX_PATH = 256;

        public const uint SHGFI_ICON = 256;

        public const uint SHGFI_DISPLAYNAME = 512;

        public const uint SHGFI_TYPENAME = 1024;

        public const uint SHGFI_ATTRIBUTES = 2048;

        public const uint SHGFI_ICONLOCATION = 4096;

        public const uint SHGFI_EXETYPE = 8192;

        public const uint SHGFI_SYSICONINDEX = 16384;

        public const uint SHGFI_LINKOVERLAY = 32768;

        public const uint SHGFI_SELECTED = 65536;

        public const uint SHGFI_ATTR_SPECIFIED = 131072;

        public const uint SHGFI_LARGEICON = 0;

        public const uint SHGFI_SMALLICON = 1;

        public const uint SHGFI_OPENICON = 2;

        public const uint SHGFI_SHELLICONSIZE = 4;

        public const uint SHGFI_PIDL = 8;

        public const uint SHGFI_USEFILEATTRIBUTES = 16;

        public const uint SHGFI_ADDOVERLAYS = 32;

        public const uint SHGFI_OVERLAYINDEX = 64;

        public const uint FILE_ATTRIBUTE_DIRECTORY = 16;

        public const uint FILE_ATTRIBUTE_NORMAL = 128;

        public NativeMethods()
        {
        }

        [DllImport("User32.dll", CharSet = CharSet.None, ExactSpelling = false)]
        internal static extern int DestroyIcon(IntPtr hIcon);

        [DllImport("Shell32.dll", CharSet = CharSet.None, ExactSpelling = false)]
        internal static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes,
            ref NativeMethods.SHFILEINFO psfi, uint cbFileInfo, uint uFlags);

        internal struct SHFILEINFO
        {
            public const int NAMESIZE = 80;

            public IntPtr hIcon;

            public int iIcon;

            public uint dwAttributes;

            public string szDisplayName;

            public string szTypeName;
        }
    }
}