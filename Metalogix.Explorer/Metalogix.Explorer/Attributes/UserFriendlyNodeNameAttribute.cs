using System;

namespace Metalogix.Explorer.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class UserFriendlyNodeNameAttribute : Attribute
    {
        private readonly string _name;

        public string Name
        {
            get { return this._name; }
        }

        public UserFriendlyNodeNameAttribute(string name)
        {
            this._name = name;
        }
    }
}