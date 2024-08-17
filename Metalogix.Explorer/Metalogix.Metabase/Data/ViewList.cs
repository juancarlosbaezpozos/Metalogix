using Metalogix.Metabase;
using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Metalogix.Metabase.Data
{
    public class ViewList : IEnumerable
    {
        private Workspace m_parentWorkspace;

        private PropertyDescriptorCollection m_baseProperties;

        private ArrayList m_viewList = new ArrayList();

        public PropertyDescriptorCollection BaseProperties
        {
            get { return this.m_baseProperties; }
            set
            {
                this.m_baseProperties = value;
                foreach (View view in this)
                {
                    view.ViewProperties.SyncBaseProperties(this.m_baseProperties);
                }
            }
        }

        public int Count
        {
            get { return this.m_viewList.Count; }
        }

        public View this[int index]
        {
            get { return (View)this.m_viewList[index]; }
            set { this.m_viewList[index] = value; }
        }

        public Workspace ParentWorkspace
        {
            get { return this.m_parentWorkspace; }
        }

        public View SelectedView
        {
            get
            {
                View view;
                IEnumerator enumerator = this.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        View current = (View)enumerator.Current;
                        if (!current.IsSelected)
                        {
                            continue;
                        }

                        view = current;
                        return view;
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

                return view;
            }
        }

        public ViewList(Workspace parentWorkspace, PropertyDescriptorCollection baseProperties, string strViewListXML)
        {
            this.m_parentWorkspace = parentWorkspace;
            this.m_baseProperties = baseProperties;
            try
            {
                if (!string.IsNullOrEmpty(strViewListXML))
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(strViewListXML);
                    foreach (XmlNode xmlNodes in xmlDocument.SelectNodes("//View"))
                    {
                        this.Add(new View(this, baseProperties, xmlNodes));
                    }
                }
            }
            catch (Exception exception)
            {
            }

            if (this.Count == 0)
            {
                View view = new View(this, baseProperties, null)
                {
                    Name = "Default View"
                };
                this.Add(view);
            }

            if (this.SelectedView == null)
            {
                this[0].IsSelected = true;
            }
        }

        public int Add(View value)
        {
            return this.m_viewList.Add(value);
        }

        public void Clear()
        {
            this.m_viewList.Clear();
        }

        public bool Contains(View value)
        {
            return this.m_viewList.Contains(value);
        }

        public View Find(string strViewName)
        {
            View view;
            IEnumerator enumerator = this.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    View current = (View)enumerator.Current;
                    if (current.Name != strViewName)
                    {
                        continue;
                    }

                    view = current;
                    return view;
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

            return view;
        }

        public IEnumerator GetEnumerator()
        {
            return this.m_viewList.GetEnumerator();
        }

        public int IndexOf(View value)
        {
            return this.m_viewList.IndexOf(value);
        }

        public void Insert(int index, View value)
        {
            this.m_viewList.Insert(index, value);
        }

        public void Remove(View value)
        {
            this.m_viewList.Remove(value);
        }

        public void RemoveAt(int index)
        {
            this.m_viewList.RemoveAt(index);
        }

        public string ToXml()
        {
            StringBuilder stringBuilder = new StringBuilder(100);
            XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder))
            {
                Formatting = Formatting.Indented
            };
            this.ToXml(xmlTextWriter);
            return stringBuilder.ToString();
        }

        public void ToXml(XmlTextWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("ViewList");
            foreach (View view in this)
            {
                view.ToXml(xmlWriter);
            }

            xmlWriter.WriteEndElement();
        }
    }
}