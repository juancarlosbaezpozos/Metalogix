using System;

namespace Metalogix.Actions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ImageAttribute : Attribute
    {
        private string m_value;

        public string ImageName
        {
            get { return this.m_value; }
        }

        public ImageAttribute(string val)
        {
            this.m_value = val;
        }
    }
}