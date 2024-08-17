using System;

namespace Metalogix.SharePoint.UI.WinForms.Transform
{
    public class FieldToManagedMetadataConfigConstants
    {
        public const byte C_MAX_INTERNALNAME_TEXTBOX_LEN = 250;

        public const byte C_MAX_TAXONOMY_INPUT_LEN = 250;

        public const string C_REGEX_TAXONOMY_INVALID_CHARS = "[\\x22\\x3B\\x3C\\x3E\\x7C\\x09]";

        public const string C_REGEX_INTERNALNAME = "^[A-Za-z\\x5F][0-9A-Za-z\\x5F]+$";

        public FieldToManagedMetadataConfigConstants()
        {
        }
    }
}