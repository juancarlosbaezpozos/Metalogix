using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using DevExpress.XtraVerticalGrid;

namespace Metalogix.Xml
{
    public class XMLPropertyGrid : UserControl
    {
        private class XMLNodeWrapper : ICustomTypeDescriptor
        {
            private class XMLAttributeProperty : PropertyDescriptor
            {
                private XmlAttribute m_attr;

                public override string Category => m_attr.OwnerElement.Name;

                public override Type ComponentType => typeof(XmlNode);

                public override bool IsReadOnly => true;

                public override Type PropertyType => typeof(string);

                public XMLAttributeProperty(XmlAttribute attr)
                    : base(attr.Name, null)
                {
                    m_attr = attr;
                }

                public override bool CanResetValue(object component)
                {
                    return false;
                }

                public override object GetValue(object component)
                {
                    if (component == null)
                    {
                        return null;
                    }
                    if (!(component is XmlNode))
                    {
                        throw new Exception("Invalid type supplied to GetValue");
                    }
                    XmlNode xmlNodes = (XmlNode)component;
                    if (xmlNodes.Attributes[Name] == null)
                    {
                        return null;
                    }
                    return xmlNodes.Attributes[Name].Value;
                }

                public override void ResetValue(object component)
                {
                }

                public override void SetValue(object component, object value)
                {
                }

                public override bool ShouldSerializeValue(object component)
                {
                    return false;
                }
            }

            private XmlNode m_node;

            public XmlNode XMLNode => m_node;

            public XMLNodeWrapper(XmlNode node)
            {
                m_node = node;
            }

            public AttributeCollection GetAttributes()
            {
                return TypeDescriptor.GetAttributes(this, noCustomTypeDesc: true);
            }

            public string GetClassName()
            {
                return TypeDescriptor.GetClassName(this, noCustomTypeDesc: true);
            }

            public string GetComponentName()
            {
                return TypeDescriptor.GetComponentName(this, noCustomTypeDesc: true);
            }

            public TypeConverter GetConverter()
            {
                return TypeDescriptor.GetConverter(this, noCustomTypeDesc: true);
            }

            public EventDescriptor GetDefaultEvent()
            {
                return TypeDescriptor.GetDefaultEvent(this, noCustomTypeDesc: true);
            }

            public PropertyDescriptor GetDefaultProperty()
            {
                return TypeDescriptor.GetDefaultProperty(this, noCustomTypeDesc: true);
            }

            public object GetEditor(Type editorBaseType)
            {
                return TypeDescriptor.GetEditor(this, editorBaseType, noCustomTypeDesc: true);
            }

            public EventDescriptorCollection GetEvents(Attribute[] attributes)
            {
                return TypeDescriptor.GetEvents(this, attributes, noCustomTypeDesc: true);
            }

            public EventDescriptorCollection GetEvents()
            {
                return TypeDescriptor.GetEvents(this, noCustomTypeDesc: true);
            }

            public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
            {
                return GetProperties();
            }

            public PropertyDescriptorCollection GetProperties()
            {
                List<PropertyDescriptor> propertyDescriptors = new List<PropertyDescriptor>();
                foreach (XmlAttribute attribute in m_node.Attributes)
                {
                    propertyDescriptors.Add(new XMLAttributeProperty(attribute));
                }
                return new PropertyDescriptorCollection(propertyDescriptors.ToArray());
            }

            public object GetPropertyOwner(PropertyDescriptor pd)
            {
                return m_node;
            }
        }

        private IContainer components;

        private PropertyGridControl w_propertyGrid;

        public XmlNode SelectedNode
        {
            get
            {
                if (w_propertyGrid.SelectedObject == null)
                {
                    return null;
                }
                return ((XMLNodeWrapper)w_propertyGrid.SelectedObject).XMLNode;
            }
            set
            {
                if (value != null)
                {
                    w_propertyGrid.SelectedObject = new XMLNodeWrapper(value);
                    return;
                }
                try
                {
                    w_propertyGrid.SelectedObject = null;
                }
                catch (Exception)
                {
                }
            }
        }

        public XMLPropertyGrid()
        {
            InitializeComponent();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.Xml.XMLPropertyGrid));
            this.w_propertyGrid = new DevExpress.XtraVerticalGrid.PropertyGridControl();
            ((System.ComponentModel.ISupportInitialize)this.w_propertyGrid).BeginInit();
            base.SuspendLayout();
            componentResourceManager.ApplyResources(this.w_propertyGrid, "w_propertyGrid");
            this.w_propertyGrid.Name = "w_propertyGrid";
            componentResourceManager.ApplyResources(this, "$this");
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.Controls.Add(this.w_propertyGrid);
            base.Name = "XMLPropertyGrid";
            ((System.ComponentModel.ISupportInitialize)this.w_propertyGrid).EndInit();
            base.ResumeLayout(false);
        }
    }
}
