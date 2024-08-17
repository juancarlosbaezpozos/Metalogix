using System;

namespace Metalogix.Actions.Incremental.Database
{
    public sealed class MappingDatabase
    {
        private readonly IMappingsDatabaseAdapter _adapter;

        public IMappingsDatabaseAdapter Adapter
        {
            get { return this._adapter; }
        }

        public MappingDatabase(IMappingsDatabaseAdapter adapter)
        {
            this._adapter = adapter;
        }

        public MappingConnection Connect()
        {
            return new MappingConnection(this);
        }
    }
}