using Metalogix.Metabase.Interfaces;
using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Metalogix.Metabase.Data
{
    public class ViewPropertyList : IEnumerable, IStringHash
    {
        private ArrayList m_viewPropertyList = new ArrayList();

        public int Count
        {
            get { return this.m_viewPropertyList.Count; }
        }

        public int DisplayCount
        {
            get
            {
                int num = 0;
                foreach (ViewProperty viewProperty in this)
                {
                    if (!viewProperty.IsDisplayed)
                    {
                        continue;
                    }

                    num++;
                }

                return num;
            }
        }

        public ViewProperty this[int index]
        {
            get { return (ViewProperty)this.m_viewPropertyList[index]; }
            set { this.m_viewPropertyList[index] = value; }
        }

        public ViewPropertyList(PropertyDescriptorCollection baseProperties, XmlNode viewProperyListNode)
        {
            if (baseProperties == null)
            {
                throw new ArgumentNullException("baseProperties");
            }

            if (viewProperyListNode != null)
            {
                foreach (XmlNode xmlNodes in viewProperyListNode.SelectNodes("./ViewProperty"))
                {
                    try
                    {
                        XmlNode xmlNodes1 = xmlNodes.SelectSingleNode("./PropertyName");
                        string str = (xmlNodes1 != null ? xmlNodes1.InnerText : "");
                        XmlNode xmlNodes2 = xmlNodes.SelectSingleNode("./ColumnWidth");
                        int num = (xmlNodes2 != null
                            ? Convert.ToInt32(xmlNodes2.InnerText)
                            : ViewProperty.DEFAULT_COLUMN_WIDTH);
                        XmlNode xmlNodes3 = xmlNodes.SelectSingleNode("./IsDisplayed");
                        bool flag = (xmlNodes3 != null
                            ? xmlNodes3.InnerText == "True"
                            : ViewProperty.DEFAULT_COLUMN_DISPLAY);
                        PropertyDescriptor propertyDescriptor = ViewPropertyList.FindProperty(baseProperties, str);
                        if (propertyDescriptor != null && this.Find(str) == null)
                        {
                            this.Add(new ViewProperty(propertyDescriptor, flag, num));
                        }
                    }
                    catch (Exception exception)
                    {
                    }
                }
            }

            this.SyncBaseProperties(baseProperties);
        }

        public int Add(ViewProperty value)
        {
            return this.m_viewPropertyList.Add(value);
        }

        public void Clear()
        {
            this.m_viewPropertyList.Clear();
        }

        public bool Contains(ViewProperty value)
        {
            return this.m_viewPropertyList.Contains(value);
        }

        public bool ContainsKey(string strKey)
        {
            ViewProperty viewProperty = this.Find(strKey);
            if (viewProperty == null)
            {
                return false;
            }

            return viewProperty.IsDisplayed;
        }

        public ViewProperty Find(string strPropertyName)
        {
            ViewProperty viewProperty;
            IEnumerator enumerator = this.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    ViewProperty current = (ViewProperty)enumerator.Current;
                    if (current.PropertyDescriptor == null || current.PropertyDescriptor.Name == null ||
                        !(current.PropertyDescriptor.Name.ToLower() == strPropertyName.ToLower()))
                    {
                        continue;
                    }

                    viewProperty = current;
                    return viewProperty;
                }

                return null;
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }

            return viewProperty;
        }

        private static PropertyDescriptor FindProperty(PropertyDescriptorCollection propertyList,
            string strPropertyName)
        {
            PropertyDescriptor propertyDescriptor;
            IEnumerator enumerator = propertyList.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    PropertyDescriptor current = (PropertyDescriptor)enumerator.Current;
                    if (current.Name != strPropertyName)
                    {
                        continue;
                    }

                    propertyDescriptor = current;
                    return propertyDescriptor;
                }

                return null;
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }

            return propertyDescriptor;
        }

        public IEnumerator GetEnumerator()
        {
            return this.m_viewPropertyList.GetEnumerator();
        }

        public int IndexOf(ViewProperty value)
        {
            return this.m_viewPropertyList.IndexOf(value);
        }

        public void Insert(int index, ViewProperty value)
        {
            this.m_viewPropertyList.Insert(index, value);
        }

        public void Remove(ViewProperty value)
        {
            this.m_viewPropertyList.Remove(value);
        }

        public void RemoveAt(int index)
        {
            this.m_viewPropertyList.RemoveAt(index);
        }

        public void SyncBaseProperties(PropertyDescriptorCollection baseProperties)
        {
            foreach (PropertyDescriptor baseProperty in baseProperties)
            {
                if (this.Find(baseProperty.Name) != null)
                {
                    continue;
                }

                DefaultColumnWidthAttribute item =
                    (DefaultColumnWidthAttribute)baseProperty.Attributes[typeof(DefaultColumnWidthAttribute)];
                int num = (item != null ? item.ColumnWidth : ViewProperty.DEFAULT_COLUMN_WIDTH);
                DefaultDisplaySettingAttribute defaultDisplaySettingAttribute =
                    (DefaultDisplaySettingAttribute)baseProperty.Attributes[typeof(DefaultDisplaySettingAttribute)];
                this.Add(new ViewProperty(baseProperty,
                    (defaultDisplaySettingAttribute == null ? false : defaultDisplaySettingAttribute.ShowInGridView),
                    num));
            }

            for (int i = 0; i < this.Count; i++)
            {
                if (ViewPropertyList.FindProperty(baseProperties, this[i].PropertyDescriptor.Name) == null)
                {
                    this.RemoveAt(i);
                }
            }
        }

        public string ToXml()
        {
            StringBuilder stringBuilder = new StringBuilder(1024);
            XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder))
            {
                Formatting = Formatting.Indented
            };
            this.ToXml(xmlTextWriter);
            return stringBuilder.ToString();
        }

        public void ToXml(XmlTextWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("ViewPropertyList");
            foreach (ViewProperty viewProperty in this)
            {
                viewProperty.ToXml(xmlWriter);
            }

            xmlWriter.WriteEndElement();
        }
    }
}