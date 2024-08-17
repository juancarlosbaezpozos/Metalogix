using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using System.Xml;

namespace Metalogix.Permissions
{
    public class MappableRoleRegistry : SerializableTable<string, MappableRoleCollection>
    {
        public MappableRoleCollection FirstMappings
        {
            get
            {
                MappableRoleCollection item;
                if (this.m_dictionary.Count > 0)
                {
                    using (IEnumerator<string> enumerator = this.m_dictionary.Keys.GetEnumerator())
                    {
                        if (enumerator.MoveNext())
                        {
                            string current = enumerator.Current;
                            item = this.m_dictionary[current];
                        }
                        else
                        {
                            return null;
                        }
                    }

                    return item;
                }

                return null;
            }
        }

        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override MappableRoleCollection this[string key]
        {
            get { return this.GetMappings(key); }
            set { base[key] = value; }
        }

        public MappableRoleRegistry()
        {
            this.m_dictionary = new Dictionary<string, MappableRoleCollection>();
        }

        public MappableRoleRegistry(XmlNode xml)
        {
            this.FromXML(xml);
        }

        public void AddOrUpdateMapping(string key, MappableRole value)
        {
            try
            {
                if (this[key] == null)
                {
                    this.m_dictionary.Add(key, new MappableRoleCollection());
                }

                this.m_dictionary[key].Add(value);
            }
            finally
            {
                if (this.OnMappingsChanged != null)
                {
                    CollectionChangeEventArgs collectionChangeEventArg =
                        new CollectionChangeEventArgs(CollectionChangeAction.Add, value);
                    this.OnMappingsChanged(key, collectionChangeEventArg);
                }
            }
        }

        public static string GenerateKey(Node source, Node target)
        {
            if (source == null || target == null)
            {
                return string.Empty;
            }

            return string.Concat(source.DisplayUrl, target.DisplayUrl);
        }

        public MappableRoleCollection GetMappings(string key)
        {
            MappableRoleCollection mappableRoleCollection;
            MappableRoleCollection item = null;
            try
            {
                if (this.m_dictionary.ContainsKey(key))
                {
                    item = this.m_dictionary[key];
                }

                mappableRoleCollection = item;
            }
            finally
            {
                if (this.OnMappingsChanged != null)
                {
                    CollectionChangeEventArgs collectionChangeEventArg =
                        new CollectionChangeEventArgs(CollectionChangeAction.Refresh, item);
                    this.OnMappingsChanged(key, collectionChangeEventArg);
                }
            }

            return mappableRoleCollection;
        }

        public void RemoveMapping(string key, MappableRole value)
        {
            try
            {
                if (this[key] != null)
                {
                    this.m_dictionary[key].Remove(this.m_dictionary[key][value.RoleName]);
                }
            }
            finally
            {
                if (this.OnMappingsChanged != null)
                {
                    CollectionChangeEventArgs collectionChangeEventArg =
                        new CollectionChangeEventArgs(CollectionChangeAction.Remove, value);
                    this.OnMappingsChanged(key, collectionChangeEventArg);
                }
            }
        }

        public event CollectionChangeEventHandler OnMappingsChanged;
    }
}