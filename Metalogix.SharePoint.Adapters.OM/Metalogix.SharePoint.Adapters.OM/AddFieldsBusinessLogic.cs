using Microsoft.SharePoint;
using System;

namespace Metalogix.SharePoint.Adapters.OM
{
    public static class AddFieldsBusinessLogic
    {
        public static bool IsAllowedToUpdateCalculatedFieldSchema(SPField targetField, string sourceFieldType)
        {
            if (!string.Equals(sourceFieldType, "Calculated"))
            {
                return false;
            }

            if (targetField == null)
            {
                return true;
            }

            if (targetField == null || !targetField.IsUpdatable())
            {
                return false;
            }

            return targetField.Type == (SPFieldType)17;
        }
    }
}