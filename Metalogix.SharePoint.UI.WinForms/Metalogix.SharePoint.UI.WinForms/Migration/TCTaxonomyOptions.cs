using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Metalogix.Actions;
using Metalogix.DataStructures.Generic;
using Metalogix.SharePoint.Actions.Transform;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.Options.Transform;
using Metalogix.SharePoint.Taxonomy;
using Metalogix.SharePoint.UI.WinForms.Properties;
using Metalogix.Transformers;
using Metalogix.Transformers.Interfaces;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Components;
using Metalogix.UI.WinForms.Interfaces;
using TooltipsTest;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	[ControlImage("Metalogix.SharePoint.UI.WinForms.Icons.Migration.TaxonomyOptions.png")]
	[ControlName("Managed Metadata Options")]
	public class TCTaxonomyOptions : MigrationElementsScopabbleTabbableControl, ITransformerTabConfig
	{
		private delegate void SetCheckBoxDelegate(bool value);

		private SPTaxonomyOptions m_options;

		private SiteColumnToManagedMetadata siteColumnToManagedMetadata;

		private FieldToManagedMetadata fieldToManagedMetadata;

		private FieldValueToManagedMetadata fieldValueToManagedMetadata;

		private ContentTypeToManagedMetadata contentTypeToManagedMetadata;

		private ITransformer i_FieldToManagedMetadata;

		private ITransformer i_FieldValueToManagedMetadata;

		private ITransformer i_SiteColumnToManagedMetadata;

		private ITransformer i_ContentTypeToManagedMetadata;

		private SPTermStoreCollection m_SourceTermstores;

		private SPTermStoreCollection m_TargetTermstores;

		private CommonSerializableTable<object, object> m_workingMap;

		private TransformerCollection transformers;

		private Metalogix.Actions.Action m_action;

		private IContainer components;

		private CheckEdit w_cbCopyReferencedManagedMetadata;

		private CheckEdit cbTransformField;

		private SimpleButton btnConfigureTransformerOptions;

		private SimpleButton btnConfigureSiteColumnTransformerOptions;

		private CheckEdit cbTransformSiteColumns;

		private PanelControl _pnlMapTermStores;

		private HelpTipButton w_helpMapTermStores;

		private SimpleButton w_bMapTermStores;

		private CheckEdit w_cbMapTermStores;

		private PanelControl _pnlCopyReferencedMMD;

		private HelpTipButton w_helpCopyReferencedManagedMetadata;

		private PanelControl _pnlTransformSiteColumns;

		private HelpTipButton _helpTransformSiteColumns;

		private PanelControl _pnlTransformLists;

		private HelpTipButton _helpTransformField;

		private PanelControl _pnlResolveByName;

		private HelpTipButton _helpResolveByName;

		private CheckEdit _cbResolveByName;

		public Metalogix.Actions.Action Action
		{
			get
			{
				return m_action;
			}
			set
			{
				m_action = value;
			}
		}

		public SPTaxonomyOptions Options
		{
			get
			{
				return m_options;
			}
			set
			{
				m_options = value;
				LoadUI();
			}
		}

		public TransformerCollection Transformers
		{
			get
			{
				return transformers;
			}
			set
			{
				transformers = value;
				fieldToManagedMetadata = null;
				fieldValueToManagedMetadata = null;
				siteColumnToManagedMetadata = null;
				contentTypeToManagedMetadata = null;
				i_FieldToManagedMetadata = null;
				i_FieldValueToManagedMetadata = null;
				i_SiteColumnToManagedMetadata = null;
				i_ContentTypeToManagedMetadata = null;
				SetupFieldToManagedMetadataTransformers();
			}
		}

		public TaxonomySetupOptions VisOptions { get; set; }

		public TCTaxonomyOptions()
		{
			InitializeComponent();
			cbTransformField.Enabled = false;
			cbTransformField.Checked = false;
			btnConfigureTransformerOptions.Enabled = false;
			cbTransformSiteColumns.Enabled = false;
			cbTransformSiteColumns.Checked = false;
			btnConfigureSiteColumnTransformerOptions.Enabled = false;
			cbTransformSiteColumns.CheckedChanged -= OnTranformSiteColumnsOnCheckedChanged;
			cbTransformSiteColumns.CheckedChanged += OnTranformSiteColumnsOnCheckedChanged;
			cbTransformField.CheckedChanged -= OnTranformFieldOnCheckedChanged;
			cbTransformField.CheckedChanged += OnTranformFieldOnCheckedChanged;
			Type type = GetType();
			w_helpMapTermStores.SetResourceString(type.FullName + w_cbMapTermStores.Name, type);
			w_helpCopyReferencedManagedMetadata.SetResourceString(type.FullName + w_cbCopyReferencedManagedMetadata.Name, type);
			_helpTransformSiteColumns.SetResourceString(type.FullName + cbTransformSiteColumns.Name, type);
			_helpTransformField.SetResourceString(type.FullName + cbTransformField.Name, type);
			_helpResolveByName.SetResourceString(type.FullName + _cbResolveByName.Name, type);
		}

		private void _cbResolveByName_EditValueChanging(object sender, ChangingEventArgs e)
		{
			if (!(bool)e.NewValue && FlatXtraMessageBox.Show(string.Format(Resources.FS_DisablingResolveByNameWarning, _cbResolveByName.Text, Environment.NewLine), Resources.WarningCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
			{
				e.Cancel = true;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private SPTermStoreCollection GetTermStoresFromNode(object node)
		{
			SPTermStoreCollection sPTermStoreCollection = null;
			if (node is SPBaseServer)
			{
				sPTermStoreCollection = (node as SPBaseServer).TermStores;
			}
			else if (node is SPWeb)
			{
				sPTermStoreCollection = (node as SPWeb).TermStores;
			}
			else if (node is SPList)
			{
				if ((node as SPList).ParentWeb != null)
				{
					sPTermStoreCollection = (node as SPList).ParentWeb.TermStores;
				}
			}
			else if (node is SPFolder)
			{
				if ((node as SPFolder).ParentList != null && (node as SPFolder).ParentList.ParentWeb != null)
				{
					sPTermStoreCollection = (node as SPFolder).ParentList.ParentWeb.TermStores;
				}
			}
			else if (node is SPListItem && (node as SPListItem).ParentList != null && (node as SPListItem).ParentList.ParentWeb != null)
			{
				sPTermStoreCollection = (node as SPListItem).ParentList.ParentWeb.TermStores;
			}
			if (sPTermStoreCollection == null)
			{
				throw new ArgumentNullException("Unable to obtain Target Termstores. termStores is null.");
			}
			return sPTermStoreCollection;
		}

		public override void HandleMessage(TabbableControl sender, string sMessage, object oValue)
		{
			if (!string.Equals(sender.Name, "TCTransformation") || !string.Equals(sMessage, "TransformerCollectionItemRemoved"))
			{
				return;
			}
			if (object.Equals(oValue, typeof(FieldToManagedMetadata).ToString()) || object.Equals(oValue, typeof(FieldValueToManagedMetadata).ToString()))
			{
				RemoveFieldListOptions();
				if (i_SiteColumnToManagedMetadata != null)
				{
					RemoveSiteColumnOptions();
				}
			}
			if (object.Equals(oValue, typeof(SiteColumnToManagedMetadata).ToString()) || object.Equals(oValue, typeof(ContentTypeToManagedMetadata).ToString()))
			{
				RemoveSiteColumnOptions();
				RemoveFieldListOptions();
			}
		}

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.TCTaxonomyOptions));
			this.w_cbCopyReferencedManagedMetadata = new DevExpress.XtraEditors.CheckEdit();
			this.cbTransformField = new DevExpress.XtraEditors.CheckEdit();
			this.btnConfigureTransformerOptions = new DevExpress.XtraEditors.SimpleButton();
			this.btnConfigureSiteColumnTransformerOptions = new DevExpress.XtraEditors.SimpleButton();
			this.cbTransformSiteColumns = new DevExpress.XtraEditors.CheckEdit();
			this._pnlMapTermStores = new DevExpress.XtraEditors.PanelControl();
			this.w_helpMapTermStores = new TooltipsTest.HelpTipButton();
			this.w_bMapTermStores = new DevExpress.XtraEditors.SimpleButton();
			this.w_cbMapTermStores = new DevExpress.XtraEditors.CheckEdit();
			this._pnlCopyReferencedMMD = new DevExpress.XtraEditors.PanelControl();
			this.w_helpCopyReferencedManagedMetadata = new TooltipsTest.HelpTipButton();
			this._pnlTransformSiteColumns = new DevExpress.XtraEditors.PanelControl();
			this._helpTransformSiteColumns = new TooltipsTest.HelpTipButton();
			this._pnlTransformLists = new DevExpress.XtraEditors.PanelControl();
			this._helpTransformField = new TooltipsTest.HelpTipButton();
			this._pnlResolveByName = new DevExpress.XtraEditors.PanelControl();
			this._helpResolveByName = new TooltipsTest.HelpTipButton();
			this._cbResolveByName = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)this.w_cbCopyReferencedManagedMetadata.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.cbTransformField.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.cbTransformSiteColumns.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this._pnlMapTermStores).BeginInit();
			this._pnlMapTermStores.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.w_helpMapTermStores).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbMapTermStores.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this._pnlCopyReferencedMMD).BeginInit();
			this._pnlCopyReferencedMMD.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.w_helpCopyReferencedManagedMetadata).BeginInit();
			((System.ComponentModel.ISupportInitialize)this._pnlTransformSiteColumns).BeginInit();
			this._pnlTransformSiteColumns.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this._helpTransformSiteColumns).BeginInit();
			((System.ComponentModel.ISupportInitialize)this._pnlTransformLists).BeginInit();
			this._pnlTransformLists.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this._helpTransformField).BeginInit();
			((System.ComponentModel.ISupportInitialize)this._pnlResolveByName).BeginInit();
			this._pnlResolveByName.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this._helpResolveByName).BeginInit();
			((System.ComponentModel.ISupportInitialize)this._cbResolveByName.Properties).BeginInit();
			base.SuspendLayout();
			this.w_cbCopyReferencedManagedMetadata.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.w_cbCopyReferencedManagedMetadata.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.w_cbCopyReferencedManagedMetadata.Location = new System.Drawing.Point(6, 0);
			this.w_cbCopyReferencedManagedMetadata.Name = "w_cbCopyReferencedManagedMetadata";
			this.w_cbCopyReferencedManagedMetadata.Properties.AutoWidth = true;
			this.w_cbCopyReferencedManagedMetadata.Properties.Caption = "Copy referenced managed metadata";
			this.w_cbCopyReferencedManagedMetadata.Size = new System.Drawing.Size(200, 19);
			this.w_cbCopyReferencedManagedMetadata.TabIndex = 0;
			this.cbTransformField.Location = new System.Drawing.Point(6, 0);
			this.cbTransformField.Name = "cbTransformField";
			this.cbTransformField.Properties.AutoWidth = true;
			this.cbTransformField.Properties.Caption = "Transform specified columns in lists to managed metadata columns";
			this.cbTransformField.Size = new System.Drawing.Size(340, 19);
			this.cbTransformField.TabIndex = 0;
			this.btnConfigureTransformerOptions.Enabled = false;
			this.btnConfigureTransformerOptions.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btnConfigureTransformerOptions.Location = new System.Drawing.Point(360, 0);
			this.btnConfigureTransformerOptions.Name = "btnConfigureTransformerOptions";
			this.btnConfigureTransformerOptions.Size = new System.Drawing.Size(35, 21);
			this.btnConfigureTransformerOptions.TabIndex = 1;
			this.btnConfigureTransformerOptions.Text = "...";
			this.btnConfigureTransformerOptions.Click += new System.EventHandler(OnBtnConfigureTransformerOptionsClick);
			this.btnConfigureSiteColumnTransformerOptions.Enabled = false;
			this.btnConfigureSiteColumnTransformerOptions.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btnConfigureSiteColumnTransformerOptions.Location = new System.Drawing.Point(360, 0);
			this.btnConfigureSiteColumnTransformerOptions.Name = "btnConfigureSiteColumnTransformerOptions";
			this.btnConfigureSiteColumnTransformerOptions.Size = new System.Drawing.Size(35, 21);
			this.btnConfigureSiteColumnTransformerOptions.TabIndex = 1;
			this.btnConfigureSiteColumnTransformerOptions.Text = "...";
			this.btnConfigureSiteColumnTransformerOptions.Click += new System.EventHandler(OnBtnConfigureSiteColumnTransformerOptionsClick);
			this.cbTransformSiteColumns.Location = new System.Drawing.Point(6, 0);
			this.cbTransformSiteColumns.Name = "cbTransformSiteColumns";
			this.cbTransformSiteColumns.Properties.AutoWidth = true;
			this.cbTransformSiteColumns.Properties.Caption = "Transform specified site columns to managed metadata columns";
			this.cbTransformSiteColumns.Size = new System.Drawing.Size(328, 19);
			this.cbTransformSiteColumns.TabIndex = 0;
			this._pnlMapTermStores.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this._pnlMapTermStores.Controls.Add(this.w_helpMapTermStores);
			this._pnlMapTermStores.Controls.Add(this.w_bMapTermStores);
			this._pnlMapTermStores.Controls.Add(this.w_cbMapTermStores);
			this._pnlMapTermStores.Location = new System.Drawing.Point(0, 30);
			this._pnlMapTermStores.Name = "_pnlMapTermStores";
			this._pnlMapTermStores.Size = new System.Drawing.Size(442, 23);
			this._pnlMapTermStores.TabIndex = 1;
			this.w_helpMapTermStores.AnchoringControl = null;
			this.w_helpMapTermStores.BackColor = System.Drawing.Color.Transparent;
			this.w_helpMapTermStores.CommonParentControl = null;
			this.w_helpMapTermStores.Image = (System.Drawing.Image)resources.GetObject("w_helpMapTermStores.Image");
			this.w_helpMapTermStores.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.w_helpMapTermStores.Location = new System.Drawing.Point(310, 0);
			this.w_helpMapTermStores.MaximumSize = new System.Drawing.Size(20, 20);
			this.w_helpMapTermStores.MinimumSize = new System.Drawing.Size(20, 20);
			this.w_helpMapTermStores.Name = "w_helpMapTermStores";
			this.w_helpMapTermStores.RealOffset = null;
			this.w_helpMapTermStores.RelativeOffset = null;
			this.w_helpMapTermStores.Size = new System.Drawing.Size(20, 20);
			this.w_helpMapTermStores.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.w_helpMapTermStores.TabIndex = 2;
			this.w_helpMapTermStores.TabStop = false;
			this.w_bMapTermStores.Enabled = false;
			this.w_bMapTermStores.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.w_bMapTermStores.Location = new System.Drawing.Point(264, 0);
			this.w_bMapTermStores.Name = "w_bMapTermStores";
			this.w_bMapTermStores.Size = new System.Drawing.Size(35, 21);
			this.w_bMapTermStores.TabIndex = 1;
			this.w_bMapTermStores.Text = "...";
			this.w_bMapTermStores.Click += new System.EventHandler(w_bMapTermStores_Click);
			this.w_cbMapTermStores.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.w_cbMapTermStores.Location = new System.Drawing.Point(6, 0);
			this.w_cbMapTermStores.Name = "w_cbMapTermStores";
			this.w_cbMapTermStores.Properties.AutoWidth = true;
			this.w_cbMapTermStores.Properties.Caption = "Map term stores for migration";
			this.w_cbMapTermStores.Size = new System.Drawing.Size(165, 19);
			this.w_cbMapTermStores.TabIndex = 0;
			this.w_cbMapTermStores.CheckedChanged += new System.EventHandler(w_cbMapTermStores_CheckedChanged);
			this._pnlCopyReferencedMMD.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this._pnlCopyReferencedMMD.Controls.Add(this.w_helpCopyReferencedManagedMetadata);
			this._pnlCopyReferencedMMD.Controls.Add(this.w_cbCopyReferencedManagedMetadata);
			this._pnlCopyReferencedMMD.Location = new System.Drawing.Point(0, 56);
			this._pnlCopyReferencedMMD.Name = "_pnlCopyReferencedMMD";
			this._pnlCopyReferencedMMD.Size = new System.Drawing.Size(442, 23);
			this._pnlCopyReferencedMMD.TabIndex = 2;
			this.w_helpCopyReferencedManagedMetadata.AnchoringControl = null;
			this.w_helpCopyReferencedManagedMetadata.BackColor = System.Drawing.Color.Transparent;
			this.w_helpCopyReferencedManagedMetadata.CommonParentControl = null;
			this.w_helpCopyReferencedManagedMetadata.Image = (System.Drawing.Image)resources.GetObject("w_helpCopyReferencedManagedMetadata.Image");
			this.w_helpCopyReferencedManagedMetadata.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.w_helpCopyReferencedManagedMetadata.Location = new System.Drawing.Point(310, 0);
			this.w_helpCopyReferencedManagedMetadata.MaximumSize = new System.Drawing.Size(20, 20);
			this.w_helpCopyReferencedManagedMetadata.MinimumSize = new System.Drawing.Size(20, 20);
			this.w_helpCopyReferencedManagedMetadata.Name = "w_helpCopyReferencedManagedMetadata";
			this.w_helpCopyReferencedManagedMetadata.RealOffset = null;
			this.w_helpCopyReferencedManagedMetadata.RelativeOffset = null;
			this.w_helpCopyReferencedManagedMetadata.Size = new System.Drawing.Size(20, 20);
			this.w_helpCopyReferencedManagedMetadata.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.w_helpCopyReferencedManagedMetadata.TabIndex = 2;
			this.w_helpCopyReferencedManagedMetadata.TabStop = false;
			this._pnlTransformSiteColumns.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this._pnlTransformSiteColumns.Controls.Add(this._helpTransformSiteColumns);
			this._pnlTransformSiteColumns.Controls.Add(this.cbTransformSiteColumns);
			this._pnlTransformSiteColumns.Controls.Add(this.btnConfigureSiteColumnTransformerOptions);
			this._pnlTransformSiteColumns.Location = new System.Drawing.Point(0, 82);
			this._pnlTransformSiteColumns.Name = "_pnlTransformSiteColumns";
			this._pnlTransformSiteColumns.Size = new System.Drawing.Size(442, 23);
			this._pnlTransformSiteColumns.TabIndex = 3;
			this._helpTransformSiteColumns.AnchoringControl = null;
			this._helpTransformSiteColumns.BackColor = System.Drawing.Color.Transparent;
			this._helpTransformSiteColumns.CommonParentControl = null;
			this._helpTransformSiteColumns.Image = (System.Drawing.Image)resources.GetObject("_helpTransformSiteColumns.Image");
			this._helpTransformSiteColumns.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this._helpTransformSiteColumns.Location = new System.Drawing.Point(406, 0);
			this._helpTransformSiteColumns.MaximumSize = new System.Drawing.Size(20, 20);
			this._helpTransformSiteColumns.MinimumSize = new System.Drawing.Size(20, 20);
			this._helpTransformSiteColumns.Name = "_helpTransformSiteColumns";
			this._helpTransformSiteColumns.RealOffset = null;
			this._helpTransformSiteColumns.RelativeOffset = null;
			this._helpTransformSiteColumns.Size = new System.Drawing.Size(20, 20);
			this._helpTransformSiteColumns.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this._helpTransformSiteColumns.TabIndex = 2;
			this._helpTransformSiteColumns.TabStop = false;
			this._pnlTransformLists.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this._pnlTransformLists.Controls.Add(this._helpTransformField);
			this._pnlTransformLists.Controls.Add(this.cbTransformField);
			this._pnlTransformLists.Controls.Add(this.btnConfigureTransformerOptions);
			this._pnlTransformLists.Location = new System.Drawing.Point(0, 108);
			this._pnlTransformLists.Name = "_pnlTransformLists";
			this._pnlTransformLists.Size = new System.Drawing.Size(442, 23);
			this._pnlTransformLists.TabIndex = 4;
			this._helpTransformField.AnchoringControl = null;
			this._helpTransformField.BackColor = System.Drawing.Color.Transparent;
			this._helpTransformField.CommonParentControl = null;
			this._helpTransformField.Image = (System.Drawing.Image)resources.GetObject("_helpTransformField.Image");
			this._helpTransformField.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this._helpTransformField.Location = new System.Drawing.Point(406, 0);
			this._helpTransformField.MaximumSize = new System.Drawing.Size(20, 20);
			this._helpTransformField.MinimumSize = new System.Drawing.Size(20, 20);
			this._helpTransformField.Name = "_helpTransformField";
			this._helpTransformField.RealOffset = null;
			this._helpTransformField.RelativeOffset = null;
			this._helpTransformField.Size = new System.Drawing.Size(20, 20);
			this._helpTransformField.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this._helpTransformField.TabIndex = 2;
			this._helpTransformField.TabStop = false;
			this._pnlResolveByName.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this._pnlResolveByName.Controls.Add(this._helpResolveByName);
			this._pnlResolveByName.Controls.Add(this._cbResolveByName);
			this._pnlResolveByName.Location = new System.Drawing.Point(0, 4);
			this._pnlResolveByName.Name = "_pnlResolveByName";
			this._pnlResolveByName.Size = new System.Drawing.Size(442, 23);
			this._pnlResolveByName.TabIndex = 0;
			this._helpResolveByName.AnchoringControl = null;
			this._helpResolveByName.BackColor = System.Drawing.Color.Transparent;
			this._helpResolveByName.CommonParentControl = null;
			this._helpResolveByName.Image = (System.Drawing.Image)resources.GetObject("_helpResolveByName.Image");
			this._helpResolveByName.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this._helpResolveByName.Location = new System.Drawing.Point(310, 0);
			this._helpResolveByName.MaximumSize = new System.Drawing.Size(20, 20);
			this._helpResolveByName.MinimumSize = new System.Drawing.Size(20, 20);
			this._helpResolveByName.Name = "_helpResolveByName";
			this._helpResolveByName.RealOffset = null;
			this._helpResolveByName.RelativeOffset = null;
			this._helpResolveByName.Size = new System.Drawing.Size(20, 20);
			this._helpResolveByName.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this._helpResolveByName.TabIndex = 2;
			this._helpResolveByName.TabStop = false;
			this._cbResolveByName.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this._cbResolveByName.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this._cbResolveByName.Location = new System.Drawing.Point(6, 0);
			this._cbResolveByName.Name = "_cbResolveByName";
			this._cbResolveByName.Properties.AutoWidth = true;
			this._cbResolveByName.Properties.Caption = "Resolve managed metadata by name and hierarchy";
			this._cbResolveByName.Size = new System.Drawing.Size(270, 19);
			this._cbResolveByName.TabIndex = 0;
			base.Controls.Add(this._pnlResolveByName);
			base.Controls.Add(this._pnlTransformLists);
			base.Controls.Add(this._pnlTransformSiteColumns);
			base.Controls.Add(this._pnlCopyReferencedMMD);
			base.Controls.Add(this._pnlMapTermStores);
			base.Name = "TCTaxonomyOptions";
			base.Size = new System.Drawing.Size(445, 144);
			((System.ComponentModel.ISupportInitialize)this.w_cbCopyReferencedManagedMetadata.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.cbTransformField.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.cbTransformSiteColumns.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this._pnlMapTermStores).EndInit();
			this._pnlMapTermStores.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.w_helpMapTermStores).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbMapTermStores.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this._pnlCopyReferencedMMD).EndInit();
			this._pnlCopyReferencedMMD.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.w_helpCopyReferencedManagedMetadata).EndInit();
			((System.ComponentModel.ISupportInitialize)this._pnlTransformSiteColumns).EndInit();
			this._pnlTransformSiteColumns.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this._helpTransformSiteColumns).EndInit();
			((System.ComponentModel.ISupportInitialize)this._pnlTransformLists).EndInit();
			this._pnlTransformLists.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this._helpTransformField).EndInit();
			((System.ComponentModel.ISupportInitialize)this._pnlResolveByName).EndInit();
			this._pnlResolveByName.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this._helpResolveByName).EndInit();
			((System.ComponentModel.ISupportInitialize)this._cbResolveByName.Properties).EndInit();
			base.ResumeLayout(false);
		}

		protected override void LoadUI()
		{
			base.LoadUI();
			_cbResolveByName.EditValueChanging -= _cbResolveByName_EditValueChanging;
			if (!IsTaxonomySupported())
			{
				m_options.CopyReferencedManagedMetadata = false;
				m_options.MapTermStores = false;
				w_cbCopyReferencedManagedMetadata.Checked = false;
				w_cbMapTermStores.Checked = false;
				_pnlCopyReferencedMMD.Enabled = false;
				_pnlMapTermStores.Enabled = false;
				_pnlResolveByName.Enabled = false;
			}
			else
			{
				w_cbCopyReferencedManagedMetadata.Enabled = base.Scope != SharePointObjectScope.Server;
				w_cbCopyReferencedManagedMetadata.Checked = m_options.CopyReferencedManagedMetadata;
				_pnlCopyReferencedMMD.Enabled = w_cbCopyReferencedManagedMetadata.Enabled;
				w_cbMapTermStores.Enabled = true;
				w_cbMapTermStores.Checked = m_options.MapTermStores;
				_cbResolveByName.Checked = m_options.ResolveManagedMetadataByName;
			}
			if (SourceAndTargetAreTenant() && TermstoresCanBeAutomaticallyMapped())
			{
				w_cbMapTermStores.Checked = true;
				w_cbMapTermStores.Enabled = false;
				w_bMapTermStores.Enabled = false;
				w_helpMapTermStores.Enabled = false;
				CommonSerializableTable<object, object> workingMap = new CommonSerializableTable<object, object> { 
				{
					base.SourceTermstores[0],
					base.TargetTermstores[0]
				} };
				m_workingMap = workingMap;
			}
			_cbResolveByName.EditValueChanging += _cbResolveByName_EditValueChanging;
			cbTransformField.Enabled = base.Scope != 0 && TargetSupportsTaxonomy();
			cbTransformField.Checked = m_options.TransformListAndField;
			_pnlTransformLists.Enabled = cbTransformField.Enabled;
			cbTransformSiteColumns.Enabled = base.Scope != 0 && SourceIsWebAndTargetSupportsTaxonomy();
			cbTransformSiteColumns.Checked = m_options.TransformSiteColumns;
			_pnlTransformSiteColumns.Enabled = cbTransformSiteColumns.Enabled;
			if (VisOptions > TaxonomySetupOptions.None)
			{
				if (!VisOptions.HasFlag(TaxonomySetupOptions.ResolveByName))
				{
					HideControl(_pnlResolveByName);
				}
				if (!VisOptions.HasFlag(TaxonomySetupOptions.MapTermstores))
				{
					HideControl(_pnlMapTermStores);
				}
				if (!VisOptions.HasFlag(TaxonomySetupOptions.ReferencedMMD))
				{
					HideControl(_pnlCopyReferencedMMD);
				}
				if (!VisOptions.HasFlag(TaxonomySetupOptions.TransformList))
				{
					HideControl(_pnlTransformLists);
				}
				if (!VisOptions.HasFlag(TaxonomySetupOptions.TransformSiteColumns))
				{
					HideControl(_pnlTransformSiteColumns);
				}
			}
		}

		private void MapToTermstoreMappingTable()
		{
			try
			{
				if (m_options == null || base.SourceTermstores == null || base.TargetTermstores == null)
				{
					return;
				}
				CommonSerializableTable<object, object> commonSerializableTable = new CommonSerializableTable<object, object>();
				if (m_workingMap != null)
				{
					foreach (object key in m_workingMap.Keys)
					{
						Guid id = ((SPTermStore)key).Id;
						Guid id2 = ((SPTermStore)m_workingMap[key]).Id;
						SPTermStore sPTermStore = base.SourceTermstores[id];
						SPTermStore sPTermStore2 = base.TargetTermstores[id2];
						if (sPTermStore != null && sPTermStore2 != null)
						{
							commonSerializableTable.Add(sPTermStore, sPTermStore2);
						}
					}
				}
				else
				{
					foreach (string key2 in m_options.TermstoreNameMappingTable.Keys)
					{
						string g = m_options.TermstoreNameMappingTable[key2];
						SPTermStore sPTermStore3 = base.SourceTermstores[new Guid(key2)];
						SPTermStore sPTermStore4 = base.TargetTermstores[new Guid(g)];
						if (sPTermStore3 != null && sPTermStore4 != null)
						{
							commonSerializableTable.Add(sPTermStore3, sPTermStore4);
						}
					}
				}
				m_workingMap = commonSerializableTable;
			}
			catch
			{
				m_workingMap = new CommonSerializableTable<object, object>();
			}
		}

		private void OnBtnConfigureSiteColumnTransformerOptionsClick(object sender, EventArgs e)
		{
			if (siteColumnToManagedMetadata.Configure(Action, Context.Sources, Context.Targets))
			{
				if (!cbTransformField.Checked)
				{
					cbTransformField.Checked = true;
				}
				SaveUI();
			}
		}

		private void OnBtnConfigureTransformerOptionsClick(object sender, EventArgs e)
		{
			fieldToManagedMetadata.Configure(Action, Context.Sources, Context.Targets);
		}

		private void OnTranformFieldOnCheckedChanged(object sender, EventArgs e)
		{
			try
			{
				if (transformers == null)
				{
					throw new ArgumentNullException("Transformers");
				}
				if (Context == null)
				{
					throw new ArgumentNullException("Context");
				}
				if (Action == null)
				{
					throw new ArgumentNullException("Action");
				}
			}
			catch (ArgumentNullException ex)
			{
				ArgumentNullException ex2 = ex;
				cbTransformField.CheckedChanged -= OnTranformFieldOnCheckedChanged;
				cbTransformField.Checked = false;
				cbTransformField.CheckedChanged += OnTranformFieldOnCheckedChanged;
				GlobalServices.ErrorHandler.HandleException(Resources.FMMDCConfigDialogTitle, string.Format(Resources.FMMDCNotSupported, ex2.ParamName), ErrorIcon.Warning);
				return;
			}
			if (!cbTransformField.Checked && fieldToManagedMetadata != null && fieldToManagedMetadata.Options.Items.Count > 0 && FlatXtraMessageBox.Show(Resources.FMMDCWarningRemoveMsg, Resources.ConfigurationExistsWarning, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
			{
				cbTransformField.CheckedChanged -= OnTranformFieldOnCheckedChanged;
				cbTransformField.Checked = true;
				cbTransformField.CheckedChanged += OnTranformFieldOnCheckedChanged;
			}
			btnConfigureTransformerOptions.Enabled = cbTransformField.Checked;
		}

		private void OnTranformSiteColumnsOnCheckedChanged(object sender, EventArgs e)
		{
			try
			{
				if (transformers == null)
				{
					throw new ArgumentNullException("Transformers");
				}
				if (Context == null)
				{
					throw new ArgumentNullException("Context");
				}
				if (Action == null)
				{
					throw new ArgumentNullException("Action");
				}
			}
			catch (ArgumentNullException ex)
			{
				ArgumentNullException ex2 = ex;
				cbTransformSiteColumns.CheckedChanged -= OnTranformSiteColumnsOnCheckedChanged;
				cbTransformSiteColumns.Checked = false;
				cbTransformSiteColumns.CheckedChanged += OnTranformSiteColumnsOnCheckedChanged;
				GlobalServices.ErrorHandler.HandleException(Resources.SCMMDCConfigDialogTitle, string.Format(Resources.FMMDCNotSupported, ex2.ParamName), ErrorIcon.Warning);
				return;
			}
			if (!cbTransformSiteColumns.Checked && siteColumnToManagedMetadata != null && siteColumnToManagedMetadata.Options.Items.Count > 0 && FlatXtraMessageBox.Show(Resources.FMMDCWarningRemoveMsg, Resources.ConfigurationExistsWarning, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
			{
				cbTransformSiteColumns.CheckedChanged -= OnTranformSiteColumnsOnCheckedChanged;
				cbTransformSiteColumns.Checked = true;
				cbTransformSiteColumns.CheckedChanged += OnTranformSiteColumnsOnCheckedChanged;
			}
			btnConfigureSiteColumnTransformerOptions.Enabled = cbTransformSiteColumns.Checked;
		}

		private void RemoveFieldListOptions()
		{
			SetTransformListFieldCheckBoxSilent(value: false);
			btnConfigureTransformerOptions.Enabled = false;
			i_FieldToManagedMetadata = null;
			i_FieldValueToManagedMetadata = null;
			fieldToManagedMetadata = null;
			fieldValueToManagedMetadata = null;
		}

		private void RemoveSiteColumnOptions()
		{
			SetTransformSiteColumnCheckBoxSilent(value: false);
			btnConfigureSiteColumnTransformerOptions.Enabled = false;
			i_SiteColumnToManagedMetadata = null;
			siteColumnToManagedMetadata = null;
			i_ContentTypeToManagedMetadata = null;
			contentTypeToManagedMetadata = null;
		}

		public override bool SaveUI()
		{
			bool flag = false;
			m_options.MapTermStores = w_cbMapTermStores.Checked;
			m_options.ResolveManagedMetadataByName = _cbResolveByName.Checked;
			if (m_workingMap == null)
			{
				MapToTermstoreMappingTable();
			}
			else if (!m_options.MapTermStores)
			{
				m_workingMap.Clear();
			}
			m_options.TermstoreNameMappingTable.Clear();
			if (m_workingMap != null && m_workingMap.Count > 0)
			{
				foreach (SPTermStore key in m_workingMap.Keys)
				{
					SPTermStore sPTermStore2 = (SPTermStore)m_workingMap[key];
					if (!m_options.TermstoreNameMappingTable.ContainsKey(key.Id.ToString()))
					{
						m_options.TermstoreNameMappingTable.Add(key.Id.ToString(), sPTermStore2.Id.ToString());
					}
					else
					{
						m_options.TermstoreNameMappingTable[key.Id.ToString()] = sPTermStore2.Id.ToString();
					}
				}
			}
			m_options.CopyReferencedManagedMetadata = w_cbCopyReferencedManagedMetadata.Checked;
			if (transformers == null)
			{
				return true;
			}
			m_options.TransformListAndField = cbTransformField.Checked;
			m_options.TransformSiteColumns = cbTransformSiteColumns.Checked;
			if (m_options.TransformSiteColumns)
			{
				if (siteColumnToManagedMetadata == null)
				{
					siteColumnToManagedMetadata = new SiteColumnToManagedMetadata();
				}
				if (transformers.Find(typeof(SiteColumnToManagedMetadata)) == null)
				{
					flag = true;
					siteColumnToManagedMetadata.ReadOnly = true;
					transformers.Add(siteColumnToManagedMetadata);
				}
				if (contentTypeToManagedMetadata == null)
				{
					contentTypeToManagedMetadata = new ContentTypeToManagedMetadata();
				}
				if (transformers.Find(typeof(ContentTypeToManagedMetadata)) == null)
				{
					flag = true;
					contentTypeToManagedMetadata.ReadOnly = true;
					transformers.Add(contentTypeToManagedMetadata);
				}
				if (fieldToManagedMetadata == null)
				{
					fieldToManagedMetadata = new FieldToManagedMetadata();
				}
				if (transformers.Find(typeof(FieldToManagedMetadata)) == null)
				{
					flag = true;
					fieldToManagedMetadata.ReadOnly = true;
					transformers.Add(fieldToManagedMetadata);
				}
				if (fieldValueToManagedMetadata == null)
				{
					fieldValueToManagedMetadata = new FieldValueToManagedMetadata();
				}
				if (transformers.Find(fieldValueToManagedMetadata.Name) == null)
				{
					flag = true;
					fieldValueToManagedMetadata.ReadOnly = true;
					transformers.Add(fieldValueToManagedMetadata);
				}
			}
			else
			{
				if (siteColumnToManagedMetadata == null)
				{
					siteColumnToManagedMetadata = (SiteColumnToManagedMetadata)transformers.Find(typeof(SiteColumnToManagedMetadata));
				}
				if (siteColumnToManagedMetadata != null && transformers.Remove(siteColumnToManagedMetadata))
				{
					flag = true;
				}
				if (contentTypeToManagedMetadata == null)
				{
					contentTypeToManagedMetadata = (ContentTypeToManagedMetadata)transformers.Find(typeof(ContentTypeToManagedMetadata));
				}
				if (contentTypeToManagedMetadata != null && transformers.Remove(contentTypeToManagedMetadata))
				{
					flag = true;
				}
				siteColumnToManagedMetadata = null;
				contentTypeToManagedMetadata = null;
			}
			if (!m_options.TransformSiteColumns)
			{
				if (m_options.TransformListAndField)
				{
					if (fieldToManagedMetadata == null)
					{
						fieldToManagedMetadata = new FieldToManagedMetadata();
					}
					if (transformers.Find(typeof(FieldToManagedMetadata)) == null)
					{
						flag = true;
						transformers.Add(fieldToManagedMetadata);
					}
					if (fieldValueToManagedMetadata == null)
					{
						fieldValueToManagedMetadata = new FieldValueToManagedMetadata();
					}
					if (transformers.Find(fieldValueToManagedMetadata.Name) == null)
					{
						flag = true;
						transformers.Add(fieldValueToManagedMetadata);
					}
				}
				else
				{
					if (fieldToManagedMetadata == null)
					{
						fieldToManagedMetadata = (FieldToManagedMetadata)transformers.Find(typeof(FieldToManagedMetadata));
					}
					if (fieldToManagedMetadata != null && transformers.Remove(fieldToManagedMetadata))
					{
						flag = true;
					}
					if (fieldValueToManagedMetadata == null)
					{
						fieldValueToManagedMetadata = (FieldValueToManagedMetadata)transformers.Find(typeof(FieldValueToManagedMetadata));
					}
					if (fieldValueToManagedMetadata != null && transformers.Remove(fieldValueToManagedMetadata))
					{
						flag = true;
					}
					fieldToManagedMetadata = null;
					fieldValueToManagedMetadata = null;
				}
			}
			if (fieldToManagedMetadata != null)
			{
				List<FieldToManagedMetadataOption> list = new List<FieldToManagedMetadataOption>();
				foreach (FieldToManagedMetadataOption item2 in fieldToManagedMetadata.Options.Items)
				{
					if (item2.ListFilterExpression == null)
					{
						list.Add(item2);
					}
				}
				foreach (FieldToManagedMetadataOption item3 in list)
				{
					fieldToManagedMetadata.Options.Items.Remove(item3);
				}
				list.Clear();
			}
			if (siteColumnToManagedMetadata != null && fieldToManagedMetadata != null && fieldValueToManagedMetadata != null)
			{
				foreach (SiteColumnToManagedMetadataOption item4 in siteColumnToManagedMetadata.Options.Items)
				{
					FieldToManagedMetadataOption fieldToManagedMetadataOption2 = new FieldToManagedMetadataOption();
					fieldToManagedMetadataOption2.SetFromOptions(item4);
					fieldToManagedMetadataOption2.ListFieldFilterExpression = item4.SiteColumnFilterExpression.Clone();
					fieldToManagedMetadata.Options.Items.Insert(0, fieldToManagedMetadataOption2);
				}
			}
			if (fieldToManagedMetadata != null && fieldValueToManagedMetadata != null)
			{
				fieldValueToManagedMetadata.Options.Items.ClearCollection();
				fieldValueToManagedMetadata.Options.Items.AddRangeToCollection(fieldToManagedMetadata.Options.Items.ToArray());
			}
			if (flag)
			{
				SendMessage("TransformerCollectionChanged", null);
			}
			return true;
		}

		private void SetTransformListFieldCheckBoxSilent(bool value)
		{
			if (base.InvokeRequired)
			{
				SetCheckBoxDelegate method = SetTransformListFieldCheckBoxSilent;
				object[] args = new object[1] { value };
				Invoke(method, args);
			}
			else
			{
				cbTransformField.CheckedChanged -= OnTranformFieldOnCheckedChanged;
				cbTransformField.Checked = value;
				cbTransformField.CheckedChanged += OnTranformFieldOnCheckedChanged;
			}
		}

		private void SetTransformSiteColumnCheckBoxSilent(bool value)
		{
			if (base.InvokeRequired)
			{
				SetCheckBoxDelegate method = SetTransformSiteColumnCheckBoxSilent;
				object[] args = new object[1] { value };
				Invoke(method, args);
			}
			else
			{
				cbTransformSiteColumns.CheckedChanged -= OnTranformSiteColumnsOnCheckedChanged;
				cbTransformSiteColumns.Checked = value;
				cbTransformSiteColumns.CheckedChanged += OnTranformSiteColumnsOnCheckedChanged;
			}
		}

		private void SetupFieldToManagedMetadataTransformers()
		{
			if (transformers == null)
			{
				cbTransformField.Enabled = false;
				cbTransformSiteColumns.Enabled = false;
				return;
			}
			if (i_FieldToManagedMetadata == null)
			{
				i_FieldToManagedMetadata = transformers.Find(typeof(FieldToManagedMetadata));
			}
			if (i_FieldValueToManagedMetadata == null)
			{
				i_FieldValueToManagedMetadata = transformers.Find(typeof(FieldValueToManagedMetadata));
			}
			if (i_SiteColumnToManagedMetadata == null)
			{
				i_SiteColumnToManagedMetadata = transformers.Find(typeof(SiteColumnToManagedMetadata));
			}
			if (i_ContentTypeToManagedMetadata == null)
			{
				i_ContentTypeToManagedMetadata = transformers.Find(typeof(ContentTypeToManagedMetadata));
			}
			if (i_FieldToManagedMetadata != null)
			{
				cbTransformField.CheckedChanged -= OnTranformFieldOnCheckedChanged;
				cbTransformField.Checked = true;
				cbTransformField.CheckedChanged += OnTranformFieldOnCheckedChanged;
				btnConfigureTransformerOptions.Enabled = true;
			}
			else
			{
				i_FieldToManagedMetadata = new FieldToManagedMetadata();
			}
			fieldToManagedMetadata = (FieldToManagedMetadata)i_FieldToManagedMetadata;
			if (i_FieldValueToManagedMetadata == null)
			{
				i_FieldValueToManagedMetadata = new FieldValueToManagedMetadata();
			}
			fieldValueToManagedMetadata = (FieldValueToManagedMetadata)i_FieldValueToManagedMetadata;
			if (i_SiteColumnToManagedMetadata != null)
			{
				cbTransformSiteColumns.CheckedChanged -= OnTranformSiteColumnsOnCheckedChanged;
				cbTransformSiteColumns.Checked = true;
				cbTransformSiteColumns.CheckedChanged += OnTranformSiteColumnsOnCheckedChanged;
				btnConfigureSiteColumnTransformerOptions.Enabled = true;
			}
			else
			{
				i_SiteColumnToManagedMetadata = new SiteColumnToManagedMetadata();
			}
			siteColumnToManagedMetadata = (SiteColumnToManagedMetadata)i_SiteColumnToManagedMetadata;
			if (i_ContentTypeToManagedMetadata == null)
			{
				i_ContentTypeToManagedMetadata = new ContentTypeToManagedMetadata();
			}
			contentTypeToManagedMetadata = (ContentTypeToManagedMetadata)i_ContentTypeToManagedMetadata;
		}

		private bool SourceIsWebAndTargetSupportsTaxonomy()
		{
			SPNode sPNode = null;
			SPNode sPNode2 = null;
			if (SourceNodes == null || TargetNodes == null)
			{
				return false;
			}
			sPNode = SourceNodes[0] as SPWeb;
			sPNode2 = TargetNodes[0] as SPNode;
			if (sPNode == null || sPNode2 == null)
			{
				return false;
			}
			if (!sPNode2.Adapter.SharePointVersion.IsSharePoint2010OrLater)
			{
				return false;
			}
			return sPNode2.Adapter.SupportsTaxonomy;
		}

		private bool TargetSupportsTaxonomy()
		{
			SPNode sPNode = null;
			if (TargetNodes == null)
			{
				return false;
			}
			if (!(TargetNodes[0] is SPNode sPNode2))
			{
				return false;
			}
			if (!sPNode2.Adapter.SharePointVersion.IsSharePoint2010OrLater)
			{
				return false;
			}
			return sPNode2.Adapter.SupportsTaxonomy;
		}

		private void w_bMapTermStores_Click(object sender, EventArgs e)
		{
			using (SerializableTermstoreMapper serializableTermstoreMapper = new SerializableTermstoreMapper())
			{
				MapToTermstoreMappingTable();
				serializableTermstoreMapper.Mappings = (SerializableTable<object, object>)m_workingMap.Clone();
				serializableTermstoreMapper.SourceTermstoreCollection = base.SourceTermstores;
				serializableTermstoreMapper.TargetTermstoreCollection = base.TargetTermstores;
				serializableTermstoreMapper.ShowDialog();
				if (serializableTermstoreMapper.DialogResult == DialogResult.OK)
				{
					m_workingMap = (CommonSerializableTable<object, object>)serializableTermstoreMapper.Mappings;
				}
			}
		}

		private void w_cbMapTermStores_CheckedChanged(object sender, EventArgs e)
		{
			w_bMapTermStores.Enabled = w_cbMapTermStores.Checked;
		}
	}
}
