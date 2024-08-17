using System;

namespace PreEmptive.SoS.Client.Messages
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
    public sealed class BinaryAttribute : Attribute
    {
        private string guid;

        public string Guid
        {
            get { return this.guid; }
        }

        public BinaryAttribute(string guid)
        {
            this.guid = guid;
        }
    }
}