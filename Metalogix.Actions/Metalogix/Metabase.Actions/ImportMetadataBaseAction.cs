using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.Metabase;
using Metalogix.Metabase.DataTypes;
using Metalogix.Metabase.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Metalogix.Metabase.Actions
{
    public class ImportMetadataBaseAction : MetabaseAction<ImportFromCSVOptions>
    {
        protected PropertyDescriptorCollection _importableProperties =
            new PropertyDescriptorCollection(new PropertyDescriptor[0]);

        protected PropertyDescriptorCollection _exportableProperties =
            new PropertyDescriptorCollection(new PropertyDescriptor[0]);

        protected PropertyDescriptorCollection _metabaseProperties =
            new PropertyDescriptorCollection(new PropertyDescriptor[0]);

        protected List<KeyValuePair<PropertyDescriptor, Type>> _matchedColumns =
            new List<KeyValuePair<PropertyDescriptor, Type>>();

        protected List<int> _matchedColumnIndexes = new List<int>();

        protected Dictionary<int, KeyValuePair<string, Type>> _unmatchedColumns =
            new Dictionary<int, KeyValuePair<string, Type>>();

        protected List<PropertyDescriptor> _properties = new List<PropertyDescriptor>();

        protected ExplorerNode _explorerNode;

        protected Workspace _workspace;

        protected int _sourceURLIndex = -1;

        protected int _metalogixIDIndex = -1;

        protected string _displaySource = string.Empty;

        protected string _displayTarget = string.Empty;

        protected bool IsExcelFile { get; set; }

        public ImportMetadataBaseAction()
        {
        }

        protected void AddMatchedColumn(int index, PropertyDescriptor propertyDescriptor_0, Type columnType)
        {
            KeyValuePair<PropertyDescriptor, Type> keyValuePair =
                new KeyValuePair<PropertyDescriptor, Type>(propertyDescriptor_0, columnType);
            this._matchedColumns.Add(keyValuePair);
            this._matchedColumnIndexes.Add(index);
            if (!(propertyDescriptor_0 is NodePropertyDescriptor))
            {
                NodePropertyDescriptor nodePropertyDescriptor =
                    new NodePropertyDescriptor(propertyDescriptor_0, MetabasePDTarget.Record);
            }

            if (!this._properties.Contains(propertyDescriptor_0))
            {
                this._properties.Add(propertyDescriptor_0);
                return;
            }

            this._properties[index] = propertyDescriptor_0;
        }

        protected void AddUnmatchedColumn(int index, string columnName, Type columnType)
        {
            KeyValuePair<string, Type> keyValuePair = new KeyValuePair<string, Type>(columnName, columnType);
            this._unmatchedColumns.Add(index, keyValuePair);
            NodePropertyDescriptor nodePropertyDescriptor =
                new NodePropertyDescriptor(new RecordPropertyDescriptor(columnName, columnType),
                    MetabasePDTarget.Record);
            this._properties.Add(nodePropertyDescriptor);
        }

        protected void CreateNewColumns(LogItem operation)
        {
            if (!this.ActionOptions.CreateNewColumns)
            {
                return;
            }

            foreach (int key in this._unmatchedColumns.Keys)
            {
                KeyValuePair<string, Type> item = this._unmatchedColumns[key];
                string str = this.RectifyColumn(item.Key);
                Type value = item.Value;
                PropertyDescriptor propertyDescriptor = null;
                if (str != "MetalogixID")
                {
                    LogItem logItem = new LogItem("Adding Property Column", str, string.Empty, this._displayTarget,
                        ActionOperationStatus.Running);
                    base.FireOperationStarted(logItem);
                    LogItem logItem1 = operation;
                    string details = logItem1.Details;
                    string str1 = string.Concat("Adding property column \"", str, "\"", Environment.NewLine);
                    string str2 = str1;
                    operation.Information = str1;
                    logItem1.Details = string.Concat(details, str2);
                    base.FireOperationUpdated(operation);
                    try
                    {
                        propertyDescriptor =
                            this._workspace.AddProperty(str, value, "Misc", null, null, false, false, true);
                        LogItem logItem2 = logItem;
                        string details1 = logItem2.Details;
                        string str3 = string.Concat("Property column added successfully", Environment.NewLine);
                        string str4 = str3;
                        logItem.Information = str3;
                        logItem2.Details = string.Concat(details1, str4);
                        logItem.Status = ActionOperationStatus.Completed;
                    }
                    catch (Exception exception1)
                    {
                        Exception exception = exception1;
                        LogItem logItem3 = logItem;
                        string details2 = logItem3.Details;
                        string str5 = string.Concat("Error: ", exception.Message, Environment.NewLine);
                        string str6 = str5;
                        logItem.Information = str5;
                        logItem3.Details = string.Concat(details2, str6);
                        LogItem logItem4 = logItem;
                        string details3 = logItem4.Details;
                        string[] newLine = new string[]
                        {
                            details3, "********** Extended Information **********", Environment.NewLine,
                            exception.StackTrace, Environment.NewLine, "******************************************",
                            Environment.NewLine
                        };
                        logItem4.Details = string.Concat(newLine);
                        logItem.Status = ActionOperationStatus.Failed;
                    }

                    base.FireOperationFinished(logItem);
                }

                if (propertyDescriptor == null)
                {
                    continue;
                }

                this.AddMatchedColumn(key, new NodePropertyDescriptor(propertyDescriptor, MetabasePDTarget.Record),
                    value);
            }

            this._workspace.CommitChanges();
        }

        protected virtual Type DetermineColumnType(int columnIndex, ref string columnName)
        {
            return null;
        }

        protected void EnsureType(ref Type type)
        {
            if (type != typeof(string) && type != typeof(DateTime) && type != typeof(int) && type != typeof(Url) &&
                type != typeof(TextMoniker) && type != typeof(bool) && type != typeof(decimal))
            {
                type = typeof(string);
            }
        }

        protected void GetColumnTypeFromColumnHeader(ref string columnName, out Type columnType)
        {
            columnType = typeof(string);
            int num = columnName.LastIndexOf('(');
            if (num > 0)
            {
                try
                {
                    string str = columnName.Substring(num + 1, columnName.Length - num - 2);
                    columnType = Type.GetType(str);
                    if (columnType == null)
                    {
                        AssemblyName[] referencedAssemblies = Assembly.GetExecutingAssembly().GetReferencedAssemblies();
                        for (int i = 0; i < (int)referencedAssemblies.Length; i++)
                        {
                            AssemblyName assemblyName = referencedAssemblies[i];
                            columnType = Assembly.Load(assemblyName).GetType(str);
                            if (columnType != null)
                            {
                                break;
                            }
                        }
                    }

                    columnName = columnName.Substring(0, num).Trim();
                }
                catch
                {
                }
            }
        }

        protected Type GetColumnTypeFromColumnValue(int columnIndex, Type determinedType, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            bool flag = false;
            if (bool.TryParse(value, out flag) && (determinedType == typeof(bool) || determinedType == null))
            {
                determinedType = typeof(bool);
                return determinedType;
            }

            int num = 0;
            if (int.TryParse(value, out num) && (determinedType == typeof(int) || determinedType == null))
            {
                determinedType = typeof(int);
                return determinedType;
            }

            decimal num1 = new decimal(0);
            if (decimal.TryParse(value, out num1) && (determinedType == typeof(decimal) || determinedType == null))
            {
                determinedType = typeof(decimal);
                return determinedType;
            }

            DateTime dateTime = new DateTime();
            if (DateTime.TryParse(value, out dateTime) &&
                (determinedType == typeof(DateTime) || determinedType == null))
            {
                determinedType = typeof(DateTime);
                return determinedType;
            }

            Uri uri = null;
            try
            {
                uri = new Uri(value);
            }
            catch
            {
            }

            if (uri != null && (determinedType == typeof(Url) || determinedType == null))
            {
                determinedType = typeof(Url);
                return determinedType;
            }

            determinedType = (value.Length > 256 ? typeof(TextMoniker) : typeof(string));
            return determinedType;
        }

        // Metalogix.Metabase.Actions.ImportMetadataBaseAction
        protected string GetDataString(string fileName)
        {
            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            byte[] array = new byte[4];
            fileStream.Read(array, 0, 4);
            Encoding encoding;
            int num;
            if (array[0] == 239 && array[1] == 187 && array[2] == 191)
            {
                encoding = new UTF8Encoding();
                num = 3;
            }
            else if ((array[0] == 254 && array[1] == 255) || (array[1] == 254 && array[0] == 255))
            {
                encoding = new UnicodeEncoding();
                num = 2;
            }
            else if ((array[0] == 0 && array[1] == 0 && array[2] == 254 && array[3] == 255) ||
                     (array[3] == 0 && array[2] == 0 && array[1] == 254 && array[0] == 255))
            {
                encoding = new UTF32Encoding();
                num = 4;
            }
            else
            {
                encoding = new ASCIIEncoding();
                num = 0;
            }

            byte[] array2 = new byte[fileStream.Length - (long)num];
            fileStream.Position = (long)num;
            fileStream.Read(array2, 0, array2.Length - 1);
            fileStream.Flush();
            fileStream.Close();
            return encoding.GetString(array2);
        }

        protected int GetKeyColumnIndex(LogItem operation)
        {
            if (this.ActionOptions.CreateNewColumns)
            {
                this.CreateNewColumns(operation);
            }

            int num = (this.ActionOptions.IgnoreMetalogixId || this._metalogixIDIndex == -1
                ? this._sourceURLIndex
                : this._metalogixIDIndex);
            if (num == -1)
            {
                LogItem logItem = operation;
                string details = logItem.Details;
                string str =
                    string.Concat(
                        "Error: Could not find any key column. Please ensure your Excel file includes 'SourceURL' or ''MetalogixID' columns, or both.",
                        Environment.NewLine);
                string str1 = str;
                operation.Information = str;
                logItem.Details = string.Concat(details, str1);
                operation.Status = ActionOperationStatus.Failed;
            }

            return num;
        }

        protected virtual void Import(LogItem operation)
        {
            LogItem logItem = operation;
            string details = logItem.Details;
            string str = string.Concat("Importing Items", Environment.NewLine);
            string str1 = str;
            operation.Information = str;
            logItem.Details = string.Concat(details, str1);
            base.FireOperationUpdated(operation);
            this.InitializeMembers();
        }

        protected ActionOperationStatus ImportItem(int index, string[] columnValues, int int_0,
            out LogItem itemOperation, out Record record)
        {
            LogItem logItem;
            PropertyDescriptor item;
            if (!this.ActionOptions.FailureLogging)
            {
                logItem = new LogItem("Import Item", index.ToString(), string.Empty, this._displayTarget,
                    ActionOperationStatus.Running);
            }
            else
            {
                logItem = null;
            }

            itemOperation = logItem;
            if (itemOperation != null)
            {
                LogItem logItem1 = itemOperation;
                string details = logItem1.Details;
                string str = string.Concat("Searching for existing Record", Environment.NewLine);
                string str1 = str;
                itemOperation.Information = str;
                logItem1.Details = string.Concat(details, str1);
                base.FireOperationStarted(itemOperation);
            }

            record = null;
            try
            {
                if (int_0 == this._sourceURLIndex)
                {
                    record = (Record)this._workspace.Records.FindItem(this._properties[int_0], columnValues[int_0]);
                }
                else
                {
                    record = this._workspace.Records.FindByGUID(columnValues[int_0]);
                }
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                if (itemOperation == null)
                {
                    itemOperation = new LogItem("Import Item", index.ToString(), string.Empty, this._displayTarget,
                        ActionOperationStatus.Running);
                    base.FireOperationStarted(itemOperation);
                }

                LogItem logItem2 = itemOperation;
                string details1 = logItem2.Details;
                string str2 = string.Concat("Error: ", exception.Message, Environment.NewLine);
                string str3 = str2;
                itemOperation.Information = str2;
                logItem2.Details = string.Concat(details1, str3);
                LogItem logItem3 = itemOperation;
                logItem3.Details = string.Concat(logItem3.Details, exception.StackTrace, Environment.NewLine);
                itemOperation.Status = ActionOperationStatus.Failed;
                base.FireOperationFinished(itemOperation);
                return ActionOperationStatus.Failed;
            }

            if (record == null)
            {
                if (!this.ActionOptions.AddNewRows)
                {
                    if (itemOperation != null)
                    {
                        LogItem logItem4 = itemOperation;
                        string details2 = logItem4.Details;
                        string str4 = string.Concat("Skipped: Cannot find a row with ", this._properties[int_0].Name,
                            "\".", Environment.NewLine);
                        string str5 = str4;
                        itemOperation.Information = str4;
                        logItem4.Details = string.Concat(details2, str5);
                        itemOperation.Status = ActionOperationStatus.Skipped;
                        base.FireOperationFinished(itemOperation);
                    }

                    return ActionOperationStatus.Skipped;
                }

                if (itemOperation != null)
                {
                    LogItem logItem5 = itemOperation;
                    string details3 = logItem5.Details;
                    string str6 = string.Concat("Adding new Item", Environment.NewLine);
                    string str7 = str6;
                    itemOperation.Information = str6;
                    logItem5.Details = string.Concat(details3, str7);
                    base.FireOperationUpdated(itemOperation);
                }

                if (this._sourceURLIndex >= 0)
                {
                    item = this._properties[this._sourceURLIndex];
                }
                else
                {
                    item = null;
                }

                if (item == null)
                {
                    if (itemOperation == null)
                    {
                        itemOperation = new LogItem("Import Item", index.ToString(), string.Empty, this._displayTarget,
                            ActionOperationStatus.Running);
                        base.FireOperationStarted(itemOperation);
                    }

                    LogItem logItem6 = itemOperation;
                    string details4 = logItem6.Details;
                    string str8 = string.Concat("Error: SourceURL property required to create new Item",
                        Environment.NewLine);
                    string str9 = str8;
                    itemOperation.Information = str8;
                    logItem6.Details = string.Concat(details4, str9);
                    itemOperation.Status = ActionOperationStatus.Failed;
                    base.FireOperationFinished(itemOperation);
                    return ActionOperationStatus.Failed;
                }

                record = (Record)this._workspace.Records.AddNew();
                record.SourceURL = new Url(columnValues[this._sourceURLIndex]);
            }

            return ActionOperationStatus.Running;
        }

        protected ActionOperationStatus ImportItemMetadata(int index, string[] columnValues, int int_0,
            int sourceURLIndex, LogItem itemOperation, Record record)
        {
            string str = (sourceURLIndex > 0 ? columnValues[sourceURLIndex] : string.Empty);
            str = string.Format("{0} (Ln {1})", str, index);
            if (itemOperation != null)
            {
                LogItem logItem = itemOperation;
                string details = logItem.Details;
                string str1 = string.Concat("Importing item properties", Environment.NewLine);
                string str2 = str1;
                itemOperation.Information = str1;
                logItem.Details = string.Concat(details, str2);
                itemOperation.ItemName = str;
                base.FireOperationUpdated(itemOperation);
            }

            ActionOperationStatus actionOperationStatu =
                (itemOperation != null ? itemOperation.Status : ActionOperationStatus.Running);
            for (int i = 0; i < this._properties.Count; i++)
            {
                if (i != int_0 && this._properties[i] != null && !(this._properties[i].Name == "MetalogixID"))
                {
                    try
                    {
                        PropertyDescriptor item = this._properties[i];
                        if (item != null)
                        {
                            object value = item.GetValue(record);
                            string str3 = columnValues[i];
                            if (typeof(ISmartDataType).IsAssignableFrom(item.PropertyType))
                            {
                                this.SetPropertyValue(item, record, str3);
                            }
                            else if ((value == null || value.ToString() != str3) &&
                                     (this.ActionOptions.OverwriteFullRows || !string.IsNullOrEmpty(str3) &&
                                         !string.IsNullOrEmpty(str3.Trim())))
                            {
                                if (item.Name == "MigratedURL")
                                {
                                    str3 = " ";
                                }

                                this.SetPropertyValue(item, record, str3);
                            }
                        }
                    }
                    catch (Exception exception1)
                    {
                        Exception exception = exception1;
                        if (itemOperation == null)
                        {
                            itemOperation = new LogItem("Import Item", str, string.Empty, this._displayTarget,
                                ActionOperationStatus.Running);
                            base.FireOperationStarted(itemOperation);
                        }

                        LogItem logItem1 = itemOperation;
                        string details1 = logItem1.Details;
                        string[] name = new string[]
                        {
                            "Warning: Could not edit property (", this._properties[i].Name,
                            "). Encountered exception: ", exception.Message, Environment.NewLine
                        };
                        string str4 = string.Concat(name);
                        string str5 = str4;
                        itemOperation.Information = str4;
                        logItem1.Details = string.Concat(details1, str5);
                        itemOperation.Status = ActionOperationStatus.Warning;
                        base.FireOperationUpdated(itemOperation);
                        actionOperationStatu = ActionOperationStatus.Warning;
                    }
                }
            }

            record.CommitChanges();
            if (itemOperation != null)
            {
                if (itemOperation.Status == ActionOperationStatus.Running)
                {
                    LogItem logItem2 = itemOperation;
                    string details2 = logItem2.Details;
                    string str6 = string.Concat("Item imported successfully", Environment.NewLine);
                    string str7 = str6;
                    itemOperation.Information = str6;
                    logItem2.Details = string.Concat(details2, str7);
                    itemOperation.Status = ActionOperationStatus.Completed;
                    actionOperationStatu = ActionOperationStatus.Completed;
                }

                base.FireOperationFinished(itemOperation);
            }

            return actionOperationStatu;
        }

        private void InitializeMembers()
        {
            this._matchedColumns.Clear();
            this._matchedColumnIndexes.Clear();
            this._unmatchedColumns.Clear();
            this._properties.Clear();
            this._metabaseProperties = this._explorerNode.GetProperties();
        }

        protected string LocalFilePathFromUrl(string sLocalRootDir, string sUrl)
        {
            Uri uri = new Uri(sUrl);
            string str = Path.Combine(sLocalRootDir, string.Concat(uri.Host, uri.LocalPath));
            if (string.IsNullOrEmpty(Path.GetFileName(str)))
            {
                str = Path.Combine(str, "_index");
            }

            FileInfo fileInfo = new FileInfo(str);
            if (!fileInfo.Directory.Exists)
            {
                fileInfo.Directory.Create();
            }

            return fileInfo.FullName;
        }

        protected string[] QualifiedSplit(string toSplit, char[] delimiters, QualifiedSplitMode mode)
        {
            int num = toSplit.IndexOf('\"');
            if (num < 0)
            {
                return toSplit.Split(delimiters);
            }

            List<int> nums = new List<int>();
            List<string> strs = new List<string>();
            while (num >= 0)
            {
                nums.Add(num);
                num = toSplit.IndexOf('\"', num + 1);
            }

            int num1 = 0;
            int item = nums[0];
            int num2 = (nums.Count > 1 ? nums[num1 + 1] : toSplit.Length);
            num1 += 2;
            int num3 = 0;
            int num4 = toSplit.IndexOfAny(delimiters);
            while (num4 >= 0)
            {
                if (num4 < item)
                {
                    strs.Add(toSplit.Substring(num3, num4 - num3));
                    num3 = num4 + 1;
                    num4 = toSplit.IndexOfAny(delimiters, num4 + 1);
                }
                else if (num4 <= num2)
                {
                    num4 = toSplit.IndexOfAny(delimiters, num4 + 1);
                }
                else
                {
                    item = (nums.Count > num1 ? nums[num1] : toSplit.Length);
                    num2 = (nums.Count > num1 + 1 ? nums[num1 + 1] : toSplit.Length);
                    num1 += 2;
                }
            }

            strs.Add(toSplit.Substring(num3));
            char[] chrArray = new char[] { '\r', default(char) };
            for (int i = 0; i < strs.Count; i++)
            {
                strs[i] = strs[i].TrimEnd(chrArray);
            }

            if (mode == QualifiedSplitMode.SPLIT_COLUMNS)
            {
                for (int j = 0; j < strs.Count; j++)
                {
                    string str = strs[j];
                    if (str.StartsWith('\"'.ToString()) && str.EndsWith('\"'.ToString()))
                    {
                        strs[j] = str.Substring(1, str.Length - 2);
                    }
                }

                for (int k = 0; k < strs.Count; k++)
                {
                    strs[k] = strs[k].Replace("\"\"", "\"");
                }
            }

            return strs.ToArray();
        }

        protected string RectifyColumn(string sVal)
        {
            sVal = sVal.Replace("\n", "");
            sVal = sVal.Replace("\t", "");
            sVal = sVal.Replace("\r", "");
            sVal = sVal.Replace("\0", "");
            return sVal;
        }

        protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
        {
            this.RunOperation(new object[] { target });
        }

        protected override void RunOperation(object[] oParams)
        {
            LogItem logItem = new LogItem("Importing data", string.Empty, string.Empty, string.Empty,
                ActionOperationStatus.Running);
            base.FireOperationStarted(logItem);
            IXMLAbleList xMLAbleLists = oParams[0] as IXMLAbleList;
            this._explorerNode = xMLAbleLists[0] as ExplorerNode;
            if (this._explorerNode == null)
            {
                throw new Exception("Importing data can only be run on a node.");
            }

            this._workspace = this._explorerNode.Connection.Node.Record.ParentWorkspace;
            if (this._workspace == null)
            {
                LogItem logItem1 = logItem;
                string details = logItem1.Details;
                string str = string.Concat("Workspace could not be found. It may have been deleted.",
                    Environment.NewLine);
                string str1 = str;
                logItem.Information = str;
                logItem1.Details = string.Concat(details, str1);
                logItem.Status = ActionOperationStatus.MissingOnTarget;
                base.FireOperationFinished(logItem);
                return;
            }

            this._displaySource = this.ActionOptions.SourceFileName;
            this._displayTarget = this._explorerNode.Connection.Node.Url;
            logItem.Target = this._displayTarget;
            base.FireOperationUpdated(logItem);
            base.FireSourceLinkChanged(this._displaySource);
            this.ActionOptions.AddNewRows = true;
            this.Import(logItem);
            base.FireOperationFinished(logItem);
        }

        protected string SetMatchedColumns(int int_0, string columnName)
        {
            Type type = this.DetermineColumnType(int_0, ref columnName);
            PropertyDescriptor item = this._metabaseProperties[columnName];
            if (item == null)
            {
                item = this._importableProperties[columnName];
                if (item == null)
                {
                    item = this._exportableProperties[columnName];
                    if (item == null)
                    {
                        this.EnsureType(ref type);
                        this.AddUnmatchedColumn(int_0, columnName, type);
                    }
                    else if (!item.IsReadOnly)
                    {
                        this.AddUnmatchedColumn(int_0, columnName, type);
                    }
                    else
                    {
                        this.AddMatchedColumn(int_0, item, type);
                    }
                }
                else
                {
                    this.AddMatchedColumn(int_0, item, type);
                }
            }
            else
            {
                this.EnsureType(ref type);
                this.AddMatchedColumn(int_0, item, type);
            }

            return columnName;
        }

        protected void SetPropertyValue(PropertyDescriptor propertyDescriptor_0, Record record_0, object metadataValue)
        {
            if (metadataValue != null)
            {
                if (!this.IsExcelFile && propertyDescriptor_0.PropertyType == typeof(DateTime) &&
                    !(metadataValue is DateTime))
                {
                    DateTime now = DateTime.Now;
                    if (!DateTime.TryParseExact(metadataValue.ToString(), "yyyy-MM-ddtthh:mm:ssz", null,
                            DateTimeStyles.None, out now))
                    {
                        metadataValue = null;
                    }
                    else
                    {
                        if (now.Year < 1753)
                        {
                            now = now.AddYears(1753 - now.Year);
                        }

                        metadataValue = now;
                    }
                }
                else if (typeof(ISmartDataType).IsAssignableFrom(propertyDescriptor_0.PropertyType))
                {
                    RecordPropertyDescriptor item =
                        record_0.GetProperties()[propertyDescriptor_0.Name] as RecordPropertyDescriptor;
                    if (item == null)
                    {
                        metadataValue = null;
                    }
                    else
                    {
                        string str = metadataValue as string;
                        metadataValue = DataTypeUtils.CreateInstance(propertyDescriptor_0.PropertyType, item);
                        (metadataValue as ISmartDataType).DeserializeFromUserFriendlyString(str);
                    }
                }
                else if (!typeof(TextMoniker).IsAssignableFrom(propertyDescriptor_0.PropertyType))
                {
                    metadataValue = Record.DeserializeValue(record_0, propertyDescriptor_0.Name, metadataValue,
                        propertyDescriptor_0.PropertyType);
                }
                else
                {
                    string str1 = metadataValue as string;
                    metadataValue = Record.DeserializeValue(record_0, propertyDescriptor_0.Name, metadataValue,
                        propertyDescriptor_0.PropertyType);
                    TextMoniker textMoniker = metadataValue as TextMoniker;
                    if (textMoniker != null && str1 != null)
                    {
                        textMoniker.SetFullText(str1);
                    }
                }
            }

            if (metadataValue != null && propertyDescriptor_0 is RecordPropertyDescriptor &&
                propertyDescriptor_0.PropertyType == typeof(string) && metadataValue.ToString().Length > 256)
            {
                record_0.ParentWorkspace.ExpandPropertyStorage((RecordPropertyDescriptor)propertyDescriptor_0);
                propertyDescriptor_0 =
                    record_0.ParentWorkspace.Records.GetItemProperties(null)[propertyDescriptor_0.Name];
            }

            if (!propertyDescriptor_0.IsReadOnly ||
                typeof(TextMoniker).IsAssignableFrom(propertyDescriptor_0.PropertyType))
            {
                propertyDescriptor_0.SetValue(record_0, metadataValue);
            }
        }

        protected void SetSourceUrlAndMetalogixIDIndex()
        {
            this._sourceURLIndex = -1;
            this._metalogixIDIndex = -1;
            for (int i = 0; i < this._properties.Count; i++)
            {
                PropertyDescriptor item = this._properties[i];
                if (item != null)
                {
                    if (item.Name == "SourceURL")
                    {
                        this._sourceURLIndex = i;
                    }

                    if (item.Name == "MetalogixID")
                    {
                        this._metalogixIDIndex = i;
                    }
                }
            }
        }
    }
}