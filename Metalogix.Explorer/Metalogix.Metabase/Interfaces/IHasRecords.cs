using Metalogix.Metabase;
using System;

namespace Metalogix.Metabase.Interfaces
{
    public interface IHasRecords
    {
        Record GetFirstRecord();

        int GetRecordCount();

        RecordCollection GetRecords();
    }
}