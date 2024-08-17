using System;

namespace Metalogix.Explorer.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class SuppressActionAttribute : Attribute
    {
        public string SuppressAction;

        public SuppressActionAttribute(string action)
        {
            this.SuppressAction = action;
        }
    }
}