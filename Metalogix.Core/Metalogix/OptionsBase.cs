using Metalogix.Actions;
using Metalogix.Data;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Security;
using System.Text;
using System.Xml;

namespace Metalogix
{
    public class OptionsBase : IXmlable, ICustomTypeDescriptor
    {
        private readonly static EncryptedValueParameterAttribute s_encrypedAttribute;

        static OptionsBase()
        {
            OptionsBase.s_encrypedAttribute = new EncryptedValueParameterAttribute(true);
        }

        public OptionsBase()
        {
        }

        public OptionsBase Clone()
        {
            OptionsBase optionsBase = (OptionsBase)Activator.CreateInstance(this.GetType());
            optionsBase.FromXML(this.ToXML());
            return optionsBase;
        }

        protected virtual object DeserializeOption(XmlNode optionNode, Type optionType, bool isEncrypted,
            out bool isDeserialized)
        {
            SecureString secureString;
            object xmlNode = null;
            bool flag = false;
            if (optionNode != null && optionNode.Attributes != null && optionNode.Attributes["Type"] != null &&
                (optionNode.Attributes["Type"].Value == optionType.FullName ||
                 TypeUtils.UpdateType(optionNode.Attributes["Type"].Value) == optionType.AssemblyQualifiedName))
            {
                if (typeof(IXMLAbleList).IsAssignableFrom(optionType) || typeof(IXmlable).IsAssignableFrom(optionType))
                {
                    if (optionType.GetConstructor(new Type[] { typeof(XmlNode) }) == null)
                    {
                        Type[] typeArray = new Type[] { typeof(XmlNode) };
                        MethodInfo method = optionType.GetMethod("FromXML", BindingFlags.Instance | BindingFlags.Public,
                            null, CallingConventions.HasThis, typeArray, null);
                        if (method != null)
                        {
                            object obj = Activator.CreateInstance(optionType);
                            object[] objArray = new object[] { optionNode };
                            method.Invoke(obj, objArray);
                            xmlNode = obj;
                            flag = true;
                        }
                    }
                    else
                    {
                        object[] objArray1 = new object[] { optionNode.CloneNode(true) };
                        xmlNode = Activator.CreateInstance(optionType, objArray1);
                        flag = true;
                    }
                }
                else if (typeof(XmlNode).IsAssignableFrom(optionType))
                {
                    xmlNode = XmlUtility.StringToXmlNode(optionNode.InnerXml);
                    flag = true;
                }
                else if (!optionType.IsEnum)
                {
                    bool flag1 = false;
                    if (optionType.GetInterface("IConvertible", true) != null)
                    {
                        xmlNode = Convert.ChangeType(optionNode.InnerText, optionType);
                        flag1 = true;
                    }
                    else if (optionType.IsGenericType &&
                             typeof(Nullable<>).IsAssignableFrom(optionType.GetGenericTypeDefinition()))
                    {
                        Type genericArguments = optionType.GetGenericArguments()[0];
                        xmlNode = Convert.ChangeType(optionNode.InnerText, genericArguments);
                        flag1 = true;
                    }

                    if (flag1)
                    {
                        if (!isEncrypted)
                        {
                            flag = true;
                        }
                        else
                        {
                            string str = xmlNode as string;
                            if (!string.IsNullOrEmpty(str) &&
                                Cryptography.IsEncryptedUnderCurrentUserContext(str, out secureString))
                            {
                                xmlNode = secureString.ToInsecureString();
                                flag = true;
                            }
                        }
                    }
                }
                else
                {
                    xmlNode = Enum.Parse(optionType, optionNode.InnerText);
                    flag = true;
                }
            }

            isDeserialized = flag;
            return xmlNode;
        }

        public override bool Equals(object obj)
        {
            if (this.GetType() != obj.GetType())
            {
                return false;
            }

            return this.ToXML() == ((OptionsBase)obj).ToXML();
        }

        public void FromXML(string sXml)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(sXml);
            this.FromXML(xmlDocument.DocumentElement);
        }

        public virtual void FromXML(XmlNode xmlNode)
        {
            bool flag;
            foreach (PropertyDescriptor property in this.GetProperties())
            {
                if (property.IsReadOnly)
                {
                    continue;
                }

                try
                {
                    XmlNode xmlNodes = xmlNode.SelectSingleNode(string.Concat("./", property.Name));
                    bool flag1 = property.Attributes.Matches(OptionsBase.s_encrypedAttribute);
                    object obj = this.DeserializeOption(xmlNodes, property.PropertyType, flag1, out flag);
                    if (flag)
                    {
                        property.SetValue(this, obj);
                    }
                }
                catch (Exception exception)
                {
                }
            }
        }

        public virtual AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this.GetType());
        }

        public virtual string GetClassName()
        {
            return TypeDescriptor.GetClassName(this.GetType());
        }

        public virtual string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this.GetType());
        }

        public virtual TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this.GetType());
        }

        public virtual EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this.GetType());
        }

        public virtual PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this.GetType());
        }

        public virtual object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this.GetType(), editorBaseType);
        }

        public virtual EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this.GetType(), attributes);
        }

        public virtual EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this.GetType());
        }

        public override int GetHashCode()
        {
            return this.ToXML().GetHashCode();
        }

        public virtual PropertyDescriptorCollection GetProperties()
        {
            return this.GetProperties(null);
        }

        public virtual PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return TypeDescriptor.GetProperties(this.GetType(), attributes);
        }

        public virtual object GetPropertyOwner(PropertyDescriptor pd)
        {
            if (pd == null)
            {
                return this;
            }

            if (!this.GetProperties().Contains(pd))
            {
                return null;
            }

            return this;
        }

        protected string SerializeOption(PropertyDescriptor propDescriptor, OptionsBase options)
        {
            StringBuilder stringBuilder = new StringBuilder();
            this.SerializeOption(propDescriptor, options, new XmlTextWriter(new StringWriter(stringBuilder)));
            return stringBuilder.ToString();
        }

        protected virtual void SerializeOption(PropertyDescriptor propDescriptor, OptionsBase options,
            XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement(propDescriptor.Name);
            xmlWriter.WriteAttributeString("Type", propDescriptor.PropertyType.FullName);
            bool flag = propDescriptor.Attributes.Matches(OptionsBase.s_encrypedAttribute);
            object value = propDescriptor.GetValue(options);
            if (typeof(IXMLAbleList).IsAssignableFrom(propDescriptor.PropertyType))
            {
                if (value != null)
                {
                    ((IXMLAbleList)value).ToXML(xmlWriter);
                }
            }
            else if (typeof(IXmlable).IsAssignableFrom(propDescriptor.PropertyType))
            {
                if (value != null)
                {
                    ((IXmlable)value).ToXML(xmlWriter);
                }
            }
            else if (typeof(XmlNode).IsAssignableFrom(propDescriptor.PropertyType))
            {
                if (value != null)
                {
                    xmlWriter.WriteRaw(((XmlNode)value).OuterXml);
                }
            }
            else if (typeof(IConvertible).IsAssignableFrom(propDescriptor.PropertyType) ||
                     propDescriptor.PropertyType.IsGenericType &&
                     typeof(Nullable<>).IsAssignableFrom(propDescriptor.PropertyType.GetGenericTypeDefinition()))
            {
                string empty = string.Empty;
                if (value != null)
                {
                    string str = Convert.ChangeType(value, typeof(string)) as string;
                    empty = str ?? empty;
                }

                if (flag)
                {
                    empty = Cryptography.EncryptText(empty.ToSecureString(), Cryptography.ProtectionScope.CurrentUser,
                        null);
                }

                xmlWriter.WriteValue(empty);
            }

            xmlWriter.WriteEndElement();
        }

        public virtual void SetFromOptions(OptionsBase options)
        {
            PropertyDescriptorCollection properties = options.GetProperties();
            foreach (PropertyDescriptor property in this.GetProperties())
            {
                if (property.IsReadOnly)
                {
                    continue;
                }

                PropertyDescriptor propertyDescriptor = properties.Find(property.Name, true);
                if (propertyDescriptor == null ||
                    !property.PropertyType.IsAssignableFrom(propertyDescriptor.PropertyType))
                {
                    continue;
                }

                property.SetValue(this, propertyDescriptor.GetValue(options));
            }
        }

        public DynamicOptions Subtract(OptionsBase B)
        {
            bool flag;
            OptionsBase optionsBase = this;
            DynamicOptions dynamicOption = new DynamicOptions();
            PropertyDescriptorCollection properties = B.GetProperties();
            foreach (PropertyDescriptor property in optionsBase.GetProperties())
            {
                PropertyDescriptor propertyDescriptor = properties.Find(property.Name, true);
                object value = property.GetValue(optionsBase);
                bool flag1 = true;
                if (propertyDescriptor != null && property.PropertyType == propertyDescriptor.PropertyType)
                {
                    object obj = propertyDescriptor.GetValue(B);
                    if (value != null)
                    {
                        if (!value.Equals(obj))
                        {
                            string str = this.SerializeOption(property, optionsBase);
                            string str1 = this.SerializeOption(propertyDescriptor, B);
                            if (str != null)
                            {
                                if (str != str1)
                                {
                                    value = this.DeserializeOption(XmlUtility.StringToXmlNode(str),
                                        property.PropertyType, false, out flag);
                                    if (!flag)
                                    {
                                        flag1 = false;
                                    }
                                }
                                else
                                {
                                    flag1 = false;
                                }
                            }
                        }
                        else
                        {
                            flag1 = false;
                        }
                    }
                    else if (obj == null)
                    {
                        flag1 = false;
                    }
                }

                if (!flag1)
                {
                    continue;
                }

                dynamicOption.SetOptionValue<object>(property.Name, value);
            }

            return dynamicOption;
        }

        public string ToXML()
        {
            StringBuilder stringBuilder = new StringBuilder();
            XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
            xmlTextWriter.WriteStartElement("ActionOptions");
            xmlTextWriter.WriteAttributeString("Type", this.GetType().AssemblyQualifiedName);
            this.ToXML(xmlTextWriter);
            xmlTextWriter.WriteEndElement();
            return stringBuilder.ToString();
        }

        public virtual void ToXML(XmlWriter xmlWriter)
        {
            foreach (PropertyDescriptor property in this.GetProperties())
            {
                if (property.IsReadOnly)
                {
                    continue;
                }

                this.SerializeOption(property, this, xmlWriter);
            }
        }
    }
}