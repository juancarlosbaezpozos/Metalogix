using Metalogix.Explorer;
using Metalogix.Metabase.Data;
using System;
using System.Collections;
using System.ComponentModel;

namespace Metalogix.Metabase
{
    public class WorkspaceFieldCollection : FieldCollection
    {
        private readonly Workspace m_workspace;

        public int Count
        {
            get
            {
                if (this.m_workspace == null)
                {
                    return 0;
                }

                return this.m_workspace.GetProperties().Count;
            }
        }

        public string XML
        {
            get { throw new NotImplementedException(); }
        }

        internal WorkspaceFieldCollection(Workspace workspace)
        {
            this.m_workspace = workspace;
        }

        public IEnumerator GetEnumerator()
        {
            return new WorkspaceFieldCollection.WorkspaceFieldCollectionEnumerator(this.m_workspace);
        }

        private class WorkspaceFieldCollectionEnumerator : IEnumerator
        {
            private PropertyDescriptorCollection m_properties;

            private ViewList m_views;

            private int m_iIndex;

            public object Current
            {
                get
                {
                    if (this.m_iIndex >= this.m_properties.Count)
                    {
                        return null;
                    }

                    PropertyDescriptor item = this.m_properties[this.m_iIndex];
                    bool isDisplayed = this.m_views.SelectedView.ViewProperties.Find(item.Name).IsDisplayed;
                    return new WorkspaceField(item.Name, item.DisplayName, isDisplayed);
                }
            }

            internal WorkspaceFieldCollectionEnumerator(Workspace workspace)
            {
                if (workspace == null)
                {
                    return;
                }

                this.m_views = workspace.GetViews();
                this.m_properties = workspace.GetProperties();
            }

            public bool MoveNext()
            {
                this.m_iIndex++;
                return this.m_iIndex < this.m_properties.Count;
            }

            public void Reset()
            {
            }
        }
    }
}