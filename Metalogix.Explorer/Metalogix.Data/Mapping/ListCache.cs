using System;
using System.Collections.Generic;
using System.Reflection;

namespace Metalogix.Data.Mapping
{
    public sealed class ListCache
    {
        private static List<IListView> m_listViews;

        private static List<IListTransformer> m_listTransformers;

        private static List<IListFilter> m_listFilters;

        private static List<IListGrouper> m_listGroupers;

        private static List<IListPickerAction> m_listPickerActions;

        private static List<IListSummaryAction> m_listSummaryActions;

        private static List<IListPickerComparer> m_listPickerComparers;

        public static List<IListFilter> ListFilters
        {
            get
            {
                if (ListCache.m_listFilters == null)
                {
                    ListCache.m_listFilters = ListCache.GetList<IListFilter>();
                }

                return ListCache.m_listFilters;
            }
        }

        public static List<IListGrouper> ListGroupers
        {
            get
            {
                if (ListCache.m_listGroupers == null)
                {
                    ListCache.m_listGroupers = ListCache.GetList<IListGrouper>();
                }

                return ListCache.m_listGroupers;
            }
        }

        public static List<IListPickerAction> ListPickerActions
        {
            get
            {
                if (ListCache.m_listPickerActions == null)
                {
                    ListCache.m_listPickerActions = ListCache.GetList<IListPickerAction>();
                }

                return ListCache.m_listPickerActions;
            }
        }

        public static List<IListPickerComparer> ListPickerComparers
        {
            get
            {
                if (ListCache.m_listPickerComparers == null)
                {
                    ListCache.m_listPickerComparers = ListCache.GetList<IListPickerComparer>();
                }

                return ListCache.m_listPickerComparers;
            }
        }

        public static List<IListSummaryAction> ListSummaryActions
        {
            get
            {
                if (ListCache.m_listSummaryActions == null)
                {
                    ListCache.m_listSummaryActions = ListCache.GetList<IListSummaryAction>();
                }

                return ListCache.m_listSummaryActions;
            }
        }

        public static List<IListTransformer> ListTransformers
        {
            get
            {
                if (ListCache.m_listTransformers == null)
                {
                    ListCache.m_listTransformers = ListCache.GetList<IListTransformer>();
                }

                return ListCache.m_listTransformers;
            }
        }

        public static List<IListView> ListViews
        {
            get
            {
                if (ListCache.m_listViews == null)
                {
                    ListCache.m_listViews = ListCache.GetList<IListView>();
                }

                return ListCache.m_listViews;
            }
        }

        public ListCache()
        {
        }

        private static List<T> GetList<T>()
        {
            List<T> ts = new List<T>();
            Assembly entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly != null)
            {
                foreach (AssemblyName assemblyName in new List<AssemblyName>(entryAssembly.GetReferencedAssemblies()))
                {
                    try
                    {
                        Type[] exportedTypes = Assembly.Load(assemblyName).GetExportedTypes();
                        for (int i = 0; i < (int)exportedTypes.Length; i++)
                        {
                            Type type = exportedTypes[i];
                            if (!type.IsAbstract && !type.IsInterface &&
                                (int)type.GetCustomAttributes(typeof(IgnoreAttribute), true).Length == 0 &&
                                (type.GetInterface(typeof(T).FullName) != null || type.IsSubclassOf(typeof(T))))
                            {
                                try
                                {
                                    ts.Add((T)Activator.CreateInstance(type));
                                }
                                catch (Exception exception)
                                {
                                }
                            }
                        }
                    }
                    catch (Exception exception1)
                    {
                        if (!exception1.Message.StartsWith("Could not load file or assembly 'DevExpress.Sparkline",
                                StringComparison.OrdinalIgnoreCase))
                        {
                            throw;
                        }
                    }
                }
            }

            return ts;
        }
    }
}