using System;

namespace Metalogix.Metabase.DataTypes
{
    public interface IAsyncLoadingObject
    {
        event EventHandler FinishedLoading;
    }
}