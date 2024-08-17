using System;

namespace Microsoft.XmlDiffPatch
{
    internal enum XmlDiffNodeType
    {
        XmlDeclaration = -2,
        DocumentType = -1,
        None = 0,
        Element = 1,
        Attribute = 2,
        Text = 3,
        CDATA = 4,
        EntityReference = 5,
        ProcessingInstruction = 7,
        Comment = 8,
        Document = 9,
        SignificantWhitespace = 14,
        Namespace = 100,
        ShrankNode = 101
    }
}