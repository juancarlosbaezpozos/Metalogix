using Metalogix.Actions;
using Metalogix.Data.CSV;
using Metalogix.Explorer;
using Metalogix.Licensing;
using Metalogix.Metabase;
using Metalogix.Metabase.Attributes;
using Metalogix.Metabase.DataTypes;
using Metalogix.Metabase.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace Metalogix.Metabase.Actions
{
    [Image("Metalogix.Actions.Icons.ImportFromCSV.ico")]
    [LaunchAsJob(true)]
    [LicensedProducts(ProductFlags.CMCPublicFolder | ProductFlags.CMCWebsite | ProductFlags.CMCeRoom |
                      ProductFlags.CMCOracleAndStellent | ProductFlags.CMCDocumentum | ProductFlags.CMCBlogsAndWikis |
                      ProductFlags.CMCGoogle | ProductFlags.SRM | ProductFlags.CMWebComponents)]
    [MenuText(
        "Metadata Modifications {1-Transform} > CSV Actions {4 - CSV} > Import Contents from a CSV File... {4 - CSV}")]
    [Name("Import Contents from a CSV File")]
    [RequiresTargetMetabaseConnection(true)]
    [RunAsync(true)]
    [ShowStatusDialog(true)]
    [TargetCardinality(Cardinality.One)]
    [TargetType(typeof(Connection))]
    [UsesStickySettings(true)]
    public class ImportFromCSVAction : ImportMetadataBaseAction
    {
        protected CsvDocument _csvDocument;

        public ImportFromCSVAction()
        {
        }

        public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
        {
            if (!base.AppliesTo(sourceSelections, targetSelections))
            {
                return false;
            }

            return ((Connection)targetSelections[0]).Node.Parent == null;
        }

        protected virtual void CommitImportFromCSV(LogItem operation)
        {
            LogItem logItem = operation;
            string details = logItem.Details;
            string str = string.Concat("Importing Items", Environment.NewLine);
            string str1 = str;
            operation.Information = str;
            logItem.Details = string.Concat(details, str1);
            base.FireOperationUpdated(operation);
            if (this.ActionOptions.CreateNewColumns)
            {
                base.CreateNewColumns(operation);
            }

            int num = (this.ActionOptions.IgnoreMetalogixId || this._metalogixIDIndex == -1
                ? this._sourceURLIndex
                : this._metalogixIDIndex);
            if (num == -1)
            {
                LogItem logItem1 = operation;
                string details1 = logItem1.Details;
                string str2 =
                    string.Concat(
                        "Error: Could not find any key column. Please ensure your CSV file includes 'SourceURL' or ''MetalogixID' columns, or both.",
                        Environment.NewLine);
                string str3 = str2;
                operation.Information = str2;
                logItem1.Details = string.Concat(details1, str3);
                operation.Status = ActionOperationStatus.Failed;
                return;
            }

            int num1 = 0;
            try
            {
                int num2 = 0;
                while (num2 < this._csvDocument.GetRowCount())
                {
                    if (base.CheckForAbort())
                    {
                        return;
                    }
                    else
                    {
                        switch (this.ImportItem(num2, this._csvDocument.Rows[num2], num))
                        {
                            case ActionOperationStatus.Running:
                            {
                                num1++;
                                goto case ActionOperationStatus.MissingOnTarget;
                            }
                            case ActionOperationStatus.Completed:
                            {
                                num1++;
                                goto case ActionOperationStatus.MissingOnTarget;
                            }
                            case ActionOperationStatus.Warning:
                            {
                                if (operation.Status == ActionOperationStatus.Failed)
                                {
                                    goto case ActionOperationStatus.MissingOnTarget;
                                }

                                operation.Status = ActionOperationStatus.Warning;
                                goto case ActionOperationStatus.MissingOnTarget;
                            }
                            case ActionOperationStatus.Failed:
                            {
                                operation.Status = ActionOperationStatus.Failed;
                                goto case ActionOperationStatus.MissingOnTarget;
                            }
                            case ActionOperationStatus.Same:
                            case ActionOperationStatus.Different:
                            case ActionOperationStatus.MissingOnSource:
                            case ActionOperationStatus.MissingOnTarget:
                            {
                                num2++;
                                continue;
                            }
                            case ActionOperationStatus.Skipped:
                            {
                                if (operation.Status == ActionOperationStatus.Failed ||
                                    operation.Status == ActionOperationStatus.Warning)
                                {
                                    goto case ActionOperationStatus.MissingOnTarget;
                                }

                                operation.Status = ActionOperationStatus.Skipped;
                                goto case ActionOperationStatus.MissingOnTarget;
                            }
                            default:
                            {
                                goto case ActionOperationStatus.MissingOnTarget;
                            }
                        }
                    }
                }
            }
            catch (MLLicenseException mLLicenseException1)
            {
                MLLicenseException mLLicenseException = mLLicenseException1;
                LogItem logItem2 = operation;
                string details2 = logItem2.Details;
                string str4 = string.Concat(mLLicenseException.Message, Environment.NewLine);
                string str5 = str4;
                operation.Information = str4;
                logItem2.Details = string.Concat(details2, str5);
                operation.Status = ActionOperationStatus.Failed;
            }

            if (operation.Status == ActionOperationStatus.Running)
            {
                operation.AddCompletionDetail("Records Imported", (long)num1);
                LogItem logItem3 = operation;
                string details3 = logItem3.Details;
                LogItem logItem4 = operation;
                string str6 = string.Format(string.Concat("{0} {1} imported successfully", Environment.NewLine), num1,
                    (num1 == 1 ? "record" : "records"));
                string str7 = str6;
                logItem4.Information = str6;
                logItem3.Details = string.Concat(details3, str7);
            }

            operation.Status = ActionOperationStatus.Completed;
        }

        protected override Type DetermineColumnType(int columnIndex, ref string columnName)
        {
            Type type = typeof(string);
            if (!columnName.EndsWith(")", StringComparison.Ordinal))
            {
                Type columnTypeFromColumnValue = null;
                for (int i = 0; i < this._csvDocument.GetRowCount(); i++)
                {
                    string[] values = this._csvDocument.Rows[i].Values;
                    if (columnIndex < (int)values.Length)
                    {
                        if (columnTypeFromColumnValue == typeof(TextMoniker))
                        {
                            break;
                        }

                        string str = values[columnIndex];
                        if (!string.IsNullOrEmpty(str))
                        {
                            columnTypeFromColumnValue =
                                base.GetColumnTypeFromColumnValue(columnIndex, columnTypeFromColumnValue, str);
                        }
                    }
                }

                type = columnTypeFromColumnValue;
            }
            else
            {
                base.GetColumnTypeFromColumnHeader(ref columnName, out type);
            }

            return type;
        }

        protected override void Import(LogItem operation)
        {
            base.Import(operation);
            this._csvDocument = this.LoadFile(operation);
            if (this._csvDocument != null)
            {
                this.MatchColumnsFromCSV(operation);
                this.PreviewImportFromSCV(operation);
                if (base.CheckForAbort())
                {
                    return;
                }

                this.CommitImportFromCSV(operation);
            }
        }

        protected ActionOperationStatus ImportItem(int index, CsvRow csvRow, int int_0)
        {
            LogItem logItem = null;
            Record record = null;
            string[] values = csvRow.Values;
            ActionOperationStatus actionOperationStatu = base.ImportItem(index, values, int_0, out logItem, out record);
            if (actionOperationStatu != ActionOperationStatus.Skipped)
            {
                if (actionOperationStatu != ActionOperationStatus.Failed)
                {
                    return base.ImportItemMetadata(index, values, int_0, this._sourceURLIndex, logItem, record);
                }
            }

            return actionOperationStatu;
        }

        protected CsvDocument LoadFile(LogItem operation)
        {
            LogItem logItem = operation;
            string details = logItem.Details;
            string str = string.Concat("Reading data from file : ", this._displaySource, Environment.NewLine);
            string str1 = str;
            operation.Information = str;
            logItem.Details = string.Concat(details, str1);
            base.FireOperationUpdated(operation);
            CsvDocument csvDocument = new CsvDocument();
            try
            {
                csvDocument.Load(this.ActionOptions.SourceFileName);
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                LogItem logItem1 = operation;
                string details1 = logItem1.Details;
                string str2 = string.Concat("Error: Exception encountered while reading file: ", exception.Message,
                    Environment.NewLine);
                string str3 = str2;
                operation.Information = str2;
                logItem1.Details = string.Concat(details1, str3);
                LogItem logItem2 = operation;
                string details2 = logItem2.Details;
                string[] newLine = new string[]
                {
                    details2, "********** Extended Information **********", Environment.NewLine, exception.StackTrace,
                    Environment.NewLine, "******************************************", Environment.NewLine
                };
                logItem2.Details = string.Concat(newLine);
                operation.Status = ActionOperationStatus.Failed;
                return null;
            }

            LogItem logItem3 = operation;
            string details3 = logItem3.Details;
            string str4 = string.Concat("Parsing file contents", Environment.NewLine);
            string str5 = str4;
            operation.Information = str4;
            logItem3.Details = string.Concat(details3, str5);
            base.FireOperationUpdated(operation);
            if (csvDocument.GetHeaderCount() == 0 && csvDocument.GetRowCount() == 0)
            {
                if (operation.Status == ActionOperationStatus.Running)
                {
                    operation.Status = ActionOperationStatus.Failed;
                    LogItem logItem4 = operation;
                    string details4 = logItem4.Details;
                    string str6 = "There are no contents to be imported";
                    string str7 = str6;
                    operation.Information = str6;
                    logItem4.Details = string.Concat(details4, str7);
                }

                return null;
            }

            LogItem logItem5 = operation;
            string details5 = logItem5.Details;
            string str8 = string.Concat("Resolving column headers", Environment.NewLine);
            string str9 = str8;
            operation.Information = str8;
            logItem5.Details = string.Concat(details5, str9);
            base.FireOperationUpdated(operation);
            if (csvDocument.GetHeaderCount() > 1)
            {
                return csvDocument;
            }

            LogItem logItem6 = operation;
            string details6 = logItem6.Details;
            string str10 = "Error: No property columns";
            string str11 = str10;
            operation.Information = str10;
            logItem6.Details = string.Concat(details6, str11);
            operation.Status = ActionOperationStatus.Failed;
            return null;
        }

        protected void MatchColumnsFromCSV(LogItem operation)
        {
            LogItem logItem = operation;
            string details = logItem.Details;
            string str = string.Concat("Determining column types", Environment.NewLine);
            string str1 = str;
            operation.Information = str;
            logItem.Details = string.Concat(details, str1);
            base.FireOperationUpdated(operation);
            for (int i = 0; i < this._csvDocument.GetHeaderCount(); i++)
            {
                base.SetMatchedColumns(i, this._csvDocument.Headers[i]);
            }

            base.SetSourceUrlAndMetalogixIDIndex();
        }

        protected void PreviewImportFromSCV(LogItem operation)
        {
            if (!this.ActionOptions.Preview)
            {
                return;
            }

            if (this.OnPreview == null)
            {
                return;
            }

            LogItem logItem = new LogItem("Generating Preview", string.Empty, string.Empty, string.Empty,
                ActionOperationStatus.Running);
            base.FireOperationStarted(logItem);
            try
            {
                if (this.OnPreview != null)
                {
                    this.OnPreview(this, this._matchedColumns, this._unmatchedColumns);
                }

                LogItem logItem1 = logItem;
                string details = logItem1.Details;
                string str = string.Concat("CSV Preview Completed", Environment.NewLine);
                string str1 = str;
                logItem.Information = str;
                logItem1.Details = string.Concat(details, str1);
                logItem.Status = ActionOperationStatus.Completed;
                if (base.CheckForAbort())
                {
                    LogItem logItem2 = operation;
                    string details1 = logItem2.Details;
                    string str2 = string.Concat("CSV import has been cancelled by the user", Environment.NewLine);
                    string str3 = str2;
                    operation.Information = str2;
                    logItem2.Details = string.Concat(details1, str3);
                    operation.Status = ActionOperationStatus.Completed;
                }
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                LogItem logItem3 = logItem;
                logItem3.Details = string.Concat(logItem3.Details, string.Format("Error: {0}", exception.Message),
                    Environment.NewLine);
                logItem.Status = ActionOperationStatus.Failed;
            }

            base.FireOperationFinished(logItem);
        }

        public event ImportFromCSVAction.ImportFromCSVPreviewHandler OnPreview;

        public delegate void ImportFromCSVPreviewHandler(ImportFromCSVAction action,
            List<KeyValuePair<PropertyDescriptor, Type>> matchedColumns,
            Dictionary<int, KeyValuePair<string, Type>> unmatchedColumns);
    }
}