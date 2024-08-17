using System;
using System.ComponentModel;

namespace Metalogix.Metabase.Interfaces
{
    public interface IDataboundList
    {
        void BatchDelete(object[] arrObjects);

        void BatchUpdate(object[] arrObjects, PropertyDescriptor property, object oValue);

        void BeginEdit();

        void CommitChanges();

        void EndEdit();
    }
}