using Metalogix.Actions;
using Metalogix.DataStructures;
using Metalogix.Explorer;
using Metalogix.UI.WinForms.Actions;
using Metalogix.UI.WinForms.Properties;
using Metalogix.UI.WinForms.Widgets;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Explorer
{
    [DesignTimeVisible(false)]
    public partial class ItemCollectionView : HasSelectableObjects
    {
        protected NodeCollectionChangedHandler m_handlerCollectionChanged;

        protected ListItemCollection m_dataSource;

        protected FieldCollection m_viewFields;

        private bool m_bPropertyGridVisible;

        protected bool m_bDataSourceIsVersionCollection;

        private IContainer components;

        protected SplitContainer w_splitContainer;

        protected ImageList w_imageFileTypes;

        protected NodePropertyGrid w_propertyGrid;

        [Browsable(false)]
        public virtual IDataConverter<object, string> DataConverter
        {
            get;
            set;
        }

        public virtual ListItemCollection DataSource
        {
            get
            {
                return this.m_dataSource;
            }
            set
            {
            }
        }

        protected bool DataSourceIsVersionCollection
        {
            get
            {
                return this.m_bDataSourceIsVersionCollection;
            }
            set
            {
                this.m_bDataSourceIsVersionCollection = value;
            }
        }

        public virtual bool ShowPropertyGrid
        {
            get
            {
                return this.m_bPropertyGridVisible;
            }
            set
            {
                this.m_bPropertyGridVisible = value;
                if (this.m_bPropertyGridVisible)
                {
                    this.w_splitContainer.Panel1Collapsed = false;
                    return;
                }
                this.w_splitContainer.Panel1Collapsed = true;
            }
        }

        public virtual FieldCollection ViewFields
        {
            get
            {
                if (this.m_viewFields != null)
                {
                    return this.m_viewFields;
                }
                if (this.DataSource == null || this.DataSource.ParentList == null)
                {
                    return null;
                }
                return this.DataSource.ParentList.Fields;
            }
            set
            {
                this.m_viewFields = value;
            }
        }

        public ItemCollectionView()
        {
            this.InitializeComponent();
            this.w_imageFileTypes.Images.Add("Desc.ico", Resources.Desc);
            this.w_imageFileTypes.Images.Add("Asc.ico", Resources.Asc);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        protected virtual void FireSelectedItemChangedEvent(ListItemCollection items)
        {
            if (this.SelectedItemsChanged != null)
            {
                this.SelectedItemsChanged(items);
            }
        }

        protected virtual ListItemCollection GetDataSourceTypedItemCollection(ListItemCollection items)
        {
            if (this.DataSource == null)
            {
                return null;
            }
            Type type = this.DataSource.GetType();
            Type[] typeArray = new Type[] { items.GetType() };
            if (type.GetConstructor(typeArray) == null)
            {
                return items;
            }
            object[] objArray = new object[] { items };
            return (ListItemCollection)Activator.CreateInstance(type, objArray);
        }

        private void InitializeComponent()
        {
            this.components = new Container();
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ItemCollectionView));
            this.w_splitContainer = new SplitContainer();
            this.w_propertyGrid = new NodePropertyGrid();
            this.w_imageFileTypes = new ImageList(this.components);
            base.SuspendLayout();
            this.w_splitContainer.BackColor = SystemColors.Control;
            componentResourceManager.ApplyResources(this.w_splitContainer, "w_splitContainer");
            this.w_splitContainer.Name = "w_splitContainer";
            this.w_splitContainer.Panel1.Controls.Add(this.w_propertyGrid);
            componentResourceManager.ApplyResources(this.w_propertyGrid, "w_propertyGrid");
            this.w_propertyGrid.DataSource = null;
            this.w_propertyGrid.Name = "w_propertyGrid";
            this.w_imageFileTypes.ColorDepth = ColorDepth.Depth8Bit;
            componentResourceManager.ApplyResources(this.w_imageFileTypes, "w_imageFileTypes");
            this.w_imageFileTypes.TransparentColor = Color.Transparent;
            componentResourceManager.ApplyResources(this, "$this");
            base.AutoScaleMode = AutoScaleMode.Font;
            base.Controls.Add(this.w_splitContainer);
            base.Name = "AbstractItemCollectionView";
            base.ResumeLayout(false);
        }

        protected virtual void On_dataSource_CollectionChanged(NodeCollectionChangeType changeType, Node changedNode)
        {
        }

        public event ItemCollectionView.SelectedItemsChangedHandler SelectedItemsChanged;

        protected delegate IXMLAbleList GetSelectedObjectsDelegate();

        public delegate void SelectedItemsChangedHandler(ListItemCollection item);
    }
}