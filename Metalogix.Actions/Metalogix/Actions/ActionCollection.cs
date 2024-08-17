using Metalogix;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Metalogix.Actions
{
    public class ActionCollection : SerializableIndexedList<Action>
    {
        private static readonly object s_oLockAvailableActions = new object();

        private static ActionCollection s_availableActions = null;

        public static ActionCollection AvailableActions
        {
            get
            {
                if (ActionCollection.s_availableActions == null)
                {
                    lock (ActionCollection.s_oLockAvailableActions)
                    {
                        if (ActionCollection.s_availableActions == null)
                        {
                            ActionCollection.s_availableActions = ActionCollection.LoadActionsFromAssembly();
                        }
                    }
                }

                return ActionCollection.s_availableActions;
            }
        }

        public override bool IsReadOnly
        {
            get { return false; }
        }

        public Action this[Type actionType]
        {
            get { return base[actionType.FullName]; }
        }

        public Action this[string actionTypeFullName]
        {
            get { return base[actionTypeFullName]; }
        }

        public static void RefreshAvailableActions()
        {
            lock (ActionCollection.s_oLockAvailableActions)
            {
                ActionCollection.s_availableActions = ActionCollection.LoadActionsFromAssembly();
            }
        }

        private static ActionCollection LoadActionsFromAssembly()
        {
            var actionCollection = new ActionCollection();
            try
            {
                var subTypesOf = Catalogs.GetSubTypesOf(typeof(Action), AssemblyTiers.Referenced);
                var array = subTypesOf;
                for (var i = 0; i < array.Length; i++)
                {
                    var type = array[i];
                    try
                    {
                        if (ActionLicenseProvider.Instance.IsValid(type))
                        {
                            var obj = Activator.CreateInstance(type);
                            var item = (Action)obj;
                            if (!actionCollection.Contains(item))
                            {
                                actionCollection.Add(item);
                            }
                        }
                    }
                    catch (Exception ex1)
                    {
                    }
                }
            }
            catch (Exception ex2)
            {
            }

            return actionCollection;
        }

        public ActionCollection() : base("ActionType")
        {
        }

        protected override object GetKey(Action item)
        {
            return item.GetType().FullName;
        }

        public override bool Contains(Action a)
        {
            if (this.Count > 0)
            {
                return this[a] != null;
            }

            return false;
        }

        public ActionCollection GetActions(string sNameFilter)
        {
            var actionCollection = new ActionCollection();
            foreach (var current in ((IEnumerable<Action>)this))
            {
                if (current.Name.IndexOf(sNameFilter, StringComparison.Ordinal) >= 0)
                {
                    actionCollection.AddToCollection(current);
                }
            }

            return actionCollection;
        }

        public ActionCollection GetActions(Type actionType)
        {
            var actionCollection = new ActionCollection();
            foreach (var current in ((IEnumerable<Action>)this))
            {
                var type = current.GetType();
                if (!type.IsAbstract)
                {
                    if (actionType.IsInterface && type.GetInterface(actionType.FullName) != null)
                    {
                        actionCollection.AddToCollection(current);
                    }
                    else if (actionType.IsClass && type.IsSubclassOf(actionType))
                    {
                        actionCollection.AddToCollection(current);
                    }
                }
            }

            return actionCollection;
        }

        public ActionCollection GetApplicableActions(IXMLAbleList souceSelections, IXMLAbleList targetSelections)
        {
            var actionCollection = new ActionCollection();
            if (!ActionCollection.IsValidCollection(souceSelections) ||
                !ActionCollection.IsValidCollection(targetSelections))
            {
                return actionCollection;
            }

            var dictionary = new Dictionary<Action, long>();
            foreach (var current in ((IEnumerable<Action>)this))
            {
                Stopwatch.StartNew();
                try
                {
                    if (current.ShowInMenus &&
                        current.AppliesTo(
                            (current.SourceCardinality == Cardinality.Zero) ? NodeCollection.Empty : souceSelections,
                            targetSelections))
                    {
                        actionCollection.AddToCollection(current);
                    }
                }
                catch (Exception)
                {
                }

                dictionary.Add(current, Stopwatch.GetTimestamp());
            }

            return actionCollection;
        }

        private static bool IsValidCollection(IXMLAbleList collection)
        {
            return collection != null && (collection.Count == 0 || collection[0] != null);
        }
    }
}