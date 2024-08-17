using Metalogix;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Metalogix.Actions
{
    public sealed class ActionConfigProvider : IActionConfigProvider
    {
        private readonly static object _LOCK;

        private static volatile ActionConfigProvider _INSTANCE;

        private readonly object m_lock = new object();

        private volatile Dictionary<Type, Type> m_data;

        private volatile Dictionary<Type, bool> m_dataRequired;

        public static ActionConfigProvider Instance
        {
            get
            {
                if (ActionConfigProvider._INSTANCE != null)
                {
                    return ActionConfigProvider._INSTANCE;
                }

                lock (ActionConfigProvider._LOCK)
                {
                    if (ActionConfigProvider._INSTANCE == null)
                    {
                        ActionConfigProvider actionConfigProvider = new ActionConfigProvider();
                        actionConfigProvider.LoadActionConfigs();
                        ActionConfigProvider._INSTANCE = actionConfigProvider;
                    }
                }

                return ActionConfigProvider._INSTANCE;
            }
        }

        static ActionConfigProvider()
        {
            ActionConfigProvider._LOCK = new object();
        }

        private ActionConfigProvider()
        {
            this.m_data = new Dictionary<Type, Type>();
            this.m_dataRequired = new Dictionary<Type, bool>();
        }

        public IActionConfig GetActionConfig(Metalogix.Actions.Action action)
        {
            IActionConfig actionConfig;
            try
            {
                actionConfig = this.GetActionConfig(action.GetType(), false, false);
            }
            catch (MissingActionConfigException missingActionConfigException)
            {
                throw new MissingActionConfigException(action);
            }

            return actionConfig;
        }

        private IActionConfig GetActionConfig(Type actionType, bool requiresConfig, bool alreadyDefined)
        {
            bool flag;
            if (this.m_data.ContainsKey(actionType))
            {
                return (IActionConfig)Activator.CreateInstance(this.m_data[actionType]);
            }

            if (alreadyDefined)
            {
                flag = requiresConfig;
            }
            else if (requiresConfig)
            {
                flag = true;
            }
            else
            {
                flag = (!this.m_dataRequired.ContainsKey(actionType) ? false : this.m_dataRequired[actionType]);
            }

            bool flag1 = flag;
            bool flag2 = (alreadyDefined ? true : this.m_dataRequired.ContainsKey(actionType));
            Type baseType = actionType.BaseType;
            if (baseType != null)
            {
                if (baseType != typeof(Metalogix.Actions.Action))
                {
                    return this.GetActionConfig(baseType, flag1, flag2);
                }
            }

            if (flag1)
            {
                throw new MissingActionConfigException();
            }

            return null;
        }

        private void LoadActionConfigs()
        {
            this.LoadActionConfigs(this.m_data);
        }

        private void LoadActionConfigs(Dictionary<Type, Type> output)
        {
            try
            {
                Type[] typesByInterface = Catalogs.GetTypesByInterface(typeof(IActionConfig), AssemblyTiers.Referenced);
                for (int i = 0; i < (int)typesByInterface.Length; i++)
                {
                    Type type = typesByInterface[i];
                    if (type.IsDefined(typeof(ActionConfigAttribute), true))
                    {
                        object[] customAttributes = type.GetCustomAttributes(typeof(ActionConfigAttribute), true);
                        for (int j = 0; j < (int)customAttributes.Length; j++)
                        {
                            Type[] actionTypes = ((ActionConfigAttribute)customAttributes[j]).ActionTypes;
                            for (int k = 0; k < (int)actionTypes.Length; k++)
                            {
                                Type type1 = actionTypes[k];
                                if (!output.ContainsKey(type1))
                                {
                                    output.Add(type1, type);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to load action config types.", exception);
            }
        }

        internal void RegisterAction(Metalogix.Actions.Action action)
        {
            Type type = action.GetType();
            if (this.m_dataRequired.ContainsKey(type))
            {
                return;
            }

            lock (this.m_lock)
            {
                if (!this.m_dataRequired.ContainsKey(type))
                {
                    object[] customAttributes =
                        action.GetType().GetCustomAttributes(typeof(ActionConfigRequiredAttribute), true);
                    int num = 0;
                    if (0 < (int)customAttributes.Length)
                    {
                        ActionConfigRequiredAttribute actionConfigRequiredAttribute =
                            (ActionConfigRequiredAttribute)customAttributes[num];
                        this.m_dataRequired.Add(type, actionConfigRequiredAttribute.IsRequired);
                    }
                }
            }
        }
    }
}