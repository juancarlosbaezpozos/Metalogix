using DevExpress.XtraBars;
using DevExpress.XtraBars.Helpers.Docking;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using Metalogix.UI.WinForms;
using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

namespace Metalogix.Xml
{
    public partial class XMLComparer : UserControl
    {
        private bool m_bShowLabels = true;

        private bool m_bShowViewBar = true;

        private XMLComparer.DisplayModeType m_displayMode;

        private bool m_bUpdatingUI;

        private IContainer components;

        private SplitContainer w_splitContainer;

        private PanelControl w_panelTop1;

        private LabelControl w_labelSource;

        private MemoEdit w_textBox1;

        private XMLPropertyGrid w_xmlPropertyGrid1;

        private MemoEdit w_textBox2;

        private XMLPropertyGrid w_xmlPropertyGrid2;

        private PanelControl w_panelTop2;

        private LabelControl w_labelTarget;

        private BarManager _barManager;

        private Bar _viewBar;

        private BarSubItem barSubItem1;

        private BarDockControl barDockControlTop;

        private BarDockControl barDockControlBottom;

        private BarDockControl barDockControlLeft;

        private BarDockControl barDockControlRight;

        private BarCheckItem _barChkTextView;

        private BarCheckItem _barChkGridView;

        private TextEditContextMenu _textEditContextMenu;

        public XMLComparer.DisplayModeType DisplayMode
        {
            get
            {
                return this.m_displayMode;
            }
            set
            {
                this.m_displayMode = value;
                this.UpdateUI();
            }
        }

        public string LabelSource
        {
            get
            {
                return this.w_labelSource.Text;
            }
            set
            {
                this.w_labelSource.Text = value;
            }
        }

        public string LabelTarget
        {
            get
            {
                return this.w_labelTarget.Text;
            }
            set
            {
                this.w_labelTarget.Text = value;
            }
        }

        public XmlNode SelectedNode1
        {
            get
            {
                return this.w_xmlPropertyGrid1.SelectedNode;
            }
            set
            {
                this.w_xmlPropertyGrid1.SelectedNode = value;
            }
        }

        public XmlNode SelectedNode2
        {
            get
            {
                return this.w_xmlPropertyGrid2.SelectedNode;
            }
            set
            {
                string outerXml;
                this.w_xmlPropertyGrid2.SelectedNode = value;
                MemoEdit wTextBox2 = this.w_textBox2;
                if (value == null)
                {
                    outerXml = null;
                }
                else
                {
                    outerXml = value.OuterXml;
                }
                wTextBox2.Text = outerXml;
            }
        }

        public string SelectedText1
        {
            get
            {
                return this.w_textBox1.Text;
            }
            set
            {
                try
                {
                    this.w_textBox1.Text = value;
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(value);
                    this.w_xmlPropertyGrid1.SelectedNode = xmlDocument.FirstChild;
                }
                catch
                {
                    this.w_xmlPropertyGrid1.SelectedNode = null;
                }
            }
        }

        public string SelectedText2
        {
            get
            {
                return this.w_textBox2.Text;
            }
            set
            {
                try
                {
                    this.w_textBox2.Text = value;
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(value);
                    this.w_xmlPropertyGrid2.SelectedNode = xmlDocument.FirstChild;
                }
                catch
                {
                    this.w_xmlPropertyGrid2.SelectedNode = null;
                }
            }
        }

        public bool ShowLabels
        {
            get
            {
                return this.m_bShowLabels;
            }
            set
            {
                this.m_bShowLabels = value;
                this.UpdateUI();
            }
        }

        public bool ShowViewBar
        {
            get
            {
                return this.m_bShowViewBar;
            }
            set
            {
                this.m_bShowViewBar = value;
                this.UpdateUI();
            }
        }

        public XMLComparer()
        {
            this.InitializeComponent();
            this.UpdateUI();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new Container();
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(XMLComparer));
            this.w_splitContainer = new SplitContainer();
            this.w_textBox1 = new MemoEdit();
            this.w_xmlPropertyGrid1 = new XMLPropertyGrid();
            this.w_panelTop1 = new PanelControl();
            this.w_labelSource = new LabelControl();
            this.w_textBox2 = new MemoEdit();
            this.w_xmlPropertyGrid2 = new XMLPropertyGrid();
            this.w_panelTop2 = new PanelControl();
            this.w_labelTarget = new LabelControl();
            this._barManager = new BarManager(this.components);
            this._viewBar = new Bar();
            this.barSubItem1 = new BarSubItem();
            this._barChkTextView = new BarCheckItem();
            this._barChkGridView = new BarCheckItem();
            this.barDockControlTop = new BarDockControl();
            this.barDockControlBottom = new BarDockControl();
            this.barDockControlLeft = new BarDockControl();
            this.barDockControlRight = new BarDockControl();
            this._textEditContextMenu = new TextEditContextMenu();
            this.w_splitContainer.Panel1.SuspendLayout();
            this.w_splitContainer.Panel2.SuspendLayout();
            this.w_splitContainer.SuspendLayout();
            ((ISupportInitialize)this.w_textBox1.Properties).BeginInit();
            ((ISupportInitialize)this.w_panelTop1).BeginInit();
            this.w_panelTop1.SuspendLayout();
            ((ISupportInitialize)this.w_textBox2.Properties).BeginInit();
            ((ISupportInitialize)this.w_panelTop2).BeginInit();
            this.w_panelTop2.SuspendLayout();
            ((ISupportInitialize)this._barManager).BeginInit();
            base.SuspendLayout();
            componentResourceManager.ApplyResources(this.w_splitContainer, "w_splitContainer");
            this.w_splitContainer.Name = "w_splitContainer";
            this.w_splitContainer.Panel1.Controls.Add(this.w_textBox1);
            this.w_splitContainer.Panel1.Controls.Add(this.w_xmlPropertyGrid1);
            this.w_splitContainer.Panel1.Controls.Add(this.w_panelTop1);
            this.w_splitContainer.Panel2.Controls.Add(this.w_textBox2);
            this.w_splitContainer.Panel2.Controls.Add(this.w_xmlPropertyGrid2);
            this.w_splitContainer.Panel2.Controls.Add(this.w_panelTop2);
            componentResourceManager.ApplyResources(this.w_textBox1, "w_textBox1");
            this.w_textBox1.Name = "w_textBox1";
            this.w_textBox1.Properties.ContextMenuStrip = this._textEditContextMenu;
            this.w_textBox1.Properties.ReadOnly = true;
            componentResourceManager.ApplyResources(this.w_xmlPropertyGrid1, "w_xmlPropertyGrid1");
            this.w_xmlPropertyGrid1.Name = "w_xmlPropertyGrid1";
            this.w_xmlPropertyGrid1.SelectedNode = null;
            this.w_panelTop1.Controls.Add(this.w_labelSource);
            componentResourceManager.ApplyResources(this.w_panelTop1, "w_panelTop1");
            this.w_panelTop1.Name = "w_panelTop1";
            componentResourceManager.ApplyResources(this.w_labelSource, "w_labelSource");
            this.w_labelSource.Name = "w_labelSource";
            componentResourceManager.ApplyResources(this.w_textBox2, "w_textBox2");
            this.w_textBox2.Name = "w_textBox2";
            this.w_textBox2.Properties.ContextMenuStrip = this._textEditContextMenu;
            this.w_textBox2.Properties.ReadOnly = true;
            componentResourceManager.ApplyResources(this.w_xmlPropertyGrid2, "w_xmlPropertyGrid2");
            this.w_xmlPropertyGrid2.Name = "w_xmlPropertyGrid2";
            this.w_xmlPropertyGrid2.SelectedNode = null;
            this.w_panelTop2.Controls.Add(this.w_labelTarget);
            componentResourceManager.ApplyResources(this.w_panelTop2, "w_panelTop2");
            this.w_panelTop2.Name = "w_panelTop2";
            componentResourceManager.ApplyResources(this.w_labelTarget, "w_labelTarget");
            this.w_labelTarget.Name = "w_labelTarget";
            this._barManager.Bars.AddRange(new Bar[] { this._viewBar });
            this._barManager.DockControls.Add(this.barDockControlTop);
            this._barManager.DockControls.Add(this.barDockControlBottom);
            this._barManager.DockControls.Add(this.barDockControlLeft);
            this._barManager.DockControls.Add(this.barDockControlRight);
            this._barManager.Form = this;
            BarItems items = this._barManager.Items;
            BarItem[] barItemArray = new BarItem[] { this.barSubItem1, this._barChkTextView, this._barChkGridView };
            items.AddRange(barItemArray);
            this._barManager.MaxItemId = 6;
            this._viewBar.BarName = "Tools";
            this._viewBar.DockCol = 0;
            this._viewBar.DockRow = 0;
            this._viewBar.DockStyle = BarDockStyle.Top;
            LinksInfo linksPersistInfo = this._viewBar.LinksPersistInfo;
            LinkPersistInfo[] linkPersistInfo = new LinkPersistInfo[] { new LinkPersistInfo(this.barSubItem1) };
            linksPersistInfo.AddRange(linkPersistInfo);
            this._viewBar.OptionsBar.AllowQuickCustomization = false;
            this._viewBar.OptionsBar.DrawDragBorder = false;
            componentResourceManager.ApplyResources(this._viewBar, "_viewBar");
            componentResourceManager.ApplyResources(this.barSubItem1, "barSubItem1");
            this.barSubItem1.Id = 0;
            LinksInfo linksInfo = this.barSubItem1.LinksPersistInfo;
            LinkPersistInfo[] linkPersistInfoArray = new LinkPersistInfo[] { new LinkPersistInfo(this._barChkTextView), new LinkPersistInfo(this._barChkGridView) };
            linksInfo.AddRange(linkPersistInfoArray);
            this.barSubItem1.Name = "barSubItem1";
            componentResourceManager.ApplyResources(this._barChkTextView, "_barChkTextView");
            this._barChkTextView.Id = 4;
            this._barChkTextView.Name = "_barChkTextView";
            this._barChkTextView.ItemClick += new ItemClickEventHandler(this.On_TextView_Clicked);
            componentResourceManager.ApplyResources(this._barChkGridView, "_barChkGridView");
            this._barChkGridView.Id = 5;
            this._barChkGridView.Name = "_barChkGridView";
            this._barChkGridView.ItemClick += new ItemClickEventHandler(this.On_GridView_Clicked);
            this.barDockControlTop.CausesValidation = false;
            componentResourceManager.ApplyResources(this.barDockControlTop, "barDockControlTop");
            this.barDockControlBottom.CausesValidation = false;
            componentResourceManager.ApplyResources(this.barDockControlBottom, "barDockControlBottom");
            this.barDockControlLeft.CausesValidation = false;
            componentResourceManager.ApplyResources(this.barDockControlLeft, "barDockControlLeft");
            this.barDockControlRight.CausesValidation = false;
            componentResourceManager.ApplyResources(this.barDockControlRight, "barDockControlRight");
            this._textEditContextMenu.Name = "TextEditContextMenu";
            componentResourceManager.ApplyResources(this._textEditContextMenu, "_textEditContextMenu");
            componentResourceManager.ApplyResources(this, "$this");
            base.AutoScaleMode = AutoScaleMode.Font;
            base.Controls.Add(this.w_splitContainer);
            base.Controls.Add(this.barDockControlLeft);
            base.Controls.Add(this.barDockControlRight);
            base.Controls.Add(this.barDockControlBottom);
            base.Controls.Add(this.barDockControlTop);
            base.Name = "XMLComparer";
            this.w_splitContainer.Panel1.ResumeLayout(false);
            this.w_splitContainer.Panel2.ResumeLayout(false);
            this.w_splitContainer.ResumeLayout(false);
            ((ISupportInitialize)this.w_textBox1.Properties).EndInit();
            ((ISupportInitialize)this.w_panelTop1).EndInit();
            this.w_panelTop1.ResumeLayout(false);
            this.w_panelTop1.PerformLayout();
            ((ISupportInitialize)this.w_textBox2.Properties).EndInit();
            ((ISupportInitialize)this.w_panelTop2).EndInit();
            this.w_panelTop2.ResumeLayout(false);
            this.w_panelTop2.PerformLayout();
            ((ISupportInitialize)this._barManager).EndInit();
            base.ResumeLayout(false);
        }

        private void On_GridView_Clicked(object sender, ItemClickEventArgs e)
        {
            if (this._barChkGridView.Checked)
            {
                this._barChkTextView.Checked = false;
                this.DisplayMode = XMLComparer.DisplayModeType.GridView;
                return;
            }
            this._barChkTextView.Checked = true;
            this.DisplayMode = XMLComparer.DisplayModeType.TextView;
        }

        private void On_TextView_Clicked(object sender, ItemClickEventArgs e)
        {
            if (this._barChkTextView.Checked)
            {
                this._barChkGridView.Checked = false;
                this.DisplayMode = XMLComparer.DisplayModeType.TextView;
                return;
            }
            this._barChkGridView.Checked = true;
            this.DisplayMode = XMLComparer.DisplayModeType.GridView;
        }

        public void UpdateUI()
        {
            if (this.m_bUpdatingUI)
            {
                return;
            }
            try
            {
                try
                {
                    this._viewBar.Visible = this.ShowViewBar;
                    this.w_panelTop1.Visible = this.ShowLabels;
                    this.w_panelTop2.Visible = this.ShowLabels;
                    this.m_bUpdatingUI = true;
                    this.w_textBox1.Visible = this.DisplayMode == XMLComparer.DisplayModeType.TextView;
                    this.w_textBox2.Visible = this.DisplayMode == XMLComparer.DisplayModeType.TextView;
                    this.w_xmlPropertyGrid1.Visible = this.DisplayMode == XMLComparer.DisplayModeType.GridView;
                    this.w_xmlPropertyGrid2.Visible = this.DisplayMode == XMLComparer.DisplayModeType.GridView;
                    this._barChkTextView.Checked = this.DisplayMode == XMLComparer.DisplayModeType.TextView;
                    this._barChkGridView.Checked = this.DisplayMode == XMLComparer.DisplayModeType.GridView;
                }
                catch
                {
                }
            }
            finally
            {
                this.m_bUpdatingUI = false;
            }
        }

        public enum DisplayModeType
        {
            TextView,
            GridView
        }
    }
}