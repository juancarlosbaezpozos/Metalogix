using System;
using System.IO;
using System.Text;
using System.Web;

namespace Metalogix
{
    public class WebUtils
    {
        public WebUtils()
        {
        }

        private static int FromHexNoCheck(char digit)
        {
            if (digit <= '9')
            {
                return digit - 48;
            }

            if (digit <= 'F')
            {
                return digit - 65 + 10;
            }

            return digit - 97 + 10;
        }

        private static bool IsHexDigit(char digit)
        {
            if (48 <= digit && digit <= '9')
            {
                return true;
            }

            if (97 <= digit && digit <= 'f')
            {
                return true;
            }

            if (65 > digit)
            {
                return false;
            }

            return digit <= 'F';
        }

        private static bool IsHexEscapedChar(string str, int nIndex, int nPathLength)
        {
            if (nIndex == 2147483647 || nIndex + 1 == 2147483647)
            {
                throw new ArgumentException("Input cannot be equal to or one less than the int max.", "nIndex");
            }

            if (nIndex + 2 >= nPathLength || str[nIndex] != '%' || !WebUtils.IsHexDigit(str[nIndex + 1]) ||
                !WebUtils.IsHexDigit(str[nIndex + 2]))
            {
                return false;
            }

            if (str[nIndex + 1] != '0')
            {
                return true;
            }

            return str[nIndex + 2] != '0';
        }

        private static string UrlDecodeHelper(string stringToDecode, int length, bool decodePlus)
        {
            if (stringToDecode == null || stringToDecode.Length == 0)
            {
                return stringToDecode;
            }

            StringBuilder stringBuilder = new StringBuilder(length);
            byte[] numArray = null;
            int num = 0;
            while (num < length)
            {
                char chr = stringToDecode[num];
                if (chr < ' ')
                {
                    num++;
                }
                else if (decodePlus && chr == '+')
                {
                    stringBuilder.Append(" ");
                    num++;
                }
                else if (!WebUtils.IsHexEscapedChar(stringToDecode, num, length))
                {
                    stringBuilder.Append(chr);
                    num++;
                }
                else
                {
                    if (numArray == null)
                    {
                        numArray = new byte[(length - num) / 3];
                    }

                    int num1 = 0;
                    do
                    {
                        int num2 = WebUtils.FromHexNoCheck(stringToDecode[num + 1]) * 16 +
                                   WebUtils.FromHexNoCheck(stringToDecode[num + 2]);
                        int num3 = num1;
                        num1 = num3 + 1;
                        numArray[num3] = (byte)num2;
                        num += 3;
                    } while (WebUtils.IsHexEscapedChar(stringToDecode, num, length));

                    stringBuilder.Append(Encoding.UTF8.GetChars(numArray, 0, num1));
                }
            }

            if (length < stringToDecode.Length)
            {
                stringBuilder.Append(stringToDecode.Substring(length));
            }

            return stringBuilder.ToString();
        }

        public static string UrlPathDecode(string urlToDecode)
        {
            return WebUtils.UrlPathDecode(urlToDecode, true);
        }

        public static string UrlPathDecode(string urlToDecode, bool allowHashParameter)
        {
            if (string.IsNullOrEmpty(urlToDecode))
            {
                return urlToDecode;
            }

            int length = urlToDecode.Length;
            int num = urlToDecode.IndexOf('?');
            if (num == -1)
            {
                num = length;
            }

            if (allowHashParameter)
            {
                int num1 = urlToDecode.IndexOf('#');
                if (num1 != -1 && num1 < num)
                {
                    num = num1;
                }
            }

            return WebUtils.UrlDecodeHelper(urlToDecode, num, false);
        }

        public static bool ValidLink(string link, bool requireFileExists = true)
        {
            if (string.IsNullOrEmpty(link))
            {
                return false;
            }

            if (Uri.IsWellFormedUriString(HttpUtility.UrlPathEncode(link), UriKind.Absolute))
            {
                return true;
            }

            if (requireFileExists)
            {
                if (Directory.Exists(link))
                {
                    return true;
                }

                return File.Exists(link);
            }

            if (link.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
            {
                return false;
            }

            return Path.IsPathRooted(link);
        }
    }
}