using System;

namespace Metalogix.Actions.Incremental.Database.Adapters
{
    public static class DatabaseCommandTexts
    {
        public const string SELECT_IDENTITY = "SELECT @@Identity";

        public const string CHECK_MAPPINGS_TABLE =
            "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='Mappings'";

        public const string CREATE_TABLE_MAPPINGS =
            "CREATE TABLE Mappings (\r\n                SourceID nvarchar(100) NOT NULL,\r\n                SourceURL nvarchar(500) NOT NULL,\r\n                TargetID nvarchar(100) NOT NULL,\r\n                TargetURL nvarchar(500) NOT NULL,\r\n                TargetType nvarchar(200) ,\r\n                ExtendedProperties nvarchar(500)               \r\n                )";

        public const string CREATE_INDEX_SOURCE_ENTRY_ON_MAPPINGS =
            "CREATE INDEX IX_SOURCE_ENTRY ON Mappings (SourceID, SourceURL)";

        public const string INSERT_MAPPINGS =
            "INSERT INTO MAPPINGS VALUES (@SourceID, @SourceURL, @TargetID, @TargetURL, @TargetType, @ExtendedProperties)";

        public const string UPDATE_MAPPINGS =
            "UPDATE MAPPINGS SET TargetID=@TargetID, TargetURL=@TargetURL, TargetType=@TargetType, ExtendedProperties = @ExtendedProperties WHERE SourceID=@SourceID AND SourceURL = @SourceURL AND (ExtendedProperties = @ExtendedProperties OR ExtendedProperties IS NULL)";

        public const string SELECT_MAPPING_COUNT =
            "SELECT COUNT(*) FROM MAPPINGS WHERE SourceID=@SourceID AND SourceURL = @SourceURL AND (ExtendedProperties = @ExtendedProperties OR ExtendedProperties IS NULL)";

        public const string CHECK_MAPPING_ENTRY_EXIST =
            "SELECT COUNT(*) from MAPPINGS WHERE SourceID=@SourceID AND SourceURL = @SourceURL AND TargetID = @TargetID AND TargetURL = @targetURL AND TargetType = @TargetType AND (ExtendedProperties = @ExtendedProperties OR ExtendedProperties IS NULL)";

        public const string SELECT_TARGET_MAPPING =
            "SELECT * from MAPPINGS WHERE SourceID=@SourceID AND SourceURL = @SourceURL AND (ExtendedProperties = @ExtendedProperties OR ExtendedProperties IS NULL)";

        public const string SELECT_MAPPINGS_IN_SCOPE_BY_TARGET_TYPE =
            "SELECT * from MAPPINGS WHERE SourceURL LIKE @SourceURL AND TargetURL LIKE @TargetURL AND TargetType = @TargetType";

        public const string CHECK_TARGET_URL_COLUMN_SIZE =
            "SELECT CHARACTER_MAXIMUM_LENGTH FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Mappings' AND COLUMN_NAME = 'TargetURL'";

        public const string UPDATE_TARGET_URL_COLUMN_SIZE = "ALTER TABLE Mappings ALTER COLUMN TargetUrl nvarchar(500)";

        public const string SOURCE_ID_PARAM = "@SourceID";

        public const string SOURCE_URL_PARAM = "@SourceURL";

        public const string TARGET_ID_PARAM = "@TargetID";

        public const string TARGET_URL_PARAM = "@TargetURL";

        public const string TARGET_TYPE_PARAM = "@TargetType";

        public const string EXTENDED_PROPERTIES_PARAM = "@ExtendedProperties";

        public const string INCREMENTAL_MAPPING_DATABASE = "IncrementalMapping.sdf";

        public const string ADD_EXTENDED_PROPERTIES_COLUMN =
            "ALTER TABLE Mappings Add ExtendedProperties nvarchar(500)";

        public const string CHECK_EXTENDED_PROPERTIES_COLUMN_EXISTS =
            "SELECT 1 AS ExtendedPropertiesColumnCount FROM INFORMATION_SCHEMA.COLUMNS WHERE (TABLE_NAME = 'Mappings') AND (COLUMN_NAME = 'ExtendedProperties')";
    }
}