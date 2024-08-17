using Metalogix;
using Metalogix.Core;
using Metalogix.Explorer.Properties;
using Metalogix.Metabase.Interfaces;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Metalogix.Metabase
{
    public class ConfigurationVariables : Metalogix.Core.ConfigurationVariables
    {
        private const string DefaultMetabaseAdapterKey = "DefaultMetabaseAdapter";

        private const string SQLMetabaseContextKey = "SQLMetabaseContext";

        private const string AutoProvisionNewMetabaseFileKey = "AutoProvisionNewMetabaseFile";

        private const string FileMetabaseContextKey = "FileMetabaseContext";

        private static AdapterCallWrapper delegate_MetabaseCallWrapper;

        public static bool AutoProvisionNewMetabaseFile
        {
            get
            {
                return Metalogix.Core.ConfigurationVariables
                    .GetConfigurationValue<bool>("AutoProvisionNewMetabaseFile");
            }
            set
            {
                Metalogix.Core.ConfigurationVariables.SetConfigurationValue<bool>("AutoProvisionNewMetabaseFile",
                    value);
            }
        }

        public static string DefaultMetabaseAdapter
        {
            get
            {
                return Metalogix.Core.ConfigurationVariables.GetConfigurationValue<string>("DefaultMetabaseAdapter");
            }
            set
            {
                Metalogix.Core.ConfigurationVariables.SetConfigurationValue<string>("DefaultMetabaseAdapter", value);
            }
        }

        public static AdapterCallWrapper DefaultMetabaseCallWrapper
        {
            get
            {
                AdapterCallWrapper delegateMetabaseCallWrapper =
                    Metalogix.Metabase.ConfigurationVariables.delegate_MetabaseCallWrapper;
                if (delegateMetabaseCallWrapper == null)
                {
                    delegateMetabaseCallWrapper = (AdapterCallWrapperAction wrapperAction) => wrapperAction();
                    Metalogix.Metabase.ConfigurationVariables.delegate_MetabaseCallWrapper =
                        delegateMetabaseCallWrapper;
                }

                return delegateMetabaseCallWrapper;
            }
            set { Metalogix.Metabase.ConfigurationVariables.delegate_MetabaseCallWrapper = value; }
        }

        public static string FileMetabaseContext
        {
            get { return Metalogix.Core.ConfigurationVariables.GetConfigurationValue<string>("FileMetabaseContext"); }
            set { Metalogix.Core.ConfigurationVariables.SetConfigurationValue<string>("FileMetabaseContext", value); }
        }

        public static string SQLMetabaseContext
        {
            get { return Metalogix.Core.ConfigurationVariables.GetConfigurationValue<string>("SQLMetabaseContext"); }
            set { Metalogix.Core.ConfigurationVariables.SetConfigurationValue<string>("SQLMetabaseContext", value); }
        }

        static ConfigurationVariables()
        {
            Metalogix.Core.ConfigurationVariables.InitializeConfigurationVariable<string>(
                ResourceScope.ApplicationAndUserSpecific, "DefaultMetabaseAdapter", "SqlCe");
            Metalogix.Core.ConfigurationVariables.InitializeConfigurationVariable<string>(
                ResourceScope.ApplicationAndUserSpecific, Resources.SQLMetabaseContextKey, string.Empty);
            Metalogix.Core.ConfigurationVariables.InitializeConfigurationVariable<bool>(
                ResourceScope.ApplicationAndUserSpecific, Resources.AutoProvisionNewMetabaseFileKey, true);
            Metalogix.Core.ConfigurationVariables.InitializeConfigurationVariable<string>(
                ResourceScope.ApplicationAndUserSpecific, Resources.FileMetabaseContextKey,
                ApplicationData.ApplicationPath);
            Metalogix.Core.ConfigurationVariables.ConfigurationVariablesChanged +=
                new Metalogix.Core.ConfigurationVariables.ConfigurationVariablesChangedHander(Metalogix.Metabase
                    .ConfigurationVariables.ConfigurationVariables_ConfigurationVariablesChanged);
        }

        public ConfigurationVariables()
        {
        }

        private static void ConfigurationVariables_ConfigurationVariablesChanged(object sender,
            Metalogix.Core.ConfigurationVariables.ConfigVarsChangedArgs e)
        {
            if (string.IsNullOrEmpty(e.VariableName))
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