using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Metalogix.Explorer;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Properties;
using Metalogix.SharePoint.UI.WinForms.Properties;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Components;

namespace Metalogix.SharePoint.UI.WinForms.Administration
{
    public class NodePropertiesDialog : CollapsableForm
    {
        private TextEdit w_txtNodeName;

        private TextEdit w_txtNodeAddress;

        private TextEdit w_txtNodeType;

        private SimpleButton w_btnClose;

        private LabelControl w_lblNodeName;

        private LabelControl w_lblAddress;

        private LabelControl w_lblNodeType;

        private TextEdit w_tbListTemplate;

        private LabelControl w_lblListTemplate;

        private TextEdit w_txtLocation;

        private LabelControl label1;

        private Panel w_plDBReferences;

        private TextEdit w_tbLinkName;

        private LabelControl w_lblHostName;

        private SimpleButton w_btnEditHost;

        private Panel w_plLocation;

        private Panel w_plListTemplate;

        private LabelControl w_lblNodeXML;

        private MemoEdit w_memoNodeXml;

        private LabelControl w_lblServerInfo;

        private MemoEdit w_memoServerInfo;

        private Panel w_plServerInfo;

        private Panel w_plConnectionType;

        private LabelControl w_labelConnectionType;

        private TextEdit w_txtConnectionType;

        private TextEdit w_txtConnectedAs;

        private LabelControl w_labelConnectedAs;

        private SimpleButton w_btnCustomTemplatePath;

        private TextEdit w_tbCustomTemplatePath;

        private LabelControl w_lblTemplate;

        private TextEditContextMenu _textEditMenu;

        private Container components;

        private Node m_node;

        public Node Node
        {
            get
            {
                return m_node;
            }
            set
            {
                m_node = value;
                if (m_node == null)
                {
                    w_txtNodeAddress.Text = "";
                    w_txtNodeName.Text = "";
                    w_txtNodeType.Text = "";
                    w_tbListTemplate.Text = "";
                    w_txtLocation.Text = "";
                    w_txtConnectedAs.Text = "";
                    w_txtConnectionType.Text = "";
                    return;
                }
                w_txtNodeAddress.Text = (string.IsNullOrEmpty(m_node.LinkableUrl) ? m_node.DisplayUrl : m_node.LinkableUrl);
                w_txtNodeName.Text = m_node.Name;
                w_txtNodeType.Text = m_node.GetType().ToString();
                SPNode sPNode = m_node as SPNode;
                SPConnection sPConnection = m_node as SPConnection;
                SPList sPList = m_node as SPList;
                IDBReader iDBReader = ((sPNode == null) ? null : (sPNode.Adapter as IDBReader));
                IDBReader iDBReader2 = iDBReader;
                if (sPConnection == null || m_node.Parent != null)
                {
                    HideControl(w_plConnectionType);
                }
                else
                {
                    w_txtConnectedAs.Text = sPConnection.Adapter.Credentials.UserName;
                    w_txtConnectionType.Text = sPConnection.Adapter.ConnectionTypeDisplayString;
                }
                if (sPConnection == null || m_node.Parent != null || iDBReader2 != null)
                {
                    HideControl(w_plServerInfo);
                }
                else
                {
                    w_memoServerInfo.Text = sPConnection.Adapter.SystemInfo.ToString();
                    if (w_memoServerInfo.Text.Length > 0)
                    {
                        w_memoServerInfo.Text += "\r\n";
                    }
                    MemoEdit memoEdit = w_memoServerInfo;
                    memoEdit.Text = memoEdit.Text + "SharePoint Version: " + sPConnection.Adapter.SharePointVersion.ToString();
                }
                if (sPList == null)
                {
                    HideControl(w_plListTemplate);
                }
                else
                {
                    sPList.RefreshXML();
                    TextEdit textEdit = w_txtNodeType;
                    object obj = textEdit.Text;
                    object[] array = new object[4] { obj, " (", sPList.BaseType, ")" };
                    textEdit.Text = string.Concat(array);
                    TextEdit textEdit2 = w_tbListTemplate;
                    textEdit2.Text = (int)sPList.BaseType + ", " + sPList.BaseTemplate;
                }
                w_txtLocation.Text = m_node.Location.ToXML();
                if (sPNode == null || m_node.Parent != null || iDBReader2 == null)
                {
                    HideControl(w_plDBReferences);
                }
                else
                {
                    w_tbLinkName.Text = sPNode.Adapter.ServerLinkName;
                    w_tbCustomTemplatePath.Text = iDBReader2.CustomTemplatePath;
                }
                try
                {
                    w_memoNodeXml.Text = m_node.XML;
                }
                catch (Exception ex)
                {
                    w_memoNodeXml.Text = "Error fetching node XML: " + ex.Message + "\n" + ex.StackTrace;
                }
                HideControl(w_plLocation);
                MinimumSize = base.Size;
                MemoEdit memoEdit2 = w_memoNodeXml;
                int num = w_memoNodeXml.Location.X;
                memoEdit2.Location = new Point(num, w_lblNodeXML.Location.Y);
                int num2 = w_btnClose.Location.Y - 24;
                if (num2 < w_memoNodeXml.Location.Y + w_memoNodeXml.Size.Height)
                {
                    MemoEdit memoEdit3 = w_memoNodeXml;
                    int num3 = w_memoNodeXml.Size.Width;
                    memoEdit3.MinimumSize = new Size(num3, num2 - w_memoNodeXml.Location.Y);
                    w_memoNodeXml.Size = w_memoNodeXml.MinimumSize;
                }
            }
        }

        public NodePropertiesDialog()
        {
            InitializeComponent();
            base.Icon = Icon.FromHandle(Metalogix.SharePoint.UI.WinForms.Properties.Resources.Properties16.GetHicon());
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Administration.NodePropertiesDialog));
            this.w_lblNodeName = new DevExpress.XtraEditors.LabelControl();
            this.w_lblAddress = new DevExpress.XtraEditors.LabelControl();
            this.w_lblNodeType = new DevExpress.XtraEditors.LabelControl();
            this.w_txtNodeName = new DevExpress.XtraEditors.TextEdit();
            this._textEditMenu = new Metalogix.UI.WinForms.TextEditContextMenu();
            this.w_txtNodeAddress = new DevExpress.XtraEditors.TextEdit();
            this.w_txtNodeType = new DevExpress.XtraEditors.TextEdit();
            this.w_btnClose = new DevExpress.XtraEditors.SimpleButton();
            this.w_tbListTemplate = new DevExpress.XtraEditors.TextEdit();
            this.w_lblListTemplate = new DevExpress.XtraEditors.LabelControl();
            this.w_txtLocation = new DevExpress.XtraEditors.TextEdit();
            this.label1 = new DevExpress.XtraEditors.LabelControl();
            this.w_plDBReferences = new System.Windows.Forms.Panel();
            this.w_btnCustomTemplatePath = new DevExpress.XtraEditors.SimpleButton();
            this.w_tbCustomTemplatePath = new DevExpress.XtraEditors.TextEdit();
            this.w_lblTemplate = new DevExpress.XtraEditors.LabelControl();
            this.w_btnEditHost = new DevExpress.XtraEditors.SimpleButton();
            this.w_tbLinkName = new DevExpress.XtraEditors.TextEdit();
            this.w_lblHostName = new DevExpress.XtraEditors.LabelControl();
            this.w_plLocation = new System.Windows.Forms.Panel();
            this.w_plListTemplate = new System.Windows.Forms.Panel();
            this.w_lblNodeXML = new DevExpress.XtraEditors.LabelControl();
            this.w_memoNodeXml = new DevExpress.XtraEditors.MemoEdit();
            this.w_lblServerInfo = new DevExpress.XtraEditors.LabelControl();
            this.w_memoServerInfo = new DevExpress.XtraEditors.MemoEdit();
            this.w_plServerInfo = new System.Windows.Forms.Panel();
            this.w_plConnectionType = new System.Windows.Forms.Panel();
            this.w_txtConnectedAs = new DevExpress.XtraEditors.TextEdit();
            this.w_txtConnectionType = new DevExpress.XtraEditors.TextEdit();
            this.w_labelConnectedAs = new DevExpress.XtraEditors.LabelControl();
            this.w_labelConnectionType = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)this.w_txtNodeName.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.w_txtNodeAddress.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.w_txtNodeType.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.w_tbListTemplate.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.w_txtLocation.Properties).BeginInit();
            this.w_plDBReferences.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.w_tbCustomTemplatePath.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.w_tbLinkName.Properties).BeginInit();
            this.w_plLocation.SuspendLayout();
            this.w_plListTemplate.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.w_memoNodeXml.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.w_memoServerInfo.Properties).BeginInit();
            this.w_plServerInfo.SuspendLayout();
            this.w_plConnectionType.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.w_txtConnectedAs.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.w_txtConnectionType.Properties).BeginInit();
            base.SuspendLayout();
            resources.ApplyResources(this.w_lblNodeName, "w_lblNodeName");
            this.w_lblNodeName.Name = "w_lblNodeName";
            resources.ApplyResources(this.w_lblAddress, "w_lblAddress");
            this.w_lblAddress.Name = "w_lblAddress";
            resources.ApplyResources(this.w_lblNodeType, "w_lblNodeType");
            this.w_lblNodeType.Name = "w_lblNodeType";
            resources.ApplyResources(this.w_txtNodeName, "w_txtNodeName");
            this.w_txtNodeName.Name = "w_txtNodeName";
            this.w_txtNodeName.Properties.ContextMenuStrip = this._textEditMenu;
            this.w_txtNodeName.Properties.ReadOnly = true;
            this.w_txtNodeName.KeyDown += new System.Windows.Forms.KeyEventHandler(On_TextBox_KeyDown);
            this._textEditMenu.Name = "TextEditContextMenu";
            resources.ApplyResources(this._textEditMenu, "_textEditMenu");
            resources.ApplyResources(this.w_txtNodeAddress, "w_txtNodeAddress");
            this.w_txtNodeAddress.Name = "w_txtNodeAddress";
            this.w_txtNodeAddress.Properties.ContextMenuStrip = this._textEditMenu;
            this.w_txtNodeAddress.Properties.ReadOnly = true;
            this.w_txtNodeAddress.KeyDown += new System.Windows.Forms.KeyEventHandler(On_TextBox_KeyDown);
            resources.ApplyResources(this.w_txtNodeType, "w_txtNodeType");
            this.w_txtNodeType.Name = "w_txtNodeType";
            this.w_txtNodeType.Properties.ContextMenuStrip = this._textEditMenu;
            this.w_txtNodeType.Properties.ReadOnly = true;
            this.w_txtNodeType.KeyDown += new System.Windows.Forms.KeyEventHandler(On_TextBox_KeyDown);
            resources.ApplyResources(this.w_btnClose, "w_btnClose");
            this.w_btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.w_btnClose.Name = "w_btnClose";
            resources.ApplyResources(this.w_tbListTemplate, "w_tbListTemplate");
            this.w_tbListTemplate.Name = "w_tbListTemplate";
            this.w_tbListTemplate.Properties.ContextMenuStrip = this._textEditMenu;
            this.w_tbListTemplate.Properties.ReadOnly = true;
            this.w_tbListTemplate.KeyDown += new System.Windows.Forms.KeyEventHandler(On_TextBox_KeyDown);
            resources.ApplyResources(this.w_lblListTemplate, "w_lblListTemplate");
            this.w_lblListTemplate.Name = "w_lblListTemplate";
            resources.ApplyResources(this.w_txtLocation, "w_txtLocation");
            this.w_txtLocation.Name = "w_txtLocation";
            this.w_txtLocation.Properties.ContextMenuStrip = this._textEditMenu;
            this.w_txtLocation.Properties.ReadOnly = true;
            this.w_txtLocation.KeyDown += new System.Windows.Forms.KeyEventHandler(On_TextBox_KeyDown);
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            resources.ApplyResources(this.w_plDBReferences, "w_plDBReferences");
            this.w_plDBReferences.Controls.Add(this.w_btnCustomTemplatePath);
            this.w_plDBReferences.Controls.Add(this.w_tbCustomTemplatePath);
            this.w_plDBReferences.Controls.Add(this.w_lblTemplate);
            this.w_plDBReferences.Controls.Add(this.w_btnEditHost);
            this.w_plDBReferences.Controls.Add(this.w_tbLinkName);
            this.w_plDBReferences.Controls.Add(this.w_lblHostName);
            this.w_plDBReferences.Name = "w_plDBReferences";
            resources.ApplyResources(this.w_btnCustomTemplatePath, "w_btnCustomTemplatePath");
            this.w_btnCustomTemplatePath.Name = "w_btnCustomTemplatePath";
            this.w_btnCustomTemplatePath.Click += new System.EventHandler(On_CustomTemplatePath_Clicked);
            resources.ApplyResources(this.w_tbCustomTemplatePath, "w_tbCustomTemplatePath");
            this.w_tbCustomTemplatePath.Name = "w_tbCustomTemplatePath";
            this.w_tbCustomTemplatePath.Properties.ContextMenuStrip = this._textEditMenu;
            this.w_tbCustomTemplatePath.Properties.ReadOnly = true;
            resources.ApplyResources(this.w_lblTemplate, "w_lblTemplate");
            this.w_lblTemplate.Name = "w_lblTemplate";
            resources.ApplyResources(this.w_btnEditHost, "w_btnEditHost");
            this.w_btnEditHost.Name = "w_btnEditHost";
            this.w_btnEditHost.Click += new System.EventHandler(ON_LinkName_Edited);
            resources.ApplyResources(this.w_tbLinkName, "w_tbLinkName");
            this.w_tbLinkName.Name = "w_tbLinkName";
            this.w_tbLinkName.Properties.ContextMenuStrip = this._textEditMenu;
            this.w_tbLinkName.Properties.ReadOnly = true;
            this.w_tbLinkName.KeyDown += new System.Windows.Forms.KeyEventHandler(On_TextBox_KeyDown);
            resources.ApplyResources(this.w_lblHostName, "w_lblHostName");
            this.w_lblHostName.Name = "w_lblHostName";
            resources.ApplyResources(this.w_plLocation, "w_plLocation");
            this.w_plLocation.Controls.Add(this.label1);
            this.w_plLocation.Controls.Add(this.w_txtLocation);
            this.w_plLocation.Name = "w_plLocation";
            resources.ApplyResources(this.w_plListTemplate, "w_plListTemplate");
            this.w_plListTemplate.Controls.Add(this.w_lblListTemplate);
            this.w_plListTemplate.Controls.Add(this.w_tbListTemplate);
            this.w_plListTemplate.Name = "w_plListTemplate";
            resources.ApplyResources(this.w_lblNodeXML, "w_lblNodeXML");
            this.w_lblNodeXML.Name = "w_lblNodeXML";
            resources.ApplyResources(this.w_memoNodeXml, "w_memoNodeXml");
            this.w_memoNodeXml.Name = "w_memoNodeXml";
            this.w_memoNodeXml.Properties.ContextMenuStrip = this._textEditMenu;
            this.w_memoNodeXml.Properties.ReadOnly = true;
            this.w_memoNodeXml.KeyDown += new System.Windows.Forms.KeyEventHandler(On_TextBox_KeyDown);
            resources.ApplyResources(this.w_lblServerInfo, "w_lblServerInfo");
            this.w_lblServerInfo.Name = "w_lblServerInfo";
            resources.ApplyResources(this.w_memoServerInfo, "w_memoServerInfo");
            this.w_memoServerInfo.Name = "w_memoServerInfo";
            this.w_memoServerInfo.Properties.ContextMenuStrip = this._textEditMenu;
            this.w_memoServerInfo.Properties.ReadOnly = true;
            this.w_memoServerInfo.KeyDown += new System.Windows.Forms.KeyEventHandler(On_TextBox_KeyDown);
            resources.ApplyResources(this.w_plServerInfo, "w_plServerInfo");
            this.w_plServerInfo.Controls.Add(this.w_memoServerInfo);
            this.w_plServerInfo.Controls.Add(this.w_lblServerInfo);
            this.w_plServerInfo.Name = "w_plServerInfo";
            this.w_plConnectionType.Controls.Add(this.w_txtConnectedAs);
            this.w_plConnectionType.Controls.Add(this.w_txtConnectionType);
            this.w_plConnectionType.Controls.Add(this.w_labelConnectedAs);
            this.w_plConnectionType.Controls.Add(this.w_labelConnectionType);
            resources.ApplyResources(this.w_plConnectionType, "w_plConnectionType");
            this.w_plConnectionType.Name = "w_plConnectionType";
            resources.ApplyResources(this.w_txtConnectedAs, "w_txtConnectedAs");
            this.w_txtConnectedAs.Name = "w_txtConnectedAs";
            this.w_txtConnectedAs.Properties.Appearance.BackColor = (System.Drawing.Color)resources.GetObject("w_txtConnectedAs.Properties.Appearance.BackColor");
            this.w_txtConnectedAs.Properties.Appearance.Options.UseBackColor = true;
            this.w_txtConnectedAs.Properties.ContextMenuStrip = this._textEditMenu;
            this.w_txtConnectedAs.Properties.ReadOnly = true;
            this.w_txtConnectedAs.KeyDown += new System.Windows.Forms.KeyEventHandler(On_TextBox_KeyDown);
            resources.ApplyResources(this.w_txtConnectionType, "w_txtConnectionType");
            this.w_txtConnectionType.Name = "w_txtConnectionType";
            this.w_txtConnectionType.Properties.Appearance.BackColor = (System.Drawing.Color)resources.GetObject("w_txtConnectionType.Properties.Appearance.BackColor");
            this.w_txtConnectionType.Properties.Appearance.Options.UseBackColor = true;
            this.w_txtConnectionType.Properties.ContextMenuStrip = this._textEditMenu;
            this.w_txtConnectionType.Properties.ReadOnly = true;
            this.w_txtConnectionType.KeyDown += new System.Windows.Forms.KeyEventHandler(On_TextBox_KeyDown);
            resources.ApplyResources(this.w_labelConnectedAs, "w_labelConnectedAs");
            this.w_labelConnectedAs.Name = "w_labelConnectedAs";
            resources.ApplyResources(this.w_labelConnectionType, "w_labelConnectionType");
            this.w_labelConnectionType.Name = "w_labelConnectionType";
            base.AcceptButton = this.w_btnClose;
            resources.ApplyResources(this, "$this");
            base.CancelButton = this.w_btnClose;
            base.Controls.Add(this.w_plConnectionType);
            base.Controls.Add(this.w_plServerInfo);
            base.Controls.Add(this.w_memoNodeXml);
            base.Controls.Add(this.w_lblNodeXML);
            base.Controls.Add(this.w_btnClose);
            base.Controls.Add(this.w_plListTemplate);
            base.Controls.Add(this.w_plLocation);
            base.Controls.Add(this.w_plDBReferences);
            base.Controls.Add(this.w_txtNodeType);
            base.Controls.Add(this.w_txtNodeAddress);
            base.Controls.Add(this.w_txtNodeName);
            base.Controls.Add(this.w_lblNodeType);
            base.Controls.Add(this.w_lblAddress);
            base.Controls.Add(this.w_lblNodeName);
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "NodePropertiesDialog";
            base.ShowInTaskbar = false;
            ((System.ComponentModel.ISupportInitialize)this.w_txtNodeName.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.w_txtNodeAddress.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.w_txtNodeType.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.w_tbListTemplate.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.w_txtLocation.Properties).EndInit();
            this.w_plDBReferences.ResumeLayout(false);
            this.w_plDBReferences.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)this.w_tbCustomTemplatePath.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.w_tbLinkName.Properties).EndInit();
            this.w_plLocation.ResumeLayout(false);
            this.w_plLocation.PerformLayout();
            this.w_plListTemplate.ResumeLayout(false);
            this.w_plListTemplate.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)this.w_memoNodeXml.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.w_memoServerInfo.Properties).EndInit();
            this.w_plServerInfo.ResumeLayout(false);
            this.w_plServerInfo.PerformLayout();
            this.w_plConnectionType.ResumeLayout(false);
            this.w_plConnectionType.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)this.w_txtConnectedAs.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.w_txtConnectionType.Properties).EndInit();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void On_CustomTemplatePath_Clicked(object sender, EventArgs e)
        {
            if ((!(m_node is SPNode) || !((m_node as SPNode).Adapter.Reader is IDBReader)) && !((m_node as SPNode).Adapter.Reader is SharePointReader))
            {
                return;
            }
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog
            {
                Description = Metalogix.SharePoint.Properties.Resources.Template_Location_Set,
                ShowNewFolderButton = false
            };
            try
            {
                string sharePointIsapiFolderFromRegistry = Utils.GetSharePointIsapiFolderFromRegistry();
                string text = sharePointIsapiFolderFromRegistry.Trim('\\');
                text = text.Substring(0, text.LastIndexOf("\\"));
                text += "\\TEMPLATE";
                folderBrowserDialog.SelectedPath = text;
            }
            catch
            {
            }
            if (DialogResult.OK != folderBrowserDialog.ShowDialog())
            {
                return;
            }
            string selectedPath = folderBrowserDialog.SelectedPath;
            if (DialogResult.OK == FlatXtraMessageBox.Show(this, Metalogix.SharePoint.Properties.Resources.Template_Location_Change_Warning, Metalogix.SharePoint.Properties.Resources.WarningString, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation))
            {
                if (SetCustomTemplateLocation((SPConnection)m_node, selectedPath))
                {
                    w_tbCustomTemplatePath.Text = selectedPath;
                    Metalogix.Explorer.Settings.SaveActiveConnections();
                    FlatXtraMessageBox.Show(this, string.Format(Metalogix.SharePoint.Properties.Resources.Template_Location_Change_Successful, selectedPath), Metalogix.SharePoint.Properties.Resources.Success, MessageBoxButtons.OK);
                }
                else
                {
                    FlatXtraMessageBox.Show(this, Metalogix.SharePoint.Properties.Resources.Template_Location_Change_Failed_Message, Metalogix.SharePoint.Properties.Resources.Template_Location_Change_Failed_Caption, MessageBoxButtons.OK);
                }
            }
        }

        private void ON_LinkName_Edited(object sender, EventArgs e)
        {
            if ((!(m_node is SPNode) || !((m_node as SPNode).Adapter.Reader is IDBReader)) && !((m_node as SPNode).Adapter.Reader is SharePointReader))
            {
                return;
            }
            HostNameDialog hostNameDialog = new HostNameDialog
            {
                HostName = w_tbLinkName.Text
            };
            hostNameDialog.ShowDialog();
            if (hostNameDialog.DialogResult == DialogResult.Cancel)
            {
                return;
            }
            string hostName = hostNameDialog.HostName;
            if (hostName != null)
            {
                if (SetLinkName((SPConnection)m_node, hostName))
                {
                    w_tbLinkName.Text = hostName;
                    w_txtNodeAddress.Text = m_node.LinkableUrl;
                    Metalogix.Explorer.Settings.SaveActiveConnections();
                    FlatXtraMessageBox.Show(string.Format(Metalogix.SharePoint.Properties.Resources.HostName_Change_Successful, w_tbLinkName.Text), Metalogix.SharePoint.Properties.Resources.Success, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                else
                {
                    FlatXtraMessageBox.Show(Metalogix.SharePoint.Properties.Resources.HostName_Change_Failed_Message, Metalogix.SharePoint.Properties.Resources.HostName_Change_Failed_Caption, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            }
        }

        private void On_TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (sender is TextBox textBox && e.Modifiers == Keys.Control && e.KeyCode == Keys.A)
            {
                textBox.SelectAll();
            }
        }

        public bool SetCustomTemplateLocation(SPConnection conn, string sCustomTemplatePath)
        {
            if (!(conn.Adapter is IDBReader))
            {
                return false;
            }
            bool flag = ((IDBReader)conn.Adapter).SetCustomTemplatePath(sCustomTemplatePath);
            if (flag && conn.ChildrenFetched)
            {
                foreach (SPNode child in conn.Children)
                {
                    if (typeof(SPConnection).IsAssignableFrom(child.GetType()))
                    {
                        SetCustomTemplateLocation((SPConnection)child, sCustomTemplatePath);
                    }
                }
            }
            return flag;
        }

        public bool SetLinkName(SPConnection conn, string sHostName)
        {
            if (!(conn.Adapter is IDBReader))
            {
                return false;
            }
            bool flag = ((IDBReader)conn.Adapter).SetLinkName(sHostName);
            if (flag && conn.ChildrenFetched)
            {
                foreach (SPNode child in conn.Children)
                {
                    if (typeof(SPConnection).IsAssignableFrom(child.GetType()))
                    {
                        SetLinkName((SPConnection)child, sHostName);
                    }
                }
            }
            return flag;
        }
    }
}
