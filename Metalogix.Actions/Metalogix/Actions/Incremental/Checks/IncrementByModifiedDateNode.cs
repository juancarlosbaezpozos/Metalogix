using Metalogix.Actions.Incremental;
using System;
using System.Runtime.CompilerServices;

namespace Metalogix.Actions.Incremental.Checks
{
    public sealed class IncrementByModifiedDateNode : IIncrementableNode
    {
        public string Id
        {
            get { return JustDecompileGenerated_get_Id(); }
            set { JustDecompileGenerated_set_Id(value); }
        }

        private string JustDecompileGenerated_Id_k__BackingField;

        public string JustDecompileGenerated_get_Id()
        {
            return this.JustDecompileGenerated_Id_k__BackingField;
        }

        public void JustDecompileGenerated_set_Id(string value)
        {
            this.JustDecompileGenerated_Id_k__BackingField = value;
        }

        public DateTime ModifiedDate
        {
            get { return JustDecompileGenerated_get_ModifiedDate(); }
            set { JustDecompileGenerated_set_ModifiedDate(value); }
        }

        private DateTime JustDecompileGenerated_ModifiedDate_k__BackingField;

        public DateTime JustDecompileGenerated_get_ModifiedDate()
        {
            return this.JustDecompileGenerated_ModifiedDate_k__BackingField;
        }

        public void JustDecompileGenerated_set_ModifiedDate(DateTime value)
        {
            this.JustDecompileGenerated_ModifiedDate_k__BackingField = value;
        }

        public IncrementByModifiedDateNode()
        {
        }
    }
}