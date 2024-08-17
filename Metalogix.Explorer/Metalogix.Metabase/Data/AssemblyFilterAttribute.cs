using System;

namespace Metalogix.Metabase.Data
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class AssemblyFilterAttribute : Attribute
    {
        private readonly string m_sTargetProduct;

        public string TargetProduct
        {
            get { return this.m_sTargetProduct; }
        }

        public AssemblyFilterAttribute(string sTargetProduct)
        {
            this.m_sTargetProduct = sTargetProduct;
        }
    }
}