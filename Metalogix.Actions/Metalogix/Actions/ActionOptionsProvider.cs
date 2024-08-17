using Metalogix;
using Metalogix.DataResolution;
using Metalogix.DataStructures.Generic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Xml;

namespace Metalogix.Actions
{
    public class ActionOptionsProvider
    {
        private static ActionOptionsProvider s_defaultsProvider;

        private static ActionOptionsProvider s_templatesProvider;

        private readonly Dictionary<string, List<ActionOptionsTemplate>> _actionOptionsTemplates =
            new Dictionary<string, List<ActionOptionsTemplate>>();

        private readonly ReaderWriterLockSlim _readWriteLock = new ReaderWriterLockSlim();

        protected Metalogix.DataResolution.DataResolver DataResolver { get; set; }

        public static string DefaultActionDefaultsFolderPath
        {
            get { return Path.Combine(ApplicationData.ApplicationPath, "ActionDefaults"); }
        }

        public static string DefaultActionTemplatesFolderPath
        {
            get { return Path.Combine(ApplicationData.ApplicationPath, "ActionTemplates"); }
        }

        public static ActionOptionsProvider DefaultsProvider
        {
            get { return ActionOptionsProvider.s_defaultsProvider; }
            set { ActionOptionsProvider.s_defaultsProvider = value; }
        }

        private static string OldDefaultActionOptionsFilePath
        {
            get { return Path.Combine(ApplicationData.ApplicationPath, "DefaultActionOptions.xml"); }
        }

        public static ActionOptionsProvider TemplatesProvider
        {
            get { return ActionOptionsProvider.s_templatesProvider; }
            set { ActionOptionsProvider.s_templatesProvider = value; }
        }

        public ActionOptionsProvider(Metalogix.DataResolution.DataResolver dataResolver)
        {
            this.DataResolver = dataResolver;
            this.LoadTemplates();
        }

        private void AddTemplateToCache(ActionOptionsTemplate template)
        {
            List<ActionOptionsTemplate> actionOptionsTemplates;
            if (!this._actionOptionsTemplates.TryGetValue(template.ActionTypeName, out actionOptionsTemplates))
            {
                actionOptionsTemplates = new List<ActionOptionsTemplate>();
                this._actionOptionsTemplates.Add(template.ActionTypeName, actionOptionsTemplates);
            }

            actionOptionsTemplates.Add(template);
        }

        public ActionOptionsTemplate CreateNewTemplate(string templateName, Type actionType, string optionsXml)
        {
            ActionOptionsTemplate actionOptionsTemplate;
            this._readWriteLock.EnterWriteLock();
            try
            {
                actionOptionsTemplate = this.CreateNewTemplate(templateName, actionType.FullName, optionsXml);
            }
            finally
            {
                this._readWriteLock.ExitWriteLock();
            }

            this.FireTemplateAdded(actionOptionsTemplate);
            return actionOptionsTemplate;
        }

        protected ActionOptionsTemplate CreateNewTemplate(string templateName, string actionType, string optionsXml)
        {
            ActionOptionsTemplate actionOptionsTemplate =
                new ActionOptionsTemplate(this, Guid.NewGuid(), templateName, actionType, optionsXml);
            this.SaveTemplate(actionOptionsTemplate);
            return actionOptionsTemplate;
        }

        public void DeleteAllTemplates()
        {
            this._readWriteLock.EnterWriteLock();
            try
            {
                this.DataResolver.ClearAllData();
                this._actionOptionsTemplates.Clear();
            }
            finally
            {
                this._readWriteLock.ExitWriteLock();
            }

            this.FireTemplatesCleared();
        }

        protected void DeleteDataAtKey(Guid guid_0)
        {
            this.DataResolver.DeleteDataAtKey(this.GetStringKeyFromGuid(guid_0));
        }

        public void DeleteTemplate(ActionOptionsTemplate template)
        {
            this.DeleteTemplates(new ActionOptionsTemplate[] { template });
        }

        public void DeleteTemplates(IEnumerable<ActionOptionsTemplate> templates)
        {
            this._readWriteLock.EnterWriteLock();
            try
            {
                foreach (ActionOptionsTemplate template in templates)
                {
                    this.RemoveActionOptionsTemplateFromCache(template);
                    this.DeleteDataAtKey(template.StorageKey);
                }
            }
            finally
            {
                this._readWriteLock.ExitWriteLock();
            }

            this.FireTemplatesDeleted(templates.ToArray<ActionOptionsTemplate>());
        }

        protected void FireTemplateAdded(ActionOptionsTemplate template)
        {
            if (this.TemplateAdded != null)
            {
                this.TemplateAdded(template);
            }
        }

        protected void FireTemplatesCleared()
        {
            if (this.TemplatesCleared != null)
            {
                this.TemplatesCleared();
            }
        }

        protected void FireTemplatesDeleted(ActionOptionsTemplate[] templates)
        {
            if (this.TemplatesDeleted != null)
            {
                this.TemplatesDeleted(templates);
            }
        }

        public ActionOptionsTemplate[] GetAllOptionsTemplates()
        {
            ActionOptionsTemplate[] array;
            if (!this._readWriteLock.IsWriteLockHeld)
            {
                this._readWriteLock.EnterReadLock();
            }

            try
            {
                List<ActionOptionsTemplate> actionOptionsTemplates = new List<ActionOptionsTemplate>();
                foreach (KeyValuePair<string, List<ActionOptionsTemplate>> _actionOptionsTemplate in this
                             ._actionOptionsTemplates)
                {
                    actionOptionsTemplates.AddRange(_actionOptionsTemplate.Value);
                }

                array = actionOptionsTemplates.ToArray();
            }
            finally
            {
                if (!this._readWriteLock.IsWriteLockHeld)
                {
                    this._readWriteLock.ExitReadLock();
                }
            }

            return array;
        }

        protected IEnumerable<Guid> GetAvailableTemplateKeys()
        {
            List<Guid> guids = new List<Guid>();
            foreach (string availableDataKey in this.DataResolver.GetAvailableDataKeys())
            {
                guids.Add(new Guid(availableDataKey));
            }

            return guids;
        }

        public ActionOptionsTemplate[] GetOptionsTemplatesForAction(Type actionType)
        {
            if (actionType == null)
            {
                throw new ArgumentNullException("actionType");
            }

            return this.GetOptionsTemplatesForAction(actionType.FullName);
        }

        public ActionOptionsTemplate[] GetOptionsTemplatesForAction(string actionTypeFullName)
        {
            List<ActionOptionsTemplate> actionOptionsTemplates;
            ActionOptionsTemplate[] actionOptionsTemplateArray;
            if (actionTypeFullName == null)
            {
                throw new ArgumentNullException("actionTypeFullName");
            }

            this._readWriteLock.EnterReadLock();
            try
            {
                actionOptionsTemplateArray =
                    (!this._actionOptionsTemplates.TryGetValue(actionTypeFullName, out actionOptionsTemplates)
                        ? new ActionOptionsTemplate[0]
                        : actionOptionsTemplates.ToArray());
            }
            finally
            {
                this._readWriteLock.ExitReadLock();
            }

            return actionOptionsTemplateArray;
        }

        protected string GetStringKeyFromGuid(Guid guid_0)
        {
            return guid_0.ToString("N");
        }

        protected bool GetTemplateIsInDataSource(ActionOptionsTemplate template)
        {
            List<ActionOptionsTemplate> actionOptionsTemplates;
            if (!this._actionOptionsTemplates.TryGetValue(template.ActionTypeName, out actionOptionsTemplates))
            {
                return false;
            }

            return (
                from actionOptionsTemplate_0 in actionOptionsTemplates
                where actionOptionsTemplate_0.StorageKey == template.StorageKey
                select actionOptionsTemplate_0).Count<ActionOptionsTemplate>() > 0;
        }

        public ActionOptionsTemplate ImportTemplate(Stream stream)
        {
            return new ActionOptionsTemplate(this, Guid.NewGuid(), stream);
        }

        protected void LoadTemplates()
        {
            this._readWriteLock.EnterWriteLock();
            try
            {
                this._actionOptionsTemplates.Clear();
                foreach (string availableDataKey in this.DataResolver.GetAvailableDataKeys())
                {
                    using (Stream memoryStream = new MemoryStream(this.DataResolver.GetDataAtKey(availableDataKey)))
                    {
                        this.AddTemplateToCache(new ActionOptionsTemplate(this, new Guid(availableDataKey),
                            memoryStream));
                    }
                }
            }
            finally
            {
                this._readWriteLock.ExitWriteLock();
            }
        }

        private void RemoveActionOptionsTemplateFromCache(ActionOptionsTemplate template)
        {
            List<ActionOptionsTemplate> actionOptionsTemplates;
            if (this._actionOptionsTemplates.TryGetValue(template.ActionTypeName, out actionOptionsTemplates))
            {
                actionOptionsTemplates.Remove(template);
                if (actionOptionsTemplates.Count == 0)
                {
                    this._actionOptionsTemplates.Remove(template.ActionTypeName);
                }
            }
        }

        public static void ResetDefaultOptions()
        {
            ActionOptionsProvider defaultsProvider = ActionOptionsProvider.DefaultsProvider;
            if (defaultsProvider != null)
            {
                defaultsProvider.DeleteAllTemplates();
            }
        }

        internal void SaveTemplate(ActionOptionsTemplate template)
        {
            bool flag = false;
            bool isWriteLockHeld = !this._readWriteLock.IsWriteLockHeld;
            bool flag1 = isWriteLockHeld;
            if (isWriteLockHeld)
            {
                this._readWriteLock.EnterWriteLock();
            }

            try
            {
                XmlWriterSettings xmlWriterSetting = new XmlWriterSettings()
                {
                    OmitXmlDeclaration = true
                };
                StringBuilder stringBuilder = new StringBuilder();
                using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, xmlWriterSetting))
                {
                    template.ToXml(xmlWriter);
                    xmlWriter.Flush();
                }

                byte[] bytes = xmlWriterSetting.Encoding.GetBytes(stringBuilder.ToString());
                this.DataResolver.WriteDataAtKey(this.GetStringKeyFromGuid(template.StorageKey), bytes);
                if (!this.GetTemplateIsInDataSource(template))
                {
                    this.AddTemplateToCache(template);
                    flag = true;
                }
            }
            finally
            {
                if (flag1)
                {
                    this._readWriteLock.ExitWriteLock();
                }
            }

            if (flag1 && flag)
            {
                this.FireTemplateAdded(template);
            }
        }

        public static void SetOptionsToDefault(Metalogix.Actions.Action action)
        {
            ActionOptionsProvider defaultsProvider = ActionOptionsProvider.DefaultsProvider;
            if (defaultsProvider == null || action == null || action.Options == null)
            {
                return;
            }

            ActionOptionsTemplate[] optionsTemplatesForAction =
                defaultsProvider.GetOptionsTemplatesForAction(action.GetType());
            if ((int)optionsTemplatesForAction.Length == 0)
            {
                return;
            }

            optionsTemplatesForAction[0].SetOptions(action, false);
        }

        public static void UpdateDefaultOptionsXml(Metalogix.Actions.Action action)
        {
            ActionOptionsProvider defaultsProvider = ActionOptionsProvider.DefaultsProvider;
            if (defaultsProvider == null || action == null || action.Options == null)
            {
                return;
            }

            string fullName = action.GetType().FullName;
            string xML = action.Options.ToXML();
            ActionOptionsTemplate[] optionsTemplatesForAction =
                defaultsProvider.GetOptionsTemplatesForAction(action.GetType());
            if ((int)optionsTemplatesForAction.Length == 0)
            {
                defaultsProvider.CreateNewTemplate(string.Concat("Default Settings: ", fullName), fullName, xML);
                return;
            }

            optionsTemplatesForAction[0].OptionsXml = xML;
            optionsTemplatesForAction[0].Commit();
        }

        public static void UpgradeOldDefaultOptionsFile()
        {
            if (File.Exists(ActionOptionsProvider.OldDefaultActionOptionsFilePath))
            {
                ActionOptionsProvider defaultsProvider = ActionOptionsProvider.DefaultsProvider;
                if (defaultsProvider == null)
                {
                    throw new Exception(
                        "Failed to upgrade default action options file. No action options provider specified.");
                }

                SerializableTable<string, string> commonSerializableTable =
                    new CommonSerializableTable<string, string>();
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(ActionOptionsProvider.OldDefaultActionOptionsFilePath);
                commonSerializableTable.FromXML(xmlDocument.FirstChild);
                foreach (KeyValuePair<string, string> keyValuePair in commonSerializableTable)
                {
                    if (defaultsProvider._actionOptionsTemplates.ContainsKey(keyValuePair.Key))
                    {
                        continue;
                    }

                    defaultsProvider.CreateNewTemplate(string.Concat("Default Settings: ", keyValuePair.Key),
                        keyValuePair.Key, keyValuePair.Value);
                }

                File.Delete(ActionOptionsProvider.OldDefaultActionOptionsFilePath);
            }
        }

        public event ActionOptionsProvider.ActionTemplateDelegate TemplateAdded;

        public event ActionOptionsProvider.ActionTemplatesClearedDelegate TemplatesCleared;

        public event ActionOptionsProvider.ActionTemplatesDelegate TemplatesDeleted;

        public delegate void ActionTemplateDelegate(ActionOptionsTemplate template);

        public delegate void ActionTemplatesClearedDelegate();

        public delegate void ActionTemplatesDelegate(ActionOptionsTemplate[] templates);
    }
}