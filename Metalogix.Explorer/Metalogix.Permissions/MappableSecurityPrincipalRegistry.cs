using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using System.Xml;

namespace Metalogix.Permissions
{
    public class MappableSecurityPrincipalRegistry : SerializableTable<string, MappableSecurityPrincipalCollection>
    {
        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override MappableSecurityPrincipalCollection this[string key]
        {
            get { return this.GetMappings(key); }
            set { base[key] = value; }
        }

        public MappableSecurityPrincipalRegistry()
        {
            this.m_dictionary = new Dictionary<string, MappableSecurityPrincipalCollection>();
        }

        public MappableSecurityPrincipalRegistry(XmlNode xml)
        {
            this.FromXML(xml);
        }

        public void AddOrUpdateMapping(string key, MappableSecurityPrincipal value)
        {
            try
            {
                if (this[key] == null)
                {
                    this.m_dictionary.Add(key, new MappableSecurityPrincipalCollection(null));
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

        public MappableSecurityPrincipalCollection GetMappings(string key)
        {
            MappableSecurityPrincipalCollection mappableSecurityPrincipalCollection;
            MappableSecurityPrincipalCollection item = null;
            try
            {
                if (this.m_dictionary.ContainsKey(key))
                {
                    item = this.m_dictionary[key];
                }

                mappableSecurityPrincipalCollection = item;
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

            return mappableSecurityPrincipalCollection;
        }

        public void RemoveMapping(string key, MappableSecurityPrincipal value)
        {
            try
            {
                if (this[key] != null)
                {
                    this.m_dictionary[key].Remove(this.m_dictionary[key][value.PrincipalName]);
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