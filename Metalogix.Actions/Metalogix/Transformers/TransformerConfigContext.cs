using Metalogix.Actions;
using Metalogix.Transformers.Interfaces;
using System;
using System.ComponentModel;

namespace Metalogix.Transformers
{
    public sealed class TransformerConfigContext
    {
        private readonly Metalogix.Actions.Action m_action;

        private readonly ITransformer m_transformer;

        private readonly Metalogix.Actions.ActionContext m_context;

        [ReadOnly(true)]
        public Metalogix.Actions.Action Action
        {
            get { return this.m_action; }
        }

        [ReadOnly(true)]
        public Metalogix.Actions.ActionContext ActionContext
        {
            get { return this.m_context; }
        }

        [ReadOnly(true)]
        public ITransformer Transformer
        {
            get { return this.m_transformer; }
        }

        public TransformerConfigContext(ITransformer transformer, Metalogix.Actions.Action action,
            Metalogix.Actions.ActionContext context)
        {
            this.m_transformer = transformer;
            this.m_action = action;
            this.m_context = context;
        }

        public T GetAction<T>()
            where T : Metalogix.Actions.Action
        {
            return (T)this.m_action;
        }

        public T GetActionOptions<T>()
            where T : ActionOptions
        {
            return (T)this.m_action.Options;
        }

        public T GetTransformer<T>()
            where T : ITransformer
        {
            return (T)this.m_transformer;
        }

        public T GetTransformerOptions<T>()
            where T : TransformerOptions
        {
            return (T)this.m_transformer.TransformerOptions;
        }
    }
}