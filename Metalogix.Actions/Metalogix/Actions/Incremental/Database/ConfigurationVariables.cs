using Metalogix;
using Metalogix.Actions.Incremental.Database.Adapters;
using Metalogix.Actions.Properties;
using Metalogix.Core;
using Metalogix.Metabase;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Metalogix.Actions.Incremental.Database
{
    public class ConfigurationVariables : Metalogix.Core.ConfigurationVariables
    {
        private const string DefaultMappingDBAdapterKey = "DefaultMappingDBAdapter";

        private const string SQLMappingDBContextKey = "SQLMappingDBContext";

        private const string FileMappingDBContextKey = "FileMappingDBContext";

        private static AdapterCallWrapper delegate_MappingDatabaseCallWrapper;

        public static AdapterCallWrapper DefaultMappingDatabaseCallWrapper
        {
            get
            {
                AdapterCallWrapper delegateMappingDatabaseCallWrapper = Metalogix.Actions.Incremental.Database
                    .ConfigurationVariables.delegate_MappingDatabaseCallWrapper;
                if (delegateMappingDatabaseCallWrapper == null)
                {
                    delegateMappingDatabaseCallWrapper = (AdapterCallWrapperAction wrapperAction) => wrapperAction();
                    Metalogix.Actions.Incremental.Database.ConfigurationVariables.delegate_MappingDatabaseCallWrapper =
                        delegateMappingDatabaseCallWrapper;
                }

                return delegateMappingDatabaseCallWrapper;
            }
            set
            {
                Metalogix.Actions.Incremental.Database.ConfigurationVariables.delegate_MappingDatabaseCallWrapper =
                    value;
            }
        }

        public static string DefaultMappingDBAdapter
        {
            get
            {
                return Metalogix.Core.ConfigurationVariables.GetConfigurationValue<string>("DefaultMappingDBAdapter");
            }
            set
            {
                Metalogix.Core.ConfigurationVariables.SetConfigurationValue<string>("DefaultMappingDBAdapter", value);
            }
        }

        public static string FileMappingDBContext
        {
            get { return Metalogix.Core.ConfigurationVariables.GetConfigurationValue<string>("FileMappingDBContext"); }
            set { Metalogix.Core.ConfigurationVariables.SetConfigurationValue<string>("FileMappingDBContext", value); }
        }

        public static string SQLMappingDBContext
        {
            get { return Metalogix.Core.ConfigurationVariables.GetConfigurationValue<string>("SQLMappingDBContext"); }
            set { Metalogix.Core.ConfigurationVariables.SetConfigurationValue<string>("SQLMappingDBContext", value); }
        }

        static ConfigurationVariables()
        {
            Metalogix.Core.ConfigurationVariables.InitializeConfigurationVariable<string>(
                ResourceScope.ApplicationAndUserSpecific, "DefaultMappingDBAdapter",
                Convert.ToString(DatabaseAdapterType.SqlCe));
            Metalogix.Core.ConfigurationVariables.InitializeConfigurationVariable<string>(
                ResourceScope.ApplicationAndUserSpecific, Resources.SQLMappingDBContextKey, string.Empty);
            Metalogix.Core.ConfigurationVariables.InitializeConfigurationVariable<string>(
                ResourceScope.ApplicationAndUserSpecific, Resources.FileMappingDBContextKey,
                Path.Combine(ApplicationData.ApplicationPath, "IncrementalMapping.sdf"));
            Metalogix.Core.ConfigurationVariables.ConfigurationVariablesChanged +=
                new Metalogix.Core.ConfigurationVariables.ConfigurationVariablesChangedHander(Metalogix.Actions
                    .Incremental.Database.ConfigurationVariables.ConfigurationVariables_ConfigurationVariablesChanged);
        }

        public ConfigurationVariables()
        {
        }

        private static void ConfigurationVariables_ConfigurationVariablesChanged(object sender,
            Metalogix.Core.ConfigurationVariables.ConfigVarsChangedArgs configVarsChangedArgs_0)
        {
            if (string.IsNullOrEmpty(configVarsChangedArgs_0.VariableName))
            {
                FieldInfo[] fields =
                    typeof(Metalogix.Metabase.ConfigurationVariables).GetFields(BindingFlags.Static |
                        BindingFlags.NonPublic);
                for (int i = 0; i < (int)fields.Length; i++)
                {
                    FieldInfo fieldInfo = fields[i];
                    if (!fieldInfo.IsLiteral && !fieldInfo.FieldType.IsValueType || fieldInfo.FieldType.IsGenericType &&
                        fieldInfo.FieldType ==
                        typeof(Nullable<>).MakeGenericType(fieldInfo.FieldType.GetGenericArguments()))
                    {
                        fieldInfo.SetValue(null, null);
                    }
                }
            }
        }
    }
}