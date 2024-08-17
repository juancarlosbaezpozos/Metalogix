using System;

namespace Microsoft.XmlDiffPatch
{
    internal enum XmlDiffOperation
    {
        Match,
        Add,
        Remove,
        ChangeElementName,
        ChangeElementAttr1,
        ChangeElementAttr2,
        ChangeElementAttr3,
        ChangeElementNameAndAttr1,
        ChangeElementNameAndAttr2,
        ChangeElementNameAndAttr3,
        ChangePI,
        ChangeER,
        ChangeCharacterData,
        ChangeXmlDeclaration,
        ChangeDTD,
        Undefined,
        ChangeAttr
    }
}