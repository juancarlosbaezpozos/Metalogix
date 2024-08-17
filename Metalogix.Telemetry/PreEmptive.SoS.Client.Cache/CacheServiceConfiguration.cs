using PreEmptive.SoS.Client.Messages;
using System;
using System.Collections;
using System.IO;
using System.Reflection;

namespace PreEmptive.SoS.Client.Cache
{
    public class CacheServiceConfiguration
    {
        private Hashtable properties;

        private ApplicationInformation application;

        private BusinessInformation business;

        private BinaryInformation binary;

        private string instanceId;

        private IFlowController flowController;

        private bool hashSensitiveData;

        private bool useSSL;

        private System.Reflection.Assembly assembly;

        private bool omitPII;

        private static string applicationid;

        private static string companyid;

        public static string ApiLanguage
        {
            get { return ".NET CLR"; }
        }

        public static string ApiVersion
        {
            get { return "3.1.1.0"; }
        }

        public ApplicationInformation Application
        {
            get { return this.application; }
            set
            {
                this.application = value;
                if (this.application.Id != Guid.Empty)
                {
                    Guid id = this.application.Id;
                    CacheServiceConfiguration.applicationid = id.ToString().ToUpper();
                }
            }
        }

        public static string ApplicationId
        {
            get { return CacheServiceConfiguration.applicationid; }
        }

        internal System.Reflection.Assembly Assembly
        {
            get { return this.assembly; }
        }

        public BinaryInformation Binary
        {
            get { return this.binary; }
            set { this.binary = value; }
        }

        public BusinessInformation Business
        {
            get { return this.business; }
            set
            {
                this.business = value;
                if (this.business.CompanyId != Guid.Empty)
                {
                    Guid companyId = this.business.CompanyId;
                    CacheServiceConfiguration.companyid = companyId.ToString().ToUpper();
                }
            }
        }

        public static string CompanyId
        {
            get { return CacheServiceConfiguration.companyid; }
        }

        public IFlowController FlowController
        {
            get
            {
                if (this.flowController == null)
                {
                    this.flowController = new DefaultFlowController();
                }

                return this.flowController;
            }
            set { this.flowController = value; }
        }

        public bool HashSensitiveData
        {
            get { return this.hashSensitiveData; }
            set { this.hashSensitiveData = value; }
        }

        public string InstanceId
        {
            get { return this.instanceId; }
            set { this.instanceId = value; }
        }

        public bool OmitPersonalInformation
        {
            get { return this.omitPII; }
            set { this.omitPII = value; }
        }

        public bool UseSSL
        {
            get { return this.useSSL; }
            set { this.useSSL = value; }
        }

        public CacheServiceConfiguration()
        {
        }

        public CacheServiceConfiguration(string instanceId) : this()
        {
            this.instanceId = instanceId;
        }

        public CacheServiceConfiguration(string location, string fullName, string instanceId) : this(instanceId)
        {
            AppDomainSetup appDomainSetup = new AppDomainSetup()
            {
                ApplicationBase = Path.GetDirectoryName(location)
            };
            AppDomain appDomain = AppDomain.CreateDomain("My domain", null, appDomainSetup);
            this.InitializeAttributes(appDomain.Load(fullName));
        }

        public CacheServiceConfiguration(System.Reflection.Assembly applicationAssembly)
        {
            this.InitializeAttributes(applicationAssembly);
        }

        public CacheServiceConfiguration(System.Reflection.Assembly applicationAssembly, string instanceId) : this(
            applicationAssembly)
        {
            this.instanceId = instanceId;
        }

        public CacheServiceConfiguration(string path, string instanceId) : this(
            System.Reflection.Assembly.LoadFrom(path), instanceId)
        {
        }

        public object GetProperty(string name)
        {
            if (this.properties == null)
            {
                return null;
            }

            return this.properties[name];
        }

        public object GetProperty(string name, object defaultValue)
        {
            if (name == null)
            {
                throw new ArgumentException("Argument cannot be null", "name");
            }

            if (this.properties == null)
            {
                return defaultValue;
            }

            object item = this.properties[name];
            if (item != null)
            {
                return item;
            }

            return defaultValue;
        }

        private void InitializeAttributes(System.Reflection.Assembly applicationAssembly)
        {
            if (applicationAssembly == null)
            {
                throw new ArgumentException("Argument cannot be null", "applicationAssembly");
            }

            this.Application = ApplicationInformation.CreateFromTaggedAssembly(applicationAssembly);
            this.Business = BusinessInformation.CreateFromTaggedAssembly(applicationAssembly);
            this.assembly = applicationAssembly;
        }

        public void RemoveProperty(string name)
        {
            if (this.properties == null || name == null)
            {
                return;
            }

            this.properties.Remove(name);
        }

        public void SetProperty(string propertyName, object propertyValue)
        {
            if (propertyName == null || propertyName.Trim().Length == 0)
            {
                throw new ArgumentException("Argument cannot be null or empty", "propertyName");
            }

            if (propertyValue == null)
            {
                throw new ArgumentException("Argument cannot be null", "propertyValue");
            }

            if (this.properties == null)
            {
                this.properties = new Hashtable();
            }

            this.properties[propertyName] = propertyValue;
        }
    }
}