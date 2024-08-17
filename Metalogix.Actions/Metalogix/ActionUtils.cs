using Metalogix.Actions;
using Metalogix.Actions.Properties;
using Metalogix.Data;
using System;
using System.Reflection;

namespace Metalogix
{
    public static class ActionUtils
    {
        public static string GetTypePluralizedName(Type type)
        {
            string pluralName = "";
            object[] customAttributes = type.GetCustomAttributes(typeof(PluralNameAttribute), true);
            if ((int)customAttributes.Length <= 0)
            {
                customAttributes = type.GetCustomAttributes(typeof(NameAttribute), true);
                pluralName = ((int)customAttributes.Length <= 0
                    ? string.Concat(type, "s")
                    : string.Concat(((NameAttribute)customAttributes[0]).Name, "s"));
            }
            else
            {
                pluralName = ((PluralNameAttribute)customAttributes[0]).PluralName;
            }

            return pluralName;
        }

        public static void TryActionUntilTrueElseThrowIfMaxTriesReached(int maxTries, Func<bool> action)
        {
            for (int i = 0; i < maxTries; i++)
            {
                if (i == maxTries)
                {
                    throw new InvalidOperationException(
                        string.Format(Resources.ActionUtils_TryThenThrowIfFail_MaxTriesReached, maxTries));
                }

                try
                {
                    if (action())
                    {
                        break;
                    }
                }
                catch (Exception exception)
                {
                }
            }
        }
    }
}