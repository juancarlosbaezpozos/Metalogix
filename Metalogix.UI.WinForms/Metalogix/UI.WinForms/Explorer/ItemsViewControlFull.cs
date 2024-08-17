using Metalogix.DataStructures;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.UI.WinForms.Actions;
using Metalogix.UI.WinForms.Components;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Resources;
using System.Threading;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Explorer
{
    [DesignTimeVisible(false)]
    public partial class ItemsViewControlFull : HasSelectableObjects
    {
        protected bool m_bShowingPropertyGrids;

        protected bool m_bShowingVersionHistory;

        protected bool m_bItemsViewLastFocused = true;

        protected FieldCollection m_ViewFields;

        private ItemCollectionView m_listItemControl;

        private ItemCollectionView m_listItemVersionControl;

        private IContainer components;

        protected SplitContainer w_splitContainer;

        protected MarqueeBar w_marqueeBar;

        protected ImageList imageListQuickSearch;

        [Browsable(false)]
        public IDataConverter<object, string> DataConverter
        {
            get
            {
                if (this.ListItemControl == null)
                {
                    return null;
                }
                return this.ListItemControl.DataConverter;
            }
            set
            {
                if (this.ListItemControl != null)
                {
                    this.ListItemControl.DataConverter = value;
                }
                if (this.ListItemVersionControl != null)
                {
                    this.ListItemVersionControl.DataConverter = value;
                }
            }
        }

        public virtual ListItemCollection DataSource
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        public ContextMenuStrip ItemsViewContextMenu
        {
            get
            {
                return this.ContextMenuStrip;
            }
            set
            {
                this.ContextMenuStrip = value;
            }
        }

        public ItemCollectionView ListItemControl
        {
            get
            {
                return this.m_listItemControl;
            }
            set
            {
                this.m_listItemControl = value;
                if (this.m_listItemControl != null)
                {
                    this.SetupListItemControl();
                }
            }
        }

        public ItemCollectionView ListItemVersionControl
        {
            get
            {
                return this.m_listItemVersionControl;
            }
            set
            {
                this.m_listItemVersionControl = value;
                if (this.m_listItemVersionControl != null)
                {
                    this.SetupListItemVersionControl();
                }
            }
        }

        public virtual FieldCollection ViewFields
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        public ItemsViewControlFull()
        {
            this.InitializeComponent();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        protected void FireSelectedListItemsChanged(ListItemCollection selectedItems)
        {
            if (this.SelectedListItemsChanged != null)
            {
                this.SelectedListItemsChanged(selectedItems);
            }
        }

        private void InitializeComponent()
        {
            this.components = new Container();
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ItemsViewControlFull));
            this.w_splitContainer = new SplitContainer();
            this.w_marqueeBar = new MarqueeBar();
            this.imageListQuickSearch = new ImageList(this.components);
            base.SuspendLayout();
            this.w_splitContainer.BackColor = SystemColors.Control;
            this.w_splitContainer.BorderStyle = BorderStyle.FixedSingle;
            componentResourceManager.ApplyResources(this.w_splitContainer, "w_splitContainer");
            this.w_splitContainer.Name = "w_splitContainer";
            componentResourceManager.ApplyResources(this.w_marqueeBar, "w_marqueeBar");
            this.w_marqueeBar.ForeColor = SystemColors.ControlLight;
            this.w_marqueeBar.Name = "w_marqueeBar";
            this.w_marqueeBar.ShapeSpacing = 8;
            this.imageListQuickSearch.ImageStream = (ImageListStreamer)componentResourceManager.GetObject("imageListQuickSearch.ImageStream");
            this.imageListQuickSearch.TransparentColor = Color.Transparent;
            this.imageListQuickSearch.Images.SetKeyName(0, "QuickSearchCancel.ico");
            this.imageListQuickSearch.Images.SetKeyName(1, "QuickSearch.ico");
            componentResourceManager.ApplyResources(this, "$this");
            base.AutoScaleMode = AutoScaleMode.Font;
            base.Controls.Add(this.w_splitContainer);
            base.Name = "ItemsViewControlFull";
            base.ResumeLayout(false);
        }

        protected virtual void On_ItemViewEntered(object obj, EventArgs e)
        {
            this.m_bItemsViewLastFocused = obj == this.ListItemControl;
        }

        private void SetupListItemControl()
        {
        }

        private void SetupListItemVersionControl()
        {
        }

        protected virtual void UpdateVersioning(object itemsObj)
        {
            try
            {
                ListItemCollection listItemCollection = itemsObj as ListItemCollection;
                if (listItemCollection == null || listItemCollection.Count != 1)
                {
                    this.ListItemVersionControl.DataSource = null;
                }
                else
                {
                    this.ListItemVersionControl.DataSource = ((ListItem)listItemCollection[0]).VersionHistory;
                }
            }
            catch (Exception exception)
            {
            }
        }

        protected virtual void UpdateVersioningAsync(ListItemCollection itemsSelected)
        {
        }

        public event ItemsViewControlFull.SelectedListItemChangedHandler SelectedListItemsChanged;

        public delegate void SelectedListItemChangedHandler(ListItemCollection items);
    }
}