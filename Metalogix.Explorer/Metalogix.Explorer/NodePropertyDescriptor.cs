using Metalogix.Metabase;
using System;
using System.ComponentModel;
using System.Reflection;

namespace Metalogix.Explorer
{
    public class NodePropertyDescriptor : PropertyDescriptor
    {
        private PropertyDescriptor m_pdBase;

        private MetabasePDTarget m_appliesTo;

        public MetabasePDTarget AppliesTo
        {
            get { return this.m_appliesTo; }
        }

        public override AttributeCollection Attributes
        {
            get { return this.m_pdBase.Attributes; }
        }

        public Type BaseComponentType
        {
            get { return this.m_pdBase.ComponentType; }
        }

        public override string Category
        {
            get
            {
                string category;
                try
                {
                    string str = base.Category;
                    if (!string.Equals("Misc", str, StringComparison.OrdinalIgnoreCase))
                    {
                        category = str;
                    }
                    else
                    {
                        category = (this.m_appliesTo != MetabasePDTarget.Base ? str : this.m_pdBase.ComponentType.Name);
                    }
                }
                catch
                {
                    category = base.Category;
                }

                return category;
            }
        }

        public override Type ComponentType
        {
            get { return typeof(Node); }
        }

        public object DefaultValue
        {
            get
            {
                if (!(this.m_pdBase is RecordPropertyDescriptor))
                {
                    return null;
                }

                return ((RecordPropertyDescriptor)this.m_pdBase).DefaultValue;
            }
        }

        public override string DisplayName
        {
            get
            {
                if (this.m_pdBase == null)
                {
                    return this.Name;
                }

                return this.m_pdBase.DisplayName;
            }
        }

        public override bool IsReadOnly
        {
            get { return this.m_pdBase.IsReadOnly; }
        }

        public override Type PropertyType
        {
            get { return this.m_pdBase.PropertyType; }
        }

        public override bool SupportsChangeEvents
        {
            get { return this.m_pdBase.SupportsChangeEvents; }
        }

        public NodePropertyDescriptor(PropertyDescriptor pdBase, MetabasePDTarget target) : base(pdBase)
        {
            this.m_pdBase = pdBase;
            this.m_appliesTo = target;
        }

        public override bool CanResetValue(object component)
        {
            Node node = component as Node;
            if (node == null)
            {
                return false;
            }

            if (this.m_appliesTo != MetabasePDTarget.Record)
            {
                return this.m_pdBase.CanResetValue(component);
            }

            return this.m_pdBase.CanResetValue(node.Record);
        }

        public string GetConfigurationValue(string sKey)
        {
            if (!(this.m_pdBase is RecordPropertyDescriptor))
            {
                return null;
            }

            return ((RecordPropertyDescriptor)this.m_pdBase).Configuration[sKey];
        }

        public override object GetValue(object component)
        {
            object value;
            Node node = component as Node;
            if (node == null)
            {
                return null;
            }

            switch (this.m_appliesTo)
            {
                case MetabasePDTarget.Record:
                {
                    return this.m_pdBase.GetValue(node.Record);
                }
                case MetabasePDTarget.Base:
                {
                    try
                    {
                        PropertyInfo property = node.GetType().GetProperty(this.m_pdBase.Name);
                        value = property.GetValue(node, null);
                    }
                    catch
                    {
                        value = node[this.m_pdBase.Name];
                    }

                    return value;
                }
                default:
                {
                    return this.m_pdBase.GetValue(node.Record);
                }
            }
        }

        public override void ResetValue(object component)
        {
            Node node = component as Node;
            if (node == null)
            {
                return;
            }

            switch (this.m_appliesTo)
            {
                case MetabasePDTarget.Record:
                {
                    this.m_pdBase.ResetValue(node.Record);
                    return;
                }
                case MetabasePDTarget.Base:
                {
                    this.m_pdBase.ResetValue(component);
                    return;
                }
                default:
                {
                    return;
                }
            }
        }

        public override void SetValue(object component, object value)
        {
            if (component == null)
            {
                return;
            }

            if (component is Node)
            {
                Node node = component as Node;
                switch (this.m_appliesTo)
                {
                    case MetabasePDTarget.Record:
                    {
                        node.Record.SetPropertyValue(this, value);
                        break;
                    }
                    case MetabasePDTarget.Base:
                    {
                        PropertyInfo property = node.GetType().GetProperty(this.m_pdBase.Name);
                        if (property == null)
                        {
                            return;
                        }

                        property.SetValue(component, value, null);
                        break;
                    }
                    default:
                    {
                        goto case MetabasePDTarget.Record;
                    }
                }
            }

            if (component is Record)
            {
                Record record = component as Record;
                if (this.m_appliesTo == MetabasePDTarget.Record)
                {
                    this.m_pdBase.SetValue(record, value);
                }
            }
        }

        public override bool ShouldSerializeValue(object component)
        {
            Node node = component as Node;
            if (node == null)
            {
                return false;
            }

            if (this.m_appliesTo != MetabasePDTarget.Record)
            {
                return this.m_pdBase.ShouldSerializeValue(component);
            }

            return this.m_pdBase.ShouldSerializeValue(node.Record);
        }

        public override string ToString()
        {
            return this.DisplayName;
        }
    }
}