using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Metalogix.Data.Filters;
using Metalogix.Explorer;
using Metalogix.SharePoint.Options.Transform;
using Metalogix.SharePoint.UI.WinForms.Migration.Mapping;
using Metalogix.SharePoint.UI.WinForms.Properties;
using Metalogix.Transformers;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Explorer;

namespace Metalogix.SharePoint.UI.WinForms.Transform
{
    public class SiteColumnToManagedMetadataConfigDialog : GenericSiteLevelMappingDialog
    {
        private SiteColumnToManagedMetadataOptionCollection m_OriginalOptions;

        private SiteColumnToManagedMetadataOptionCollection m_WorkingOptionsSet;

        private TransformerConfigContext m_Context;

        private SPWeb m_TargetWeb;

        private bool _modified;

        private IContainer components;

        public TransformerConfigContext Context
        {
            get
            {
                return m_Context;
            }
            set
            {
                m_Context = value;
            }
        }

        public SiteColumnToManagedMetadataOptionCollection OriginalOptions
        {
            get
            {
                return m_OriginalOptions;
            }
            set
            {
                m_OriginalOptions = value;
            }
        }

        private SPWeb TargetWeb
        {
            get
            {
                if (m_TargetWeb == null)
                {
                    m_TargetWeb = Context.ActionContext.Targets[0] as SPWeb;
                }
                return m_TargetWeb;
            }
        }

        protected override IUrlParser UrlParser
        {
            get
            {
                IUrlParser urlParser = _urlParser;
                if (urlParser == null)
                {
                    urlParser = (_urlParser = new PlainTextParser());
                }
                return urlParser;
            }
        }

        public SiteColumnToManagedMetadataOptionCollection WorkingOptionsSet
        {
            get
            {
                return m_WorkingOptionsSet;
            }
            set
            {
                m_WorkingOptionsSet = value;
                LoadOptionsUI();
            }
        }

        public SiteColumnToManagedMetadataConfigDialog(NodeCollection nodeCollection)
            : base(nodeCollection)
        {
            Type[] collection = new Type[2]
            {
                typeof(SPWeb),
                typeof(SPSite)
            };
            base.ExplorerSelectableTypes = new List<Type>(collection);
            base.DialogTitle = Resources.SCMMDCConfigDialogTitle;
            base.ApplyOptionText = Resources.SCMMDCConfigDialogApplyOption;
            base.CreateRuleText = Resources.FMMDCConfigDialogCreateRule;
            base.AllowRuleModification = false;
            base.ConfirmRemoval = true;
            base.OptionsLabelText = "Configurations";
            SetupListViewColumns(typeof(SiteColumnToManagedMetadataOption));
            UpdateUI();
            SetDialogText();
            _modified = false;
        }

        protected override void CheckValidNode(ExplorerTreeNode treeNode, ref bool bEnabledCreateNodeOption, ref bool bEnabledCreateListRuleOption)
        {
            bEnabledCreateNodeOption = false;
            bEnabledCreateListRuleOption = false;
            if (treeNode != null && (treeNode.Node is SPWeb || treeNode.Node is SPSite))
            {
                treeNode.UpdateChildrenUI();
                bEnabledCreateNodeOption = treeNode.Node is SPWeb || treeNode.Node is SPSite;
            }
        }

        protected override void CreateListRule()
        {
        }

        protected override void CreateNewApplyOptions(ExplorerTreeNode treeNode)
        {
            if (!(treeNode.Node is SPWeb sPWeb))
            {
                return;
            }
            Cursor = Cursors.WaitCursor;
            SiteColumnToManagedMetadataOption siteColumnToManagedMetadataOption = new SiteColumnToManagedMetadataOption();
            siteColumnToManagedMetadataOption.SiteFilterExpression = new FilterExpression(FilterOperand.Equals, typeof(SPWeb), "DisplayUrl", sPWeb.DisplayUrl, bIsCaseSensitive: false, bIsBaseFilter: false);
            SiteColumnToManagedMetadataOption option = siteColumnToManagedMetadataOption;
            DialogResult dialogResult;
            using (SiteColumnToManagedMetadataConfigItemDialog siteColumnToManagedMetadataConfigItemDialog = new SiteColumnToManagedMetadataConfigItemDialog(isNewItem: true, Context, option))
            {
                dialogResult = siteColumnToManagedMetadataConfigItemDialog.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    WorkingOptionsSet.Items.Add(siteColumnToManagedMetadataConfigItemDialog.Option.Copy());
                    _modified = true;
                    siteColumnToManagedMetadataConfigItemDialog.Option = null;
                }
            }
            if (dialogResult == DialogResult.OK)
            {
                MappingOption option2 = new MappingOption
                {
                    SubObject = WorkingOptionsSet.Items[WorkingOptionsSet.Items.Count - 1]
                };
                AddOption(option2);
                UpdateOptionsColumnWidths();
            }
            Cursor = Cursors.Default;
        }

        protected override void CreateRule()
        {
            Cursor = Cursors.WaitCursor;
            DialogResult dialogResult;
            using (SiteColumnToManagedMetadataConfigItemDialog siteColumnToManagedMetadataConfigItemDialog = new SiteColumnToManagedMetadataConfigItemDialog(isNewItem: true, Context, new SiteColumnToManagedMetadataOption()))
            {
                dialogResult = siteColumnToManagedMetadataConfigItemDialog.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    WorkingOptionsSet.Items.Add(siteColumnToManagedMetadataConfigItemDialog.Option.Copy());
                    _modified = true;
                    siteColumnToManagedMetadataConfigItemDialog.Option = null;
                }
            }
            if (dialogResult == DialogResult.OK)
            {
                MappingOption option = new MappingOption
                {
                    SubObject = WorkingOptionsSet.Items[WorkingOptionsSet.Items.Count - 1]
                };
                AddOption(option);
                UpdateOptionsColumnWidths();
            }
            Cursor = Cursors.Default;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        protected override void EditOptionAppliedValue(MappingOption option)
        {
            Cursor = Cursors.WaitCursor;
            int index = base.MappingList.IndexOf(option);
            DialogResult dialogResult;
            using (SiteColumnToManagedMetadataConfigItemDialog siteColumnToManagedMetadataConfigItemDialog = new SiteColumnToManagedMetadataConfigItemDialog(isNewItem: false, Context, WorkingOptionsSet.Items[index] as SiteColumnToManagedMetadataOption))
            {
                dialogResult = siteColumnToManagedMetadataConfigItemDialog.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    _modified = true;
                    siteColumnToManagedMetadataConfigItemDialog.Option = null;
                }
            }
            if (dialogResult == DialogResult.OK)
            {
                option.SubObject = WorkingOptionsSet.Items[index];
                UpdateItemInUI(option);
                UpdateOptionsColumnWidths();
            }
            Cursor = Cursors.Default;
        }

        protected override void EditRule(MappingOption option)
        {
        }

        protected override void FormClosingConfirmation(ref FormClosingEventArgs e)
        {
            if (_modified)
            {
                e.Cancel = FlatXtraMessageBox.Show(Resources.FMMDCConfirmChangesMsg, Resources.FMMDCConfirmChangesCap, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No;
            }
        }

        private new void InitializeComponent()
        {
            base.SuspendLayout();
            base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.ClientSize = new System.Drawing.Size(532, 434);
            base.Name = "SiteColumnToManagedMetadataConfigDialog";
            this.Text = "SiteToManagedMetadataConfigDialog";
            base.ResumeLayout(false);
        }

        protected override void LoadOptionsUI()
        {
            foreach (SiteColumnToManagedMetadataOption item in WorkingOptionsSet.Items)
            {
                AddOption(new MappingOption
                {
                    SubObject = item
                });
            }
            UpdateOptionsColumnWidths();
            w_ecwlExplorer.ExpandRoot();
        }

        protected override void RemoveOption(MappingOption option)
        {
            int num = base.MappingList.IndexOf(option);
            if (num != -1)
            {
                _modified = true;
                WorkingOptionsSet.Items.RemoveAt(num);
            }
            base.RemoveOption(option);
        }

        protected override void SaveUI()
        {
            if (_modified)
            {
                _modified = false;
                m_OriginalOptions.Items.ClearCollection();
                m_OriginalOptions.FromXML(m_WorkingOptionsSet.ToXML());
            }
        }
    }
}
