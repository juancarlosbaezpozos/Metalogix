using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Metalogix.Data.Filters;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.Properties;
using Metalogix.SharePoint.UI.WinForms.Migration;
using Metalogix.SharePoint.UI.WinForms.Migration.Mapping;
using Metalogix.UI.WinForms.Data.Filters;
using Metalogix.UI.WinForms.Explorer;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
    public class ContentTypeApplicationOptionsSiteLevelConfigDialog : GenericSiteLevelMappingDialog
    {
        private IEnumerable<SPContentType> m_targetContentTypes;

        private SerializableList<ContentTypeApplicationOptionsCollection> m_ListOptions;

        private IContainer components;

        public SerializableList<ContentTypeApplicationOptionsCollection> Options
        {
            get
            {
                return m_ListOptions;
            }
            set
            {
                m_ListOptions = value;
                LoadOptionsUI();
            }
        }

        public IEnumerable<SPContentType> TargetContentTypes
        {
            get
            {
                return m_targetContentTypes;
            }
            set
            {
                m_targetContentTypes = value;
            }
        }

        protected override IUrlParser UrlParser
        {
            get
            {
                IUrlParser urlParser = _urlParser;
                if (urlParser == null)
                {
                    urlParser = (_urlParser = new ContentTypeApplicationOptionsCollectionFilterParser());
                }
                return urlParser;
            }
        }

        public ContentTypeApplicationOptionsSiteLevelConfigDialog(NodeCollection nodeCollection)
            : base(nodeCollection)
        {
            base.ExplorerSelectableTypes = new List<Type>(new Type[1] { typeof(SPList) });
            base.DialogTitle = Resources.CTypeDialogTitle;
            base.ApplyOptionText = Resources.CTypeDialogApplyOption;
            base.CreateRuleText = Resources.CTypeDialogCreateRule;
            base.OptionsLabelText = Resources.CTypeDialogOptionsLabel;
            base.OptionColumnText = Resources.CTypeDialogOptionsColumn;
            base.OptionAppliedValuesText = Resources.CTypeDialogOptionsValue;
            base.EditOptionText = Resources.CTypeDialogEditOption;
            base.RemoveOptionText = Resources.CTypeDialogRemoveOption;
            UpdateUI();
            SetDialogText();
        }

        protected override void CreateNewApplyOptions(ExplorerTreeNode treeNode)
        {
            if (!(treeNode.Node is SPList sPList))
            {
                return;
            }
            foreach (MappingOption mapping in base.MappingList)
            {
                if (!((ContentTypeApplicationOptionsCollection)mapping.SubObject).AppliesTo(sPList))
                {
                    continue;
                }
                EditOptionAppliedValue(mapping);
                return;
            }
            ContentTypeApplicationOptionsCollection contentTypeApplicationOptionsCollection = new ContentTypeApplicationOptionsCollection();
            contentTypeApplicationOptionsCollection.AppliesToFilter = new FilterExpression(FilterOperand.Equals, typeof(SPList), "DisplayUrl", sPList.DisplayUrl, bIsCaseSensitive: false, bIsBaseFilter: false);
            ContentTypeApplicationOptionsCollection contentTypeApplicationOptionsCollection2 = contentTypeApplicationOptionsCollection;
            ContentTypeApplicationOptionsConfigDialog contentTypeApplicationOptionsConfigDialog = new ContentTypeApplicationOptionsConfigDialog(sPList, m_targetContentTypes)
            {
                Options = contentTypeApplicationOptionsCollection2
            };
            if (contentTypeApplicationOptionsConfigDialog.ShowDialog() == DialogResult.OK)
            {
                AddOption(GetMappingOption(contentTypeApplicationOptionsCollection2));
                UpdateOptionsColumnWidths();
            }
        }

        protected override void CreateRule()
        {
            FilterExpressionEditorDialog filterExpressionEditorDialog = new FilterExpressionEditorDialog();
            filterExpressionEditorDialog.Title = "List Filter Conditions";
            filterExpressionEditorDialog.FilterableTypes = new FilterBuilderType(new List<Type> { typeof(SPList) }, bAllowFreeFormEntry: false);
            FilterExpressionEditorDialog filterExpressionEditorDialog2 = filterExpressionEditorDialog;
            filterExpressionEditorDialog2.ShowDialog();
            if (filterExpressionEditorDialog2.DialogResult == DialogResult.OK)
            {
                ContentTypeApplicationOptionsCollection options = new ContentTypeApplicationOptionsCollection();
                ContentTypeApplicationOptionsConfigDialog contentTypeApplicationOptionsConfigDialog = new ContentTypeApplicationOptionsConfigDialog(m_targetContentTypes)
                {
                    Options = options,
                    HideColumnMappingOption = true
                };
                MappingOption mappingOption = new MappingOption
                {
                    Option = filterExpressionEditorDialog2.FilterExpression.GetLogicString()
                };
                if (contentTypeApplicationOptionsConfigDialog.ShowDialog() == DialogResult.OK)
                {
                    ListViewItem.ListViewSubItem listViewSubItem = new ListViewItem.ListViewSubItem();
                    options = contentTypeApplicationOptionsConfigDialog.Options;
                    options.AppliesToFilter = filterExpressionEditorDialog2.FilterExpression;
                    mappingOption.SubObject = options;
                    mappingOption.OptionAppliedValue = GetOptionsCollectionDisplayString(contentTypeApplicationOptionsConfigDialog.Options);
                    AddOption(mappingOption);
                }
                UpdateOptionsColumnWidths();
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

        protected override void EditOptionAppliedValue(MappingOption option)
        {
            ContentTypeApplicationOptionsCollection contentTypeApplicationOptionsCollection = (ContentTypeApplicationOptionsCollection)option.SubObject;
            Node node = null;
            if (contentTypeApplicationOptionsCollection.AppliesToFilter is FilterExpression filterExpression && filterExpression.Property.Equals("DisplayUrl") && filterExpression.Operand == FilterOperand.Equals)
            {
                node = base.NodeCollection.GetNodeByUrl(filterExpression.Pattern);
            }
            ContentTypeApplicationOptionsConfigDialog contentTypeApplicationOptionsConfigDialog = null;
            contentTypeApplicationOptionsConfigDialog = ((node == null) ? new ContentTypeApplicationOptionsConfigDialog(m_targetContentTypes)
            {
                Options = (ContentTypeApplicationOptionsCollection)option.SubObject,
                HideColumnMappingOption = true
            } : new ContentTypeApplicationOptionsConfigDialog((SPList)node, m_targetContentTypes)
            {
                Options = (ContentTypeApplicationOptionsCollection)option.SubObject
            });
            if (contentTypeApplicationOptionsConfigDialog.ShowDialog() == DialogResult.OK)
            {
                option.OptionAppliedValue = GetOptionsCollectionDisplayString((ContentTypeApplicationOptionsCollection)option.SubObject);
                UpdateOptionsColumnWidths();
            }
        }

        protected override void EditRule(MappingOption option)
        {
            ContentTypeApplicationOptionsCollection contentTypeApplicationOptionsCollection = (ContentTypeApplicationOptionsCollection)option.SubObject;
            FilterExpressionEditorDialog filterExpressionEditorDialog = new FilterExpressionEditorDialog();
            filterExpressionEditorDialog.FilterableTypes = new FilterBuilderType(new List<Type> { typeof(SPList) }, bAllowFreeFormEntry: false);
            filterExpressionEditorDialog.FilterExpression = contentTypeApplicationOptionsCollection.AppliesToFilter;
            filterExpressionEditorDialog.Title = "List Filter Conditions";
            FilterExpressionEditorDialog filterExpressionEditorDialog2 = filterExpressionEditorDialog;
            if (filterExpressionEditorDialog2.ShowDialog() == DialogResult.OK)
            {
                contentTypeApplicationOptionsCollection.AppliesToFilter = filterExpressionEditorDialog2.FilterExpression;
                option.Option = filterExpressionEditorDialog2.FilterExpression.GetLogicString();
            }
        }

        private MappingOption GetMappingOption(ContentTypeApplicationOptionsCollection collection)
        {
            MappingOption mappingOption = new MappingOption();
            if (collection.AppliesToFilter != null)
            {
                mappingOption.Option = collection.AppliesToFilter.GetLogicString();
            }
            else
            {
                mappingOption.Option = "";
            }
            mappingOption.SubObject = collection;
            mappingOption.OptionAppliedValue = GetOptionsCollectionDisplayString(collection);
            return mappingOption;
        }

        private string GetOptionsCollectionDisplayString(ContentTypeApplicationOptionsCollection collection)
        {
            string text = "";
            foreach (ContentTypeApplicationOptions datum in collection.Data)
            {
                text = text + (string.IsNullOrEmpty(text) ? "" : ", ") + datum.ContentTypeName;
            }
            return text;
        }

        private new void InitializeComponent()
        {
            base.InitializeComponent();
        }

        protected override void LoadOptionsUI()
        {
            ClearOptions();
            if (Options == null)
            {
                UpdateUI();
                return;
            }
            foreach (ContentTypeApplicationOptionsCollection option in Options)
            {
                AddOption(GetMappingOption(option.Clone()));
            }
            UpdateUI();
            UpdateOptionsColumnWidths();
        }

        protected override void SaveUI()
        {
            Options.Clear();
            foreach (MappingOption mapping in base.MappingList)
            {
                Options.Add((ContentTypeApplicationOptionsCollection)mapping.SubObject);
            }
        }
    }
}