using Metalogix;
using Metalogix.DataResolution;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Metalogix.Core.ObjectResolution
{
    public static class DefaultResolverSettings
    {
        private readonly static string SettingsFile;

        private readonly static DataResolver FileDataResolver;

        public readonly static Dictionary<string, string> ResolversFromPowerShell;

        static DefaultResolverSettings()
        {
            DefaultResolverSettings.SettingsFile =
                Path.Combine(ApplicationData.ApplicationPath, "DefaultResolvers.xml");
            DefaultResolverSettings.FileDataResolver = new FileTableDataResolver(DefaultResolverSettings.SettingsFile);
            DefaultResolverSettings.ResolversFromPowerShell = new Dictionary<string, string>();
        }

        public static IEnumerable<KeyValuePair<string, string>> GetAllSettings()
        {
            return
                from key in DefaultResolverSettings.FileDataResolver.GetAvailableDataKeys()
                select new KeyValuePair<string, string>(key,
                    DefaultResolverSettings.FileDataResolver.GetStringDataAtKey(key));
        }

        public static string GetSetting(string name)
        {
            return DefaultResolverSettings.FileDataResolver.GetStringDataAtKey(name);
        }

        public static void InitializeSetting(string name, string defaultValue)
        {
            string stringDataAtKey = DefaultResolverSettings.FileDataResolver.GetStringDataAtKey(name);
            if (!string.IsNullOrEmpty(stringDataAtKey))
            {
                DefaultResolverSettings.UpdateVersionNumber(name, stringDataAtKey);
                return;
            }

            DefaultResolverSettings.FileDataResolver.WriteStringDataAtKey(name, defaultValue);
        }

        public static void SaveSetting(string name, string value)
        {
            DefaultResolverSettings.FileDataResolver.WriteStringDataAtKey(name, value);
        }

        public static void SetResolversFromPowershell(string name, string defaultValue)
        {
            if (DefaultResolverSettings.ResolversFromPowerShell.ContainsKey(name))
            {
                DefaultResolverSettings.ResolversFromPowerShell[name] = defaultValue;
                return;
            }

            DefaultResolverSettings.ResolversFromPowerShell.Add(name, defaultValue);
        }

        private static void UpdateVersionNumber(string name, string keyValue)
        {
            string str = Convert.ToString(Assembly.GetExecutingAssembly().GetName().Version);
            Match match = (new Regex("Version=([^,]+)")).Match(keyValue);
            if (match.Success && !str.Equals(match.Groups[1].Value, StringComparison.Ordinal))
            {
                string str1 = keyValue.Replace(match.Groups[1].Value, str);
                DefaultResolverSettings.FileDataResolver.WriteStringDataAtKey(name, str1);
            }
        }
    }
}