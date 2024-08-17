using System;
using System.Collections.Generic;

namespace Metalogix.Metabase.DataTypes
{
    public interface ISmartDataType : IMetabaseDataType, IComparable<ISmartDataType>, IEquatable<ISmartDataType>
    {
        void Deserialize(string sSerializedValue);

        void DeserializeFromUserFriendlyString(string val);

        List<string> RetrievePossibleValues(object val);

        string Serialize();

        string SerializeToUserFriendlyString(bool isExcelFile = false);
    }
}