using System;

namespace Metalogix.Metabase.Adapters
{
    public static class DatabaseCommandTexts
    {
        public const string SELECT_IDENTITY = "SELECT @@Identity";

        public const string CHECK_WORKSPACES_TABLE =
            "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='Workspaces'";

        public const string CREATE_TABLE_WORKSPACES =
            "CREATE TABLE Workspaces (\r\n                WorkspaceID nvarchar(36) PRIMARY KEY,\r\n                Name nvarchar(255),\r\n                CustomPropertyDefinitions ntext,\r\n                BaseType ntext,\r\n                ViewDefinitions ntext,\r\n                PropertySummaries ntext,\r\n                Settings ntext,\r\n                DateCreated datetime,\r\n                DateModified datetime\r\n                )";

        public const string CREATE_TABLE_ITEMS =
            "CREATE TABLE Items (\r\n                ItemID nvarchar(36) PRIMARY KEY,\r\n                ItemNum integer,\r\n                WorkspaceID nvarchar(36),\r\n                CreationDate datetime,\r\n                ModificationDate datetime\r\n                )";

        public const string CREATE_TABLE_ITEMPROPERTIES =
            "CREATE TABLE ItemProperties (\r\n                PropertyName nvarchar(50) NOT NULL,\r\n                ItemID nvarchar(36) NOT NULL,\r\n                ShortTextValue nvarchar(255),\r\n                LongTextValue ntext,\r\n                TextBlobValue ntext,\r\n                NumberValue numeric (18,2),\r\n                DateValue datetime,\r\n                PRIMARY KEY(PropertyName, ItemID)\r\n                )";

        public const string CREATE_INDEX_WORKSPACEID_ON_WORKSPACES =
            "CREATE UNIQUE INDEX IX_WORKSPACEID ON Workspaces (WorkspaceID)";

        public const string CREATE_INDEX_ITEMID_ON_ITEMS = "CREATE UNIQUE INDEX IX_ITEMID ON Items (ItemID)";

        public const string CREATE_INDEX_WORKSPACEID_ON_ITEMS = "CREATE INDEX IX_WORKSPACEID ON Items (WorkspaceID)";

        public const string CREATE_INDEX_ITEMID_ON_ITEMPROPERTIES = "CREATE INDEX IX_ITEMID ON ItemProperties (ItemID)";

        public const string INSERT_WORKSPACE =
            "INSERT INTO Workspaces (\r\n                WorkspaceID,\r\n                Name,\r\n                BaseType,\r\n                DateCreated,\r\n                DateModified\r\n                ) VALUES (\r\n                @WorkspaceID,\r\n                @Name,\r\n                @BaseType,\r\n                @DateCreated,\r\n                @DateModified\r\n                )";

        public const string CHECK_NUMERIC_DATA_TYPE =
            "SELECT NUMERIC_SCALE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='ItemProperties' AND COLUMN_NAME='NumberValue'";

        public const string UPDATE_NUMERIC_DATA_TYPE =
            "ALTER TABLE ItemProperties ALTER COLUMN NumberValue numeric(18,2)";
    }
}