using System;

namespace Metalogix.Metabase
{
    [Serializable]
    internal class MetabaseException : Exception
    {
        public MetabaseException(string message) : base(message)
        {
        }
    }
}