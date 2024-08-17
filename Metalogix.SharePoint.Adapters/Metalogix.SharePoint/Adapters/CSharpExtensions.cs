using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.Adapters
{
    public static class CSharpExtensions
    {
        public static bool In<T>(this T item, params T[] items)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            return items.Contains<T>(item);
        }
    }
}