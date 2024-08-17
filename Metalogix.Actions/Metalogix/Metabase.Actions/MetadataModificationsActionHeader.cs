using Metalogix.Actions;
using Metalogix.Metabase.Attributes;
using System;

namespace Metalogix.Metabase.Actions
{
    [Image("Metalogix.Actions.Icons.MetadataModifications.png")]
    [MenuText("Metadata Modifications {1-Transform}")]
    [RequiresTargetMetabaseConnection(true)]
    [ShowInMenus(true)]
    [TargetCardinality(Cardinality.OneOrMore)]
    public class MetadataModificationsActionHeader : MetabaseActionHeader
    {
        public MetadataModificationsActionHeader()
        {
        }
    }
}