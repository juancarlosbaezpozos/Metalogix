using System;

namespace Metalogix.Actions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ActionConfigAttribute : Attribute
    {
        private readonly Type[] _actionTypes;

        public Type[] ActionTypes
        {
            get { return this._actionTypes; }
        }

        public ActionConfigAttribute(params Type[] actionTypes)
        {
            this._actionTypes = actionTypes;
        }
    }
}