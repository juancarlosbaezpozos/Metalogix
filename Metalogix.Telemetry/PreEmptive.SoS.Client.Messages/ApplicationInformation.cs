using PreEmptive.SoS.Client.MessageProxies;
using System;
using System.IO;
using System.Reflection;

namespace PreEmptive.SoS.Client.Messages
{
    [Serializable]
    public sealed class ApplicationInformation
    {
        private Guid guid_0;

        private string name;

        private string version;

        private string applicationType;

        public string ApplicationType
        {
            get { return this.applicationType; }
        }

        public Guid Id
        {
            get { return this.guid_0; }
        }

        public string Name
        {
            get { return this.name; }
        }

        public string Version
        {
            get { return this.version; }
        }

        private ApplicationInformation(Assembly assembly, Guid guid_1)
        {
            if (guid_1 == Guid.Empty)
            {
                throw new ArgumentException("Argument cannot be null or empty", "id");
            }

            this.InitializeNameFromAssembly(assembly);
            this.InitializeVersionFromAssembly(assembly);
            this.InitializeAppTypeFromAssembly(assembly);
            this.guid_0 = guid_1;
        }

        public ApplicationInformation(Guid guid_1) : this(Assembly.GetCallingAssembly(), guid_1)
        {
        }

        public ApplicationInformation(Guid guid_1, string name, string version) : this(Assembly.GetCallingAssembly(),
            guid_1)
        {
            this.name = name;
            this.version = version;
        }

        public ApplicationInformation(Guid guid_1, string name, string version, string applicationType) : this(
            Assembly.GetCallingAssembly(), guid_1)
        {
            this.name = name;
            this.version = version;
            this.applicationType = applicationType;
        }

        public static PreEmptive.SoS.Client.Messages.ApplicationInformation CreateFromTaggedAssembly(Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentException("Argument cannot be null", "assembly");
            }

            PreEmptive.SoS.Client.Messages.ApplicationInformation applicationInformation = null;
            object[] customAttributes = assembly.GetCustomAttributes(typeof(ApplicationAttribute), false);
            if (customAttributes != null && (int)customAttributes.Length > 0)
            {
                ApplicationAttribute applicationAttribute = (ApplicationAttribute)customAttributes[0];
                Guid guid = new Guid(applicationAttribute.Guid);
                applicationInformation = new PreEmptive.SoS.Client.Messages.ApplicationInformation(guid,
                    applicationAttribute.Name, applicationAttribute.Version, applicationAttribute.ApplicationType);
                if (applicationInformation.Version == null)
                {
                    applicationInformation.InitializeVersionFromAssembly(assembly);
                }

                if (applicationInformation.Name == null)
                {
                    applicationInformation.InitializeNameFromAssembly(assembly);
                }

                if (applicationInformation.applicationType == null)
                {
                    applicationInformation.InitializeAppTypeFromAssembly(assembly);
                }
            }

            return applicationInformation;
        }

        public override bool Equals(object object_0)
        {
            PreEmptive.SoS.Client.Messages.ApplicationInformation object0 =
                object_0 as PreEmptive.SoS.Client.Messages.ApplicationInformation;
            if (object0 == null)
            {
                return false;
            }

            return object0.Id.Equals(this.guid_0);
        }

        internal void FillInProxy(PreEmptive.SoS.Client.MessageProxies.MessageCache messageCache_0)
        {
            if (messageCache_0.Application == null)
            {
                messageCache_0.Application = new PreEmptive.SoS.Client.MessageProxies.ApplicationInformation();
            }

            messageCache_0.Application.Id = this.Id;
            messageCache_0.Application.Name = this.Name;
            messageCache_0.Application.Version = this.Version;
            messageCache_0.Application.ApplicationType = this.ApplicationType;
        }

        private static string GetAssemblyExtension(Assembly assembly)
        {
            return Path.GetExtension(assembly.CodeBase);
        }

        public override int GetHashCode()
        {
            return this.guid_0.GetHashCode();
        }

        internal void InitializeAppTypeFromAssembly(Assembly assembly)
        {
            this.applicationType = PreEmptive.SoS.Client.Messages.ApplicationInformation.GetAssemblyExtension(assembly);
        }

        internal void InitializeNameFromAssembly(Assembly assembly)
        {
            this.name = assembly.GetName().Name;
        }

        internal void InitializeVersionFromAssembly(Assembly assembly)
        {
            this.version = assembly.GetName().Version.ToString();
        }
    }
}