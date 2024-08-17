using Metalogix;
using Metalogix.Actions;
using Metalogix.Actions.Properties;
using Metalogix.Core;
using Metalogix.Transformers.Attributes;
using Metalogix.Transformers.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Metalogix.Transformers
{
    public sealed class TransformerConfigurationProvider
    {
        private static string s_sTransformerInterfaceName;

        private static string s_sConfigInterfaceName;

        private readonly static object s_oLock;

        private static volatile TransformerConfigurationProvider s_instance;

        private Dictionary<Type, Type> m_data;

        public static TransformerConfigurationProvider Instance
        {
            get
            {
                if (TransformerConfigurationProvider.s_instance == null)
                {
                    lock (TransformerConfigurationProvider.s_oLock)
                    {
                        if (TransformerConfigurationProvider.s_instance == null)
                        {
                            TransformerConfigurationProvider transformerConfigurationProvider =
                                new TransformerConfigurationProvider();
                            transformerConfigurationProvider.LoadTransformerConfigs();
                            TransformerConfigurationProvider.s_instance = transformerConfigurationProvider;
                        }
                    }
                }

                return TransformerConfigurationProvider.s_instance;
            }
        }

        static TransformerConfigurationProvider()
        {
            TransformerConfigurationProvider.s_sTransformerInterfaceName = typeof(ITransformer).FullName;
            TransformerConfigurationProvider.s_sConfigInterfaceName = typeof(ITransformerConfig).FullName;
            TransformerConfigurationProvider.s_oLock = new object();
            TransformerConfigurationProvider.s_instance = null;
            ConfigurationVariables.ConfigurationVariablesChanged +=
                new ConfigurationVariables.ConfigurationVariablesChangedHander(TransformerConfigurationProvider
                    .EnvironmentVariables_EnvironmentVariablesChanged);
        }

        private TransformerConfigurationProvider()
        {
        }

        private static void EnvironmentVariables_EnvironmentVariablesChanged(object sender,
            ConfigurationVariables.ConfigVarsChangedArgs configVarsChangedArgs_0)
        {
            if (configVarsChangedArgs_0.VariableName == Resources.EnableCustomTransformersKey)
            {
                lock (TransformerConfigurationProvider.s_oLock)
                {
                    TransformerConfigurationProvider.s_instance = null;
                }
            }
        }

        public ITransformerConfig GetTransformerConfig(ITransformer transformer)
        {
            Type type = transformer.GetType();
            if (type.IsGenericType)
            {
                type = type.GetGenericTypeDefinition();
            }

            Type transformerConfig = this.GetTransformerConfig(type);
            if (transformerConfig == null)
            {
                return null;
            }

            return Activator.CreateInstance(transformerConfig) as ITransformerConfig;
        }

        private Type GetTransformerConfig(Type transformerType)
        {
            if (transformerType == null)
            {
                return null;
            }

            if (transformerType.GetInterface(TransformerConfigurationProvider.s_sTransformerInterfaceName) == null)
            {
                return null;
            }

            if (this.m_data.ContainsKey(transformerType))
            {
                return this.m_data[transformerType];
            }

            return this.GetTransformerConfig(transformerType.BaseType);
        }

        private void LoadTransformerConfigs()
        {
            Dictionary<Type, Type> types = new Dictionary<Type, Type>();
            AssemblyTiers assemblyTier = AssemblyTiers.Referenced | AssemblyTiers.Signed;
            if (ActionConfigurationVariables.EnableCustomTransformers)
            {
                assemblyTier |= AssemblyTiers.Unsigned;
            }

            Type[] typesByInterface = Catalogs.GetTypesByInterface(typeof(ITransformerConfig), assemblyTier);
            for (int i = 0; i < (int)typesByInterface.Length; i++)
            {
                Type type = typesByInterface[i];
                try
                {
                    if (type.IsDefined(typeof(TransformerConfigAttribute), true))
                    {
                        object[] customAttributes = type.GetCustomAttributes(typeof(TransformerConfigAttribute), true);
                        for (int j = 0; j < (int)customAttributes.Length; j++)
                        {
                            Type[] transformerTypes =
                                ((TransformerConfigAttribute)customAttributes[j]).TransformerTypes;
                            for (int k = 0; k < (int)transformerTypes.Length; k++)
                            {
                                Type genericTypeDefinition = transformerTypes[k];
                                if (genericTypeDefinition.IsGenericType)
                                {
                                    genericTypeDefinition = genericTypeDefinition.GetGenericTypeDefinition();
                                }

                                if (!types.ContainsKey(genericTypeDefinition))
                                {
                                    types.Add(genericTypeDefinition, type);
                                }
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                }
            }

            this.m_data = types;
        }
    }
}