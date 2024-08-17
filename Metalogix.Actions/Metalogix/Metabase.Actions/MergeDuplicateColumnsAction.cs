using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.Licensing;
using Metalogix.Metabase;
using Metalogix.Metabase.Attributes;
using Metalogix.Metabase.Data;
using Metalogix.Metabase.DataTypes;
using Metalogix.Metabase.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Metalogix.Metabase.Actions
{
    [Image("Metalogix.Actions.Icons.MergeWorkspaceColumns.ico")]
    [IsAdvanced(true)]
    [LicensedProducts(ProductFlags.UnifiedContentMatrixKey)]
    [MenuText("Metadata Modifications {1-Transform} > Merge/Duplicate Columns... {2 - Manipulate}")]
    [Name("Merge/Duplicate Columns")]
    [RequiresTargetMetabaseConnection(true)]
    [RunAsync(true)]
    [ShowStatusDialog(true)]
    [TargetCardinality(Cardinality.OneOrMore)]
    [TargetType(typeof(Node))]
    [UsesStickySettings(true)]
    public class MergeDuplicateColumnsAction : MetabaseAction<MergeDuplicateColumnsOptions>
    {
        public MergeDuplicateColumnsAction()
        {
        }

        public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
        {
            if (!base.AppliesTo(sourceSelections, targetSelections))
            {
                return false;
            }

            if (typeof(Folder).IsAssignableFrom(targetSelections.CollectionType))
            {
                return true;
            }

            return typeof(ListItem).IsAssignableFrom(targetSelections.CollectionType);
        }

        protected void MergeColumns(Node curTarget, PropertyDescriptorCollection propertiesToCopy,
            PropertyDescriptor pdColOut, LogItem itemOperation)
        {
            base.CheckForAbort();
            if (curTarget == null)
            {
                LogItem logItem = itemOperation;
                string details = logItem.Details;
                string str = string.Concat("Error: Item is null", Environment.NewLine);
                string str1 = str;
                itemOperation.Information = str;
                logItem.Details = string.Concat(details, str1);
                itemOperation.Status = ActionOperationStatus.MissingOnSource;
                base.FireOperationFinished(itemOperation);
                return;
            }

            itemOperation.ItemName = string.Concat("Item ", curTarget.ToString());
            LogItem logItem1 = itemOperation;
            string details1 = logItem1.Details;
            string str2 = string.Concat("Copying column values", Environment.NewLine);
            string str3 = str2;
            itemOperation.Information = str2;
            logItem1.Details = string.Concat(details1, str3);
            base.FireOperationUpdated(itemOperation);
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < propertiesToCopy.Count; i++)
            {
                PropertyDescriptor item = propertiesToCopy[i];
                if (item.PropertyType != typeof(TextMoniker))
                {
                    object value = item.GetValue(curTarget);
                    stringBuilder.Append((value != null ? value.ToString() : string.Empty));
                }
                else
                {
                    stringBuilder.Append(((TextMoniker)item.GetValue(curTarget)).GetFullText());
                }

                if (i != propertiesToCopy.Count - 1)
                {
                    stringBuilder.Append(this.ActionOptions.Separator);
                }
            }

            object obj = TypeDescriptor.GetConverter(pdColOut.PropertyType).ConvertFrom(stringBuilder.ToString());
            pdColOut.SetValue(curTarget, obj);
        }

        protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
        {
            this.RunOperation(new object[] { target });
        }

        protected override void RunOperation(object[] oParams)
        {
            IXMLAbleList items = oParams[0] as IXMLAbleList;
            if (typeof(Folder).IsAssignableFrom(items.CollectionType) &&
                !typeof(ListItem).IsAssignableFrom(items.CollectionType))
            {
                items = (items[0] as Folder).GetItems();
            }

            Record record = (items[0] as Node).Record;
            string empty = string.Empty;
            if (this.ActionOptions.ColumnsIn.Count > 0)
            {
                empty = this.ActionOptions.ColumnsIn[0];
                string str = ", ";
                for (int i = 1; i < this.ActionOptions.ColumnsIn.Count; i++)
                {
                    empty = string.Concat(empty, str, this.ActionOptions.ColumnsIn[i]);
                }
            }

            LogItem logItem = new LogItem((this.ActionOptions.ColumnsIn.Count == 1 ? "Copy Column" : "Copy Columns"),
                string.Empty, empty, this.ActionOptions.ColumnOut, ActionOperationStatus.Running);
            base.FireOperationStarted(logItem);
            PropertyDescriptorCollection properties = ((Node)items[0]).GetProperties();
            PropertyDescriptorCollection propertyDescriptorCollections =
                new PropertyDescriptorCollection(new PropertyDescriptor[0]);
            for (int j = 0; j < this.ActionOptions.ColumnsIn.Count; j++)
            {
                base.CheckForAbort();
                PropertyDescriptor item = properties[this.ActionOptions.ColumnsIn[j]];
                if (item != null)
                {
                    propertyDescriptorCollections.Add(item);
                }
                else
                {
                    LogItem logItem1 = logItem;
                    string details = logItem1.Details;
                    string str1 = string.Concat("Warning: Property <", this.ActionOptions.ColumnsIn[j],
                        "> could not be found. It may have been removed.", Environment.NewLine);
                    string str2 = str1;
                    logItem.Information = str1;
                    logItem1.Details = string.Concat(details, str2);
                    logItem.Status = ActionOperationStatus.Warning;
                    base.FireOperationUpdated(logItem);
                }
            }

            if (propertyDescriptorCollections.Count == 0)
            {
                LogItem logItem2 = logItem;
                string details1 = logItem2.Details;
                string str3 = string.Concat("Error: No properties to copy", Environment.NewLine);
                string str4 = str3;
                logItem.Information = str3;
                logItem2.Details = string.Concat(details1, str4);
                logItem.Status = ActionOperationStatus.Failed;
                base.FireOperationFinished(logItem);
                return;
            }

            if (string.IsNullOrEmpty(this.ActionOptions.ColumnOut))
            {
                LogItem logItem3 = logItem;
                string details2 = logItem3.Details;
                string str5 = string.Concat("Error: Output column name cannot be empty", Environment.NewLine);
                string str6 = str5;
                logItem.Information = str5;
                logItem3.Details = string.Concat(details2, str6);
                logItem.Status = ActionOperationStatus.Failed;
                base.FireOperationFinished(logItem);
                return;
            }

            PropertyDescriptor propertyDescriptor = properties[this.ActionOptions.ColumnOut];
            if (propertyDescriptor == null)
            {
                LogItem logItem4 = logItem;
                string details3 = logItem4.Details;
                string str7 = string.Concat("Error: Output property (", this.ActionOptions.ColumnOut,
                    ") could not be found.", Environment.NewLine);
                string str8 = str7;
                logItem.Information = str7;
                logItem4.Details = string.Concat(details3, str8);
                logItem.Status = ActionOperationStatus.Failed;
                base.FireOperationFinished(logItem);
                return;
            }

            Workspace parentWorkspace = record.ParentWorkspace;
            ViewList views = parentWorkspace.GetViews();
            ViewProperty viewProperty = views.SelectedView.ViewProperties.Find(propertyDescriptor.Name);
            if (!viewProperty.IsDisplayed)
            {
                viewProperty.IsDisplayed = true;
                parentWorkspace.SetViews(views);
                parentWorkspace.CommitChanges();
            }

            LogItem logItem5 = logItem;
            string details4 = logItem5.Details;
            string str9 = string.Concat("Copying column values for each target item...", Environment.NewLine);
            string str10 = str9;
            logItem.Information = str9;
            logItem5.Details = string.Concat(details4, str10);
            base.FireOperationUpdated(logItem);
            int num = 0;
            foreach (object obj in items)
            {
                record = (obj as Node).Record;
                base.CheckForAbort();
                LogItem logItem6 = new LogItem("Copy Column", string.Empty, empty, this.ActionOptions.ColumnOut,
                    ActionOperationStatus.Running);
                base.FireOperationStarted(logItem6);
                try
                {
                    try
                    {
                        this.MergeColumns(obj as Node, propertyDescriptorCollections, propertyDescriptor, logItem6);
                        LogItem logItem7 = logItem6;
                        string details5 = logItem7.Details;
                        string str11 = string.Concat("Columns merged successfully", Environment.NewLine);
                        string str12 = str11;
                        logItem6.Information = str11;
                        logItem7.Details = string.Concat(details5, str12);
                        logItem6.Status = ActionOperationStatus.Completed;
                        base.FireOperationFinished(logItem6);
                    }
                    catch (Exception exception1)
                    {
                        Exception exception = exception1;
                        LogItem logItem8 = logItem6;
                        string details6 = logItem8.Details;
                        string str13 = string.Concat("Error: Exception encountered: ", exception.Message);
                        string str14 = str13;
                        logItem6.Information = str13;
                        logItem8.Details = string.Concat(details6, str14);
                        LogItem logItem9 = logItem6;
                        string details7 = logItem9.Details;
                        string[] newLine = new string[]
                        {
                            details7, "********** Extended Information **********", Environment.NewLine,
                            exception.StackTrace, Environment.NewLine, "******************************************",
                            Environment.NewLine
                        };
                        logItem9.Details = string.Concat(newLine);
                        logItem6.Status = ActionOperationStatus.Failed;
                        base.FireOperationFinished(logItem6);
                        num++;
                    }
                }
                finally
                {
                    record.CommitChanges();
                }
            }

            if (num > 0)
            {
                string str15 = (num == 1 ? "failure" : "failures");
                LogItem logItem10 = logItem;
                string details8 = logItem10.Details;
                string str16 = string.Concat(string.Format("Copy completed with {0} {1}.", num, str15),
                    Environment.NewLine);
                string str17 = str16;
                logItem.Information = str16;
                logItem10.Details = string.Concat(details8, str17);
                logItem.Status = ActionOperationStatus.Warning;
            }
            else if (logItem.Status != ActionOperationStatus.Warning)
            {
                LogItem logItem11 = logItem;
                string details9 = logItem11.Details;
                string str18 = string.Concat("Column copying completed successfully.", Environment.NewLine);
                string str19 = str18;
                logItem.Information = str18;
                logItem11.Details = string.Concat(details9, str19);
                logItem.Status = ActionOperationStatus.Completed;
            }
            else
            {
                LogItem logItem12 = logItem;
                string details10 = logItem12.Details;
                string str20 = string.Concat("Column copying completed with a warning.", Environment.NewLine);
                string str21 = str20;
                logItem.Information = str20;
                logItem12.Details = string.Concat(details10, str21);
            }

            base.FireOperationFinished(logItem);
        }
    }
}