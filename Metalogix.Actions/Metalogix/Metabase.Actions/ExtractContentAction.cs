using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.Licensing;
using Metalogix.Metabase;
using Metalogix.Metabase.DataTypes;
using Metalogix.Metabase.Interfaces;
using Metalogix.Metabase.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Metalogix.Metabase.Actions
{
    [LaunchAsJob(true)]
    [LicensedProducts(ProductFlags.UnifiedContentMatrixKey)]
    [RunAsync(true)]
    [ShowStatusDialog(true)]
    [SourceCardinality(Cardinality.Zero)]
    [SupportsThreeStateConfiguration(true)]
    [TargetCardinality(Cardinality.OneOrMore)]
    [TargetType(typeof(IHasRecords))]
    [UsesStickySettings(true)]
    public class ExtractContentAction : MetabaseAction<ExtractContentOptions>
    {
        protected LogItem m_itemOperation;

        protected int m_iNonMatches;

        public ExtractContentAction()
        {
        }

        protected virtual bool ExtractContent(ExplorerNode item)
        {
            this.m_itemOperation = new LogItem(this.Name, item.DisplayName, string.Empty,
                this.ActionOptions.TargetProperty, ActionOperationStatus.Running);
            base.FireOperationStarted(this.m_itemOperation);
            PropertyDescriptor propertyDescriptor = item.GetProperties().Find(this.ActionOptions.SourceProperty, false);
            if (propertyDescriptor == null)
            {
                LogItem mItemOperation = this.m_itemOperation;
                string details = mItemOperation.Details;
                LogItem logItem = this.m_itemOperation;
                string str = string.Concat("Source property could not be found", Environment.NewLine);
                string str1 = str;
                logItem.Information = str;
                mItemOperation.Details = string.Concat(details, str1);
                this.m_itemOperation.Status = ActionOperationStatus.Failed;
                base.FireOperationFinished(this.m_itemOperation);
                return false;
            }

            object value = propertyDescriptor.GetValue(item);
            if (value == null)
            {
                this.m_itemOperation.Status = ActionOperationStatus.Skipped;
                base.FireOperationFinished(this.m_itemOperation);
                return true;
            }

            if (!this.ActionOptions.SearchHtml)
            {
                this.m_itemOperation.Source = propertyDescriptor.Name;
            }
            else
            {
                this.m_itemOperation.Source = value.ToString();
            }

            base.FireOperationUpdated(this.m_itemOperation);
            PropertyDescriptor propertyDescriptor1 =
                item.GetProperties().Find(this.ActionOptions.TargetProperty, false);
            if (propertyDescriptor1 == null)
            {
                LogItem mItemOperation1 = this.m_itemOperation;
                string details1 = mItemOperation1.Details;
                LogItem logItem1 = this.m_itemOperation;
                string str2 = string.Concat("Target property could not be found", Environment.NewLine);
                string str3 = str2;
                logItem1.Information = str2;
                mItemOperation1.Details = string.Concat(details1, str3);
                this.m_itemOperation.Status = ActionOperationStatus.Failed;
                base.FireOperationFinished(this.m_itemOperation);
                return false;
            }

            List<string> strs = null;
            switch (this.ActionOptions.Logic)
            {
                case ExtractContentOptions.SearchLogic.XPath:
                {
                    strs = this.SearchXPath(item, value, propertyDescriptor.Name);
                    break;
                }
                case ExtractContentOptions.SearchLogic.RegEx:
                {
                    strs = this.SearchRegex(value);
                    break;
                }
                case ExtractContentOptions.SearchLogic.Text:
                {
                    strs = this.SearchText(value);
                    break;
                }
            }

            if (strs.Count == 0)
            {
                this.m_iNonMatches++;
            }

            if (this.ActionOptions.Detail == ExtractContentOptions.ReturnDetail.CountsOnly)
            {
                LogItem mItemOperation2 = this.m_itemOperation;
                string details2 = mItemOperation2.Details;
                LogItem logItem2 = this.m_itemOperation;
                string str4 = string.Concat("Writing count to ", propertyDescriptor1.Name, Environment.NewLine);
                string str5 = str4;
                logItem2.Information = str4;
                mItemOperation2.Details = string.Concat(details2, str5);
                base.FireOperationUpdated(this.m_itemOperation);
                propertyDescriptor1.SetValue(item, strs.Count.ToString());
                LogItem mItemOperation3 = this.m_itemOperation;
                string details3 = mItemOperation3.Details;
                LogItem logItem3 = this.m_itemOperation;
                string str6 = string.Concat("Extract Item Content finished successfully", Environment.NewLine);
                string str7 = str6;
                logItem3.Information = str6;
                mItemOperation3.Details = string.Concat(details3, str7);
            }
            else if (strs.Count != 0)
            {
                this.m_itemOperation.AddCompletionDetail("Items Matched", 1L);
                LogItem mItemOperation4 = this.m_itemOperation;
                string details4 = mItemOperation4.Details;
                LogItem logItem4 = this.m_itemOperation;
                string[] name = new string[] { "Writing query ", null, null, null, null };
                name[1] = (strs.Count == 1 ? "result" : "results");
                name[2] = " to ";
                name[3] = propertyDescriptor1.Name;
                name[4] = Environment.NewLine;
                string str8 = string.Concat(name);
                string str9 = str8;
                logItem4.Information = str8;
                mItemOperation4.Details = string.Concat(details4, str9);
                base.FireOperationUpdated(this.m_itemOperation);
                StringBuilder stringBuilder = new StringBuilder(1024);
                stringBuilder.Append(strs[0]);
                for (int i = 1; i < strs.Count; i++)
                {
                    stringBuilder.Append(this.ActionOptions.Separator);
                    stringBuilder.Append(strs[i]);
                }

                if (propertyDescriptor1.PropertyType != typeof(TextMoniker))
                {
                    string str10 = stringBuilder.ToString();
                    ExtractContentAction.ValidatePropertyType(propertyDescriptor1.PropertyType, str10);
                    propertyDescriptor1.SetValue(item, str10);
                }
                else
                {
                    ((TextMoniker)propertyDescriptor1.GetValue(item)).SetFullText(stringBuilder.ToString());
                }

                LogItem mItemOperation5 = this.m_itemOperation;
                string details5 = mItemOperation5.Details;
                LogItem logItem5 = this.m_itemOperation;
                string str11 = string.Concat("Extract Item Content from URL finished successfully",
                    Environment.NewLine);
                string str12 = str11;
                logItem5.Information = str11;
                mItemOperation5.Details = string.Concat(details5, str12);
            }
            else
            {
                propertyDescriptor1.SetValue(item, string.Empty);
                this.m_itemOperation.AddCompletionDetail("Items Unmatched", 1L);
                LogItem mItemOperation6 = this.m_itemOperation;
                string details6 = mItemOperation6.Details;
                LogItem logItem6 = this.m_itemOperation;
                string str13 = string.Concat("No results found", Environment.NewLine);
                string str14 = str13;
                logItem6.Information = str13;
                mItemOperation6.Details = string.Concat(details6, str14);
            }

            item.Record.ParentWorkspace.CommitChanges();
            this.m_itemOperation.Status = ActionOperationStatus.Completed;
            base.FireOperationFinished(this.m_itemOperation);
            this.m_itemOperation = null;
            return true;
        }

        private static string PrettyPrint(XmlNode xmlNode)
        {
            StringWriter stringWriter = new StringWriter();
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter)
            {
                IndentChar = ' ',
                Indentation = 2,
                Formatting = Formatting.Indented
            };
            xmlNode.WriteTo(xmlTextWriter);
            return stringWriter.ToString();
        }

        protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
        {
            LogItem logItem = new LogItem("Initializing Action...", string.Empty, string.Empty, string.Empty,
                ActionOperationStatus.Running);
            base.FireOperationStarted(logItem);
            if (target.Count == 0)
            {
                LogItem logItem1 = logItem;
                string details = logItem1.Details;
                string str = string.Concat("No Records selected for extraction", Environment.NewLine);
                string str1 = str;
                logItem.Information = str;
                logItem1.Details = string.Concat(details, str1);
                logItem.Status = ActionOperationStatus.MissingOnSource;
                base.FireOperationFinished(logItem);
                return;
            }

            int num = 0;
            for (int i = 0; i < target.Count; i++)
            {
                if (base.CheckForAbort())
                {
                    return;
                }

                try
                {
                    if (!this.ExtractContent((ExplorerNode)target[i]))
                    {
                        num++;
                    }
                }
                catch (Exception exception1)
                {
                    Exception exception = exception1;
                    num++;
                    if (this.m_itemOperation != null)
                    {
                        LogItem mItemOperation = this.m_itemOperation;
                        string details1 = mItemOperation.Details;
                        LogItem mItemOperation1 = this.m_itemOperation;
                        string str2 = string.Concat("Error: ", exception.Message, Environment.NewLine);
                        string str3 = str2;
                        mItemOperation1.Information = str2;
                        mItemOperation.Details = string.Concat(details1, str3);
                        LogItem mItemOperation2 = this.m_itemOperation;
                        string details2 = mItemOperation2.Details;
                        string[] newLine = new string[]
                        {
                            details2, "********** Extended Information **********", Environment.NewLine,
                            exception.StackTrace, Environment.NewLine, "******************************************",
                            Environment.NewLine
                        };
                        mItemOperation2.Details = string.Concat(newLine);
                        this.m_itemOperation.Status = ActionOperationStatus.Failed;
                        base.FireOperationFinished(this.m_itemOperation);
                        this.m_itemOperation = null;
                    }
                }
            }

            if (num <= 0)
            {
                LogItem logItem2 = logItem;
                string details3 = logItem2.Details;
                string str4 = string.Concat("Extraction completed successfully", Environment.NewLine);
                string str5 = str4;
                logItem.Information = str4;
                logItem2.Details = string.Concat(details3, str5);
                logItem.Status = ActionOperationStatus.Completed;
            }
            else
            {
                LogItem logItem3 = logItem;
                string details4 = logItem3.Details;
                LogItem logItem4 = logItem;
                object[] objArray = new object[] { "Extraction completed with ", num, null, null };
                objArray[2] = (num == 1 ? " failure" : " failures");
                objArray[3] = Environment.NewLine;
                string str6 = string.Concat(objArray);
                string str7 = str6;
                logItem4.Information = str6;
                logItem3.Details = string.Concat(details4, str7);
                logItem.Status = ActionOperationStatus.Failed;
            }

            base.FireOperationFinished(logItem);
        }

        protected override void RunOperation(object[] oParams)
        {
            if (oParams == null || (int)oParams.Length < 1)
            {
                throw new Exception(string.Format("{0} is missing parameters", this.Name));
            }

            IXMLAbleList xMLAbleLists = oParams[0] as IXMLAbleList;
            this.RunAction(new XMLAbleList(), xMLAbleLists);
        }

        protected virtual List<string> SearchRegex(object oSourceValue)
        {
            if (this.m_itemOperation != null)
            {
                LogItem mItemOperation = this.m_itemOperation;
                string details = mItemOperation.Details;
                LogItem logItem = this.m_itemOperation;
                string str = string.Concat("Running Regex query", Environment.NewLine);
                string str1 = str;
                logItem.Information = str;
                mItemOperation.Details = string.Concat(details, str1);
                base.FireOperationUpdated(this.m_itemOperation);
            }

            List<string> strs = new List<string>();
            TextMoniker textMoniker = oSourceValue as TextMoniker;
            string str2 = (textMoniker != null ? textMoniker.GetFullText() : oSourceValue.ToString());
            if (!this.ActionOptions.Multiline)
            {
                StringReader stringReader = new StringReader(str2);
                try
                {
                    do
                    {
                        Label0:
                        string str3 = stringReader.ReadLine();
                        string str4 = str3;
                        if (str3 == null)
                        {
                            break;
                        }

                        Match match = Regex.Match(str4, this.ActionOptions.SearchExpression);
                        if (match.Success)
                        {
                            strs.Add(match.Value);
                        }
                        else
                        {
                            goto Label0;
                        }
                    } while (this.ActionOptions.Detail != ExtractContentOptions.ReturnDetail.FirstMatch);
                }
                catch (ArgumentException argumentException1)
                {
                    ArgumentException argumentException = argumentException1;
                    if (this.m_itemOperation != null)
                    {
                        LogItem mItemOperation1 = this.m_itemOperation;
                        string details1 = mItemOperation1.Details;
                        LogItem logItem1 = this.m_itemOperation;
                        string str5 = string.Concat("Invalid RegEx syntax: ", argumentException.Message,
                            Environment.NewLine);
                        string str6 = str5;
                        logItem1.Information = str5;
                        mItemOperation1.Details = string.Concat(details1, str6);
                        LogItem mItemOperation2 = this.m_itemOperation;
                        string details2 = mItemOperation2.Details;
                        string[] newLine = new string[]
                        {
                            details2, "********** Extended Information **********", Environment.NewLine,
                            "The RegEx syntax you entered is invalid", Environment.NewLine,
                            "******************************************", Environment.NewLine
                        };
                        mItemOperation2.Details = string.Concat(newLine);
                        this.m_itemOperation.Status = ActionOperationStatus.Failed;
                        base.FireOperationFinished(this.m_itemOperation);
                        this.m_itemOperation = null;
                    }

                    stringReader.Close();
                }
            }
            else
            {
                MatchCollection matchCollections = Regex.Matches(str2, this.ActionOptions.SearchExpression,
                    RegexOptions.Multiline | RegexOptions.Singleline);
                if (matchCollections != null)
                {
                    IEnumerator enumerator = matchCollections.GetEnumerator();
                    try
                    {
                        do
                        {
                            Label1:
                            if (!enumerator.MoveNext())
                            {
                                break;
                            }

                            Match current = (Match)enumerator.Current;
                            if (current.Success)
                            {
                                strs.Add(current.Value);
                            }
                            else
                            {
                                goto Label1;
                            }
                        } while (this.ActionOptions.Detail != ExtractContentOptions.ReturnDetail.FirstMatch);
                    }
                    finally
                    {
                        IDisposable disposable = enumerator as IDisposable;
                        if (disposable != null)
                        {
                            disposable.Dispose();
                        }
                    }
                }
            }

            return strs;
        }

        protected virtual List<string> SearchText(object oURL)
        {
            if (this.m_itemOperation != null)
            {
                LogItem mItemOperation = this.m_itemOperation;
                string details = mItemOperation.Details;
                LogItem logItem = this.m_itemOperation;
                string str = string.Concat("Running Text query", Environment.NewLine);
                string str1 = str;
                logItem.Information = str;
                mItemOperation.Details = string.Concat(details, str1);
                base.FireOperationUpdated(this.m_itemOperation);
            }

            List<string> strs = new List<string>();
            TextMoniker textMoniker = oURL as TextMoniker;
            StringReader stringReader =
                new StringReader((textMoniker != null ? textMoniker.GetFullText() : oURL.ToString()));
            if (!this.ActionOptions.MatchCase)
            {
                do
                {
                    Label0:
                    string str2 = stringReader.ReadLine();
                    string str3 = str2;
                    if (str2 == null)
                    {
                        break;
                    }

                    if (str3.ToLower().Contains(this.ActionOptions.SearchExpression.ToLower()))
                    {
                        strs.Add(str3);
                    }
                    else
                    {
                        goto Label0;
                    }
                } while (this.ActionOptions.Detail != ExtractContentOptions.ReturnDetail.FirstMatch);
            }
            else
            {
                do
                {
                    Label1:
                    string str4 = stringReader.ReadLine();
                    string str5 = str4;
                    if (str4 == null)
                    {
                        break;
                    }
                    else if (str5.Contains(this.ActionOptions.SearchExpression))
                    {
                        strs.Add(str5);
                    }
                    else
                    {
                        goto Label1;
                    }
                } while (this.ActionOptions.Detail != ExtractContentOptions.ReturnDetail.FirstMatch);
            }

            stringReader.Close();
            return strs;
        }

        protected virtual List<string> SearchXPath(ExplorerNode item, object oSourceValue, string sSourceUrlPropName)
        {
            if (this.m_itemOperation != null)
            {
                LogItem mItemOperation = this.m_itemOperation;
                string details = mItemOperation.Details;
                LogItem logItem = this.m_itemOperation;
                string str = string.Concat("Running XPath query", Environment.NewLine);
                string str1 = str;
                logItem.Information = str;
                mItemOperation.Details = string.Concat(details, str1);
                base.FireOperationUpdated(this.m_itemOperation);
            }

            List<string> strs = new List<string>();
            TextMoniker textMoniker = oSourceValue as TextMoniker;
            string str2 = (textMoniker != null ? textMoniker.GetFullText() : oSourceValue.ToString());
            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                xmlDocument.LoadXml(str2);
            }
            catch (XmlException xmlException1)
            {
                XmlException xmlException = xmlException1;
                if (xmlException.Message.ToLower().Contains("multiple root elements"))
                {
                    xmlDocument.LoadXml(string.Concat("<XML>", str2, "</XML>"));
                    this.ActionOptions.SearchExpression = string.Concat("/XML", this.ActionOptions.SearchExpression);
                }
                else if (this.m_itemOperation != null)
                {
                    LogItem mItemOperation1 = this.m_itemOperation;
                    string details1 = mItemOperation1.Details;
                    LogItem logItem1 = this.m_itemOperation;
                    string str3 = string.Concat("Error: ", xmlException.Message, Environment.NewLine);
                    string str4 = str3;
                    logItem1.Information = str3;
                    mItemOperation1.Details = string.Concat(details1, str4);
                    LogItem mItemOperation2 = this.m_itemOperation;
                    string details2 = mItemOperation2.Details;
                    string[] newLine = new string[]
                    {
                        details2, "********** Extended Information **********", Environment.NewLine,
                        xmlException.StackTrace, Environment.NewLine, "******************************************",
                        Environment.NewLine
                    };
                    mItemOperation2.Details = string.Concat(newLine);
                }
            }

            IEnumerable enumerable = null;
            if (this.ActionOptions.Detail != ExtractContentOptions.ReturnDetail.FirstMatch)
            {
                enumerable = xmlDocument.SelectNodes(this.ActionOptions.SearchExpression);
            }
            else
            {
                XmlNode xmlNodes = xmlDocument.SelectSingleNode(this.ActionOptions.SearchExpression);
                if (xmlNodes != null)
                {
                    enumerable = new XmlNode[] { xmlNodes };
                }
            }

            if (enumerable != null)
            {
                foreach (XmlNode xmlNodes1 in enumerable)
                {
                    string innerText = null;
                    switch (this.ActionOptions.Format)
                    {
                        case ExtractContentOptions.ReturnFormat.Text:
                        {
                            innerText = xmlNodes1.InnerText;
                            break;
                        }
                        case ExtractContentOptions.ReturnFormat.InnerXML:
                        {
                            innerText = xmlNodes1.InnerXml;
                            break;
                        }
                        case ExtractContentOptions.ReturnFormat.OuterXML:
                        {
                            innerText = ExtractContentAction.PrettyPrint(xmlNodes1);
                            break;
                        }
                    }

                    strs.Add(innerText);
                }
            }

            return strs;
        }

        private static void ValidatePropertyType(Type type, string sValue)
        {
            int num;
            decimal num1;
            bool flag;
            DateTime dateTime;
            if (type != typeof(string))
            {
                if (type != typeof(TextMoniker))
                {
                    if (type == typeof(int))
                    {
                        if (!int.TryParse(sValue, out num))
                        {
                            string[] str = new string[]
                            {
                                "An object of type: \"", type.ToString(), "\" cannot be parsed from value: \"", sValue,
                                "\""
                            };
                            throw new ArgumentException(string.Concat(str));
                        }
                    }
                    else if (type == typeof(decimal))
                    {
                        if (!decimal.TryParse(sValue, out num1))
                        {
                            string[] strArrays = new string[]
                            {
                                "An object of type: \"", type.ToString(), "\" cannot be parsed from value: \"", sValue,
                                "\""
                            };
                            throw new ArgumentException(string.Concat(strArrays));
                        }
                    }
                    else if (type == typeof(bool))
                    {
                        if (!bool.TryParse(sValue, out flag))
                        {
                            string[] str1 = new string[]
                            {
                                "An object of type: \"", type.ToString(), "\" cannot be parsed from value: \"", sValue,
                                "\""
                            };
                            throw new ArgumentException(string.Concat(str1));
                        }
                    }
                    else if (type == typeof(DateTime))
                    {
                        if (!DateTime.TryParseExact(sValue, "M/d/yyyy H:mm:ss tt", new CultureInfo("", false),
                                DateTimeStyles.None, out dateTime))
                        {
                            string[] strArrays1 = new string[]
                            {
                                "An object of type: \"", type.ToString(), "\" cannot be parsed from value: \"", sValue,
                                "\""
                            };
                            throw new ArgumentException(string.Concat(strArrays1));
                        }
                    }
                    else if (type == typeof(Url))
                    {
                        try
                        {
                            if (new Uri(sValue) == null)
                            {
                                throw new Exception();
                            }
                        }
                        catch
                        {
                            string[] str2 = new string[]
                            {
                                "An object of type: \"", type.ToString(), "\" cannot be parsed from value: \"", sValue,
                                "\""
                            };
                            throw new ArgumentException(string.Concat(str2));
                        }
                    }

                    return;
                }
            }
        }
    }
}