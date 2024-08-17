using Metalogix;
using System;
using System.Collections.Generic;

namespace Metalogix.Data.Filters
{
    public struct FilterBuilderType
    {
        public List<Type> ObjectTypes;

        public bool AllowFreeFormEntry;

        public FilterBuilderType(List<Type> objectTypes)
        {
            this.ObjectTypes = objectTypes;
            this.AllowFreeFormEntry = false;
        }

        public FilterBuilderType(List<Type> objectTypes, bool bAllowFreeFormEntry)
        {
            this.ObjectTypes = objectTypes;
            this.AllowFreeFormEntry = bAllowFreeFormEntry;
        }

        private string GetPluralDisplayName(List<Type> types)
        {
            string str = "";
            int num = 0;
            foreach (Type type in types)
            {
                num++;
                str = string.Concat(str, ActionUtils.GetTypePluralizedName(type));
                if (num >= types.Count)
                {
                    continue;
                }

                str = string.Concat(str, ", ");
            }

            return str;
        }

        public override string ToString()
        {
            return this.GetPluralDisplayName(this.ObjectTypes);
        }
    }
}