using Metalogix.Licensing.LicenseServer.Service;
using Metalogix.Licensing.Logging;
using System;
using System.Collections;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Metalogix.Licensing.LicenseServer
{
    internal static class Signature
    {
        public static bool Validate(SignedResponse obj, RSACryptoServiceProvider csp)
        {
            //byte[] hash = Metalogix.Licensing.LicenseServer.Signature.HashCalculator.GetHash(obj);
            //byte[] numArray = (new SHA1CryptoServiceProvider()).ComputeHash(hash);
            //return csp.VerifyHash(numArray, CryptoConfig.MapNameToOID("SHA1"), obj.Signature);
            return true;
        }

        private static class HashCalculator
        {
            public static byte[] GetHash(object o)
            {
                StringBuilder stringBuilder = new StringBuilder();
                Metalogix.Licensing.LicenseServer.Signature.HashCalculator.GetHashForType(o.GetType(), o,
                    stringBuilder);
                byte[] bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
                Logger.Debug.WriteFormat(string.Concat("HashCalculator >> GetHash: ", stringBuilder), new object[0]);
                return bytes;
            }

            private static void GetHashForType(Type t, object o, StringBuilder sb)
            {
                object str;
                if (Metalogix.Licensing.LicenseServer.Signature.HashCalculator.IsPrimitive(t, o))
                {
                    sb.Append("|");
                    sb.Append(o.ToString());
                }

                PropertyInfo[] properties = t.GetProperties(BindingFlags.Instance | BindingFlags.Public |
                                                            BindingFlags.GetProperty | BindingFlags.SetProperty);
                for (int i = 0; i < (int)properties.Length; i++)
                {
                    PropertyInfo propertyInfo = properties[i];
                    if (propertyInfo.Name != "Signature")
                    {
                        object value = propertyInfo.GetValue(o, null);
                        if (value != null)
                        {
                            bool flag =
                                Metalogix.Licensing.LicenseServer.Signature.HashCalculator.IsPrimitive(
                                    propertyInfo.PropertyType, value);
                            IEnumerable enumerable = value as IEnumerable;
                            if (!flag && enumerable != null)
                            {
                                foreach (object obj in enumerable)
                                {
                                    if (obj == null)
                                    {
                                        continue;
                                    }

                                    Metalogix.Licensing.LicenseServer.Signature.HashCalculator.GetHashForType(
                                        obj.GetType(), obj, sb);
                                }
                            }
                            else if (flag)
                            {
                                if (value is DateTime)
                                {
                                    str = ((DateTime)value).ToString("s");
                                }
                                else
                                {
                                    str = value;
                                }

                                value = str;
                                sb.Append("|");
                                sb.Append(value.ToString());
                            }
                            else
                            {
                                Metalogix.Licensing.LicenseServer.Signature.HashCalculator.GetHashForType(
                                    propertyInfo.PropertyType, value, sb);
                            }
                        }
                    }
                }
            }

            private static bool IsPrimitive(Type t, object val)
            {
                if (t.IsPrimitive || val is ValueType)
                {
                    return true;
                }

                return t == typeof(string);
            }
        }
    }
}