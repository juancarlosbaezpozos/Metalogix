using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Metalogix.Utilities
{
    public static class EnumExtensions
    {
        public static string GetDescription(Enum value)
        {
            if (value == null)
            {
                throw new Exception("Value cannot be null");
            }

            string str = value.ToString();
            FieldInfo field = value.GetType().GetField(str);
            DescriptionAttribute[] customAttributes =
                (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if ((int)customAttributes.Length > 0)
            {
                str = customAttributes[0].Description;
            }

            return str;
        }

        public static T GetValueAsEnumValue<T>(this string enumValue)
        {
            T t = default(T);
            string str = enumValue;
            int num = 0;
            if (int.TryParse(str, out num))
            {
                if (Enum.IsDefined(typeof(T), num))
                {
                    t = (T)Enum.Parse(typeof(T), str);
                }
            }
            else if (Enum.IsDefined(typeof(T), str))
            {
                t = (T)Enum.Parse(typeof(T), str);
            }

            return t;
        }

        public static T GetValueAsEnumValue<T>(this string enumValue, T defaultValue)
        {
            T t = defaultValue;
            string str = enumValue;
            int num = 0;
            if (int.TryParse(str, out num))
            {
                if (Enum.IsDefined(typeof(T), num))
                {
                    t = (T)Enum.Parse(typeof(T), str);
                }
            }
            else if (Enum.IsDefined(typeof(T), str))
            {
                t = (T)Enum.Parse(typeof(T), str);
            }

            return t;
        }

        public static bool HasFlag(this Enum value, Enum flag)
        {
            if (flag == null)
            {
                throw new ArgumentNullException("flag");
            }

            if (value.GetType() != flag.GetType())
            {
                throw new ArgumentException("Enum type mismatch");
            }

            bool num = (Convert.ToUInt64(value) & Convert.ToUInt64(flag)) == Convert.ToUInt64(flag);
            return num;
        }
    }
}