using Metalogix.Actions;
using Metalogix.DataStructures.Generic;
using Metalogix.Transformers.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Metalogix.Transformers
{
    public static class TransformerUtils
    {
        public static T GetFirstTransformerOfTypeFrom<T>(Metalogix.Actions.Action action)
            where T : ITransformer
        {
            return TransformerUtils.GetFirstTransformerOfTypeFrom<T>(action.SupportedDefinitions);
        }

        public static T GetFirstTransformerOfTypeFrom<T>(List<ITransformerDefinition> transformers)
            where T : ITransformer
        {
            IEnumerator<T> enumerator =
                TransformerUtils.GetTransformersOfTypeWorker<T>(transformers, true).GetEnumerator();
            if (enumerator.MoveNext())
            {
                return enumerator.Current;
            }

            return default(T);
        }

        public static IEnumerable<T> GetTransformersOfTypeFrom<T>(Metalogix.Actions.Action action)
            where T : ITransformer
        {
            return TransformerUtils.GetTransformersOfTypeFrom<T>(action.SupportedDefinitions);
        }

        public static IEnumerable<T> GetTransformersOfTypeFrom<T>(List<ITransformerDefinition> transformers)
            where T : ITransformer
        {
            return TransformerUtils.GetTransformersOfTypeWorker<T>(transformers, false);
        }

        private static IEnumerable<T> GetTransformersOfTypeWorker<T>(IEnumerable<ITransformerDefinition> transformers,
            bool isSingle)
            where T : ITransformer
        {
            if (transformers != null)
            {
                bool flag = false;
                foreach (ITransformerDefinition transformerDefinition in transformers)
                {
                    using (IEnumerator<ITransformer> enumerator =
                           transformerDefinition.GetMatchingAvailableTransformers().GetEnumerator())
                    {
                        do
                        {
                            Label1:
                            if (!enumerator.MoveNext())
                            {
                                goto Label0;
                            }

                            ITransformer current = enumerator.Current;
                            Type type = current.GetType();
                            if (type == typeof(T) || type.IsSubclassOf(typeof(T)))
                            {
                                yield return (T)current;
                            }
                            else
                            {
                                goto Label1;
                            }
                        } while (!isSingle);

                        flag = true;
                    }

                    Label0:
                    if (flag)
                    {
                        break;
                    }
                }
            }
        }
    }
}