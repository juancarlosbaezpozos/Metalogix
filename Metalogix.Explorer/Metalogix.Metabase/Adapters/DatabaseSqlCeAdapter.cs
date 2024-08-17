using Metalogix.Explorer;
using Metalogix.Metabase.Interfaces;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlServerCe;
using System.IO;
using System.Runtime.CompilerServices;

namespace Metalogix.Metabase.Adapters
{
    public class DatabaseSqlCeAdapter : DatabaseAdapter
    {
        public override string AdapterType
        {
            get { return DatabaseAdapterType.SqlCe.ToString(); }
        }

        public DatabaseSqlCeAdapter(string databaseFile) : base(databaseFile)
        {
        }

        public DatabaseSqlCeAdapter(string databaseFile, Metalogix.Metabase.Interfaces.AdapterCallWrapper wrapper) :
            base(databaseFile, wrapper)
        {
        }

        protected override DbConnection Initialize()
        {
            DbConnection dbConnection;
            try
            {
                SqlCeConnection sqlCeConnection1 = null;
                base.AdapterCallWrapper(new AdapterCallWrapperAction(() =>
                {
                    SqlCeConnection sqlCeConnection = this.NewOrOpenDB(this.AdapterContext);
                    this.InitializeCommands(sqlCeConnection);
                    sqlCeConnection1 = sqlCeConnection;
                }));
                dbConnection = sqlCeConnection1;
            }
            catch (SqlCeException sqlCeException)
            {
                if (sqlCeException.NativeError == 25011)
                {
                    throw new ArgumentException("The specified file is not of a supported type");
                }

                throw;
            }

            return dbConnection;
        }

        private void InitializeCommands(SqlCeConnection connection)
        {
            this.m_dataAdapterItems = new SqlCeDataAdapter();
            this.m_dataAdapterSingleItem = new SqlCeDataAdapter();
            this.m_dataAdapterItemProperties = new SqlCeDataAdapter();
            this.m_dataAdapterSingleItemProperties = new SqlCeDataAdapter();
            this.m_cmdBuilderItemProperties =
                new SqlCeCommandBuilder((SqlCeDataAdapter)this.m_dataAdapterItemProperties);
            this.m_dataAdapterWorkspaces = new SqlCeDataAdapter();
            this.m_cmdIdentity = new SqlCeCommand("SELECT @@Identity")
            {
                Connection = connection
            };
            this.m_cmdSelectItems = new SqlCeCommand();
            this.m_cmdSelectSingleItem = new SqlCeCommand();
            this.m_cmdSelectItemProperties = new SqlCeCommand();
            this.m_cmdSelectSingleItemProperties = new SqlCeCommand();
            this.m_oleDbCmdSelectWorkspaces = new SqlCeCommand();
            string str = "";
            string str1 = "";
            this.m_cmdSelectItems.CommandText = string.Concat("SELECT ", str,
                "Items.CreationDate, Items.ModificationDate, Items.ItemID, Items.ItemNum, Items.WorkspaceID FROM Items WHERE WorkspaceID = ? ORDER BY Items.ItemNum ASC");
            this.m_cmdSelectItems.Connection = connection;
            this.m_cmdSelectItems.Parameters.Add(new SqlCeParameter("WorkspaceID", SqlDbType.NVarChar, 36));
            this.m_dataAdapterItems.SelectCommand = this.m_cmdSelectItems;
            DataTableMappingCollection tableMappings = this.m_dataAdapterItems.TableMappings;
            DataTableMapping[] dataTableMapping = new DataTableMapping[1];
            DataColumnMapping[] dataColumnMapping = new DataColumnMapping[]
            {
                new DataColumnMapping("CreationDate", "CreationDate"),
                new DataColumnMapping("ModificationDAte", "ModificationDAte"), new DataColumnMapping("ItemID", "ItemID")
            };
            dataTableMapping[0] = new DataTableMapping("Table", "Items", dataColumnMapping);
            tableMappings.AddRange(dataTableMapping);
            this.m_cmdSelectMaxItemNum = new SqlCeCommand("SELECT MAX( ItemNum ) FROM Items")
            {
                Connection = connection
            };
            this.m_cmdInsertItems =
                new SqlCeCommand(
                    "Insert INTO Items( ItemID, ItemNum, WorkspaceID, CreationDate, ModificationDate ) VALUES ( ?, ?, ?, ?, ? )")
                {
                    Connection = connection
                };
            this.m_cmdInsertItems.Parameters.Add(new SqlCeParameter("@ItemID", SqlDbType.NVarChar, 36,
                ParameterDirection.Input, false, 0, 0, "ItemID", DataRowVersion.Proposed, null));
            this.m_cmdInsertItems.Parameters.Add(new SqlCeParameter("@ItemNum", SqlDbType.Int, 4,
                ParameterDirection.Input, false, 0, 0, "ItemNum", DataRowVersion.Proposed, null));
            this.m_cmdInsertItems.Parameters.Add(new SqlCeParameter("@WorkspaceID", SqlDbType.NVarChar, 36,
                ParameterDirection.Input, false, 0, 0, "WorkspaceID", DataRowVersion.Proposed, null));
            this.m_cmdInsertItems.Parameters.Add(new SqlCeParameter("@CreationDate", SqlDbType.DateTime, 4,
                ParameterDirection.Input, false, 0, 0, "CreationDate", DataRowVersion.Proposed, null));
            this.m_cmdInsertItems.Parameters.Add(new SqlCeParameter("@ModificationDate", SqlDbType.DateTime, 4,
                ParameterDirection.Input, false, 0, 0, "ModificationDate", DataRowVersion.Proposed, null));
            this.m_dataAdapterItems.InsertCommand = this.m_cmdInsertItems;
            this.m_cmdUpdateItems =
                new SqlCeCommand(
                    "UPDATE Items SET ItemNum = @ItemNum, WorkspaceID = @WorkspaceID, CreationDate = @CreationDate, ModificationDate = @ModificationDate WHERE ItemID = @ItemID")
                {
                    Connection = connection
                };
            this.m_cmdUpdateItems.Parameters.Add(new SqlCeParameter("@ItemNum", SqlDbType.Int, 4,
                ParameterDirection.Input, false, 0, 0, "ItemNum", DataRowVersion.Original, null));
            this.m_cmdUpdateItems.Parameters.Add(new SqlCeParameter("@WorkspaceID", SqlDbType.NVarChar, 36,
                ParameterDirection.Input, false, 0, 0, "WorkspaceID", DataRowVersion.Original, null));
            this.m_cmdUpdateItems.Parameters.Add(new SqlCeParameter("@CreationDate", SqlDbType.DateTime, 4,
                ParameterDirection.Input, false, 0, 0, "CreationDate", DataRowVersion.Original, null));
            this.m_cmdUpdateItems.Parameters.Add(new SqlCeParameter("@ModificationDate", SqlDbType.DateTime, 4,
                ParameterDirection.Input, false, 0, 0, "ModificationDate", DataRowVersion.Current, null));
            this.m_cmdUpdateItems.Parameters.Add(new SqlCeParameter("@ItemID", SqlDbType.NVarChar, 36,
                ParameterDirection.Input, false, 0, 0, "ItemID", DataRowVersion.Original, null));
            this.m_dataAdapterItems.UpdateCommand = this.m_cmdUpdateItems;
            this.m_cmdDeleteItems = new SqlCeCommand("DELETE FROM Items WHERE (ItemID = @ItemID)")
            {
                Connection = connection
            };
            this.m_cmdDeleteItems.Parameters.Add(new SqlCeParameter("@ItemID", SqlDbType.NVarChar, 36,
                ParameterDirection.Input, false, 0, 0, "ItemID", DataRowVersion.Original, null));
            this.m_dataAdapterItems.DeleteCommand = this.m_cmdDeleteItems;
            this.m_cmdSelectSingleItem.CommandText =
                "SELECT Items.CreationDate, Items.ModificationDate, Items.ItemID, Items.ItemNum, Items.WorkspaceID FROM Items, ItemProperties WHERE Items.WorkspaceID = ? AND Items.ItemId = ItemProperties.ItemID AND ItemProperties.PropertyName='SourceURL' AND ItemProperties.LongTextValue LIKE ?";
            this.m_cmdSelectSingleItem.Connection = connection;
            this.m_cmdSelectSingleItem.Parameters.Add(new SqlCeParameter("WorkspaceID", SqlDbType.NVarChar, 36));
            this.m_cmdSelectSingleItem.Parameters.Add(new SqlCeParameter("SourceURL", SqlDbType.NText));
            this.m_dataAdapterSingleItem.SelectCommand = this.m_cmdSelectSingleItem;
            this.m_dataAdapterSingleItem.UpdateCommand = this.m_dataAdapterItems.UpdateCommand;
            this.m_dataAdapterSingleItem.DeleteCommand = this.m_dataAdapterItems.DeleteCommand;
            this.m_dataAdapterSingleItem.InsertCommand = this.m_dataAdapterItems.InsertCommand;
            DataTableMappingCollection dataTableMappingCollections = this.m_dataAdapterSingleItem.TableMappings;
            DataTableMapping[] dataTableMappingArray = new DataTableMapping[1];
            DataColumnMapping[] dataColumnMappingArray = new DataColumnMapping[]
            {
                new DataColumnMapping("CreationDate", "CreationDate"),
                new DataColumnMapping("ModificationDAte", "ModificationDAte"), new DataColumnMapping("ItemID", "ItemID")
            };
            dataTableMappingArray[0] = new DataTableMapping("Table", "Items", dataColumnMappingArray);
            dataTableMappingCollections.AddRange(dataTableMappingArray);
            this.m_cmdSelectItemProperties.CommandText = string.Concat(
                "SELECT ItemProperties.DateValue, ItemProperties.ItemID, ItemProperties.LongTextValue, ItemProperties.NumberValue, ItemProperties.PropertyName, ItemProperties.ShortTextValue FROM ItemProperties LEFT JOIN Items ON ItemProperties.ItemID=Items.ItemID WHERE Items.WorkspaceID = ? ",
                str1);
            this.m_cmdSelectItemProperties.Parameters.Add(new SqlCeParameter("WorkspaceID", SqlDbType.NVarChar, 36));
            this.m_cmdSelectItemProperties.Connection = connection;
            this.m_cmdInsertItemProperties =
                new SqlCeCommand(
                    "Insert INTO ItemProperties( ItemID, PropertyName, LongTextValue, ShortTextValue, NumberValue, DateValue ) VALUES ( ?, ?, ?, ?, ?, ? )")
                {
                    Connection = connection
                };
            this.m_cmdInsertItemProperties.Parameters.Add(new SqlCeParameter("@ItemID", SqlDbType.NVarChar, 36,
                ParameterDirection.Input, false, 0, 0, "ItemID", DataRowVersion.Proposed, null));
            this.m_cmdInsertItemProperties.Parameters.Add(new SqlCeParameter("@PropertyName", SqlDbType.NVarChar, 0,
                ParameterDirection.Input, false, 0, 0, "PropertyName", DataRowVersion.Proposed, null));
            this.m_cmdInsertItemProperties.Parameters.Add(new SqlCeParameter("@LongTextValue", SqlDbType.NText, 0,
                ParameterDirection.Input, true, 0, 0, "LongTextValue", DataRowVersion.Proposed, null));
            this.m_cmdInsertItemProperties.Parameters.Add(new SqlCeParameter("@ShortTextValue", SqlDbType.NVarChar, 0,
                ParameterDirection.Input, true, 0, 0, "ShortTextValue", DataRowVersion.Proposed, null));
            this.m_cmdInsertItemProperties.Parameters.Add(new SqlCeParameter("@NumberValue", SqlDbType.Float, 0,
                ParameterDirection.Input, true, 15, 15, "NumberValue", DataRowVersion.Proposed, null));
            this.m_cmdInsertItemProperties.Parameters.Add(new SqlCeParameter("@DateValue", SqlDbType.DateTime, 0,
                ParameterDirection.Input, true, 0, 0, "DateValue", DataRowVersion.Proposed, null));
            this.m_dataAdapterItemProperties.InsertCommand = this.m_cmdInsertItemProperties;
            this.m_cmdUpdateItemProperties =
                new SqlCeCommand(
                    "UPDATE ItemProperties SET  LongTextValue = ?, ShortTextValue = ?, NumberValue = ?, DateValue = ? WHERE ItemID = ? AND PropertyName = ?")
                {
                    Connection = connection
                };
            this.m_cmdUpdateItemProperties.Parameters.Add(new SqlCeParameter("@LongTextValue", SqlDbType.NText, 0,
                ParameterDirection.Input, true, 0, 0, "LongTextValue", DataRowVersion.Current, null));
            this.m_cmdUpdateItemProperties.Parameters.Add(new SqlCeParameter("@ShortTextValue", SqlDbType.NVarChar, 0,
                ParameterDirection.Input, true, 0, 0, "ShortTextValue", DataRowVersion.Current, null));
            this.m_cmdUpdateItemProperties.Parameters.Add(new SqlCeParameter("@NumberValue", SqlDbType.Float, 0,
                ParameterDirection.Input, true, 15, 15, "NumberValue", DataRowVersion.Current, null));
            this.m_cmdUpdateItemProperties.Parameters.Add(new SqlCeParameter("@DateValue", SqlDbType.DateTime, 0,
                ParameterDirection.Input, true, 0, 0, "DateValue", DataRowVersion.Current, null));
            this.m_cmdUpdateItemProperties.Parameters.Add(new SqlCeParameter("@ItemID", SqlDbType.NVarChar, 36,
                ParameterDirection.Input, false, 0, 0, "ItemID", DataRowVersion.Original, null));
            this.m_cmdUpdateItemProperties.Parameters.Add(new SqlCeParameter("@PropertyName", SqlDbType.NVarChar, 0,
                ParameterDirection.Input, false, 0, 0, "PropertyName", DataRowVersion.Original, null));
            this.m_dataAdapterItemProperties.UpdateCommand = this.m_cmdUpdateItemProperties;
            this.m_cmdUpdateItemProperiesStorage_1 = new SqlCeCommand(string.Concat(
                "UPDATE ItemProperties SET TextBlobValue = ShortTextValue WHERE PropertyName = ? AND ItemID IN (SELECT ",
                str, "ItemID FROM Items WHERE WorkspaceID = ? )"))
            {
                Connection = connection
            };
            this.m_cmdUpdateItemProperiesStorage_1.Parameters.Add(new SqlCeParameter("@PropertyName",
                SqlDbType.NVarChar, 0, ParameterDirection.Input, false, 0, 0, "PropertyName", DataRowVersion.Original,
                null));
            this.m_cmdUpdateItemProperiesStorage_1.Parameters.Add(new SqlCeParameter("@WorkspaceID", SqlDbType.NVarChar,
                36));
            this.m_dataAdapterItemProperties.SelectCommand = this.m_cmdSelectItemProperties;
            DataTableMappingCollection tableMappings1 = this.m_dataAdapterItemProperties.TableMappings;
            DataTableMapping[] dataTableMapping1 = new DataTableMapping[1];
            DataColumnMapping[] dataColumnMapping1 = new DataColumnMapping[]
            {
                new DataColumnMapping("DateValue", "DateValue"), new DataColumnMapping("ItemID", "ItemID"),
                new DataColumnMapping("LongTextValue", "LongTextValue"),
                new DataColumnMapping("NumberValue", "NumberValue"),
                new DataColumnMapping("PropertyName", "PropertyName"),
                new DataColumnMapping("ShortTextValue", "ShortTextValue")
            };
            dataTableMapping1[0] = new DataTableMapping("Table", "ItemProperties", dataColumnMapping1);
            tableMappings1.AddRange(dataTableMapping1);
            this.m_cmdSelectSingleItemProperties.CommandText =
                "SELECT ItemProperties.DateValue, ItemProperties.ItemID, ItemProperties.LongTextValue, ItemProperties.NumberValue, ItemProperties.PropertyName, ItemProperties.ShortTextValue FROM Items, ItemProperties WHERE Items.WorkspaceID = ? AND Items.ItemId = ItemProperties.ItemID AND ItemProperties.ItemID = ?";
            this.m_cmdSelectSingleItemProperties.Connection = connection;
            this.m_cmdSelectSingleItemProperties.Parameters.Add(new SqlCeParameter("WorkspaceID", SqlDbType.NVarChar,
                36));
            this.m_cmdSelectSingleItemProperties.Parameters.Add(new SqlCeParameter("ItemID", SqlDbType.NVarChar, 36));
            this.m_dataAdapterSingleItemProperties.SelectCommand = this.m_cmdSelectSingleItemProperties;
            this.m_dataAdapterSingleItemProperties.UpdateCommand = this.m_dataAdapterItemProperties.UpdateCommand;
            this.m_dataAdapterSingleItemProperties.DeleteCommand = this.m_dataAdapterItemProperties.DeleteCommand;
            this.m_dataAdapterSingleItemProperties.InsertCommand = this.m_dataAdapterItemProperties.InsertCommand;
            DataTableMappingCollection dataTableMappingCollections1 =
                this.m_dataAdapterSingleItemProperties.TableMappings;
            DataTableMapping[] dataTableMappingArray1 = new DataTableMapping[1];
            DataColumnMapping[] dataColumnMappingArray1 = new DataColumnMapping[]
            {
                new DataColumnMapping("DateValue", "DateValue"), new DataColumnMapping("ItemID", "ItemID"),
                new DataColumnMapping("LongTextValue", "LongTextValue"),
                new DataColumnMapping("NumberValue", "NumberValue"),
                new DataColumnMapping("PropertyName", "PropertyName"),
                new DataColumnMapping("ShortTextValue", "ShortTextValue")
            };
            dataTableMappingArray1[0] = new DataTableMapping("Table", "ItemProperties", dataColumnMappingArray1);
            dataTableMappingCollections1.AddRange(dataTableMappingArray1);
            this.m_oleDbCmdSelectWorkspaces.CommandText = "SELECT * FROM Workspaces";
            this.m_oleDbCmdSelectWorkspaces.Connection = connection;
            this.m_cmdBuilderWorkspaces = new SqlCeCommandBuilder((SqlCeDataAdapter)this.m_dataAdapterWorkspaces);
            this.m_dataAdapterWorkspaces.SelectCommand = this.m_oleDbCmdSelectWorkspaces;
            DataTableMappingCollection tableMappings2 = this.m_dataAdapterWorkspaces.TableMappings;
            DataTableMapping[] dataTableMapping2 = new DataTableMapping[1];
            DataColumnMapping[] dataColumnMapping2 = new DataColumnMapping[]
            {
                new DataColumnMapping("WorkspaceID", "WorkspaceID"),
                new DataColumnMapping("CustomPropertyDefinitions", "CustomPropertyDefinitions")
            };
            dataTableMapping2[0] = new DataTableMapping("Table", "Workspaces", dataColumnMapping2);
            tableMappings2.AddRange(dataTableMapping2);
            this.m_cmdBatchDeleteItems = new SqlCeCommand()
            {
                Connection = connection
            };
            this.m_cmdBatchDeleteProperty = new SqlCeCommand()
            {
                Connection = connection
            };
            this.m_cmdBatchDeleteItemProperties = new SqlCeCommand()
            {
                Connection = connection
            };
            this.m_cmdBatchUpdateItemProperties = new SqlCeCommand()
            {
                Connection = connection
            };
            this.m_cmdBatchUpdateItems = new SqlCeCommand()
            {
                Connection = connection
            };
            this.m_cmdBatchUpdateItems.Parameters.Add(new SqlCeParameter("@ModificationDate", SqlDbType.DateTime));
            this.m_cmdBatchInsertItems = new SqlCeCommand()
            {
                Connection = connection
            };
            this.m_cmdSelectTextMoniker = new SqlCeCommand()
            {
                Connection = connection,
                CommandText = "SELECT TextBlobValue FROM ItemProperties WHERE ItemID = ? AND PropertyName = ?"
            };
            this.m_cmdSelectTextMoniker.Parameters.Add(new SqlCeParameter("@ItemID", SqlDbType.NVarChar, 36,
                ParameterDirection.Input, false, 0, 0, "ItemID", DataRowVersion.Original, null));
            this.m_cmdSelectTextMoniker.Parameters.Add(new SqlCeParameter("@PropertyName", SqlDbType.NVarChar, 0,
                ParameterDirection.Input, false, 0, 0, "PropertyName", DataRowVersion.Original, null));
            this.m_cmdUpdateTextMoniker = new SqlCeCommand()
            {
                Connection = connection,
                CommandText = "UPDATE ItemProperties SET TextBlobValue = ? WHERE ItemID = ? AND PropertyName = ?"
            };
            this.m_cmdUpdateTextMoniker.Parameters.Add(new SqlCeParameter("@TextBlobValue", SqlDbType.NText));
            this.m_cmdUpdateTextMoniker.Parameters.Add(new SqlCeParameter("@ItemID", SqlDbType.NVarChar, 36,
                ParameterDirection.Input, false, 0, 0, "ItemID", DataRowVersion.Original, null));
            this.m_cmdUpdateTextMoniker.Parameters.Add(new SqlCeParameter("@PropertyName", SqlDbType.NVarChar, 0,
                ParameterDirection.Input, false, 0, 0, "PropertyName", DataRowVersion.Original, null));
        }

        private SqlCeConnection NewDB(string sDBFile)
        {
            string str = sDBFile.Substring(0, sDBFile.LastIndexOf("\\"));
            if (!Directory.Exists(str))
            {
                throw new DirectoryNotFoundException(str);
            }

            if (File.Exists(sDBFile))
            {
                File.Delete(sDBFile);
            }

            string str1 = string.Concat("DataSource=\"", sDBFile, "\";Max Database Size=4091");
            SqlCeConnection sqlCeConnection = new SqlCeConnection(str1);
            (new SqlCeEngine(str1)).CreateDatabase();
            return sqlCeConnection;
        }

        private SqlCeConnection NewOrOpenDB(string sDBFile)
        {
            if (File.Exists(sDBFile))
            {
                return this.OpenDB(sDBFile);
            }

            SqlCeConnection sqlCeConnection = this.NewDB(sDBFile);
            sqlCeConnection.Open();
            base.CreateSchema(sqlCeConnection);
            sqlCeConnection.Close();
            return sqlCeConnection;
        }

        private SqlCeConnection OpenDB(string sDBFile)
        {
            if (string.IsNullOrEmpty(sDBFile))
            {
                throw new ArgumentNullException("sDBFile", "No file specified");
            }

            if (!File.Exists(sDBFile))
            {
                throw new FileNotFoundException(string.Concat("Cannot find file: ", sDBFile));
            }

            FileAttributes attributes = File.GetAttributes(sDBFile);
            if (FileAttributes.ReadOnly.CompareTo(attributes & FileAttributes.ReadOnly) == 0)
            {
                throw new ReadOnlyException(string.Concat("Project database: ", sDBFile,
                    " is readonly.\nPlease either change the file properties or choose a different project database."));
            }

            string str = string.Concat("DataSource=\"", sDBFile, "\";Max Database Size=4091");
            SqlCeConnection connection = SqlCeUtilities.GetConnection(str, true);
            base.UpdateNumericDataType(connection);
            return connection;
        }
    }
}