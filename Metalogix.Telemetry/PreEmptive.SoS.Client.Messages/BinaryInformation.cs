using PreEmptive.SoS.Client.MessageProxies;
using System;
using System.IO;
using System.Reflection;
using System.Security;

namespace PreEmptive.SoS.Client.Messages
{
    [Serializable]
    public class BinaryInformation
    {
        private Guid guid_0;

        private string name;

        private string string_0;

        private string version;

        private string methodName;

        private DateTime modifiedDate;

        private string typeName;

        public Guid Id
        {
            get { return this.guid_0; }
        }

        public string MethodName
        {
            get { return this.methodName; }
            set { this.methodName = value; }
        }

        public DateTime ModifiedDate
        {
            get { return this.modifiedDate; }
        }

        public string Name
        {
            get { return this.name; }
        }

        public string Tag
        {
            get { return this.string_0; }
            set { this.string_0 = value; }
        }

        public string TypeName
        {
            get { return this.typeName; }
            set { this.typeName = value; }
        }

        public string Version
        {
            get { return this.version; }
        }

        public BinaryInformation(Guid guid_1)
        {
            this.guid_0 = guid_1;
        }

        public BinaryInformation(Guid guid_1, string name, string version) : this(guid_1)
        {
            this.name = name;
            this.version = version;
        }

        public BinaryInformation(Guid guid_1, string name, string version, DateTime modifiedDate) : this(guid_1, name,
            version)
        {
            this.modifiedDate = modifiedDate;
        }

        public BinaryInformation(Guid guid_1, string name, string version, string modifiedDate) : this(guid_1, name,
            version)
        {
            try
            {
                this.modifiedDate = DateTime.Parse(modifiedDate);
            }
            catch
            {
                this.modifiedDate = DateTime.Today;
            }
        }

        public static PreEmptive.SoS.Client.Messages.BinaryInformation CreateFromAssembly(Assembly assembly,
            Guid guid_1)
        {
            PreEmptive.SoS.Client.Messages.BinaryInformation binaryInformation =
                new PreEmptive.SoS.Client.Messages.BinaryInformation(guid_1);
            binaryInformation.InitializeFromAssembly(assembly);
            return binaryInformation;
        }

        public static PreEmptive.SoS.Client.Messages.BinaryInformation CreateFromFile(string path, Guid guid_1)
        {
            PreEmptive.SoS.Client.Messages.BinaryInformation binaryInformation =
                new PreEmptive.SoS.Client.Messages.BinaryInformation(guid_1);
            binaryInformation.InitializeFromFile(path);
            return binaryInformation;
        }

        public static PreEmptive.SoS.Client.Messages.BinaryInformation CreateFromTaggedAssembly(string location,
            string fullName)
        {
            AppDomainSetup appDomainSetup = new AppDomainSetup()
            {
                ApplicationBase = Path.GetDirectoryName(location)
            };
            AppDomain appDomain = AppDomain.CreateDomain("My domain", null, appDomainSetup);
            return PreEmptive.SoS.Client.Messages.BinaryInformation.CreateFromTaggedAssembly(appDomain.Load(fullName));
        }

        public static PreEmptive.SoS.Client.Messages.BinaryInformation CreateFromTaggedAssembly(string path)
        {
            return PreEmptive.SoS.Client.Messages.BinaryInformation.CreateFromTaggedAssembly(Assembly.LoadFrom(path));
        }

        public static PreEmptive.SoS.Client.Messages.BinaryInformation CreateFromTaggedAssembly(Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentException("Argument cannot be null", "assembly");
            }

            object[] customAttributes = assembly.GetCustomAttributes(typeof(BinaryAttribute), false);
            Guid empty = Guid.Empty;
            if (customAttributes != null && (int)customAttributes.Length > 0)
            {
                empty = new Guid(((BinaryAttribute)customAttributes[0]).Guid);
            }

            PreEmptive.SoS.Client.Messages.BinaryInformation binaryInformation =
                new PreEmptive.SoS.Client.Messages.BinaryInformation(empty);
            binaryInformation.InitializeFromAssembly(assembly);
            return binaryInformation;
        }

        protected virtual PreEmptive.SoS.Client.MessageProxies.BinaryInformation CreateProxy()
        {
            return new PreEmptive.SoS.Client.MessageProxies.BinaryInformation();
        }

        public override bool Equals(object object_0)
        {
            PreEmptive.SoS.Client.Messages.BinaryInformation object0 =
                object_0 as PreEmptive.SoS.Client.Messages.BinaryInformation;
            if (object0 == null)
            {
                return false;
            }

            return object0.Id.Equals(this.guid_0);
        }

        internal virtual void FillInProxy(PreEmptive.SoS.Client.MessageProxies.Message proxy)
        {
            if (proxy.Binary == null)
            {
                proxy.Binary = this.CreateProxy();
            }

            proxy.Binary.Id = this.Id;
            proxy.Binary.ModifiedDate = this.ModifiedDate;
            proxy.Binary.Name = this.Name;
            proxy.Binary.Version = this.Version;
            proxy.Binary.MethodName = this.MethodName;
            proxy.Binary.TypeName = this.TypeName;
        }

        private static DateTime GetFileModificationTime(string codebaseString)
        {
            DateTime minValue = DateTime.MinValue;
            if (codebaseString == null)
            {
                return minValue;
            }

            try
            {
                FileInfo fileInfo = new FileInfo(codebaseString);
                if (fileInfo.Exists)
                {
                    minValue = fileInfo.LastWriteTime;
                }
            }
            catch (IOException oException)
            {
            }
            catch (SecurityException securityException)
            {
            }
            catch (UnauthorizedAccessException unauthorizedAccessException)
            {
            }
            catch (NotSupportedException notSupportedException)
            {
            }

            return minValue;
        }

        public override int GetHashCode()
        {
            return this.guid_0.GetHashCode();
        }

        public void InitializeFromAssembly(Assembly assembly)
        {
            this.InitializeFromAssemblyInternal(assembly);
            this.modifiedDate =
                PreEmptive.SoS.Client.Messages.BinaryInformation.GetFileModificationTime(assembly.Location);
        }

        private void InitializeFromAssemblyInternal(Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentException("Argument cannot be null", "assembly");
            }

            this.version = assembly.GetName().Version.ToString();
            this.name = assembly.GetName().FullName;
        }

        public void InitializeFromFile(string path)
        {
            if (path == null)
            {
                throw new ArgumentException("Argument cannot be null", "path");
            }

            this.InitializeFromAssemblyInternal(Assembly.LoadFrom(path));
            this.modifiedDate = PreEmptive.SoS.Client.Messages.BinaryInformation.GetFileModificationTime(path);
        }
    }
}