using System;
using System.Collections.Generic;

namespace Metalogix.Actions.Incremental.Database
{
    public sealed class MappingConnection
    {
        private readonly MappingDatabase _database;

        public MappingConnection(MappingDatabase database)
        {
            this._database = database;
        }

        public void AddOrUpdateMapping(string sourceID, string sourceURL, string targetID, string targetURL,
            string targetType, string extendedProperties = null)
        {
            this._database.Adapter.AddOrUpdateMapping(sourceID, sourceURL, targetID, targetURL, targetType,
                extendedProperties);
        }

        public IEnumerable<Mapping> GetMappingsInScope(string sourceScopeUrl, string targetScopeUrl, string targetType)
        {
            return this._database.Adapter.GetMappingsInScope(sourceScopeUrl, targetScopeUrl, targetType);
        }

        public TargetMapping GetTargetMapping(string sourceID, string sourceURL, string extendedProperties = null)
        {
            return this._database.Adapter.GetTargetMapping(sourceID, sourceURL, extendedProperties);
        }

        public bool IsMappingEntryExist(string sourceID, string sourceURL, string targetID, string targetURL,
            string targetType, string extendedProperties = null)
        {
            return this._database.Adapter.IsMappingEntryExist(sourceID, sourceURL, targetID, targetURL, targetType,
                extendedProperties);
        }
    }
}