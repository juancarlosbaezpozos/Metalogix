using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Metalogix.Utilities
{
    public static class DictionaryExtensions
    {
        public static string GetValue(this Dictionary<string, string> dictionary, string keyName,
            string defaultValue = "")
        {
            string item = defaultValue ?? string.Empty;
            if (dictionary != null && keyName != null && dictionary.ContainsKey(keyName))
            {
                item = dictionary[keyName];
            }

            return item;
        }
    }
}