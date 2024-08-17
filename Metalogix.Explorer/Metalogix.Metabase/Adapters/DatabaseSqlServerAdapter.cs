using Metalogix.Metabase.Interfaces;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;

namespace Metalogix.Metabase.Adapters
{
    public class DatabaseSqlServerAdapter : DatabaseAdapter
    {
        public override string AdapterType
        {
            get { return DatabaseAdapterType.SqlServer.ToString(); }
        }

        public DatabaseSqlServerAdapter(string connectionString) : base(connectionString)
        {
        }

        public DatabaseSqlServerAdapter(string connectionString,
            Metalogix.Metabase.Interfaces.AdapterCallWrapper wrapper) : base(connectionString, wrapper)
        {
        }

        protected override DbConnection Initialize()
        {
            DbConnection dbConnection = null;
            base.AdapterCallWrapper(new AdapterCallWrapperAction(() =>
            {
                SqlConnection sqlConnection = null;
                try
                {
                    sqlConnection = new SqlConnection(this.AdapterContext);
                    sqlConnection.Open();
                    this.InitializeCommands(sqlConnection);
                    using (SqlCommand sqlCommand =
                           new SqlCommand(
                               "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='Workspaces'"))
                    {
                        sqlCommand.Connection = sqlConnection;
                        if ((int)sqlCommand.ExecuteScalar() == 0)
                        {
                            base.CreateSchema(sqlConnection);
                        }
                        else
                        {
                            base.UpdateNumericDataType(sqlConnection);
                            return;
                        }
                    }
                }
                finally
                {
                    if (sqlConnection != null)
                    {
                        sqlConnection.Close();
                        dbConnection = sqlConnection;
                    }
                }
            }));
            return dbConnection;
        }

        private void InitializeCommands(SqlConnection connection)
        {
            this.m_dataAdapterItems = new SqlDataAdapter();
            this.m_dataAdapterSingleItem = new SqlDataAdapter();
            this.m_dataAdapterItemProperties = new SqlDataAdapter();
            this.m_dataAdapterSingleItemProperties = new SqlDataAdapter();
            this.m_cmdBuilderItemProperties = new SqlCommandBuilder((SqlDataAdapter)this.m_dataAdapterItemProperties);
            this.m_dataAdapterWorkspaces = new SqlDataAdapter();
            this.m_cmdIdentity = new SqlCommand("SELECT @@Identity")
            {
                Connection = connection
            };
            this.m_cmdSelectItems = new SqlCommand();
            this.m_cmdSelectSingleItem = new SqlCommand();
            this.m_cmdSelectItemProperties = new SqlCommand();
            this.m_cmdSelectSingleItemProperties = new SqlCommand();
            this.m_oleDbCmdSelectWorkspaces = new SqlCommand();
            string str = "";
            string str1 = "";
            this.m_cmdSelectItems.CommandText = string.Concat("SELECT ", str,
                "Items.CreationDate, Items.ModificationDate, Items.ItemID, Items.ItemNum, Items.WorkspaceID FROM Items WHERE WorkspaceID = @WorkspaceID ORDER BY Items.ItemNum ASC");
            this.m_cmdSelectItems.Connection = connection;
            this.m_cmdSelectItems.Parameters.Add(new SqlParameter("@WorkspaceID", SqlDbType.NVarChar, 36));
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
            this.m_cmdSelectMaxItemNum = new SqlCommand("SELECT MAX( ItemNum ) FROM Items")
            {
                Connection = connection
            };
            this.m_cmdInsertItems =
                new SqlCommand(
                    "Insert INTO Items( ItemID, ItemNum, WorkspaceID, CreationDate, ModificationDate ) VALUES ( @ItemID, @ItemNum, @WorkspaceID, @CreationDate, @ModificationDate )")
                {
                    Connection = connection
                };
            this.m_cmdInsertItems.Parameters.Add(new SqlParameter("@ItemID", SqlDbType.NVarChar, 36,
                ParameterDirection.Input, false, 0, 0, "ItemID", DataRowVersion.Proposed, null));
            this.m_cmdInsertItems.Parameters.Add(new SqlParameter("@ItemNum", SqlDbType.Int, 4,
                ParameterDirection.Input, false, 0, 0, "ItemNum", DataRowVersion.Proposed, null));
            this.m_cmdInsertItems.Parameters.Add(new SqlParameter("@WorkspaceID", SqlDbType.NVarChar, 36,
                ParameterDirection.Input, false, 0, 0, "WorkspaceID", DataRowVersion.Proposed, null));
            this.m_cmdInsertItems.Parameters.Add(new SqlParameter("@CreationDate", SqlDbType.DateTime, 4,
                ParameterDirection.Input, false, 0, 0, "CreationDate", DataRowVersion.Proposed, null));
            this.m_cmdInsertItems.Parameters.Add(new SqlParameter("@ModificationDate", SqlDbType.DateTime, 4,
                ParameterDirection.Input, false, 0, 0, "ModificationDate", DataRowVersion.Proposed, null));
            this.m_dataAdapterItems.InsertCommand = this.m_cmdInsertItems;
            this.m_cmdUpdateItems =
                new SqlCommand(
                    "UPDATE Items SET ItemNum = @ItemNum, WorkspaceID = @WorkspaceID, CreationDate = @CreationDate, ModificationDate = @ModificationDate WHERE ItemID = @ItemID")
                {
                    Connection = connection
                };
            this.m_cmdUpdateItems.Parameters.Add(new SqlParameter("@ItemNum", SqlDbType.Int, 4,
                ParameterDirection.Input, false, 0, 0, "ItemNum", DataRowVersion.Original, null));
            this.m_cmdUpdateItems.Parameters.Add(new SqlParameter("@WorkspaceID", SqlDbType.NVarChar, 36,
                ParameterDirection.Input, false, 0, 0, "WorkspaceID", DataRowVersion.Original, null));
            this.m_cmdUpdateItems.Parameters.Add(new SqlParameter("@CreationDate", SqlDbType.DateTime, 4,
                ParameterDirection.Input, false, 0, 0, "CreationDate", DataRowVersion.Original, null));
            this.m_cmdUpdateItems.Parameters.Add(new SqlParameter("@ModificationDate", SqlDbType.DateTime, 4,
                ParameterDirection.Input, false, 0, 0, "ModificationDate", DataRowVersion.Current, null));
            this.m_cmdUpdateItems.Parameters.Add(new SqlParameter("@ItemID", SqlDbType.NVarChar, 36,
                ParameterDirection.Input, false, 0, 0, "ItemID", DataRowVersion.Original, null));
            this.m_dataAdapterItems.UpdateCommand = this.m_cmdUpdateItems;
            this.m_cmdDeleteItems = new SqlCommand("DELETE FROM Items WHERE (ItemID = @ItemID)")
            {
                Connection = connection
            };
            this.m_cmdDeleteItems.Parameters.Add(new SqlParameter("@ItemID", SqlDbType.NVarChar, 36,
                ParameterDirection.Input, false, 0, 0, "ItemID", DataRowVersion.Original, null));
            this.m_dataAdapterItems.DeleteCommand = this.m_cmdDeleteItems;
            this.m_cmdSelectSingleItem.CommandText =
                "SELECT Items.CreationDate, Items.ModificationDate, Items.ItemID, Items.ItemNum, Items.WorkspaceID FROM Items, ItemProperties WHERE Items.WorkspaceID = @WorkspaceID AND Items.ItemId = ItemProperties.ItemID AND ItemProperties.PropertyName='SourceURL' AND ItemProperties.LongTextValue LIKE @SourceURL";
            this.m_cmdSelectSingleItem.Connection = connection;
            this.m_cmdSelectSingleItem.Parameters.Add(new SqlParameter("@WorkspaceID", SqlDbType.NVarChar, 36));
            this.m_cmdSelectSingleItem.Parameters.Add(new SqlParameter("@SourceURL", SqlDbType.NText));
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
                "SELECT ItemProperties.DateValue, ItemProperties.ItemID, ItemProperties.LongTextValue, ItemProperties.NumberValue, ItemProperties.PropertyName, ItemProperties.ShortTextValue FROM ItemProperties LEFT JOIN Items ON ItemProperties.ItemID=Items.ItemID WHERE Items.WorkspaceID = @WorkspaceID ",
                str1);
            this.m_cmdSelectItemProperties.Parameters.Add(new SqlParameter("@WorkspaceID", SqlDbType.NVarChar, 36));
            this.m_cmdSelectItemProperties.Connection = connection;
            this.m_cmdInsertItemProperties = new SqlCommand(
                "Insert INTO ItemProperties( ItemID, PropertyName, LongTextValue, ShortTextValue, NumberValue, DateValue ) VALUES (@ItemID, @PropertyName, @LongTextValue, @ShortTextValue, @NumberValue, @DateValue)")
            {
                Connection = connection
            };
            this.m_cmdInsertItemProperties.Parameters.Add(new SqlParameter("@ItemID", SqlDbType.NVarChar, 36,
                ParameterDirection.Input, false, 0, 0, "ItemID", DataRowVersion.Proposed, null));
            this.m_cmdInsertItemProperties.Parameters.Add(new SqlParameter("@PropertyName", SqlDbType.NVarChar, 0,
                ParameterDirection.Input, false, 0, 0, "PropertyName", DataRowVersion.Proposed, null));
            this.m_cmdInsertItemProperties.Parameters.Add(new SqlParameter("@LongTextValue", SqlDbType.NText, 0,
                ParameterDirection.Input, true, 0, 0, "LongTextValue", DataRowVersion.Proposed, null));
            this.m_cmdInsertItemProperties.Parameters.Add(new SqlParameter("@ShortTextValue", SqlDbType.NVarChar, 0,
                ParameterDirection.Input, true, 0, 0, "ShortTextValue", DataRowVersion.Proposed, null));
            this.m_cmdInsertItemProperties.Parameters.Add(new SqlParameter("@NumberValue", SqlDbType.Float, 0,
                ParameterDirection.Input, true, 15, 15, "NumberValue", DataRowVersion.Proposed, null));
            this.m_cmdInsertItemProperties.Parameters.Add(new SqlParameter("@DateValue", SqlDbType.DateTime, 0,
                ParameterDirection.Input, true, 0, 0, "DateValue", DataRowVersion.Proposed, null));
            this.m_dataAdapterItemProperties.InsertCommand = this.m_cmdInsertItemProperties;
            this.m_cmdUpdateItemProperties = new SqlCommand(
                "UPDATE ItemProperties SET  LongTextValue = @LongTextValue, ShortTextValue = @ShortTextValue, NumberValue = @NumberValue, DateValue = @DateValue WHERE ItemID = @ItemID AND PropertyName = @PropertyName")
            {
                Connection = connection
            };
            this.m_cmdUpdateItemProperties.Parameters.Add(new SqlParameter("@LongTextValue", SqlDbType.NText, 0,
                ParameterDirection.Input, true, 0, 0, "LongTextValue", DataRowVersion.Current, null));
            this.m_cmdUpdateItemProperties.Parameters.Add(new SqlParameter("@ShortTextValue", SqlDbType.NVarChar, 0,
                ParameterDirection.Input, true, 0, 0, "ShortTextValue", DataRowVersion.Current, null));
            this.m_cmdUpdateItemProperties.Parameters.Add(new SqlParameter("@NumberValue", SqlDbType.Float, 0,
                ParameterDirection.Input, true, 15, 15, "NumberValue", DataRowVersion.Current, null));
            this.m_cmdUpdateItemProperties.Parameters.Add(new SqlParameter("@DateValue", SqlDbType.DateTime, 0,
                ParameterDirection.Input, true, 0, 0, "DateValue", DataRowVersion.Current, null));
            this.m_cmdUpdateItemProperties.Parameters.Add(new SqlParameter("@ItemID", SqlDbType.NVarChar, 36,
                ParameterDirection.Input, false, 0, 0, "ItemID", DataRowVersion.Original, null));
            this.m_cmdUpdateItemProperties.Parameters.Add(new SqlParameter("@PropertyName", SqlDbType.NVarChar, 0,
                ParameterDirection.Input, false, 0, 0, "PropertyName", DataRowVersion.Original, null));
            this.m_dataAdapterItemProperties.UpdateCommand = this.m_cmdUpdateItemProperties;
            this.m_cmdUpdateItemProperiesStorage_1 = new SqlCommand(string.Concat(
                "UPDATE ItemProperties SET TextBlobValue = ShortTextValue WHERE PropertyName = @PropertyName AND ItemID IN (SELECT ",
                str, "ItemID FROM Items WHERE WorkspaceID = @WorkspaceID )"))
            {
                Connection = connection
            };
            this.m_cmdUpdateItemProperiesStorage_1.Parameters.Add(new SqlParameter("@PropertyName", SqlDbType.NVarChar,
                0, ParameterDirection.Input, false, 0, 0, "PropertyName", DataRowVersion.Original, null));
            this.m_cmdUpdateItemProperiesStorage_1.Parameters.Add(new SqlParameter("@WorkspaceID", SqlDbType.NVarChar,
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
                "SELECT ItemProperties.DateValue, ItemProperties.ItemID, ItemProperties.LongTextValue, ItemProperties.NumberValue, ItemProperties.PropertyName, ItemProperties.ShortTextValue FROM Items, ItemProperties WHERE Items.WorkspaceID = @WorkspaceID AND Items.ItemId = ItemProperties.ItemID AND ItemProperties.ItemID = @ItemID";
            this.m_cmdSelectSingleItemProperties.Connection = connection;
            this.m_cmdSelectSingleItemProperties.Parameters.Add(
                new SqlParameter("@WorkspaceID", SqlDbType.NVarChar, 36));
            this.m_cmdSelectSingleItemProperties.Parameters.Add(new SqlParameter("@ItemID", SqlDbType.NVarChar, 36));
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
            this.m_cmdBuilderWorkspaces = new SqlCommandBuilder((SqlDataAdapter)this.m_dataAdapterWorkspaces);
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
            this.m_cmdBatchDeleteItems = new SqlCommand()
            {
                Connection = connection
            };
            this.m_cmdBatchDeleteProperty = new SqlCommand()
            {
                Connection = connection
            };
            this.m_cmdBatchDeleteItemProperties = new SqlCommand()
            {
                Connection = connection
            };
            this.m_cmdBatchUpdateItemProperties = new SqlCommand()
            {
                Connection = connection
            };
            this.m_cmdBatchUpdateItems = new SqlCommand()
            {
                Connection = connection
            };
            this.m_cmdBatchUpdateItems.Parameters.Add(new SqlParameter("@ModificationDate", SqlDbType.DateTime));
            this.m_cmdBatchInsertItems = new SqlCommand()
            {
                Connection = connection
            };
            this.m_cmdSelectTextMoniker = new SqlCommand()
            {
                Connection = connection,
                CommandText =
                    "SELECT TextBlobValue FROM ItemProperties WHERE ItemID = @ItemID AND PropertyName = @PropertyName"
            };
            this.m_cmdSelectTextMoniker.Parameters.Add(new SqlParameter("@ItemID", SqlDbType.NVarChar, 36,
                ParameterDirection.Input, false, 0, 0, "ItemID", DataRowVersion.Original, null));
            this.m_cmdSelectTextMoniker.Parameters.Add(new SqlParameter("@PropertyName", SqlDbType.NVarChar, 0,
                ParameterDirection.Input, false, 0, 0, "PropertyName", DataRowVersion.Original, null));
            this.m_cmdUpdateTextMoniker = new SqlCommand()
            {
                Connection = connection,
                CommandText =
                    "UPDATE ItemProperties SET TextBlobValue = @TextBlobValue WHERE ItemID = @ItemID AND PropertyName = @PropertyName"
            };
            this.m_cmdUpdateTextMoniker.Parameters.Add(new SqlParameter("@TextBlobValue", SqlDbType.NText));
            this.m_cmdUpdateTextMoniker.Parameters.Add(new SqlParameter("@ItemID", SqlDbType.NVarChar, 36,
                ParameterDirection.Input, false, 0, 0, "ItemID", DataRowVersion.Original, null));
            this.m_cmdUpdateTextMoniker.Parameters.Add(new SqlParameter("@PropertyName", SqlDbType.NVarChar, 0,
                ParameterDirection.Input, false, 0, 0, "PropertyName", DataRowVersion.Original, null));
        }
    }
}