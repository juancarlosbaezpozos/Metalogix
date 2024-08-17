using Metalogix.Actions;
using Metalogix.Metabase.Actions;
using Metalogix.Metabase.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Metabase
{
	[ActionConfig(new Type[] { typeof(ImportFromCSVAction) })]
	public class ImportFromCSVConfig : IActionConfig
	{
		protected ActionConfigContext _context;

		protected bool IsExcelFile
		{
			get;
			set;
		}

		protected bool ShowAddNewRows
		{
			get;
			set;
		}

		protected bool ShowIgnoreMetalogixID
		{
			get;
			set;
		}

		public ImportFromCSVConfig()
		{
			this.ShowIgnoreMetalogixID = true;
		}

		protected virtual void AddPreviewHandler()
		{
			this._context.GetAction<ImportFromCSVAction>().OnPreview += new ImportFromCSVAction.ImportFromCSVPreviewHandler(this.ImportFromCSVConfig_OnPreview);
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			this._context = context;
			ImportFromCSVDialog importFromCSVDialog = new ImportFromCSVDialog()
			{
				ShowAddNewRows = this.ShowAddNewRows,
				ShowIgnoreMetalogixID = this.ShowIgnoreMetalogixID,
				Options = context.GetActionOptions<ImportFromCSVOptions>(),
				IsExcelFile = this.IsExcelFile
			};
			if (importFromCSVDialog.ShowDialog() != DialogResult.OK)
			{
				return ConfigurationResult.Cancel;
			}
			this._context.ActionContext.Sources = new XMLAbleList();
			this._context.ActionOptions = importFromCSVDialog.Options;
			this.AddPreviewHandler();
			return ConfigurationResult.Run;
		}

		private void ImportFromCSVConfig_OnPreview(ImportFromCSVAction action, List<KeyValuePair<PropertyDescriptor, Type>> matchedColumns, Dictionary<int, KeyValuePair<string, Type>> unmatchedColumns)
		{
			this._context.GetAction<ImportFromCSVAction>().OnPreview -= new ImportFromCSVAction.ImportFromCSVPreviewHandler(this.ImportFromCSVConfig_OnPreview);
			this.ShowPreviewDialog(action, matchedColumns, unmatchedColumns);
		}

		protected void ShowPreviewDialog(MetabaseAction<ImportFromCSVOptions> action, List<KeyValuePair<PropertyDescriptor, Type>> matchedColumns, Dictionary<int, KeyValuePair<string, Type>> unmatchedColumns)
		{
			ImportFromCSVPreviewDialog importFromCSVPreviewDialog = new ImportFromCSVPreviewDialog(action.ActionOptions.CreateNewColumns)
			{
				ShowInTaskbar = true,
				MappedColumns = matchedColumns,
				UnmappedColumns = new List<KeyValuePair<string, Type>>(unmatchedColumns.Values)
			};
			importFromCSVPreviewDialog.HiddenColumns.Add("MetalogixID");
			importFromCSVPreviewDialog.IsExcelFile = this.IsExcelFile;
			if (importFromCSVPreviewDialog.ShowDialog() == DialogResult.Cancel)
			{
				action.Cancel();
			}
		}
	}
}