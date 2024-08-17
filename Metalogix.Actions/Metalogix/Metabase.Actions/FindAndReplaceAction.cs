using Metalogix.Actions;
using Metalogix.Actions.Properties;
using Metalogix.Explorer;
using Metalogix.Licensing;
using Metalogix.Metabase;
using Metalogix.Metabase.Attributes;
using Metalogix.Metabase.Data;
using Metalogix.Metabase.DataTypes;
using Metalogix.Metabase.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;

namespace Metalogix.Metabase.Actions
{
    [Image("Metalogix.Actions.Icons.FindAndReplace.ico")]
    [IsAdvanced(true)]
    [LaunchAsJob(true)]
    [LicensedProducts(ProductFlags.UnifiedContentMatrixKey)]
    [MenuText("Metadata Modifications {1-Transform} > Find and Replace in Column... {2 - Manipulate}")]
    [Name("Find and Replace in Column")]
    [RequiresTargetMetabaseConnection(true)]
    [RunAsync(true)]
    [Shortcut(ShortcutAction.FindAndReplace)]
    [ShowStatusDialog(true)]
    [TargetCardinality(Cardinality.OneOrMore)]
    [TargetType(typeof(Node))]
    [UsesStickySettings(true)]
    public class FindAndReplaceAction : MetabaseAction<FindAndReplaceOptions>
    {
        private static Dictionary<string, PropertyFilterOperand> _legalOperands;

        private readonly static object _lockOperands;

        public static Dictionary<string, PropertyFilterOperand> LegalOperands
        {
            get
            {
                if (FindAndReplaceAction._legalOperands != null)
                {
                    return FindAndReplaceAction._legalOperands;
                }

                lock (FindAndReplaceAction._lockOperands)
                {
                    if (FindAndReplaceAction._legalOperands == null)
                    {
                        FindAndReplaceAction._legalOperands = new Dictionary<string, PropertyFilterOperand>()
                        {
                            { Resources.FindAndReplaceOperand_Contains, PropertyFilterOperand.Contains },
                            { Resources.FindAndReplaceOperand_Equals, PropertyFilterOperand.Equals },
                            { Resources.FindAndReplaceOperand_StartsWith, PropertyFilterOperand.StartsWith },
                            { Resources.FindAndReplaceOperand_EndsWith, PropertyFilterOperand.EndsWith },
                            { Resources.FindAndReplaceOperand_RegEx, PropertyFilterOperand.RegularExpression }
                        };
                    }
                }

                return FindAndReplaceAction._legalOperands;
            }
        }

        static FindAndReplaceAction()
        {
            FindAndReplaceAction._legalOperands = null;
            FindAndReplaceAction._lockOperands = new object();
        }

        public FindAndReplaceAction()
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

        protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
        {
            this.RunOperation(new object[] { target });
        }

        protected override void RunOperation(object[] oParams)
        {
            IXMLAbleList items = oParams[0] as IXMLAbleList;
            LogItem logItem = new LogItem("Find and Replace", string.Empty, this.ActionOptions.FindValue,
                this.ActionOptions.ReplaceValue, ActionOperationStatus.Running);
            LogItem logItem1 = logItem;
            string details = logItem1.Details;
            string str = string.Concat("Building search expression", Environment.NewLine);
            string str1 = str;
            logItem.Information = str;
            logItem1.Details = string.Concat(details, str1);
            base.FireOperationStarted(logItem);
            if (items.Count == 0)
            {
                LogItem logItem2 = logItem;
                string details1 = logItem2.Details;
                string str2 = string.Concat("No target Records", Environment.NewLine);
                string str3 = str2;
                logItem.Information = str2;
                logItem2.Details = string.Concat(details1, str3);
                logItem.Status = ActionOperationStatus.Failed;
                base.FireOperationFinished(logItem);
                return;
            }

            if (typeof(Folder).IsAssignableFrom(items.CollectionType) &&
                !typeof(ListItem).IsAssignableFrom(items.CollectionType))
            {
                items = (items[0] as Folder).GetItems();
            }

            if (this.ActionOptions.IsXPathQuery)
            {
                this.RunXPathFindAndReplace(items, logItem);
                return;
            }

            Record record = (items[0] as ExplorerNode).Record;
            int num = 0;
            ReplaceExpression replaceExpression = new ReplaceExpression()
            {
                IsCaseSensitive = true,
                Operand = PropertyFilterOperand.Contains,
                Property = null,
                Value = this.ActionOptions.FindValue,
                ReplaceValue = this.ActionOptions.ReplaceValue
            };
            replaceExpression.IsCaseSensitive = this.ActionOptions.MatchCase;
            replaceExpression.MultiLine = this.ActionOptions.MultiLine;
            if (this.ActionOptions.Property != null)
            {
                replaceExpression.Property = record.GetProperties()[this.ActionOptions.Property];
            }

            replaceExpression.Operand = this.ActionOptions.FilterOperand;
            logItem.Source = replaceExpression.Value;
            logItem.Target = replaceExpression.ReplaceValue;
            string str4 = (replaceExpression.Property != null ? replaceExpression.Property.DisplayName : string.Empty);
            LogItem logItem3 = logItem;
            string details2 = logItem3.Details;
            string str5 = string.Concat("Replacing content in column \"", str4, "\"...", Environment.NewLine);
            string str6 = str5;
            logItem.Information = str5;
            logItem3.Details = string.Concat(details2, str6);
            base.FireOperationUpdated(logItem);
            int num1 = 0;
            string empty = string.Empty;
            for (int i = 0; i < items.Count; i++)
            {
                if (base.CheckForAbort())
                {
                    return;
                }

                Record record1 = (items[i] as Node).Record;
                string str7 = (record1 != null ? string.Concat("Item ", record1.RecordNum) : string.Empty);
                LogItem logItem4 = new LogItem("Find and Replace", str7, replaceExpression.Value,
                    replaceExpression.ReplaceValue, ActionOperationStatus.Running);
                base.FireOperationStarted(logItem4);
                bool flag = false;
                try
                {
                    try
                    {
                        record1.BeginEdit();
                        if (!replaceExpression.EvaluateReplace(record1))
                        {
                            logItem4.Status = ActionOperationStatus.Completed;
                            LogItem logItem5 = logItem4;
                            string details3 = logItem5.Details;
                            string str8 = string.Concat("Value does not match search expression.", Environment.NewLine,
                                "Nothing was replaced.", Environment.NewLine);
                            string str9 = str8;
                            logItem4.Information = str8;
                            logItem5.Details = string.Concat(details3, str9);
                        }
                        else
                        {
                            flag = true;
                            num1++;
                            LogItem logItem6 = logItem4;
                            string details4 = logItem6.Details;
                            string str10 = string.Concat("Value matches search expression.", Environment.NewLine);
                            string str11 = str10;
                            logItem4.Information = str10;
                            logItem6.Details = string.Concat(details4, str11);
                            base.FireOperationUpdated(logItem4);
                            logItem4.Status = ActionOperationStatus.Completed;
                            LogItem logItem7 = logItem4;
                            string details5 = logItem7.Details;
                            string str12 = string.Concat("Value successfully replaced.", Environment.NewLine);
                            string str13 = str12;
                            logItem4.Information = str12;
                            logItem7.Details = string.Concat(details5, str13);
                        }
                    }
                    catch (Exception exception1)
                    {
                        Exception exception = exception1;
                        num++;
                        empty = string.Concat(empty, logItem4.ItemName, Environment.NewLine);
                        logItem4.Status = ActionOperationStatus.Failed;
                        LogItem logItem8 = logItem4;
                        string details6 = logItem8.Details;
                        string str14 = string.Concat("Exception encountered: ", exception.Message, Environment.NewLine);
                        string str15 = str14;
                        logItem4.Information = str14;
                        logItem8.Details = string.Concat(details6, str15);
                        if (!exception.Message.EndsWith(" is not a valid value for DateTime."))
                        {
                            LogItem logItem9 = logItem4;
                            string details7 = logItem9.Details;
                            string[] newLine = new string[]
                            {
                                details7, "********** Extended Information **********", Environment.NewLine,
                                exception.StackTrace, Environment.NewLine, "******************************************",
                                Environment.NewLine
                            };
                            logItem9.Details = string.Concat(newLine);
                        }
                    }
                }
                finally
                {
                    record1.EndEdit();
                    if (flag)
                    {
                        record1.CommitChanges();
                    }
                }

                base.FireOperationFinished(logItem4);
            }

            if (num1 != 0)
            {
                if (num1 < items.Count)
                {
                    LogItem logItem10 = logItem;
                    string details8 = logItem10.Details;
                    object[] count = new object[]
                    {
                        items.Count - num1, " of ", items.Count, " items did not match the expression",
                        Environment.NewLine
                    };
                    string str16 = string.Concat(count);
                    string str17 = str16;
                    logItem.Information = str16;
                    logItem10.Details = string.Concat(details8, str17);
                }

                LogItem logItem11 = logItem;
                string details9 = logItem11.Details;
                object[] objArray = new object[]
                    { "Successfully replaced ", num1 - num, " of ", num1, " found matches", Environment.NewLine };
                string str18 = string.Concat(objArray);
                string str19 = str18;
                logItem.Information = str18;
                logItem11.Details = string.Concat(details9, str19);
            }
            else
            {
                LogItem logItem12 = logItem;
                string details10 = logItem12.Details;
                string str20 = string.Concat("No matches found", Environment.NewLine);
                string str21 = str20;
                logItem.Information = str20;
                logItem12.Details = string.Concat(details10, str21);
            }

            if (num <= 0)
            {
                logItem.Status = ActionOperationStatus.Completed;
            }
            else
            {
                LogItem logItem13 = logItem;
                string details11 = logItem13.Details;
                string str22 = string.Concat("Errored Items:", Environment.NewLine, empty);
                string str23 = str22;
                logItem.Information = str22;
                logItem13.Details = string.Concat(details11, str23);
                logItem.Status = ActionOperationStatus.Failed;
            }

            base.FireOperationFinished(logItem);
        }

        private void RunXPathFindAndReplace(IXMLAbleList target, LogItem operation)
        {
            Record record = (target[0] as ExplorerNode).Record;
            if (record == null)
            {
                LogItem logItem = operation;
                string details = logItem.Details;
                string str = string.Concat("Sample item is not a Record", Environment.NewLine);
                string str1 = str;
                operation.Information = str;
                logItem.Details = string.Concat(details, str1);
                operation.Status = ActionOperationStatus.Failed;
                base.FireOperationFinished(operation);
                return;
            }

            PropertyDescriptor propertyDescriptor =
                record.ParentWorkspace.GetProperties().Find(this.ActionOptions.Property, false);
            if (propertyDescriptor == null)
            {
                LogItem logItem1 = operation;
                string details1 = logItem1.Details;
                string str2 = "Property could not be found";
                string str3 = str2;
                operation.Information = str2;
                logItem1.Details = string.Concat(details1, str3);
                operation.Status = ActionOperationStatus.Failed;
                base.FireOperationFinished(operation);
                return;
            }

            LogItem logItem2 = operation;
            string details2 = logItem2.Details;
            string str4 = string.Concat("Replacing content in column \"", this.ActionOptions.Property, "\"",
                Environment.NewLine);
            string str5 = str4;
            operation.Information = str4;
            logItem2.Details = string.Concat(details2, str5);
            base.FireOperationUpdated(operation);
            int num = 0;
            List<string> strs = new List<string>();
            int num1 = 0;
            while (num1 < target.Count)
            {
                if (base.CheckForAbort())
                {
                    return;
                }

                Node item = (Node)target[num1];
                switch (this.SearchReplaceXPath(item.Record, propertyDescriptor))
                {
                    case -1:
                    {
                        strs.Add(item.Name);
                        goto case 0;
                    }
                    case 0:
                    {
                        num1++;
                        continue;
                    }
                    case 1:
                    {
                        num++;
                        goto case 0;
                    }
                    default:
                    {
                        goto case 0;
                    }
                }
            }

            if (num != 0)
            {
                if (num < target.Count)
                {
                    LogItem logItem3 = operation;
                    string details3 = logItem3.Details;
                    object[] count = new object[]
                    {
                        target.Count - num, " of ", target.Count, " items did not match the expression",
                        Environment.NewLine
                    };
                    string str6 = string.Concat(count);
                    string str7 = str6;
                    operation.Information = str6;
                    logItem3.Details = string.Concat(details3, str7);
                }

                LogItem logItem4 = operation;
                string details4 = logItem4.Details;
                object[] objArray = new object[]
                    { "Successfully replaced ", num - strs.Count, " of ", num, " found matches", Environment.NewLine };
                string str8 = string.Concat(objArray);
                string str9 = str8;
                operation.Information = str8;
                logItem4.Details = string.Concat(details4, str9);
            }
            else
            {
                LogItem logItem5 = operation;
                string details5 = logItem5.Details;
                string str10 = string.Concat("No matches found", Environment.NewLine);
                string str11 = str10;
                operation.Information = str10;
                logItem5.Details = string.Concat(details5, str11);
            }

            if (strs.Count <= 0)
            {
                operation.Status = ActionOperationStatus.Completed;
            }
            else
            {
                string empty = string.Empty;
                foreach (string str12 in strs)
                {
                    empty = string.Concat(empty, str12, Environment.NewLine);
                }

                LogItem logItem6 = operation;
                string details6 = logItem6.Details;
                string str13 = string.Concat("Errored Items:", Environment.NewLine, empty);
                string str14 = str13;
                operation.Information = str13;
                logItem6.Details = string.Concat(details6, str14);
                operation.Status = ActionOperationStatus.Failed;
            }

            base.FireOperationFinished(operation);
        }

        private int SearchReplaceXPath(Record record_0, PropertyDescriptor propertyDescriptor_0)
        {
            int num;
            LogItem logItem = new LogItem("Item Search and Replace", record_0.SourceURL.AbsoluteUrl,
                this.ActionOptions.FindValue, this.ActionOptions.ReplaceValue, ActionOperationStatus.Running);
            LogItem logItem1 = logItem;
            string details = logItem1.Details;
            string str = "Using XPath search and replace";
            string str1 = str;
            logItem.Information = str;
            logItem1.Details = string.Concat(details, str1);
            base.FireOperationStarted(logItem);
            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                xmlDocument.LoadXml(this.ActionOptions.ReplaceValue);
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                logItem.Status = ActionOperationStatus.Failed;
                LogItem logItem2 = logItem;
                string details1 = logItem2.Details;
                string str2 = string.Concat("Error: ", exception.Message, Environment.NewLine);
                string str3 = str2;
                logItem.Information = str2;
                logItem2.Details = string.Concat(details1, str3);
                LogItem logItem3 = logItem;
                string details2 = logItem3.Details;
                string[] newLine = new string[]
                {
                    details2, "********** Extended Information **********", Environment.NewLine, exception.StackTrace,
                    Environment.NewLine, "******************************************", Environment.NewLine
                };
                logItem3.Details = string.Concat(newLine);
                base.FireOperationFinished(logItem);
                num = -1;
                return num;
            }

            XmlNode firstChild = xmlDocument.FirstChild;
            object value = propertyDescriptor_0.GetValue(record_0);
            TextMoniker textMoniker = value as TextMoniker;
            string str4 = (textMoniker != null ? textMoniker.GetFullText() : value.ToString());
            try
            {
                xmlDocument.LoadXml(str4);
            }
            catch (Exception exception3)
            {
                Exception exception2 = exception3;
                if (!exception2.Message.ToLower().Contains("multiple root nodes"))
                {
                    logItem.Status = ActionOperationStatus.Failed;
                    LogItem logItem4 = logItem;
                    string details3 = logItem4.Details;
                    string str5 = string.Concat("Error: ", exception2.Message, Environment.NewLine);
                    string str6 = str5;
                    logItem.Information = str5;
                    logItem4.Details = string.Concat(details3, str6);
                    LogItem logItem5 = logItem;
                    string details4 = logItem5.Details;
                    string[] strArrays = new string[]
                    {
                        details4, "********** Extended Information **********", Environment.NewLine,
                        exception2.StackTrace, Environment.NewLine, "******************************************",
                        Environment.NewLine
                    };
                    logItem5.Details = string.Concat(strArrays);
                    base.FireOperationFinished(logItem);
                    num = -1;
                    return num;
                }
                else
                {
                    xmlDocument.LoadXml(string.Concat("<XML>", str4, "</XML>"));
                    this.ActionOptions.FindValue = string.Concat("/XML", this.ActionOptions.FindValue);
                    LogItem logItem6 = logItem;
                    string details5 = logItem6.Details;
                    string str7 = "Multiple root nodes, wrapping result with <XML></XML>";
                    string str8 = str7;
                    logItem.Information = str7;
                    logItem6.Details = string.Concat(details5, str8);
                    base.FireOperationUpdated(logItem);
                }
            }

            XmlNodeList xmlNodeLists = null;
            try
            {
                xmlNodeLists = xmlDocument.SelectNodes(this.ActionOptions.FindValue);
            }
            catch (Exception exception5)
            {
                Exception exception4 = exception5;
                logItem.Status = ActionOperationStatus.Failed;
                LogItem logItem7 = logItem;
                string details6 = logItem7.Details;
                string str9 = string.Concat("Error: ", exception4.Message, Environment.NewLine);
                string str10 = str9;
                logItem.Information = str9;
                logItem7.Details = string.Concat(details6, str10);
                LogItem logItem8 = logItem;
                string details7 = logItem8.Details;
                string[] newLine1 = new string[]
                {
                    details7, "********** Extended Information **********", Environment.NewLine, exception4.StackTrace,
                    Environment.NewLine, "******************************************", Environment.NewLine
                };
                logItem8.Details = string.Concat(newLine1);
                base.FireOperationFinished(logItem);
                num = -1;
                return num;
            }

            int num1 = 0;
            if (xmlNodeLists != null && xmlNodeLists.Count > 0)
            {
                for (int i = 0; i < xmlNodeLists.Count; i++)
                {
                    try
                    {
                        XmlNode parentNode = xmlNodeLists[i].ParentNode;
                        if (parentNode == null || parentNode is XmlDocument)
                        {
                            xmlDocument.LoadXml(firstChild.OuterXml);
                            break;
                        }
                        else
                        {
                            parentNode.ReplaceChild(firstChild.Clone(), xmlNodeLists[i]);
                        }
                    }
                    catch (Exception exception7)
                    {
                        Exception exception6 = exception7;
                        logItem.Status = ActionOperationStatus.Failed;
                        LogItem logItem9 = logItem;
                        string details8 = logItem9.Details;
                        object[] count = new object[]
                        {
                            "Error replacing node ", i + 1, "/", xmlNodeLists.Count, " : ", exception6.Message,
                            Environment.NewLine
                        };
                        string str11 = string.Concat(count);
                        string str12 = str11;
                        logItem.Information = str11;
                        logItem9.Details = string.Concat(details8, str12);
                        LogItem logItem10 = logItem;
                        string details9 = logItem10.Details;
                        string[] strArrays1 = new string[]
                        {
                            details9, "********** Extended Information **********", Environment.NewLine,
                            exception6.StackTrace, Environment.NewLine, "******************************************",
                            Environment.NewLine
                        };
                        logItem10.Details = string.Concat(strArrays1);
                        num1 = -1;
                    }
                }

                if (logItem.Status == ActionOperationStatus.Running && num1 == 0)
                {
                    propertyDescriptor_0.SetValue(record_0, xmlDocument.OuterXml);
                    record_0.CommitChanges();
                    logItem.Status = ActionOperationStatus.Completed;
                    num1 = 1;
                }
            }

            base.FireOperationFinished(logItem);
            return num1;
        }
    }
}