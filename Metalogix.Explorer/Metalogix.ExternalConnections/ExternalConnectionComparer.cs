using Metalogix.Data.Mapping;
using System;

namespace Metalogix.ExternalConnections
{
    public sealed class ExternalConnectionComparer : ListPickerComparer<ExternalConnection, ExternalConnection>
    {
        public ExternalConnectionComparer()
        {
        }

        public override int Compare(ExternalConnection source, ExternalConnection target)
        {
            if (source.ExternalConnectionID == 0)
            {
                return -1;
            }

            if (target.ExternalConnectionID == 0)
            {
                return 1;
            }

            if (object.Equals(source.ExternalConnectionID, target.ExternalConnectionID))
            {
                return 0;
            }

            if (source.ExternalConnectionID > target.ExternalConnectionID)
            {
                return -1;
            }

            return 1;
        }
    }
}