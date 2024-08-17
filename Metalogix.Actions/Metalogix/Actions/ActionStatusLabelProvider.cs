using Metalogix.Actions.Properties;
using System;
using System.Collections.Generic;

namespace Metalogix.Actions
{
    public class ActionStatusLabelProvider
    {
        private Dictionary<ActionStatus, string> _statusMessageMap;

        protected virtual Dictionary<ActionStatus, string> StatusMessageMap
        {
            get
            {
                if (this._statusMessageMap == null)
                {
                    this._statusMessageMap = new Dictionary<ActionStatus, string>()
                    {
                        { ActionStatus.Aborted, Resources.StatusLabel_Aborted },
                        { ActionStatus.Aborting, Resources.StatusLabel_Aborted },
                        { ActionStatus.Done, Resources.StatusLabel_Completed },
                        { ActionStatus.Failed, Resources.StatusLabel_Failed },
                        { ActionStatus.NotRunning, Resources.StatusLabel_NotRunning },
                        { ActionStatus.Paused, Resources.StatusLabel_Paused },
                        { ActionStatus.Running, Resources.StatusLabel_Running },
                        { ActionStatus.Warning, Resources.StatusLabel_CompletedWithWarning }
                    };
                }

                return this._statusMessageMap;
            }
            set { this._statusMessageMap = value; }
        }

        public ActionStatusLabelProvider()
        {
        }

        public static ActionStatusLabelProvider GetStatusLabelProvider(Metalogix.Actions.Action action)
        {
            if (action == null)
            {
                return new ActionStatusLabelProvider();
            }

            return ActionStatusLabelProvider.GetStatusLabelProvider(action.GetType());
        }

        public static ActionStatusLabelProvider GetStatusLabelProvider(Type type)
        {
            if (type == null)
            {
                return new ActionStatusLabelProvider();
            }

            StatusLabelProviderAttribute attributeFromType = StatusLabelProviderAttribute.GetAttributeFromType(type);
            if (attributeFromType == null)
            {
                return new ActionStatusLabelProvider();
            }

            if (!typeof(ActionStatusLabelProvider).IsAssignableFrom(attributeFromType.ProviderType))
            {
                throw new Exception(string.Format("{0} is not a valid action status label provider type.",
                    attributeFromType.ProviderType.FullName));
            }

            return (ActionStatusLabelProvider)Activator.CreateInstance(attributeFromType.ProviderType);
        }

        public virtual string GetStatusMessage(ActionStatus status)
        {
            return this.StatusMessageMap[status];
        }
    }
}