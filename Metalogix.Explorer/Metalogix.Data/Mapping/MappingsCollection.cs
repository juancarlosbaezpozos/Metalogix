using Metalogix.DataStructures.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace Metalogix.Data.Mapping
{
    public class MappingsCollection : SerializableList<ListSummaryItem>
    {
        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override bool IsSet
        {
            get { return false; }
        }

        public override ListSummaryItem this[ListSummaryItem key]
        {
            get
            {
                ListSummaryItem listSummaryItem;
                if (key != null)
                {
                    using (IEnumerator<ListSummaryItem> enumerator = base.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            ListSummaryItem current = enumerator.Current;
                            if (!(key.Source.Target == current.Source.Target) ||
                                !(key.Target.Target == current.Target.Target))
                            {
                                continue;
                            }

                            listSummaryItem = current;
                            return listSummaryItem;
                        }

                        return null;
                    }

                    return listSummaryItem;
                }

                return null;
            }
        }

        public ListSummaryItem this[string mappingName]
        {
            get
            {
                ListSummaryItem listSummaryItem;
                if (!string.IsNullOrEmpty(mappingName))
                {
                    using (IEnumerator<ListSummaryItem> enumerator = base.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            ListSummaryItem current = enumerator.Current;
                            if (!mappingName.Equals(current.Source.Target, StringComparison.InvariantCultureIgnoreCase))
                            {
                                continue;
                            }

                            listSummaryItem = current;
                            return listSummaryItem;
                        }

                        return null;
                    }

                    return listSummaryItem;
                }

                return null;
            }
        }

        public MappingsCollection()
        {
        }

        public MappingsCollection(ListSummaryItem[] items) : base(items)
        {
        }

        public MappingsCollection(XmlNode node)
        {
            this.FromXML(node);
        }
    }
}