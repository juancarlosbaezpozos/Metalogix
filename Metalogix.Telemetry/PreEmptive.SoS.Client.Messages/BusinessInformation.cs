using PreEmptive.SoS.Client.MessageProxies;
using System;
using System.Reflection;

namespace PreEmptive.SoS.Client.Messages
{
    [Serializable]
    public sealed class BusinessInformation
    {
        private Guid companyId;

        private string companyName;

        public Guid CompanyId
        {
            get { return this.companyId; }
            set { this.companyId = value; }
        }

        public string CompanyName
        {
            get { return this.companyName; }
            set { this.companyName = value; }
        }

        public BusinessInformation(Guid companyId)
        {
            if (companyId == Guid.Empty)
            {
                throw new ArgumentException("Argument cannot be null or empty", "companyKey");
            }

            this.companyId = companyId;
        }

        public BusinessInformation(Guid companyId, string companyName) : this(companyId)
        {
            this.companyName = companyName;
        }

        public static PreEmptive.SoS.Client.Messages.BusinessInformation CreateFromTaggedAssembly(Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentException("Argument cannot be null", "assembly");
            }

            PreEmptive.SoS.Client.Messages.BusinessInformation businessInformation = null;
            object[] customAttributes = assembly.GetCustomAttributes(typeof(BusinessAttribute), false);
            if (customAttributes != null && (int)customAttributes.Length > 0)
            {
                BusinessAttribute businessAttribute = (BusinessAttribute)customAttributes[0];
                businessInformation =
                    new PreEmptive.SoS.Client.Messages.BusinessInformation(businessAttribute.CompanyId,
                        businessAttribute.CompanyName);
                if (businessInformation.CompanyName == null)
                {
                    object[] objArray = assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                    if (objArray != null && (int)objArray.Length > 0)
                    {
                        businessInformation.CompanyName = ((AssemblyCompanyAttribute)objArray[0]).Company;
                    }
                }
            }

            return businessInformation;
        }

        public override bool Equals(object object_0)
        {
            PreEmptive.SoS.Client.Messages.BusinessInformation object0 =
                object_0 as PreEmptive.SoS.Client.Messages.BusinessInformation;
            if (object0 == null)
            {
                return false;
            }

            return object0.CompanyId.Equals(this.companyId);
        }

        internal void FillInProxy(PreEmptive.SoS.Client.MessageProxies.MessageCache messageCache_0)
        {
            if (messageCache_0.Business == null)
            {
                messageCache_0.Business = new PreEmptive.SoS.Client.MessageProxies.BusinessInformation();
            }

            messageCache_0.Business.CompanyId = this.CompanyId;
            messageCache_0.Business.CompanyName = this.CompanyName;
        }

        public override int GetHashCode()
        {
            return this.companyId.GetHashCode();
        }
    }
}