using Microsoft.SharePoint;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Metalogix.SharePoint.Adapters.OM
{
    internal class CaseInvariantFieldCollection
    {
        private readonly object _initLock = new object();

        private SPFieldCollection _baseFields;

        private volatile bool _initialised;

        private Dictionary<string, Guid> _internalNameIndexedGuids;

        private Dictionary<string, Guid> _displayNameIndexedGuids;

        private Dictionary<string, Guid> DisplayNameIndexedGuids
        {
            get
            {
                if (!this._initialised)
                {
                    lock (this._initLock)
                    {
                        if (!this._initialised)
                        {
                            this.BuildDictionaries();
                        }
                    }
                }

                return this._displayNameIndexedGuids;
            }
        }

        private Dictionary<string, Guid> InternalNameIndexedGuids
        {
            get
            {
                if (!this._initialised)
                {
                    lock (this._initLock)
                    {
                        if (!this._initialised)
                        {
                            this.BuildDictionaries();
                        }
                    }
                }

                return this._internalNameIndexedGuids;
            }
        }

        internal SPList List
        {
            get { return this._baseFields.List; }
        }

        internal SPWeb Web
        {
            get { return this._baseFields.Web; }
        }

        internal CaseInvariantFieldCollection(SPFieldCollection fields)
        {
            this._baseFields = fields;
        }

        internal string Add(SPField field)
        {
            string str = this._baseFields.Add(field);
            this.AddFieldToIndexedCollections(this._baseFields[field.Id]);
            return str;
        }

        internal string Add(string name, SPFieldType type, bool required)
        {
            string str = this._baseFields.Add(name, type, required);
            this.AddFieldToIndexedCollections(this._baseFields.GetFieldByInternalName(str));
            return str;
        }

        internal string AddFieldAsXml(string schemaXml, bool addToDefaultView, SPAddFieldOptions options)
        {
            string str = this._baseFields.AddFieldAsXml(schemaXml, addToDefaultView, options);
            this.AddFieldToIndexedCollections(this._baseFields.GetFieldByInternalName(str));
            return str;
        }

        internal string AddFieldAsXml(string schemaXml)
        {
            return this.AddFieldAsXml(schemaXml, true, 0);
        }

        private void AddFieldToIndexedCollections(SPField newField)
        {
            string lowerInvariant = newField.InternalName.ToLowerInvariant();
            string str = newField.Title.ToLowerInvariant();
            if (this.InternalNameIndexedGuids.ContainsKey(lowerInvariant))
            {
                Dictionary<string, Guid> internalNameIndexedGuids = this.InternalNameIndexedGuids;
                Guid id = newField.Id;
                internalNameIndexedGuids[lowerInvariant] = new Guid(id.ToString("B"));
            }
            else
            {
                Dictionary<string, Guid> strs = this.InternalNameIndexedGuids;
                Guid guid = newField.Id;
                strs.Add(lowerInvariant, new Guid(guid.ToString("B")));
            }

            if (!this.DisplayNameIndexedGuids.ContainsKey(str))
            {
                Dictionary<string, Guid> displayNameIndexedGuids = this.DisplayNameIndexedGuids;
                Guid id1 = newField.Id;
                displayNameIndexedGuids.Add(str, new Guid(id1.ToString("B")));
                return;
            }

            Dictionary<string, Guid> displayNameIndexedGuids1 = this.DisplayNameIndexedGuids;
            Guid guid1 = newField.Id;
            displayNameIndexedGuids1[lowerInvariant] = new Guid(guid1.ToString("B"));
        }

        private void BuildDictionaries()
        {
            if (!this._initialised)
            {
                this._internalNameIndexedGuids = new Dictionary<string, Guid>(this._baseFields.Count);
                this._displayNameIndexedGuids = new Dictionary<string, Guid>(this._baseFields.Count);
            }

            try
            {
                foreach (SPField _baseField in this._baseFields)
                {
                    if (!string.IsNullOrEmpty(_baseField.InternalName))
                    {
                        string lowerInvariant = _baseField.InternalName.ToLowerInvariant();
                        if (!this._internalNameIndexedGuids.ContainsKey(lowerInvariant))
                        {
                            Dictionary<string, Guid> strs = this._internalNameIndexedGuids;
                            Guid id = _baseField.Id;
                            strs.Add(lowerInvariant, new Guid(id.ToString("B")));
                        }
                    }

                    if (string.IsNullOrEmpty(_baseField.Title))
                    {
                        continue;
                    }

                    string str = _baseField.Title.ToLowerInvariant();
                    if (_baseField.Type == (SPFieldType)12 || _baseField.FromBaseType ||
                        this._displayNameIndexedGuids.ContainsKey(str))
                    {
                        continue;
                    }

                    Dictionary<string, Guid> strs1 = this._displayNameIndexedGuids;
                    Guid guid = _baseField.Id;
                    strs1.Add(str, new Guid(guid.ToString("B")));
                }
            }
            finally
            {
                this._initialised = true;
            }
        }

        internal void Delete(string displayName)
        {
            SPField item = this._baseFields[displayName];
            string lowerInvariant = item.InternalName.ToLowerInvariant();
            string str = item.Title.ToLowerInvariant();
            this._baseFields.Delete(displayName);
            if (this.InternalNameIndexedGuids.ContainsKey(lowerInvariant))
            {
                this.InternalNameIndexedGuids.Remove(lowerInvariant);
            }

            if (this.DisplayNameIndexedGuids.ContainsKey(str))
            {
                this.DisplayNameIndexedGuids.Remove(str);
            }
        }

        internal SPField GetFieldByDisplayName(string displayName)
        {
            SPField item = null;
            string lowerInvariant = displayName.ToLowerInvariant();
            if (this.DisplayNameIndexedGuids.ContainsKey(lowerInvariant))
            {
                item = this._baseFields[this.DisplayNameIndexedGuids[lowerInvariant]];
            }

            return item;
        }

        internal SPField GetFieldByID(Guid fieldID)
        {
            SPField item = null;
            if (this.InternalNameIndexedGuids.ContainsValue(fieldID))
            {
                item = this._baseFields[fieldID];
            }

            return item;
        }

        internal SPField GetFieldByInternalName(string internalName)
        {
            SPField item = null;
            string lowerInvariant = internalName.ToLowerInvariant();
            if (this.InternalNameIndexedGuids.ContainsKey(lowerInvariant))
            {
                item = this._baseFields[this.InternalNameIndexedGuids[lowerInvariant]];
            }

            return item;
        }

        internal SPField GetFieldByNames(string displayName, string internalName)
        {
            SPField item = null;
            string lowerInvariant = null;
            if (!string.IsNullOrEmpty(internalName))
            {
                lowerInvariant = internalName.ToLowerInvariant();
                if (this.InternalNameIndexedGuids.ContainsKey(lowerInvariant))
                {
                    item = this._baseFields[this.InternalNameIndexedGuids[lowerInvariant]];
                }
            }

            if (item == null && !string.IsNullOrEmpty(displayName))
            {
                lowerInvariant = displayName.ToLowerInvariant();
                if (this.DisplayNameIndexedGuids.ContainsKey(lowerInvariant))
                {
                    item = this._baseFields[this.DisplayNameIndexedGuids[lowerInvariant]];
                }
            }

            return item;
        }

        internal void UpdateIndexedCollection(string oldDisplayName, string newDisplayName)
        {
            string lowerInvariant = oldDisplayName.ToLowerInvariant();
            string str = newDisplayName.ToLowerInvariant();
            if (this.DisplayNameIndexedGuids.ContainsKey(lowerInvariant))
            {
                SPField item = this._baseFields[newDisplayName];
                this.DisplayNameIndexedGuids.Remove(lowerInvariant);
                Dictionary<string, Guid> displayNameIndexedGuids = this.DisplayNameIndexedGuids;
                Guid id = item.Id;
                displayNameIndexedGuids.Add(str, new Guid(id.ToString("B")));
            }
        }
    }
}