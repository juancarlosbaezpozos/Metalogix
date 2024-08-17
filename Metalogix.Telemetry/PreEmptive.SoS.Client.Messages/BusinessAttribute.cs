using System;

namespace PreEmptive.SoS.Client.Messages
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
    public sealed class BusinessAttribute : Attribute
    {
        private Guid companyId;

        private string companyName;

        private string companyDivision;

        public string CompanyDivision
        {
            get { return this.companyDivision; }
        }

        public Guid CompanyId
        {
            get { return this.companyId; }
        }

        public string CompanyName
        {
            get { return this.companyName; }
        }

        public BusinessAttribute(string companyId) : this(new Guid(companyId), null, null)
        {
        }

        public BusinessAttribute(string companyId, string companyName) : this(new Guid(companyId), companyName, null)
        {
        }

        public BusinessAttribute(string companyId, string companyName, string companyDivision)
        {
            this.companyId = new Guid(companyId);
            this.companyName = companyName;
            this.companyDivision = companyDivision;
        }

        public BusinessAttribute(Guid companyId) : this(companyId, null, null)
        {
        }

        public BusinessAttribute(Guid companyId, string companyName) : this(companyId, companyName, null)
        {
        }

        public BusinessAttribute(Guid companyId, string companyName, string companyDivision)
        {
            this.companyId = companyId;
            this.companyName = companyName;
            this.companyDivision = companyDivision;
        }
    }
}