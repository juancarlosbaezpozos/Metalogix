using Metalogix.Actions;
using Metalogix.Data.CSV;
using Metalogix.Explorer;
using Metalogix.Licensing;
using Metalogix.Metabase;
using Metalogix.Metabase.Attributes;
using Metalogix.Metabase.DataTypes;
using Metalogix.Metabase.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace Metalogix.Metabase.Actions
{
    [Image("Metalogix.Actions.Icons.ExportToCSV.ico")]
    [LaunchAsJob(true)]
    [LicensedProducts(ProductFlags.CMCPublicFolder | ProductFlags.CMCWebsite | ProductFlags.CMCeRoom |
                      ProductFlags.CMCOracleAndStellent | ProductFlags.CMCDocumentum | ProductFlags.CMCBlogsAndWikis |
                      ProductFlags.CMCGoogle | ProductFlags.SRM | ProductFlags.CMWebComponents)]
    [MenuText(
        "Metadata Modifications {1-Transform} > CSV Actions {4 - CSV} > Export Content to a CSV File... {4 - CSV}")]
    [MenuTextPlural(
        "Metadata Modifications {1-Transform} > CSV Actions {4 - CSV} > Export Contents to a CSV File... {4 - CSV}",
        PluralCondition.MultipleTargets)]
    [Name("Export Contents to a CSV File")]
    [RequiresTargetMetabaseConnection(true)]
    [RunAsync(true)]
    [ShowStatusDialog(true)]
    [SourceCardinality(Cardinality.Zero)]
    [TargetCardinality(Cardinality.OneOrMore)]
    [TargetType(typeof(Node))]
    [UsesStickySettings(true)]
    public class ExportToCSVAction : MetabaseAction<ExportToCSVOptions>
    {
        public ExportToCSVAction()
        {
        }

        private string EscapeValue(string sVal)
        {
            return sVal;
        }

        private ActionOperationStatus ExportItem(Node node, List<PropertyDescriptor> properties,
            CsvDocument csvDocument)
        {
            bool flag = false;
            try
            {
                List<string> strs = new List<string>();
                Record record = node.Record;
                if (record == null)
                {
                    strs.Add(((ExplorerNode)node).Record.ID);
                }
                else
                {
                    strs.Add(record.ID);
                }

                for (int i = 0; i < properties.Count; i++)
                {
                    string value = ExportToCSVAction.GetValue(node, properties[i]) ?? string.Empty;
                    value = this.EscapeValue(value);
                    strs.Add(value);
                }

                csvDocument.AddRow(new CsvRow(strs.ToArray()));
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                flag = true;
                LogItem logItem = new LogItem("Export Item to CSV", node.Name, string.Empty,
                    this.ActionOptions.TargetFilename, ActionOperationStatus.Running);
                base.FireOperationStarted(logItem);
                LogItem logItem1 = logItem;
                string details = logItem1.Details;
                string str = string.Concat("Error: ", exception.Message, Environment.NewLine);
                string str1 = str;
                logItem.Information = str;
                logItem1.Details = string.Concat(details, str1);
                LogItem logItem2 = logItem;
                string details1 = logItem2.Details;
                string[] newLine = new string[]
                {
                    details1, "********** Extended Information **********", Environment.NewLine, exception.StackTrace,
                    Environment.NewLine, "******************************************", Environment.NewLine
                };
                logItem2.Details = string.Concat(newLine);
                logItem.Status = ActionOperationStatus.Failed;
                base.FireOperationFinished(logItem);
            }

            if (!flag)
            {
                return ActionOperationStatus.Completed;
            }

            return ActionOperationStatus.Failed;
        }

        public static string GetValue(Node node, PropertyDescriptor propertyDescriptor_0)
        {
            object value = propertyDescriptor_0.GetValue(node);
            string str = null;
            if (value != null)
            {
                if (!(value is DateTime))
                {
                    str = (!(value is ISmartDataType)
                        ? Record.SerializeValue(value)
                        : (value as ISmartDataType).SerializeToUserFriendlyString(false));
                }
                else
                {
                    str = string.Format("{0:yyyy-MM-ddtthh:mm:ssz}", value);
                }
            }

            return str;
        }

        protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
        {
            this.RunOperation(new object[] { target });
        }

        protected override void RunOperation(object[] oParams)
        {
            IXMLAbleList xMLAbleLists = oParams[0] as IXMLAbleList;
            if (xMLAbleLists == null)
            {
                return;
            }

            Node item = xMLAbleLists[0] as Node;
            string str = string.Concat(item.Connection.Node.Name, item.Path);
            string targetFilename = this.ActionOptions.TargetFilename;
            LogItem logItem = new LogItem("Export to CSV", string.Empty, str, string.Empty,
                ActionOperationStatus.Running);
            base.FireOperationStarted(logItem);
            base.FireSourceLinkChanged(str);
            base.FireTargetLinkChanged(targetFilename);
            FileInfo fileInfo = null;
            try
            {
                fileInfo = new FileInfo(this.ActionOptions.TargetFilename);
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                LogItem logItem1 = logItem;
                string details = logItem1.Details;
                string str1 = string.Concat("Error: ", exception.Message, Environment.NewLine);
                string str2 = str1;
                logItem.Information = str1;
                logItem1.Details = string.Concat(details, str2);
                LogItem logItem2 = logItem;
                string details1 = logItem2.Details;
                string[] newLine = new string[]
                {
                    details1, "********** Extended Information **********", Environment.NewLine, exception.StackTrace,
                    Environment.NewLine, "******************************************", Environment.NewLine
                };
                logItem2.Details = string.Concat(newLine);
                logItem.Status = ActionOperationStatus.Failed;
                base.FireOperationFinished(logItem);
                return;
            }

            if (fileInfo.Directory.Exists)
            {
                if (fileInfo.Exists && fileInfo.IsReadOnly)
                {
                    LogItem logItem3 = logItem;
                    string details2 = logItem3.Details;
                    string str3 = string.Concat("Error: File is readonly \"", fileInfo.FullName, "\"",
                        Environment.NewLine);
                    string str4 = str3;
                    logItem.Information = str3;
                    logItem3.Details = string.Concat(details2, str4);
                    logItem.Status = ActionOperationStatus.Failed;
                    base.FireOperationFinished(logItem);
                    return;
                }

                fileInfo.Delete();
                int num = 0;
                int num1 = 0;
                CsvDocument csvDocument = new CsvDocument()
                {
                    IgnoreDuplicateHeaders = true
                };
                Node current = null;
                IEnumerator enumerator = xMLAbleLists.GetEnumerator();
                try
                {
                    do
                    {
                        if (!enumerator.MoveNext())
                        {
                            break;
                        }

                        current = (Node)enumerator.Current;
                    } while (current == null);
                }
                finally
                {
                    IDisposable disposable = enumerator as IDisposable;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                }

                LogItem logItem4 = logItem;
                string details3 = logItem4.Details;
                string str5 = string.Concat("Writing column headers", Environment.NewLine);
                string str6 = str5;
                logItem.Information = str5;
                logItem4.Details = string.Concat(details3, str6);
                base.FireOperationUpdated(logItem);
                PropertyDescriptorCollection properties = current.GetProperties();
                List<PropertyDescriptor> propertyDescriptors =
                    new List<PropertyDescriptor>(this.ActionOptions.ColumnsToExport.Count);
                csvDocument.AppendColumns(new string[] { "MetalogixID" });
                for (int i = 0; i < this.ActionOptions.ColumnsToExport.Count; i++)
                {
                    PropertyDescriptor propertyDescriptor = properties[this.ActionOptions.ColumnsToExport[i]];
                    if (propertyDescriptor == null)
                    {
                        LogItem logItem5 = logItem;
                        string details4 = logItem5.Details;
                        string str7 = string.Concat("Skipping missing property \"",
                            this.ActionOptions.ColumnsToExport[i], "\". It may have been removed.",
                            Environment.NewLine);
                        string str8 = str7;
                        logItem.Information = str7;
                        logItem5.Details = string.Concat(details4, str8);
                        logItem.Status = ActionOperationStatus.Warning;
                    }
                    else
                    {
                        string name = propertyDescriptor.Name;
                        if (this.ActionOptions.IncludeTypes)
                        {
                            object obj = name;
                            object[] propertyType = new object[] { obj, "(", propertyDescriptor.PropertyType, ")" };
                            name = string.Concat(propertyType);
                        }

                        if (csvDocument.AppendColumn(this.EscapeValue(name)))
                        {
                            propertyDescriptors.Add(propertyDescriptor);
                        }
                    }
                }

                foreach (Node node in xMLAbleLists)
                {
                    if (base.CheckForAbort())
                    {
                        return;
                    }
                    else
                    {
                        if (node == null)
                        {
                            continue;
                        }

                        if (this.ExportItem(node, propertyDescriptors, csvDocument) != ActionOperationStatus.Failed)
                        {
                            num++;
                        }
                        else
                        {
                            num1++;
                        }
                    }
                }

                csvDocument.SaveToFile(fileInfo.FullName, CsvDelimiterType.Comma);
                logItem.AddCompletionDetail("Records exported", (long)num);
                if (num1 <= 0)
                {
                    logItem.Status = ActionOperationStatus.Completed;
                    LogItem logItem6 = logItem;
                    string details5 = logItem6.Details;
                    string str9 =
                        string.Format(string.Concat("{0} records exported to CSV successfully.", Environment.NewLine),
                            num);
                    string str10 = str9;
                    logItem.Information = str9;
                    logItem6.Details = string.Concat(details5, str10);
                }
                else
                {
                    logItem.Status = ActionOperationStatus.Failed;
                    LogItem logItem7 = logItem;
                    string details6 = logItem7.Details;
                    string str11 =
                        string.Format(
                            string.Concat("Finished. {0} records failed to export. See item logs for more details",
                                Environment.NewLine), num1, num);
                    string str12 = str11;
                    logItem.Information = str11;
                    logItem7.Details = string.Concat(details6, str12);
                }

                base.FireOperationFinished(logItem);
                return;
            }
            else
            {
                LogItem logItem8 = logItem;
                string details7 = logItem8.Details;
                string str13 = string.Concat("Error: Directory does not exist \"", fileInfo.DirectoryName, "\"",
                    Environment.NewLine);
                string str14 = str13;
                logItem.Information = str13;
                logItem8.Details = string.Concat(details7, str14);
                logItem.Status = ActionOperationStatus.Failed;
                base.FireOperationFinished(logItem);
            }
        }
    }
}