using System;

namespace Metalogix.Metabase.Interfaces
{
    public interface IDataboundObject
    {
        void BeginEdit();

        void CommitChanges();

        void EndEdit();
    }
}