using Microsoft.SharePoint;
using System;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.Adapters.OM
{
    public static class SPExtensions
    {
        public static bool IsUpdatable(this SPField field)
        {
            if (field.Sealed || field.Hidden)
            {
                return false;
            }

            return !field.ReadOnlyField;
        }
    }
}