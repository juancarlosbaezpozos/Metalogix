using Metalogix;
using Metalogix.Licensing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Metalogix.Actions
{
    [System.ComponentModel.LicenseProvider(typeof(MLLicenseProvider))]
    public sealed class ActionLicenseProvider
    {
        private static volatile ActionLicenseProvider s_instance;

        private static readonly object s_oInstanceLock;

        private Dictionary<Type, bool> m_licensedActions;

        private License m_license;

        private bool m_dynLicenseHostLoaded;

        private object m_oLicensedActionsLock = new object();

        public static ActionLicenseProvider Instance
        {
            get
            {
                if (ActionLicenseProvider.s_instance == null)
                {
                    lock (ActionLicenseProvider.s_oInstanceLock)
                    {
                        if (ActionLicenseProvider.s_instance == null)
                        {
                            ActionLicenseProvider.s_instance = new ActionLicenseProvider();
                        }
                    }
                }

                return ActionLicenseProvider.s_instance;
            }
        }

        private Dictionary<Type, bool> LicensedActions
        {
            get
            {
                Dictionary<Type, bool> mLicensedActions;
                lock (this.m_oLicensedActionsLock)
                {
                    if (this.m_licensedActions == null)
                    {
                        this.RefreshLicensedActions();
                    }

                    mLicensedActions = this.m_licensedActions;
                }

                return mLicensedActions;
            }
        }

        static ActionLicenseProvider()
        {
            ActionLicenseProvider.s_instance = null;
            ActionLicenseProvider.s_oInstanceLock = new object();
        }

        private ActionLicenseProvider()
        {
            MLLicenseProvider.TryInitialize(null);
            MLLicenseProvider.LicenseUpdated += new EventHandler(this.On_MLLicenseProvider_LicenseUpdated);
            MLLicenseProvider.LicenseDisposed += new EventHandler(this.On_MLLicenseProvider_LicenseDisposed);
        }

        public static Type[] GetActions()
        {
            List<Type> types = new List<Type>();
            Type[] subTypesOf = Catalogs.GetSubTypesOf(typeof(Metalogix.Actions.Action), AssemblyTiers.Referenced);
            for (int i = 0; i < (int)subTypesOf.Length; i++)
            {
                Type type = subTypesOf[i];
                try
                {
                    if (type.BaseType != null && type.IsSubclassOf(typeof(Metalogix.Actions.Action)) &&
                        !type.IsAbstract && !types.Contains(type))
                    {
                        types.Add(type);
                    }
                }
                catch (Exception exception)
                {
                }
            }

            return types.ToArray();
        }

        public License GetLicense(LicenseContext context, Type type, object instance, bool allowExceptions)
        {
            return this.GetLicense(type, instance, true);
        }

        public License GetLicense(Type type, object instance)
        {
            return this.GetLicense(type, instance, true);
        }

        public License GetLicense(Type type, object instance, bool bThrowsExceptions)
        {
            bool flag;
            bool flag1;
            if (type == null)
            {
                if (bThrowsExceptions)
                {
                    throw new LicenseException(type, instance, "No type was specified");
                }

                return null;
            }

            if (!type.IsSubclassOf(typeof(Metalogix.Actions.Action)))
            {
                if (bThrowsExceptions)
                {
                    throw new LicenseException(type, instance,
                        "Invalid type was specified. The ActionLicenseProvider is only for use with Actions");
                }

                return null;
            }

            if (this.m_license == null)
            {
                License license;
                try
                {
                    this.m_license = LicenseManager.Validate(typeof(ActionLicenseProvider), null);
                    flag = false;
                    flag1 = this.LicensedActions.TryGetValue(type, out flag);
                    if (flag)
                    {
                        return this.m_license;
                    }

                    if (bThrowsExceptions)
                    {
                        throw new Exception(string.Concat("The action: '", type.FullName, "' is not licenced"));
                    }

                    return null;
                }
                catch
                {
                    if (bThrowsExceptions)
                    {
                        throw;
                    }

                    license = null;
                }

                return license;
            }

            flag = false;
            flag1 = this.LicensedActions.TryGetValue(type, out flag);
            if (flag)
            {
                return this.m_license;
            }

            if (bThrowsExceptions)
            {
                throw new Exception(string.Concat("The action: '", type.FullName, "' is not licenced"));
            }

            return null;
        }

        public bool IsValid(Type type)
        {
            if (!(this.GetLicense(type, null, false) is MLLicense))
            {
                return false;
            }

            return true;
        }

        private void On_MLLicenseProvider_LicenseDisposed(object sender, EventArgs e)
        {
            this.m_license = null;
            lock (this.m_oLicensedActionsLock)
            {
                this.m_licensedActions = null;
            }
        }

        private void On_MLLicenseProvider_LicenseUpdated(object sender, EventArgs e)
        {
            this.m_license = null;
            this.RefreshLicensedActions();
            if (this.ActionLicenseProviderUpdated != null)
            {
                this.ActionLicenseProviderUpdated();
            }
        }

        private void RefreshLicensedActions()
        {
            lock (this.m_oLicensedActionsLock)
            {
                if (this.m_licensedActions != null)
                {
                    Type[] array = this.m_licensedActions.Keys.ToArray<Type>();
                    for (int i = 0; i < (int)array.Length; i++)
                    {
                        Type type = array[i];
                        this.m_licensedActions[type] = type.CheckLicensing();
                    }
                }
                else
                {
                    Assembly mainAssembly = ApplicationData.MainAssembly;
                    if (mainAssembly == null)
                    {
                        throw new NullReferenceException("Main Assembly cannot be found.");
                    }

                    if (!ApplicationData.IsMetalogixAssembly(mainAssembly))
                    {
                        throw new Exception(string.Concat("'", mainAssembly.FullName,
                            "' is not a Metalogix assembly."));
                    }

                    if (mainAssembly.EntryPoint != null)
                    {
                        LicenseManager.Validate(mainAssembly.EntryPoint.DeclaringType);
                    }

                    Type[] actions = ActionLicenseProvider.GetActions();
                    this.m_licensedActions = new Dictionary<Type, bool>((int)actions.Length);
                    Type[] typeArray = actions;
                    for (int j = 0; j < (int)typeArray.Length; j++)
                    {
                        Type type1 = typeArray[j];
                        this.m_licensedActions.Add(type1, type1.CheckLicensing());
                    }
                }
            }
        }

        public void Validate(Type type, object instance)
        {
            ActionLicenseProvider.Instance.GetLicense(type, instance, true);
        }

        public event ActionLicenseProviderUpdatedHandler ActionLicenseProviderUpdated;
    }
}