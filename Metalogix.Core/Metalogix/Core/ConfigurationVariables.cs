using Metalogix;
using Metalogix.Core.Properties;
using Metalogix.DataStructures.Generic;
using Metalogix.Interfaces;
using Metalogix.ObjectResolution;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Metalogix.Core
{
    public class ConfigurationVariables
    {
        private static ThreadSafeDictionary<string, ConfigurationVariables.ConfigurationVariable>
            s_dynamicConfigurationVariables;

        public static ResourceTableLink LegacyEnvironmentVariables;

        public static ResourceTableLink ApplicationAndUserSpecificVariables;

        public static ResourceTableLink UserSpecificVariables;

        public static ResourceTableLink ApplicationSpecificVariables;

        public static ResourceTableLink EnvironmentVariables;

        private static string s_sAssemblyVersionString;

        public static string AssemblyVersionString
        {
            get
            {
                if (ConfigurationVariables.s_sAssemblyVersionString == null)
                {
                    ConfigurationVariables.s_sAssemblyVersionString =
                        Assembly.GetExecutingAssembly().GetName().Version.ToString();
                }

                return ConfigurationVariables.s_sAssemblyVersionString;
            }
        }

        static ConfigurationVariables()
        {
            ConfigurationVariables.LegacyEnvironmentVariables =
                new ResourceTableLink("EnvironmentSettings", ResourceScope.ApplicationAndUserSpecific);
            ConfigurationVariables.ApplicationAndUserSpecificVariables =
                new ResourceTableLink("ApplicationSettings", ResourceScope.ApplicationAndUserSpecific);
            ConfigurationVariables.UserSpecificVariables =
                new ResourceTableLink("UserSettings", ResourceScope.UserSpecific);
            ConfigurationVariables.ApplicationSpecificVariables =
                new ResourceTableLink("ApplicationConfiguration", ResourceScope.ApplicationSpecific);
            ConfigurationVariables.EnvironmentVariables =
                new ResourceTableLink("EnvironmentSettings", ResourceScope.EnvironmentSpecific);
            ConfigurationVariables.s_sAssemblyVersionString = null;
            ConfigurationVariables.UpgradeFromLegacyEnvironmentVariablesTable();
            ConfigurationVariables.s_dynamicConfigurationVariables =
                new ThreadSafeDictionary<string, ConfigurationVariables.ConfigurationVariable>();
            ApplicationData.MainAssemblyChanged += new EventHandler(ConfigurationVariables.MainAssemblyChanged);
        }

        public ConfigurationVariables()
        {
        }

        protected static void ClearApplicationVariables()
        {
            ConfigurationVariables.s_sAssemblyVersionString = null;
            ResourceTable.CachedResourceTables.Clear();
            ConfigurationVariables.FireConfigurationVariablesChanged(null);
        }

        public static bool ContainsConfigurationVariable(string name)
        {
            return ConfigurationVariables.s_dynamicConfigurationVariables.ContainsKey(name);
        }

        protected static void FireConfigurationVariablesChanged(string environmentVariableName)
        {
            if (ConfigurationVariables.ConfigurationVariablesChanged != null)
            {
                ConfigurationVariables.ConfigurationVariablesChanged(null,
                    new ConfigurationVariables.ConfigVarsChangedArgs()
                    {
                        VariableName = environmentVariableName
                    });
            }
        }

        public static T GetConfigurationValue<T>(string name)
            where T : IConvertible
        {
            if (!ConfigurationVariables.s_dynamicConfigurationVariables.ContainsKey(name))
            {
                throw new ArgumentException(Resources.ConfigValueDoesNotExist_Exception, "Name");
            }

            return ConfigurationVariables.s_dynamicConfigurationVariables[name].GetValue<T>();
        }

        public static IConfigurationVariable GetConfigurationVariable(string name)
        {
            if (!ConfigurationVariables.s_dynamicConfigurationVariables.ContainsKey(name))
            {
                throw new ArgumentException(Resources.ConfigValueDoesNotExist_Exception, "Name");
            }

            return ConfigurationVariables.s_dynamicConfigurationVariables[name];
        }

        public static string GetSerializedConfigurationVariables(ResourceScope scope)
        {
            ThreadSafeSerializableTable<string, string> threadSafeSerializableTable =
                new ThreadSafeSerializableTable<string, string>();
            foreach (ResourceTableLink validResourceTable in ConfigurationVariables.GetValidResourceTables(scope))
            {
                ResourceTable resourceTable = validResourceTable.Resolve();
                foreach (string availableResource in resourceTable.GetAvailableResources())
                {
                    if (threadSafeSerializableTable.ContainsKey(availableResource))
                    {
                        continue;
                    }

                    threadSafeSerializableTable.Add(availableResource, resourceTable.GetResource(availableResource));
                }
            }

            return threadSafeSerializableTable.ToXML();
        }

        public static string GetSetting(ResourceScope scope, string sSettingName)
        {
            string str;
            List<ResourceTableLink>.Enumerator enumerator =
                ConfigurationVariables.GetValidResourceTables(scope).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    string resource = enumerator.Current.Resolve().GetResource(sSettingName);
                    if (resource == null)
                    {
                        continue;
                    }

                    str = resource;
                    return str;
                }

                return null;
            }
            finally
            {
                ((IDisposable)enumerator).Dispose();
            }

            return str;
        }

        public static List<ResourceTableLink> GetValidResourceTables(ResourceScope scope)
        {
            List<ResourceTableLink> resourceTableLinks = new List<ResourceTableLink>();
            if ((ResourceScope.ApplicationAndUserSpecific & scope) == ResourceScope.ApplicationAndUserSpecific)
            {
                resourceTableLinks.Add(ConfigurationVariables.ApplicationAndUserSpecificVariables);
            }

            if ((ResourceScope.UserSpecific & scope) == ResourceScope.UserSpecific)
            {
                resourceTableLinks.Add(ConfigurationVariables.UserSpecificVariables);
            }

            if ((ResourceScope.ApplicationSpecific & scope) == ResourceScope.ApplicationSpecific)
            {
                resourceTableLinks.Add(ConfigurationVariables.ApplicationSpecificVariables);
            }

            if ((ResourceScope.EnvironmentSpecific & scope) == ResourceScope.EnvironmentSpecific)
            {
                resourceTableLinks.Add(ConfigurationVariables.EnvironmentVariables);
            }

            return resourceTableLinks;
        }

        public static void InitializeConfigurationVariable<TConvertible>(ResourceScope scope, string name,
            TConvertible defaultValue)
            where TConvertible : IConvertible
        {
            if (!ConfigurationVariables.s_dynamicConfigurationVariables.ContainsKey(name))
            {
                ConfigurationVariables.ConfigurationVariable<TConvertible> configurationVariable =
                    new ConfigurationVariables.ConfigurationVariable<TConvertible>(scope, name, defaultValue);
                ConfigurationVariables.s_dynamicConfigurationVariables.Add(name, configurationVariable);
                return;
            }

            ConfigurationVariables.ConfigurationVariable item =
                ConfigurationVariables.s_dynamicConfigurationVariables[name];
            if (!scope.HasFlag(item.Scope))
            {
                throw new ArgumentException(Resources.ConfigValueExists_Exception,
                    string.Format("parameter:{0}, required scope:{1}, actual scope:{2}", name, scope.ToString(),
                        item.Scope.ToString()));
            }

            if (!ConfigurationVariables.IsConvertible(item.GetValue(), typeof(TConvertible)))
            {
                throw new ArgumentException(Resources.ConfigValueExists_Exception,
                    string.Format("Type mismatch; parameter:{0}, required scope:{1}", name, scope.ToString()));
            }
        }

        private static bool IsConvertible(IConvertible value, Type targetType)
        {
            bool flag;
            try
            {
                if (!ConfigurationVariables.IsConvertibleToEnum(value, targetType))
                {
                    Convert.ChangeType(value, targetType);
                    flag = true;
                }
                else
                {
                    flag = true;
                }
            }
            catch (InvalidCastException invalidCastException)
            {
                flag = false;
            }

            return flag;
        }

        private static bool IsConvertibleToEnum(IConvertible value, Type targetType)
        {
            bool flag;
            if (!targetType.IsEnum)
            {
                return false;
            }

            try
            {
                flag = Enum.Parse(targetType, value.ToString(CultureInfo.InvariantCulture)) != null;
            }
            catch
            {
                flag = false;
            }

            return flag;
        }

        private static void MainAssemblyChanged(object sender, EventArgs args)
        {
            if (!GlobalConfigurationVariableSettings.ClearApplicationVariablesOnMainAssemblyChanged)
            {
                return;
            }

            ConfigurationVariables.ClearApplicationVariables();
        }

        public static void SetConfigurationValue<T>(string name, T value)
            where T : IConvertible
        {
            if (!ConfigurationVariables.s_dynamicConfigurationVariables.ContainsKey(name))
            {
                throw new ArgumentException(Resources.ConfigValueDoesNotExist_Exception, "Name");
            }

            ConfigurationVariables.s_dynamicConfigurationVariables[name].SetValue(value);
        }

        public static void StoreSetting(ResourceScope scope, string sSettingName, string sValue)
        {
            if ((int)scope == 0)
            {
                throw new ArgumentException(Resources.NoScopeError, "scope");
            }

            List<ResourceTableLink> validResourceTables = ConfigurationVariables.GetValidResourceTables(scope);
            validResourceTables[0].Resolve().WriteResource(sSettingName, sValue);
            if (ConfigurationVariables.ConfigurationVariablesChanged != null)
            {
                ConfigurationVariables.ConfigurationVariablesChanged(null,
                    new ConfigurationVariables.ConfigVarsChangedArgs()
                    {
                        VariableName = sSettingName
                    });
            }
        }

        private static void UpgradeFromLegacyEnvironmentVariablesTable()
        {
            ResourceTable resourceTable = ConfigurationVariables.EnvironmentVariables.Resolve();
            bool flag = false;
            using (IEnumerator<string> enumerator = resourceTable.GetAvailableResources().GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    string current = enumerator.Current;
                    flag = true;
                }
            }

            if (!flag)
            {
                ResourceTable resourceTable1 = ConfigurationVariables.LegacyEnvironmentVariables.Resolve();
                foreach (string availableResource in resourceTable1.GetAvailableResources())
                {
                    resourceTable.WriteResource(availableResource, resourceTable1.GetResource(availableResource));
                }
            }
        }

        public static event ConfigurationVariables.ConfigurationVariablesChangedHander ConfigurationVariablesChanged;

        protected abstract class ConfigurationVariable : IConfigurationVariable
        {
            public readonly string Name;

            public readonly ResourceScope Scope;

            protected ConfigurationVariable(ResourceScope scope, string name)
            {
                this.Name = name;
                this.Scope = scope;
            }

            protected TConvertType ConvertValue<TConvertType>(IConvertible value)
                where TConvertType : IConvertible
            {
                TConvertType tConvertType;
                tConvertType = (typeof(TConvertType) != typeof(IConvertible)
                    ? (TConvertType)this.ConvertValueToTargetType(value, typeof(TConvertType))
                    : (TConvertType)this.ConvertValueToUnderlyingValueType(value));
                return tConvertType;
            }

            private object ConvertValueToTargetType(IConvertible value, Type targetType)
            {
                if (!targetType.IsEnum)
                {
                    return Convert.ChangeType(value, targetType);
                }

                return this.GetValueAsEnum(value, targetType);
            }

            private object ConvertValueToUnderlyingValueType(IConvertible value)
            {
                return this.ConvertValueToTargetType(value, this.GetValueType());
            }

            public abstract IConvertible GetValue();

            public TResult GetValue<TResult>()
                where TResult : IConvertible
            {
                return this.ConvertValue<TResult>(this.GetValue());
            }

            private object GetValueAsEnum(IConvertible value, Type enumType)
            {
                if (!enumType.IsEnum)
                {
                    throw new NotSupportedException("The type is not an enum.");
                }

                return Enum.Parse(enumType, value.ToString(CultureInfo.InvariantCulture));
            }

            public abstract Type GetValueType();

            public abstract void SetValue(IConvertible value);
        }

        protected class ConfigurationVariable<T> : ConfigurationVariables.ConfigurationVariable
            where T : IConvertible
        {
            private readonly T _defaultValue;

            public ConfigurationVariable(ResourceScope scope, string name, T defaultValue) : base(scope, name)
            {
                if (!typeof(T).IsValueType && defaultValue == null)
                {
                    throw new ArgumentNullException("defaultValue");
                }

                this._defaultValue = defaultValue;
            }

            public override IConvertible GetValue()
            {
                string setting = ConfigurationVariables.GetSetting(this.Scope, this.Name);
                bool flag = !string.IsNullOrEmpty(setting);
                bool flag1 = string.IsNullOrEmpty(Convert.ToString(this._defaultValue));
                if (flag || setting != null && flag1)
                {
                    if (typeof(T) == typeof(IConvertible))
                    {
                        return setting;
                    }

                    return ConvertValue<T>(setting);
                }

                ResourceScope resourceScope = ResourceScope.EnvironmentSpecific;
                ResourceScope scope = 0;
                while ((int)scope == 0 && (int)resourceScope != 0)
                {
                    scope = this.Scope & resourceScope;
                    //resourceScope >>= ResourceScope.ApplicationAndUserSpecific;
                    resourceScope = (ResourceScope)(((int)resourceScope) >> 1);
                }

                string name = this.Name;
                T t = this._defaultValue;
                ConfigurationVariables.StoreSetting(scope, name, t.ToString(CultureInfo.InvariantCulture));
                return this._defaultValue;
            }

            public override Type GetValueType()
            {
                return typeof(T);
            }

            public override void SetValue(IConvertible value)
            {
                ConfigurationVariables.StoreSetting(this.Scope, this.Name, Convert.ToString(value));
            }
        }

        public delegate void ConfigurationVariablesChangedHander(object sender,
            ConfigurationVariables.ConfigVarsChangedArgs e);

        public class ConfigVarsChangedArgs : EventArgs
        {
            public string VariableName { get; set; }

            public ConfigVarsChangedArgs()
            {
            }
        }
    }
}