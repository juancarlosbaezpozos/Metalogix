using Metalogix;
using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.Metabase.Attributes;
using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;

namespace Metalogix.Metabase.Actions
{
    public abstract class MetabaseAction<T> : Metalogix.Actions.Action<T>
    {
        private bool? _requiresSourceMetabaseConnection;

        private bool? _requiresTargetMetabaseConnection;

        private bool? _metabaseActionsShown;

        [Browsable(false)]
        public virtual bool RequiresSourceMetabaseConnection
        {
            get
            {
                if (!this._requiresSourceMetabaseConnection.HasValue)
                {
                    object[] customAttributes =
                        base.ActionType.GetCustomAttributes(typeof(RequiresSourceMetabaseConnectionAttribute), true);
                    if ((int)customAttributes.Length != 1)
                    {
                        this._requiresSourceMetabaseConnection = new bool?(false);
                    }
                    else
                    {
                        this._requiresSourceMetabaseConnection =
                            new bool?(((RequiresSourceMetabaseConnectionAttribute)customAttributes[0]).IsRequired);
                    }
                }

                return this._requiresSourceMetabaseConnection.Value;
            }
        }

        [Browsable(false)]
        public virtual bool RequiresTargetMetabaseConnection
        {
            get
            {
                if (!this._requiresTargetMetabaseConnection.HasValue)
                {
                    object[] customAttributes =
                        base.ActionType.GetCustomAttributes(typeof(RequiresTargetMetabaseConnectionAttribute), true);
                    if ((int)customAttributes.Length != 1)
                    {
                        this._requiresTargetMetabaseConnection = new bool?(false);
                    }
                    else
                    {
                        this._requiresTargetMetabaseConnection =
                            new bool?(((RequiresTargetMetabaseConnectionAttribute)customAttributes[0]).IsRequired);
                    }
                }

                return this._requiresTargetMetabaseConnection.Value;
            }
        }

        public override bool ShowInMenus
        {
            get
            {
                if (this._metabaseActionsShown.HasValue)
                {
                    if (!base.ShowInMenus)
                    {
                        return false;
                    }

                    return this._metabaseActionsShown.Value;
                }

                if (ApplicationData.MainAssembly == null)
                {
                    return base.ShowInMenus;
                }

                object[] customAttributes =
                    ApplicationData.MainAssembly.GetCustomAttributes(typeof(ShowMetabaseActionsInMenuAttribute), true);
                if ((int)customAttributes.Length != 1)
                {
                    this._metabaseActionsShown = new bool?(false);
                }
                else
                {
                    this._metabaseActionsShown =
                        new bool?(((ShowMetabaseActionsInMenuAttribute)customAttributes[0]).IsShown);
                }

                return false;
            }
        }

        protected MetabaseAction()
        {
        }

        public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
        {
            bool flag;
            if (!base.AppliesTo(sourceSelections, targetSelections))
            {
                return false;
            }

            if (this.RequiresSourceMetabaseConnection)
            {
                foreach (Node sourceSelection in sourceSelections)
                {
                    if (sourceSelection.MetabaseConnection != null)
                    {
                        continue;
                    }

                    flag = false;
                    return flag;
                }
            }

            if (this.RequiresTargetMetabaseConnection)
            {
                foreach (Node targetSelection in targetSelections)
                {
                    if (targetSelection.MetabaseConnection != null)
                    {
                        continue;
                    }

                    flag = false;
                    return flag;
                }
            }

            return true;
        }
    }
}