using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;

namespace Metalogix
{
    public class DynamicOptions : OptionsBase, ICustomTypeDescriptor
    {
        private readonly Dictionary<string, object> _properties = new Dictionary<string, object>();

        public DynamicOptions()
        {
        }

        public override void FromXML(XmlNode xmlNode)
        {
            bool flag;
            XmlNode xmlNodes = xmlNode.SelectSingleNode("//ActionOptions");
            if (xmlNodes != null)
            {
                foreach (XmlNode childNode in xmlNodes.ChildNodes)
                {
                    XmlAttribute itemOf = childNode.Attributes["Type"];
                    Type type = Type.GetType(TypeUtils.UpdateType(itemOf.Value));
                    if (type == null)
                    {
                        continue;
                    }

                    object obj = this.DeserializeOption(childNode, type, false, out flag);
                    if (!flag)
                    {
                        continue;
                    }

                    this.SetOptionValue<object>(childNode.Name, obj);
                }
            }
        }

        private object GetOptionValue(string optionName)
        {
            if (!this._properties.ContainsKey(optionName))
            {
                return null;
            }

            return this._properties[optionName];
        }

        public T GetOptionValue<T>(string optionName)
        {
            if (this._properties.ContainsKey(optionName))
            {
                object item = this._properties[optionName];
                if (item is T)
                {
                    return (T)item;
                }
            }

            return default(T);
        }

        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            Type type;
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(base.GetType(), attributes);
            PropertyDescriptor[] dynamicOptionProperty =
                new PropertyDescriptor[this._properties.Count + properties.Count];
            int num = 0;
            foreach (KeyValuePair<string, object> _property in this._properties)
            {
                type = (_property.Value != null ? _property.Value.GetType() : typeof(object));
                dynamicOptionProperty[num] = new DynamicOptions.DynamicOptionProperty(_property.Key, type);
                num++;
            }

            foreach (PropertyDescriptor property in properties)
            {
                dynamicOptionProperty[num] = property;
                num++;
            }

            return new PropertyDescriptorCollection(dynamicOptionProperty);
        }

        public void SetOptionValue<T>(string optionName, T value)
        {
            this.SetValue(optionName, value);
        }

        private void SetValue(string name, object value)
        {
            if (this._properties.ContainsKey(name))
            {
                this._properties[name] = value;
                return;
            }

            this._properties.Add(name, value);
        }

        public class DynamicOptionProperty : PropertyDescriptor
        {
            private Type _type;

            public override Type ComponentType
            {
                get { return typeof(DynamicOptions); }
            }

            public override bool IsReadOnly
            {
                get { return false; }
            }

            public override Type PropertyType
            {
                get { return this._type; }
            }

            public DynamicOptionProperty(string name, Type type) : this(name, type, null)
            {
            }

            public DynamicOptionProperty(string name, Type type, Attribute[] attributes) : base(name, attributes)
            {
                this._type = type;
            }

            public override bool CanResetValue(object component)
            {
                return false;
            }

            public override object GetValue(object component)
            {
                DynamicOptions dynamicOption = component as DynamicOptions;
                if (dynamicOption == null)
                {
                    return null;
                }

                object optionValue = dynamicOption.GetOptionValue(this.Name);
                if (optionValue != null && !this._type.IsInstanceOfType(optionValue))
                {
                    throw new InvalidCastException();
                }

                return optionValue;
            }

            public override void ResetValue(object component)
            {
                throw new NotImplementedException();
            }

            public override void SetValue(object component, object value)
            {
                DynamicOptions dynamicOption = component as DynamicOptions;
                if (dynamicOption != null && this._type.IsInstanceOfType(value))
                {
                    dynamicOption.SetValue(this.Name, value);
                }
            }

            public override bool ShouldSerializeValue(object component)
            {
                throw new NotImplementedException();
            }
        }
    }
}