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
    public class FieldToManagedMetadataConfigDialog : GenericSiteLevelMappingDialog
    {
        private FieldToManagedMetadataOptionCollection m_OriginalOptions;

        private FieldToManagedMetadataOptionCollection m_WorkingOptionsSet;

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

        public FieldToManagedMetadataOptionCollection OriginalOptions
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

        public FieldToManagedMetadataOptionCollection WorkingOptionsSet
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

        public FieldToManagedMetadataConfigDialog(NodeCollection nodeCollection)
            : base(nodeCollection)
        {
            base.ExplorerSelectableTypes = new List<Type>(new Type[1] { typeof(SPList) });
            base.DialogTitle = Resources.FMMDCConfigDialogTitle;
            base.ApplyOptionText = Resources.FMMDCConfigDialogApplyOption;
            base.CreateRuleText = Resources.FMMDCConfigDialogCreateRule;
            base.AllowRuleModification = false;
            base.ConfirmRemoval = true;
            base.OptionsLabelText = "Configurations";
            SetupListViewColumns(typeof(FieldToManagedMetadataOption));
            UpdateUI();
            SetDialogText();
            _modified = false;
        }

        protected override void CheckValidNode(ExplorerTreeNode treeNode, ref bool bEnabledCreateNodeOption, ref bool bEnabledCreateListRuleOption)
        {
            bEnabledCreateNodeOption = false;
            bEnabledCreateListRuleOption = false;
            if (treeNode == null)
            {
                return;
            }
            if (treeNode.Node is SPList)
            {
                bEnabledCreateNodeOption = treeNode.Node is SPList;
                return;
            }
            treeNode.UpdateChildrenUI();
            if (treeNode.Node is SPList)
            {
                w_ecwlExplorer.SelectNode(treeNode);
            }
            foreach (ExplorerTreeNode node in treeNode.Nodes)
            {
                if (!(node.Node is SPList))
                {
                    continue;
                }
                w_ecwlExplorer.SelectNode(node);
                bEnabledCreateNodeOption = true;
                break;
            }
        }

        protected override void CreateListRule()
        {
        }

        protected override void CreateNewApplyOptions(ExplorerTreeNode treeNode)
        {
            if (!(treeNode.Node is SPList sPList))
            {
                return;
            }
            Cursor = Cursors.WaitCursor;
            FieldToManagedMetadataOption fieldToManagedMetadataOption = new FieldToManagedMetadataOption();
            fieldToManagedMetadataOption.ListFilterExpression = new FilterExpression(FilterOperand.Equals, typeof(SPList), "DisplayUrl", sPList.DisplayUrl, bIsCaseSensitive: false, bIsBaseFilter: false);
            FieldToManagedMetadataOption option = fieldToManagedMetadataOption;
            DialogResult dialogResult;
            using (FieldToManagedMetadataConfigItemDialog fieldToManagedMetadataConfigItemDialog = new FieldToManagedMetadataConfigItemDialog(isNewItem: true, Context, option, readOnly: false))
            {
                dialogResult = fieldToManagedMetadataConfigItemDialog.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    WorkingOptionsSet.Items.Add(fieldToManagedMetadataConfigItemDialog.Option.Copy());
                    _modified = true;
                    fieldToManagedMetadataConfigItemDialog.Option = null;
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
            using (FieldToManagedMetadataConfigItemDialog fieldToManagedMetadataConfigItemDialog = new FieldToManagedMetadataConfigItemDialog(isNewItem: true, Context, new FieldToManagedMetadataOption(), readOnly: false))
            {
                dialogResult = fieldToManagedMetadataConfigItemDialog.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    WorkingOptionsSet.Items.Add(fieldToManagedMetadataConfigItemDialog.Option.Copy());
                    _modified = true;
                    fieldToManagedMetadataConfigItemDialog.Option = null;
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
            bool flag = ValidToEdit(option, showDialog: false, removing: false);
            Cursor = Cursors.WaitCursor;
            int index = base.MappingList.IndexOf(option);
            DialogResult dialogResult;
            using (FieldToManagedMetadataConfigItemDialog fieldToManagedMetadataConfigItemDialog = new FieldToManagedMetadataConfigItemDialog(isNewItem: false, Context, WorkingOptionsSet.Items[index] as FieldToManagedMetadataOption, !flag))
            {
                dialogResult = fieldToManagedMetadataConfigItemDialog.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    _modified = true;
                    fieldToManagedMetadataConfigItemDialog.Option = null;
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
            base.Name = "FieldToManagedMetadataConfigDialog";
            this.Text = "FieldToManagedMetadataConfigDialog";
            base.ResumeLayout(false);
        }

        protected override void LoadOptionsUI()
        {
            foreach (FieldToManagedMetadataOption item in WorkingOptionsSet.Items)
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
            if (ValidToEdit(option, showDialog: true, removing: true))
            {
                int num = base.MappingList.IndexOf(option);
                if (num != -1)
                {
                    _modified = true;
                    WorkingOptionsSet.Items.RemoveAt(num);
                }
                base.RemoveOption(option);
            }
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

        private bool ValidToEdit(MappingOption option, bool showDialog, bool removing)
        {
            bool result = true;
            int index = base.MappingList.IndexOf(option);
            if (WorkingOptionsSet.Items[index] is FieldToManagedMetadataOption fieldToManagedMetadataOption && fieldToManagedMetadataOption.ListFilterExpression == null)
            {
                result = false;
                if (showDialog)
                {
                    FlatXtraMessageBox.Show(string.Format(Resources.FS_UnableToEditConfiguration, Resources.SiteColumnToMMDTransformerName, Environment.NewLine, removing ? Resources.FMMDCRemoving : Resources.FMMDCEditing), Resources.FMMDCConfigDialogTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            return result;
        }
    }
}
