using System;

namespace Metalogix.Explorer.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ExplorerNavigationFilterableAttribute : Attribute
    {
        private readonly bool _isFilterable;

        public bool IsFilterable
        {
            get { return this._isFilterable; }
        }

        public ExplorerNavigationFilterableAttribute(bool isFilterable)
        {
            this._isFilterable = isFilterable;
        }
    }
}