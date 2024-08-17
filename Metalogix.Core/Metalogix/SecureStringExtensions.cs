using Metalogix.Utilities;
using System;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text.RegularExpressions;

namespace Metalogix
{
    public static class SecureStringExtensions
    {
        public static bool DecryptEmbeddedStringsandCompareXML(this string currentXMLString,
            string xmlStringWithEncryptedAttributes)
        {
            string str = Metalogix.SecureStringExtensions.FindAndReplaceEncryptedText(currentXMLString);
            return string.Equals(str,
                Metalogix.SecureStringExtensions.FindAndReplaceEncryptedText(xmlStringWithEncryptedAttributes));
        }

        private static string DecryptMatchEvaluator(Match match)
        {
            if (match.Groups.Count <= 1)
            {
                return match.ToString();
            }

            SecureString secureString = Cryptography.DecryptText(match.Groups[1].ToString());
            return string.Format("\"{0}\"", secureString.ToInsecureString());
        }

        private static string FindAndReplaceEncryptedText(string mixedModeTextBlob)
        {
            Regex regex = new Regex("\"(AQAAANCMnd8BFdERjHoAwE.*?)\"");
            return regex.Replace(mixedModeTextBlob,
                new MatchEvaluator(Metalogix.SecureStringExtensions.DecryptMatchEvaluator));
        }
    }
}