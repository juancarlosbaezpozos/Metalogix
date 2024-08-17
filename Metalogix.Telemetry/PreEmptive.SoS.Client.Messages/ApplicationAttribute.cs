using System;

namespace PreEmptive.SoS.Client.Messages
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
    public sealed class ApplicationAttribute : Attribute
    {
        private string guid;

        private string name;

        private string version;

        private string applicationType;

        public string ApplicationType
        {
            get { return this.applicationType; }
        }

        public string Guid
        {
            get { return this.guid; }
        }

        public string Name
        {
            get { return this.name; }
        }

        public string Version
        {
            get { return this.version; }
        }

        public ApplicationAttribute(string guid) : this(guid, null, null, null)
        {
        }

        public ApplicationAttribute(string guid, string name, string version) : this(guid, name, version, null)
        {
        }

        public ApplicationAttribute(string guid, string name, string version, string applicationType)
        {
            this.guid = guid;
            this.name = name;
            this.version = version;
            this.applicationType = applicationType;
        }
    }
}