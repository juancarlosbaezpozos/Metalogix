using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Metalogix.Actions
{
    public sealed class ActionConfigContext : IDisposable
    {
        private readonly Metalogix.Actions.Action _action;

        private Metalogix.Actions.ActionContext _context;

        private Dictionary<string, object> _tempdata;

        [ReadOnly(true)]
        public Metalogix.Actions.Action Action
        {
            get { return this._action; }
        }

        [ReadOnly(true)]
        public Metalogix.Actions.ActionContext ActionContext
        {
            get { return this._context; }
        }

        public Metalogix.Actions.ActionOptions ActionOptions
        {
            get { return this.Action.Options; }
            set { this.Action.Options = value; }
        }

        public ActionConfigContext(Metalogix.Actions.Action action, Metalogix.Actions.ActionContext context)
        {
            this._action = action;
            this._context = context;
            this._tempdata = new Dictionary<string, object>();
        }

        public void Dispose()
        {
            this._tempdata = null;
        }

        public T GetAction<T>()
            where T : Metalogix.Actions.Action
        {
            return (T)this._action;
        }

        public T GetActionOptions<T>()
            where T : Metalogix.Actions.ActionOptions
        {
            return (T)this.ActionOptions;
        }

        public object GetTempData(string string_0)
        {
            return this.GetTempDataAs<object>(string_0);
        }

        public T GetTempDataAs<T>(string string_0)
        {
            if (!this._tempdata.ContainsKey(string_0))
            {
                return default(T);
            }

            return (T)this._tempdata[string_0];
        }

        public void SetTempData(string string_0, object value)
        {
            if (this._tempdata.ContainsKey(string_0))
            {
                this._tempdata[string_0] = value;
                return;
            }

            this._tempdata.Add(string_0, value);
        }
    }
}