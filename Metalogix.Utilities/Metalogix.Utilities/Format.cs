using System;

namespace Metalogix.Utilities
{
    public class Format : IFormatter
    {
        public Format()
        {
        }

        public static string FormatCount(long? iCount, string sUnits)
        {
            if (sUnits == null || !iCount.HasValue)
            {
                return Format.FormatSize(iCount);
            }

            string str = sUnits.Trim();
            if (str.EndsWith("B", StringComparison.Ordinal) || str.ToLower().Contains("bytes"))
            {
                return Format.FormatSize(iCount);
            }

            long? nullable = iCount;
            if ((nullable.GetValueOrDefault() != (long)1 ? true : !nullable.HasValue) || !(str.ToLower() == "s"))
            {
                return string.Concat(iCount, " ", str);
            }

            return string.Concat(iCount, " ", str.Substring(0, str.Length - 1));
        }

        public string FormatData(long? lData, string sUnits)
        {
            if (!lData.HasValue || sUnits == null)
            {
                return "";
            }

            string str = sUnits.Trim();
            if (str.EndsWith("B", StringComparison.Ordinal) || str.ToLower().Contains("bytes"))
            {
                return Format.FormatSize(lData);
            }

            return Format.FormatCount(lData, sUnits);
        }

        public static string FormatSize(long? iSize)
        {
            long? nullable;
            long? nullable1;
            long? nullable2;
            long? nullable3;
            long? nullable4;
            long? nullable5;
            long? nullable6;
            long? nullable7;
            long? nullable8;
            if (!iSize.HasValue)
            {
                return "";
            }

            long? nullable9 = iSize;
            if ((nullable9.GetValueOrDefault() <= (long)1000000000 ? false : nullable9.HasValue))
            {
                object[] objArray = new object[4];
                object[] objArray1 = objArray;
                long? nullable10 = iSize;
                if (nullable10.HasValue)
                {
                    nullable6 = new long?(nullable10.GetValueOrDefault() / (long)1000000000);
                }
                else
                {
                    nullable6 = null;
                }

                objArray1[0] = nullable6;
                objArray[1] = ".";
                object[] objArray2 = objArray;
                long? nullable11 = iSize;
                if (nullable11.HasValue)
                {
                    nullable7 = new long?(nullable11.GetValueOrDefault() % (long)1000000000);
                }
                else
                {
                    nullable7 = null;
                }

                long? nullable12 = nullable7;
                if (nullable12.HasValue)
                {
                    nullable8 = new long?(nullable12.GetValueOrDefault() / (long)100000000);
                }
                else
                {
                    nullable8 = null;
                }

                objArray2[2] = nullable8;
                objArray[3] = " GB";
                return string.Concat(objArray);
            }

            long? nullable13 = iSize;
            if ((nullable13.GetValueOrDefault() <= (long)1000000 ? false : nullable13.HasValue))
            {
                object[] objArray3 = new object[4];
                object[] objArray4 = objArray3;
                long? nullable14 = iSize;
                if (nullable14.HasValue)
                {
                    nullable3 = new long?(nullable14.GetValueOrDefault() / (long)1000000);
                }
                else
                {
                    nullable3 = null;
                }

                objArray4[0] = nullable3;
                objArray3[1] = ".";
                object[] objArray5 = objArray3;
                long? nullable15 = iSize;
                if (nullable15.HasValue)
                {
                    nullable4 = new long?(nullable15.GetValueOrDefault() % (long)1000000);
                }
                else
                {
                    nullable4 = null;
                }

                long? nullable16 = nullable4;
                if (nullable16.HasValue)
                {
                    nullable5 = new long?(nullable16.GetValueOrDefault() / (long)100000);
                }
                else
                {
                    nullable5 = null;
                }

                objArray5[2] = nullable5;
                objArray3[3] = " MB";
                return string.Concat(objArray3);
            }

            long? nullable17 = iSize;
            if ((nullable17.GetValueOrDefault() <= (long)1000 ? true : !nullable17.HasValue))
            {
                return string.Concat(iSize, " B");
            }

            object[] objArray6 = new object[4];
            object[] objArray7 = objArray6;
            long? nullable18 = iSize;
            if (nullable18.HasValue)
            {
                nullable = new long?(nullable18.GetValueOrDefault() / (long)1000);
            }
            else
            {
                nullable = null;
            }

            objArray7[0] = nullable;
            objArray6[1] = ".";
            object[] objArray8 = objArray6;
            long? nullable19 = iSize;
            if (nullable19.HasValue)
            {
                nullable1 = new long?(nullable19.GetValueOrDefault() % (long)1000);
            }
            else
            {
                nullable1 = null;
            }

            long? nullable20 = nullable1;
            if (nullable20.HasValue)
            {
                nullable2 = new long?(nullable20.GetValueOrDefault() / (long)100);
            }
            else
            {
                nullable2 = null;
            }

            objArray8[2] = nullable2;
            objArray6[3] = " kB";
            return string.Concat(objArray6);
        }

        public static long ParseFormattedSize(string sSize)
        {
            string[] strArrays = sSize.Split(new char[] { ' ' });
            if ((int)strArrays.Length != 2)
            {
                return (long)-1;
            }

            string str = strArrays[0];
            string str1 = strArrays[1];
            long num = (long)-1;
            double num1 = -1;
            if (double.TryParse(str, out num1))
            {
                string str2 = str1;
                string str3 = str2;
                if (str2 != null)
                {
                    if (str3 == "GB")
                    {
                        num = (long)(num1 * 1000000000);
                        return num;
                    }
                    else if (str3 == "MB")
                    {
                        num = (long)(num1 * 1000000);
                        return num;
                    }
                    else if (str3 == "kB")
                    {
                        num = (long)(num1 * 1000);
                        return num;
                    }
                    else
                    {
                        if (str3 != "B")
                        {
                            goto Label2;
                        }

                        num = (long)num1;
                        return num;
                    }
                }

                Label2:
                num = (long)num1;
            }

            return num;
        }
    }
}