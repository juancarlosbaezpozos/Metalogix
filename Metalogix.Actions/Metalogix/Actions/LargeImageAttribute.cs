using System;

namespace Metalogix.Actions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class LargeImageAttribute : Attribute
    {
        private readonly string m_value;

        public string ImageName
        {
            get { return this.m_value; }
        }

        public LargeImageAttribute(string string_0)
        {
            this.m_value = string_0;
        }

        public override string ToString()
        {
            return this.ImageName.ToString();
        }
    }
}