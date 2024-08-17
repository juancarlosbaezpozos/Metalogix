using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraTab;
using Metalogix;
using Metalogix.Actions;
using Metalogix.Interfaces;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Properties;
using Metalogix.Xml;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Resources;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Metalogix.UI.WinForms.Actions
{
    public partial class LogItemDetails : XtraForm
    {
        private LogItem _lastLogItemLoaded;

        private LogItemListControl m_parentControl;

        private static LogItemDetails m_instance;

        private IContainer components;

        private TextEdit w_txtTime;

        private TextEdit w_txtOperation;

        private LabelControl w_lblTimeStarted;

        private LabelControl w_lblOperation;

        private LabelControl w_lblInformation;

        private MemoEdit w_txtInformation;

        private LabelControl w_lblStatus;

        private TextEdit w_txtStatus;

        private SimpleButton w_btnClose;

        private LabelControl w_lblSource;

        private LabelControl w_lblTarget;

        private LabelControl w_lblItemName;

        private TextEdit w_txtItemName;

        private XMLComparer w_xmlComparer;

        private MemoEdit w_txtDetails;

        private SimpleButton w_btnPrev;

        private SimpleButton w_btnNext;

        private LinkLabel w_lnkSource;

        private LinkLabel w_lnkTarget;

        private ContextMenuStrip w_contextMenuStripSource;

        private ToolStripMenuItem copySourceUrlToolStripMenuItem;

        private ContextMenuStrip w_contextMenuStripTarget;

        private ToolStripMenuItem copyTargetUrlToolStripMenuItem;

        private ToolStripMenuItem openSourceURLInNewWindowToolStripMenuItem;

        private ToolStripMenuItem openTargetURLInNewWindowToolStripMenuItem;

        private SimpleButton w_btnCopy;

        private LabelControl w_lblTimeFinished;

        private TextEdit w_tbTimeFinished;

        private LabelControl w_lblLicenseUsed;

        private TextEdit w_tbLicenseUsed;

        private XtraTabControl _tabControl;

        private XtraTabPage _detailsTab;

        private XtraTabPage _contentTab;

        private LabelControl _lblOperationValue;

        private TextEditContextMenu _textEditContextMenu;

        public LogItemListControl ParentControl
        {
            get
            {
                return this.m_parentControl;
            }
        }

        static LogItemDetails()
        {
        }

        private LogItemDetails(LogItemListControl parentControl)
        {
            this.m_parentControl = parentControl;
            this.m_parentControl.UIUpdated += new LogItemListControl.UIUpdatedHandler(this.On_parentControl_SelectedItemChanged);
            this.m_parentControl.Disposed += new EventHandler(this.On_parentControl_Disposed);
            this.InitializeComponent();
            base.Icon = Icon.FromHandle(Resources.ViewLogs16.GetHicon());
            this.UpdateUI();
        }

        private void ClearUI()
        {
            this.w_txtTime.Text = string.Empty;
            this.w_tbTimeFinished.Text = string.Empty;
            this.w_txtStatus.Text = string.Empty;
            this.w_txtOperation.Text = string.Empty;
            this.w_txtItemName.Text = string.Empty;
            this.w_lnkSource.Text = string.Empty;
            this.w_lblLicenseUsed.Text = string.Empty;
            this.w_tbLicenseUsed.Text = string.Empty;
            this.w_lblLicenseUsed.Visible = false;
            this.w_tbLicenseUsed.Visible = false;
            this.w_lnkSource.Links.Clear();
            this.w_lnkSource.Text = string.Empty;
            this.w_lnkSource.Links.Add(this.w_lnkSource.Text.Length, 0);
            this.w_contextMenuStripSource.Items.Remove(this.openSourceURLInNewWindowToolStripMenuItem);
            this.w_lnkTarget.Links.Clear();
            this.w_lnkTarget.Text = string.Empty;
            this.w_lnkTarget.Links.Add(this.w_lnkTarget.Text.Length, 0);
            this.w_contextMenuStripTarget.Items.Remove(this.openTargetURLInNewWindowToolStripMenuItem);
            this.w_txtInformation.Text = string.Empty;
            this.w_txtDetails.Text = string.Empty;
            this.w_xmlComparer.SelectedText1 = string.Empty;
            this.w_xmlComparer.SelectedText2 = string.Empty;
        }

        protected override void Dispose(bool disposing)
        {
            LogItemDetails.m_instance = null;
            if (disposing && this.components != null)
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        public static LogItemDetails GetInstance(LogItemListControl parentControl)
        {
            if (LogItemDetails.m_instance != null)
            {
                return LogItemDetails.m_instance;
            }
            LogItemDetails.m_instance = new LogItemDetails(parentControl);
            return LogItemDetails.m_instance;
        }

        private TextEdit GetTextEdit(object sender)
        {
            ToolStripMenuItem toolStripMenuItem = sender as ToolStripMenuItem;
            if (toolStripMenuItem == null)
            {
                return sender as TextEdit;
            }
            ContextMenuStrip owner = toolStripMenuItem.Owner as ContextMenuStrip;
            if (owner == null)
            {
                return null;
            }
            TextBoxMaskBox sourceControl = owner.SourceControl as TextBoxMaskBox;
            if (sourceControl != null)
            {
                return sourceControl.OwnerEdit;
            }
            return owner.SourceControl as TextEdit;
        }

        private void InitializeComponent()
        {
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(LogItemDetails));
            this.w_txtTime = new TextEdit();
            this._textEditContextMenu = new TextEditContextMenu();
            this.w_txtOperation = new TextEdit();
            this.w_lblTimeStarted = new LabelControl();
            this.w_lblOperation = new LabelControl();
            this.w_lblInformation = new LabelControl();
            this.w_txtInformation = new MemoEdit();
            this.w_lblStatus = new LabelControl();
            this.w_txtStatus = new TextEdit();
            this.w_btnClose = new SimpleButton();
            this.w_lblSource = new LabelControl();
            this.w_lblTarget = new LabelControl();
            this.w_lblItemName = new LabelControl();
            this.w_txtItemName = new TextEdit();
            this.w_xmlComparer = new XMLComparer();
            this.w_txtDetails = new MemoEdit();
            this.w_btnPrev = new SimpleButton();
            this.w_btnNext = new SimpleButton();
            this.w_lnkSource = new LinkLabel();
            this.w_contextMenuStripSource = new ContextMenuStrip();
            this.copySourceUrlToolStripMenuItem = new ToolStripMenuItem();
            this.openSourceURLInNewWindowToolStripMenuItem = new ToolStripMenuItem();
            this.w_lnkTarget = new LinkLabel();
            this.w_contextMenuStripTarget = new ContextMenuStrip();
            this.copyTargetUrlToolStripMenuItem = new ToolStripMenuItem();
            this.openTargetURLInNewWindowToolStripMenuItem = new ToolStripMenuItem();
            this.w_btnCopy = new SimpleButton();
            this.w_lblTimeFinished = new LabelControl();
            this.w_tbTimeFinished = new TextEdit();
            this.w_lblLicenseUsed = new LabelControl();
            this.w_tbLicenseUsed = new TextEdit();
            this._tabControl = new XtraTabControl();
            this._detailsTab = new XtraTabPage();
            this._contentTab = new XtraTabPage();
            this._lblOperationValue = new LabelControl();
            ((ISupportInitialize)this.w_txtTime.Properties).BeginInit();
            ((ISupportInitialize)this.w_txtOperation.Properties).BeginInit();
            ((ISupportInitialize)this.w_txtInformation.Properties).BeginInit();
            ((ISupportInitialize)this.w_txtStatus.Properties).BeginInit();
            ((ISupportInitialize)this.w_txtItemName.Properties).BeginInit();
            ((ISupportInitialize)this.w_txtDetails.Properties).BeginInit();
            this.w_contextMenuStripSource.SuspendLayout();
            this.w_contextMenuStripTarget.SuspendLayout();
            ((ISupportInitialize)this.w_tbTimeFinished.Properties).BeginInit();
            ((ISupportInitialize)this.w_tbLicenseUsed.Properties).BeginInit();
            ((ISupportInitialize)this._tabControl).BeginInit();
            this._tabControl.SuspendLayout();
            this._detailsTab.SuspendLayout();
            this._contentTab.SuspendLayout();
            base.SuspendLayout();
            componentResourceManager.ApplyResources(this.w_txtTime, "w_txtTime");
            this.w_txtTime.Name = "w_txtTime";
            this.w_txtTime.Properties.BorderStyle = BorderStyles.NoBorder;
            this.w_txtTime.Properties.ContextMenuStrip = this._textEditContextMenu;
            this.w_txtTime.Properties.ReadOnly = true;
            this._textEditContextMenu.Name = "TextEditContextMenu";
            componentResourceManager.ApplyResources(this._textEditContextMenu, "_textEditContextMenu");
            componentResourceManager.ApplyResources(this.w_txtOperation, "w_txtOperation");
            this.w_txtOperation.Name = "w_txtOperation";
            this.w_txtOperation.Properties.BorderStyle = BorderStyles.NoBorder;
            this.w_txtOperation.Properties.ContextMenuStrip = this._textEditContextMenu;
            this.w_txtOperation.Properties.ReadOnly = true;
            this.w_lblTimeStarted.Appearance.Font = (Font)componentResourceManager.GetObject("w_lblTimeStarted.Appearance.Font");
            componentResourceManager.ApplyResources(this.w_lblTimeStarted, "w_lblTimeStarted");
            this.w_lblTimeStarted.Name = "w_lblTimeStarted";
            this.w_lblOperation.Appearance.Font = (Font)componentResourceManager.GetObject("w_lblOperation.Appearance.Font");
            componentResourceManager.ApplyResources(this.w_lblOperation, "w_lblOperation");
            this.w_lblOperation.Name = "w_lblOperation";
            this.w_lblInformation.Appearance.Font = (Font)componentResourceManager.GetObject("w_lblInformation.Appearance.Font");
            componentResourceManager.ApplyResources(this.w_lblInformation, "w_lblInformation");
            this.w_lblInformation.Name = "w_lblInformation";
            componentResourceManager.ApplyResources(this.w_txtInformation, "w_txtInformation");
            this.w_txtInformation.Name = "w_txtInformation";
            this.w_txtInformation.Properties.ContextMenuStrip = this._textEditContextMenu;
            this.w_txtInformation.Properties.ReadOnly = true;
            this.w_lblStatus.Appearance.Font = (Font)componentResourceManager.GetObject("w_lblStatus.Appearance.Font");
            componentResourceManager.ApplyResources(this.w_lblStatus, "w_lblStatus");
            this.w_lblStatus.Name = "w_lblStatus";
            componentResourceManager.ApplyResources(this.w_txtStatus, "w_txtStatus");
            this.w_txtStatus.Name = "w_txtStatus";
            this.w_txtStatus.Properties.BorderStyle = BorderStyles.NoBorder;
            this.w_txtStatus.Properties.ContextMenuStrip = this._textEditContextMenu;
            this.w_txtStatus.Properties.ReadOnly = true;
            componentResourceManager.ApplyResources(this.w_btnClose, "w_btnClose");
            this.w_btnClose.DialogResult = DialogResult.OK;
            this.w_btnClose.Name = "w_btnClose";
            this.w_btnClose.Click += new EventHandler(this.On_btnClose_Click);
            this.w_lblSource.Appearance.Font = (Font)componentResourceManager.GetObject("w_lblSource.Appearance.Font");
            componentResourceManager.ApplyResources(this.w_lblSource, "w_lblSource");
            this.w_lblSource.Name = "w_lblSource";
            this.w_lblTarget.Appearance.Font = (Font)componentResourceManager.GetObject("w_lblTarget.Appearance.Font");
            componentResourceManager.ApplyResources(this.w_lblTarget, "w_lblTarget");
            this.w_lblTarget.Name = "w_lblTarget";
            this.w_lblItemName.Appearance.Font = (Font)componentResourceManager.GetObject("w_lblItemName.Appearance.Font");
            componentResourceManager.ApplyResources(this.w_lblItemName, "w_lblItemName");
            this.w_lblItemName.Name = "w_lblItemName";
            componentResourceManager.ApplyResources(this.w_txtItemName, "w_txtItemName");
            this.w_txtItemName.Name = "w_txtItemName";
            this.w_txtItemName.Properties.BorderStyle = BorderStyles.NoBorder;
            this.w_txtItemName.Properties.ContextMenuStrip = this._textEditContextMenu;
            this.w_txtItemName.Properties.ReadOnly = true;
            this.w_xmlComparer.DisplayMode = XMLComparer.DisplayModeType.GridView;
            componentResourceManager.ApplyResources(this.w_xmlComparer, "w_xmlComparer");
            this.w_xmlComparer.LabelSource = "Source Content";
            this.w_xmlComparer.LabelTarget = "Target Content";
            this.w_xmlComparer.Name = "w_xmlComparer";
            this.w_xmlComparer.SelectedNode1 = null;
            this.w_xmlComparer.SelectedNode2 = null;
            this.w_xmlComparer.SelectedText1 = "";
            this.w_xmlComparer.SelectedText2 = "";
            this.w_xmlComparer.ShowLabels = true;
            this.w_xmlComparer.ShowViewBar = true;
            componentResourceManager.ApplyResources(this.w_txtDetails, "w_txtDetails");
            this.w_txtDetails.Name = "w_txtDetails";
            this.w_txtDetails.Properties.ContextMenuStrip = this._textEditContextMenu;
            this.w_txtDetails.Properties.ReadOnly = true;
            componentResourceManager.ApplyResources(this.w_btnPrev, "w_btnPrev");
            this.w_btnPrev.Name = "w_btnPrev";
            this.w_btnPrev.Click += new EventHandler(this.w_btnPrev_Click);
            componentResourceManager.ApplyResources(this.w_btnNext, "w_btnNext");
            this.w_btnNext.Name = "w_btnNext";
            this.w_btnNext.Click += new EventHandler(this.w_btnNext_Click);
            componentResourceManager.ApplyResources(this.w_lnkSource, "w_lnkSource");
            this.w_lnkSource.AutoEllipsis = true;
            this.w_lnkSource.ContextMenuStrip = this.w_contextMenuStripSource;
            this.w_lnkSource.Name = "w_lnkSource";
            this.w_lnkSource.LinkClicked += new LinkLabelLinkClickedEventHandler(this.On_lnkSource_LinkClicked);
            ToolStripItemCollection items = this.w_contextMenuStripSource.Items;
            ToolStripItem[] toolStripItemArray = new ToolStripItem[] { this.copySourceUrlToolStripMenuItem, this.openSourceURLInNewWindowToolStripMenuItem };
            items.AddRange(toolStripItemArray);
            this.w_contextMenuStripSource.Name = "w_contextMenuStripSource";
            componentResourceManager.ApplyResources(this.w_contextMenuStripSource, "w_contextMenuStripSource");
            this.copySourceUrlToolStripMenuItem.BackColor = Color.White;
            this.copySourceUrlToolStripMenuItem.Image = Resources.CopySelectedJobsToClipboard16;
            this.copySourceUrlToolStripMenuItem.Name = "copySourceUrlToolStripMenuItem";
            componentResourceManager.ApplyResources(this.copySourceUrlToolStripMenuItem, "copySourceUrlToolStripMenuItem");
            this.copySourceUrlToolStripMenuItem.Click += new EventHandler(this.On_CopySourceUrlToolStripMenuItem_Click);
            this.openSourceURLInNewWindowToolStripMenuItem.BackColor = Color.White;
            this.openSourceURLInNewWindowToolStripMenuItem.Image = Resources.Navigate16;
            this.openSourceURLInNewWindowToolStripMenuItem.Name = "openSourceURLInNewWindowToolStripMenuItem";
            componentResourceManager.ApplyResources(this.openSourceURLInNewWindowToolStripMenuItem, "openSourceURLInNewWindowToolStripMenuItem");
            this.openSourceURLInNewWindowToolStripMenuItem.Click += new EventHandler(this.On_OpenSourceURLInNewWindowToolStripMenuItem_Click);
            componentResourceManager.ApplyResources(this.w_lnkTarget, "w_lnkTarget");
            this.w_lnkTarget.AutoEllipsis = true;
            this.w_lnkTarget.ContextMenuStrip = this.w_contextMenuStripTarget;
            this.w_lnkTarget.Name = "w_lnkTarget";
            this.w_lnkTarget.LinkClicked += new LinkLabelLinkClickedEventHandler(this.On_lnkTarget_LinkClicked);
            ToolStripItemCollection toolStripItemCollections = this.w_contextMenuStripTarget.Items;
            ToolStripItem[] toolStripItemArray1 = new ToolStripItem[] { this.copyTargetUrlToolStripMenuItem, this.openTargetURLInNewWindowToolStripMenuItem };
            toolStripItemCollections.AddRange(toolStripItemArray1);
            this.w_contextMenuStripTarget.Name = "w_contextMenuStripTarget";
            componentResourceManager.ApplyResources(this.w_contextMenuStripTarget, "w_contextMenuStripTarget");
            this.copyTargetUrlToolStripMenuItem.Image = Resources.CopySelectedJobsToClipboard16;
            this.copyTargetUrlToolStripMenuItem.Name = "copyTargetUrlToolStripMenuItem";
            componentResourceManager.ApplyResources(this.copyTargetUrlToolStripMenuItem, "copyTargetUrlToolStripMenuItem");
            this.copyTargetUrlToolStripMenuItem.Click += new EventHandler(this.On_CopyTargetUrlToolStripMenuItem_Click);
            this.openTargetURLInNewWindowToolStripMenuItem.Image = Resources.Navigate16;
            this.openTargetURLInNewWindowToolStripMenuItem.Name = "openTargetURLInNewWindowToolStripMenuItem";
            componentResourceManager.ApplyResources(this.openTargetURLInNewWindowToolStripMenuItem, "openTargetURLInNewWindowToolStripMenuItem");
            this.openTargetURLInNewWindowToolStripMenuItem.Click += new EventHandler(this.On_OpenTargetURLInNewWindowToolStripMenuItem_Click);
            componentResourceManager.ApplyResources(this.w_btnCopy, "w_btnCopy");
            this.w_btnCopy.Name = "w_btnCopy";
            this.w_btnCopy.Click += new EventHandler(this.w_btnCopy_Click);
            this.w_lblTimeFinished.Appearance.Font = (Font)componentResourceManager.GetObject("w_lblTimeFinished.Appearance.Font");
            componentResourceManager.ApplyResources(this.w_lblTimeFinished, "w_lblTimeFinished");
            this.w_lblTimeFinished.Name = "w_lblTimeFinished";
            componentResourceManager.ApplyResources(this.w_tbTimeFinished, "w_tbTimeFinished");
            this.w_tbTimeFinished.Name = "w_tbTimeFinished";
            this.w_tbTimeFinished.Properties.BorderStyle = BorderStyles.NoBorder;
            this.w_tbTimeFinished.Properties.ContextMenuStrip = this._textEditContextMenu;
            this.w_tbTimeFinished.Properties.ReadOnly = true;
            this.w_lblLicenseUsed.Appearance.Font = (Font)componentResourceManager.GetObject("w_lblLicenseUsed.Appearance.Font");
            componentResourceManager.ApplyResources(this.w_lblLicenseUsed, "w_lblLicenseUsed");
            this.w_lblLicenseUsed.Name = "w_lblLicenseUsed";
            componentResourceManager.ApplyResources(this.w_tbLicenseUsed, "w_tbLicenseUsed");
            this.w_tbLicenseUsed.Name = "w_tbLicenseUsed";
            this.w_tbLicenseUsed.Properties.BorderStyle = BorderStyles.NoBorder;
            this.w_tbLicenseUsed.Properties.ContextMenuStrip = this._textEditContextMenu;
            this.w_tbLicenseUsed.Properties.ReadOnly = true;
            componentResourceManager.ApplyResources(this._tabControl, "_tabControl");
            this._tabControl.Name = "_tabControl";
            this._tabControl.SelectedTabPage = this._detailsTab;
            XtraTabPageCollection tabPages = this._tabControl.TabPages;
            XtraTabPage[] xtraTabPageArray = new XtraTabPage[] { this._detailsTab, this._contentTab };
            tabPages.AddRange(xtraTabPageArray);
            this._detailsTab.Controls.Add(this.w_txtDetails);
            this._detailsTab.Name = "_detailsTab";
            componentResourceManager.ApplyResources(this._detailsTab, "_detailsTab");
            this._contentTab.Controls.Add(this.w_xmlComparer);
            this._contentTab.Name = "_contentTab";
            componentResourceManager.ApplyResources(this._contentTab, "_contentTab");
            this._lblOperationValue.AutoEllipsis = true;
            componentResourceManager.ApplyResources(this._lblOperationValue, "_lblOperationValue");
            this._lblOperationValue.Name = "_lblOperationValue";
            base.AcceptButton = this.w_btnClose;
            componentResourceManager.ApplyResources(this, "$this");
            base.AutoScaleMode = AutoScaleMode.Font;
            base.CancelButton = this.w_btnClose;
            base.Controls.Add(this._lblOperationValue);
            base.Controls.Add(this._tabControl);
            base.Controls.Add(this.w_lblLicenseUsed);
            base.Controls.Add(this.w_lblTimeFinished);
            base.Controls.Add(this.w_tbTimeFinished);
            base.Controls.Add(this.w_tbLicenseUsed);
            base.Controls.Add(this.w_btnCopy);
            base.Controls.Add(this.w_lnkSource);
            base.Controls.Add(this.w_btnNext);
            base.Controls.Add(this.w_lnkTarget);
            base.Controls.Add(this.w_btnPrev);
            base.Controls.Add(this.w_lblItemName);
            base.Controls.Add(this.w_txtItemName);
            base.Controls.Add(this.w_lblSource);
            base.Controls.Add(this.w_btnClose);
            base.Controls.Add(this.w_lblTarget);
            base.Controls.Add(this.w_lblStatus);
            base.Controls.Add(this.w_txtStatus);
            base.Controls.Add(this.w_lblInformation);
            base.Controls.Add(this.w_txtInformation);
            base.Controls.Add(this.w_lblOperation);
            base.Controls.Add(this.w_lblTimeStarted);
            base.Controls.Add(this.w_txtOperation);
            base.Controls.Add(this.w_txtTime);
            base.Name = "LogItemDetails";
            base.Shown += new EventHandler(this.On_Shown);
            ((ISupportInitialize)this.w_txtTime.Properties).EndInit();
            ((ISupportInitialize)this.w_txtOperation.Properties).EndInit();
            ((ISupportInitialize)this.w_txtInformation.Properties).EndInit();
            ((ISupportInitialize)this.w_txtStatus.Properties).EndInit();
            ((ISupportInitialize)this.w_txtItemName.Properties).EndInit();
            ((ISupportInitialize)this.w_txtDetails.Properties).EndInit();
            this.w_contextMenuStripSource.ResumeLayout(false);
            this.w_contextMenuStripTarget.ResumeLayout(false);
            ((ISupportInitialize)this.w_tbTimeFinished.Properties).EndInit();
            ((ISupportInitialize)this.w_tbLicenseUsed.Properties).EndInit();
            ((ISupportInitialize)this._tabControl).EndInit();
            this._tabControl.ResumeLayout(false);
            this._detailsTab.ResumeLayout(false);
            this._contentTab.ResumeLayout(false);
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void LoadUI(LogItem item)
        {
            string upper;
            if (item == null)
            {
                this.ClearUI();
                return;
            }
            if (this._lastLogItemLoaded == item)
            {
                return;
            }
            this.w_txtTime.Text = item.TimeStamp.ToString();
            this.w_tbTimeFinished.Text = item.FinishedTime.ToString();
            this.w_txtStatus.Text = item.Status.ToString();
            this.w_txtOperation.Text = item.Operation;
            this.w_txtItemName.Text = item.ItemName;
            this.w_lnkSource.Text = item.Source;
            string actionLicensingDescriptor = item.ActionLicensingDescriptor;
            LabelControl wLblLicenseUsed = this.w_lblLicenseUsed;
            if (actionLicensingDescriptor != null)
            {
                upper = actionLicensingDescriptor.ToUpper();
            }
            else
            {
                upper = null;
            }
            wLblLicenseUsed.Text = upper;
            this.w_tbLicenseUsed.Text = item.GetFormattedLicensedData();
            if (string.IsNullOrEmpty(this.w_lblLicenseUsed.Text))
            {
                this.w_lblLicenseUsed.Visible = false;
                this.w_tbLicenseUsed.Visible = false;
            }
            this.w_lnkSource.Links.Clear();
            if (!WebUtils.ValidLink(this.w_lnkSource.Text, true))
            {
                this.w_lnkSource.Links.Add(this.w_lnkSource.Text.Length, 0);
                this.w_contextMenuStripSource.Items.Remove(this.openSourceURLInNewWindowToolStripMenuItem);
            }
            else
            {
                this.w_lnkSource.Links.Add(0, this.w_lnkSource.Text.Length);
                if (!this.w_contextMenuStripSource.Items.Contains(this.openSourceURLInNewWindowToolStripMenuItem))
                {
                    this.w_contextMenuStripSource.Items.Add(this.openSourceURLInNewWindowToolStripMenuItem);
                }
            }
            this.w_lnkTarget.Text = item.Target;
            this.w_lnkTarget.Links.Clear();
            if (!WebUtils.ValidLink(this.w_lnkTarget.Text, true))
            {
                this.w_lnkTarget.Links.Add(this.w_lnkTarget.Text.Length, 0);
                this.w_contextMenuStripTarget.Items.Remove(this.openTargetURLInNewWindowToolStripMenuItem);
            }
            else
            {
                this.w_lnkTarget.Links.Add(0, this.w_lnkTarget.Text.Length);
                if (!this.w_contextMenuStripTarget.Items.Contains(this.openTargetURLInNewWindowToolStripMenuItem))
                {
                    this.w_contextMenuStripTarget.Items.Add(this.openTargetURLInNewWindowToolStripMenuItem);
                }
            }
            this.w_txtInformation.Text = item.Information;
            this.w_txtDetails.Text = item.Details;
            this.w_xmlComparer.SelectedText1 = item.SourceContent;
            this.w_xmlComparer.SelectedText2 = item.TargetContent;
            item.ClearDetails();
            this._lastLogItemLoaded = item;
        }

        private void On_btnClose_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        private void On_Copy_Clicked(object sender, EventArgs e)
        {
            TextEdit textEdit = this.GetTextEdit(sender);
            if (textEdit == null)
            {
                return;
            }
            Clipboard.SetText(textEdit.SelectedText);
        }

        private void On_CopySourceUrlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(this.w_lnkSource.Text);
        }

        private void On_CopyTargetUrlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(this.w_lnkTarget.Text);
        }

        private void On_lnkSource_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.OpenLink(this.w_lnkSource.Text);
            }
        }

        private void On_lnkTarget_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.OpenLink(this.w_lnkTarget.Text);
            }
        }

        private void On_OpenSourceURLInNewWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.OpenLink(this.w_lnkSource.Text);
        }

        private void On_OpenTargetURLInNewWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.OpenLink(this.w_lnkTarget.Text);
        }

        private void On_parentControl_Disposed(object sender, EventArgs e)
        {
            this._lastLogItemLoaded = null;
            base.Close();
        }

        private void On_parentControl_SelectedItemChanged()
        {
            this.UpdateUI();
        }

        private void On_SelectAll_Clicked(object sender, EventArgs e)
        {
            TextEdit textEdit = this.GetTextEdit(sender);
            if (textEdit == null)
            {
                return;
            }
            textEdit.SelectAll();
        }

        private void On_Shown(object sender, EventArgs e)
        {
            this.UpdateUI();
        }

        private void OpenLink(string sFileName)
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = sFileName;
                process.StartInfo.UseShellExecute = true;
                process.Start();
            }
            catch (Exception exception)
            {
                GlobalServices.ErrorHandler.HandleException(exception);
            }
        }

        private void UpdateUI()
        {
            this.w_btnPrev.Enabled = this.ParentControl.CanSelectPreviousIndex;
            this.w_btnNext.Enabled = this.ParentControl.CanSelectNextIndex;
            if (base.Visible)
            {
                this.LoadUI(this.ParentControl.SelectedItem);
            }
        }

        private void w_btnCopy_Click(object sender, EventArgs e)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(string.Concat("Time:\t\t", this.w_txtTime.Text));
            stringBuilder.AppendLine(string.Concat("Status:\t\t", this.w_txtStatus.Text));
            stringBuilder.AppendLine(string.Concat("Operation:\t", this.w_txtOperation.Text));
            stringBuilder.AppendLine(string.Concat("Item Name:\t", this.w_txtItemName.Text));
            stringBuilder.AppendLine(string.Concat("Source:\t\t", this.w_lnkSource.Text));
            stringBuilder.AppendLine(string.Concat("Target:\t\t", this.w_lnkTarget.Text));
            stringBuilder.AppendLine(string.Concat("Information:\r\n", this.w_txtInformation.Text));
            if (this.w_xmlComparer.SelectedNode1 != null)
            {
                stringBuilder.AppendLine("\r\nSource Content:");
                for (int i = 0; i < this.w_xmlComparer.SelectedNode1.Attributes.Count; i++)
                {
                    stringBuilder.AppendLine(string.Concat(this.w_xmlComparer.SelectedNode1.Attributes[i].Name, ": ", this.w_xmlComparer.SelectedNode1.Attributes[i].Value));
                }
            }
            if (this.w_xmlComparer.SelectedNode2 != null)
            {
                stringBuilder.AppendLine("\r\nTarget Content:");
                for (int j = 0; j < this.w_xmlComparer.SelectedNode2.Attributes.Count; j++)
                {
                    stringBuilder.AppendLine(string.Concat(this.w_xmlComparer.SelectedNode2.Attributes[j].Name, ": ", this.w_xmlComparer.SelectedNode2.Attributes[j].Value));
                }
            }
            stringBuilder.AppendLine(string.Concat("\r\nDetails:\r\n", this.w_txtDetails.Text));
            Clipboard.SetText(stringBuilder.ToString());
        }

        private void w_btnNext_Click(object sender, EventArgs e)
        {
            this.ParentControl.SelectNextIndex();
        }

        private void w_btnPrev_Click(object sender, EventArgs e)
        {
            this.ParentControl.SelectPreviousIndex();
        }
    }
}