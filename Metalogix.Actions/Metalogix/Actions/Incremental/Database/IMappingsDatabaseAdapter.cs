using System;
using System.Collections.Generic;

namespace Metalogix.Actions.Incremental.Database
{
    public interface IMappingsDatabaseAdapter
    {
        Metalogix.Actions.Incremental.Database.AdapterCallWrapper AdapterCallWrapper { get; }

        string AdapterContext { get; }

        string AdapterType { get; }

        void AddOrUpdateMapping(string sourceID, string sourceURL, string targetID, string targetURL, string targetType,
            string extendedProperties);

        IEnumerable<Mapping> GetMappingsInScope(string sourceScopeUrl, string targetScopeUrl, string targetType);

        TargetMapping GetTargetMapping(string sourceID, string sourceURL, string extendedProperties);

        bool IsMappingEntryExist(string sourceID, string sourceURL, string targetID, string targetURL,
            string targetType, string extendedProperties);

        void ProvisionDatabase();
    }
}