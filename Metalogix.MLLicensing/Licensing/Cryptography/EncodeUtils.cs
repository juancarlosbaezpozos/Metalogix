using System;

namespace Metalogix.Licensing.Cryptography
{
    public class EncodeUtils
    {
        public EncodeUtils()
        {
        }

        public static string Decode6bit(string txt)
        {
            ushort num;
            string str = txt.Substring(0, 4);
            ushort num1 = Convert.ToUInt16(str, 16);
            int length = txt.Length;
            string str1 = "";
            ushort[] numArray = new ushort[8];
            for (int i = 4; i < length; i += 8)
            {
                for (int j = 0; j < 8; j++)
                {
                    char chr = txt[i + j];
                    if (chr == '>')
                    {
                        num = 63;
                    }
                    else if (chr == '<')
                    {
                        num = 62;
                    }
                    else if (chr < 'a')
                    {
                        num = (chr < 'A' ? (ushort)((byte)chr - 48) : (ushort)((byte)chr - 65 + 36));
                    }
                    else
                    {
                        num = (ushort)((byte)chr - 97 + 10);
                    }

                    numArray[j] = num;
                }

                ushort num2 = (ushort)(numArray[0] << 10 | numArray[1] << 4 | (numArray[2] & 60) >> 2);
                ushort num3 = (ushort)((numArray[2] & 3) << 14 | numArray[3] << 8 | numArray[4] << 2 |
                                       (numArray[5] & 48) >> 4);
                ushort num4 = (ushort)((numArray[5] & 15) << 12 | numArray[6] << 6 | numArray[7]);
                str1 = string.Concat(str1, (char)num2);
                str1 = string.Concat(str1, (char)num3);
                str1 = string.Concat(str1, (char)num4);
            }

            if (str1.Length > num1)
            {
                str1 = str1.Substring(0, (int)num1);
            }

            return str1;
        }

        public static string DecodeXOR(string encoded, string key)
        {
            string str = "";
            for (int i = 0; i < encoded.Length; i += 2)
            {
                int num = Convert.ToByte(encoded.Substring(i, 2), 16);
                int num1 = key[i / 2 % key.Length];
                int num2 = num ^ num1;
                str = string.Concat(str, (char)num2);
            }

            return str;
        }

        public static string Encode6bit(string txt)
        {
            char chr;
            short length = (short)txt.Length;
            string str = txt;
            short num = (short)(length % 3);
            if (num == 0)
            {
                num = 3;
            }

            for (int i = 0; i < 3 - num; i++)
            {
                str = string.Concat(str, " ");
            }

            string str1 = length.ToString("X").PadLeft(4, '0');
            byte[] numArray = new byte[8];
            for (int j = 0; j < length; j += 3)
            {
                ushort num1 = str[j];
                ushort num2 = str[j + 1];
                ushort num3 = str[j + 2];
                numArray[0] = (byte)((num1 & 64512) >> 10);
                numArray[1] = (byte)((num1 & 1008) >> 4);
                numArray[2] = (byte)((num1 & 15) << 2 | (num2 & 49152) >> 14);
                numArray[3] = (byte)((num2 & 16128) >> 8);
                numArray[4] = (byte)((num2 & 252) >> 2);
                numArray[5] = (byte)((num2 & 3) << 4 | (num3 & 61440) >> 12);
                numArray[6] = (byte)((num3 & 4032) >> 6);
                numArray[7] = (byte)(num3 & 63);
                for (int k = 0; k < 8; k++)
                {
                    byte num4 = numArray[k];
                    if (num4 < 10)
                    {
                        chr = (char)(48 + num4);
                    }
                    else if (num4 < 36)
                    {
                        chr = (char)(97 + num4 - 10);
                    }
                    else if (num4 >= 62)
                    {
                        chr = (num4 != 63 ? '>' : '<');
                    }
                    else
                    {
                        chr = (char)(65 + num4 - 36);
                    }

                    str1 = string.Concat(str1, chr);
                }
            }

            return str1;
        }

        public static string EncodeXOR(string original, string key)
        {
            string str = "";
            for (int i = 0; i < original.Length; i++)
            {
                int num = Convert.ToByte(original[i]);
                int num1 = key[i % key.Length];
                int num2 = num ^ num1;
                str = string.Concat(str, num2.ToString("X2"));
            }

            return str;
        }
    }
}