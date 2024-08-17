using Metalogix;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Metalogix.Actions
{
    public class ActionOptionsTemplate
    {
        public string ActionTypeName { get; private set; }

        public string OptionsXml { get; set; }

        public ActionOptionsProvider ParentProvider { get; private set; }

        public Guid StorageKey { get; private set; }

        public string TemplateName { get; set; }

        internal ActionOptionsTemplate(ActionOptionsProvider parent, Guid storageKey, Stream inputStream) : this(parent,
            storageKey)
        {
            ActionOptionsTemplate.JobConfiguration jobConfiguration =
                ActionOptionsTemplate.Load(inputStream, storageKey.ToString());
            this.TemplateName = jobConfiguration.TemplateName;
            this.ActionTypeName = jobConfiguration.ActionTypeName;
            this.OptionsXml = jobConfiguration.OptionsXml;
        }

        internal ActionOptionsTemplate(ActionOptionsProvider parent, Guid storageKey, string templateName,
            string actionType, string optionsXml) : this(parent, storageKey)
        {
            this.StorageKey = Guid.NewGuid();
            this.TemplateName = templateName;
            this.ActionTypeName = actionType;
            this.OptionsXml = optionsXml;
        }

        private ActionOptionsTemplate(ActionOptionsProvider parent, Guid storageKey)
        {
            if (parent == null)
            {
                throw new ArgumentNullException("parent");
            }

            this.ParentProvider = parent;
            this.StorageKey = storageKey;
        }

        private void CacheUnstickyProperties(ActionOptions originalOptions, ActionOptions templateOptions,
            List<PropertyInfo> unstickyPropertyList, List<object> unstickyPropertyValues,
            List<ActionOptions> unstickyOptionsList)
        {
            PropertyInfo[] properties =
                originalOptions.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            for (int i = 0; i < (int)properties.Length; i++)
            {
                PropertyInfo propertyInfo = properties[i];
                object[] customAttributes = propertyInfo.GetCustomAttributes(typeof(UsesStickySettingsAttribute), true);
                bool usesStickySettings = true;
                if ((int)customAttributes.Length > 0)
                {
                    usesStickySettings = ((UsesStickySettingsAttribute)customAttributes[0]).UsesStickySettings;
                }

                if (!usesStickySettings)
                {
                    unstickyPropertyList.Add(propertyInfo);
                    unstickyOptionsList.Add(templateOptions);
                    unstickyPropertyValues.Add(propertyInfo.GetValue(originalOptions, null));
                }
                else if (typeof(ActionOptions).IsAssignableFrom(propertyInfo.PropertyType))
                {
                    this.CacheUnstickyProperties(propertyInfo.GetValue(originalOptions, null) as ActionOptions,
                        propertyInfo.GetValue(templateOptions, null) as ActionOptions, unstickyPropertyList,
                        unstickyPropertyValues, unstickyOptionsList);
                }
            }
        }

        public void Commit()
        {
            this.ParentProvider.SaveTemplate(this);
        }

        public void Delete()
        {
            this.ParentProvider.DeleteTemplate(this);
        }

        public static ActionOptionsTemplate.JobConfiguration Load(Stream inputStream, string storageKey = "")
        {
            ActionOptionsTemplate.JobConfiguration value = new ActionOptionsTemplate.JobConfiguration();
            using (XmlReader xmlReader = XmlReader.Create(inputStream))
            {
                XElement root = XDocument.Load(xmlReader).Root;
                if (root == null)
                {
                    throw new Exception(
                        string.Format("The Action Template stored at key '{0}' is in an invalid format.", storageKey));
                }

                XAttribute xAttribute = root.Attribute("TemplateName");
                XAttribute xAttribute1 = root.Attribute("ActionType");
                XText xText = root.DescendantNodes().First<XNode>() as XText;
                if (xAttribute == null || xAttribute1 == null || xText == null || string.IsNullOrEmpty(xText.Value))
                {
                    throw new Exception(
                        string.Format("The Action Template stored at key '{0}' is in an invalid format.", storageKey));
                }

                value.TemplateName = xAttribute.Value;
                value.ActionTypeName = xAttribute1.Value;
                value.OptionsXml = xText.Value;
            }

            return value;
        }

        public void SetOptions(Metalogix.Actions.Action action, bool setUnstickyProperties = true)
        {
            if (action == null || action.Options == null)
            {
                return;
            }

            List<PropertyInfo> propertyInfos = new List<PropertyInfo>();
            List<ActionOptions> actionOptions = new List<ActionOptions>();
            List<object> objs = new List<object>();
            ActionOptions options = action.Options;
            ActionOptions actionOption = action.Options.Clone();
            actionOption.FromXML(this.OptionsXml);
            if (!setUnstickyProperties)
            {
                this.CacheUnstickyProperties(options, actionOption, propertyInfos, objs, actionOptions);
            }

            if (!setUnstickyProperties)
            {
                int num = 0;
                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    propertyInfo.SetValue(actionOptions[num], objs[num], null);
                    num++;
                }
            }

            options.SetFromOptions(actionOption);
        }

        public string ToXml()
        {
            StringBuilder stringBuilder = new StringBuilder(1000);
            XmlWriterSettings xmlWriterSetting = new XmlWriterSettings()
            {
                OmitXmlDeclaration = true
            };
            using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, xmlWriterSetting))
            {
                this.ToXml(xmlWriter);
                xmlWriter.Flush();
            }

            return stringBuilder.ToString();
        }

        public void ToXml(XmlWriter writer)
        {
            writer.WriteStartElement("ActionOptionsTemplate");
            writer.WriteAttributeString("TemplateName", this.TemplateName);
            writer.WriteAttributeString("ActionType", this.ActionTypeName);
            writer.WriteString(this.OptionsXml);
            writer.WriteEndElement();
        }

        public struct JobConfiguration
        {
            public string OptionsXml;

            public string TemplateName;

            public string ActionTypeName;
        }

        private static class XmlNames
        {
            public const string ActionOptionsTemplate = "ActionOptionsTemplate";

            public const string TemplateName = "TemplateName";

            public const string ActionType = "ActionType";

            public const string IsDefaultSettingsTemplate = "IsDefaultSettingsTemplate";
        }
    }
}