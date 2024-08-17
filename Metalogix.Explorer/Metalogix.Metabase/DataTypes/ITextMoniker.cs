using System;

namespace Metalogix.Metabase.DataTypes
{
    public interface ITextMoniker : IMetabaseDataType
    {
        string GetFullText();

        void SetFullText(string sValue);
    }
}