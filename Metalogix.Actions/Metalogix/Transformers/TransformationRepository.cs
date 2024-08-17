using System;
using System.Collections.Generic;

namespace Metalogix.Transformers
{
    public class TransformationRepository
    {
        public const string C_LISTXML_KEY = "listXml";

        public const string C_PROPERTY_BAG_PARENT_KEY = "PropertyBag";

        public const string C_TERMSTORE_REFRESH_CHILD_KEY = "TermStoreRefreshed";

        public const string C_TERMSTORE_PARENT_KEY_REVERSE = "$TSPKR$";

        public const string C_TERMSTORE_PARENT_KEY = "$TERMSTORE$";

        public const string C_FS_TERMSTORE_REFRESH = "$TSRF_{0}";

        public const string C_TERMSTORE_ADAPTER_CALL_REQUIRED = "$TSACR$";

        private volatile Dictionary<string, Dictionary<string, string>> _repository;

        private readonly object _objLock = new object();

        private readonly object _objEditLock = new object();

        private Dictionary<string, Dictionary<string, string>> Repository
        {
            get
            {
                if (this._repository == null)
                {
                    lock (this._objLock)
                    {
                        if (this._repository == null)
                        {
                            this._repository = new Dictionary<string, Dictionary<string, string>>();
                        }
                    }
                }

                return this._repository;
            }
        }

        public TransformationRepository()
        {
        }

        public void Add(string parentKey, string childKey, string value)
        {
            lock (this._objEditLock)
            {
                if (!this.Repository.ContainsKey(parentKey))
                {
                    this.Repository.Add(parentKey, new Dictionary<string, string>());
                }

                if (!this.Repository[parentKey].ContainsKey(childKey))
                {
                    this.Repository[parentKey].Add(childKey, value);
                }
                else
                {
                    this.Repository[parentKey][childKey] = value;
                }
            }
        }

        public void Clear()
        {
            lock (this._objEditLock)
            {
                this.Repository.Clear();
            }
        }

        public int Count()
        {
            int count = 0;
            lock (this._objEditLock)
            {
                count = this.Repository.Count;
            }

            return count;
        }

        public bool DoesParentKeyExist(string parentKey)
        {
            bool flag;
            lock (this._objEditLock)
            {
                flag = this.Repository.ContainsKey(parentKey);
            }

            return flag;
        }

        public static string GenerateKey(string prefix, string uniqueValue)
        {
            return string.Format("{0}_{1}", prefix, uniqueValue);
        }

        public string GetFirstValueForKey(string childKey)
        {
            KeyValuePair<string, Dictionary<string, string>> current;
            string str;
            lock (this._objEditLock)
            {
                string str1 = null;
                Dictionary<string, Dictionary<string, string>>.Enumerator enumerator = this.Repository.GetEnumerator();
                try
                {
                    do
                    {
                        if (!enumerator.MoveNext())
                        {
                            break;
                        }

                        current = enumerator.Current;
                    } while (!this.Repository[current.Key].TryGetValue(childKey, out str1));
                }
                finally
                {
                    ((IDisposable)enumerator).Dispose();
                }

                str = str1;
            }

            return str;
        }

        public string GetValueForKey(string parentKey, string childKey)
        {
            string str;
            string item;
            lock (this._objEditLock)
            {
                if (!this.Repository.ContainsKey(parentKey))
                {
                    item = null;
                }
                else if (this.Repository[parentKey].ContainsKey(childKey))
                {
                    item = this.Repository[parentKey][childKey];
                }
                else
                {
                    item = null;
                }

                str = item;
            }

            return str;
        }

        public Dictionary<string, string> GetValuesForKey(string parentKey)
        {
            Dictionary<string, string> strs;
            Dictionary<string, string> item;
            lock (this._objEditLock)
            {
                if (this.Repository.ContainsKey(parentKey))
                {
                    item = this.Repository[parentKey];
                }
                else
                {
                    item = null;
                }

                strs = item;
            }

            return strs;
        }

        public void Remove(string parentKey)
        {
            lock (this._objEditLock)
            {
                if (this.Repository.ContainsKey(parentKey))
                {
                    this.Repository[parentKey].Clear();
                    this.Repository[parentKey] = null;
                    this.Repository.Remove(parentKey);
                }
            }
        }

        public void Remove(string parentKey, string childKey)
        {
            lock (this._objEditLock)
            {
                if (this.Repository.ContainsKey(parentKey) && this.Repository[parentKey].ContainsKey(childKey))
                {
                    this.Repository[parentKey][childKey] = null;
                    this.Repository[parentKey].Remove(childKey);
                }
            }
        }
    }
}