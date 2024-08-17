using System;
using System.Runtime.InteropServices;

namespace Microsoft.XmlDiffPatch
{
    [ComVisible(true)]
    public enum XmlDiffOptions
    {
        None = 0,
        IgnoreChildOrder = 1,
        IgnoreComments = 2,
        IgnorePI = 4,
        IgnoreWhitespace = 8,
        IgnoreNamespaces = 16,
        IgnorePrefixes = 32,
        IgnoreXmlDecl = 64,
        IgnoreDtd = 128
    }
}