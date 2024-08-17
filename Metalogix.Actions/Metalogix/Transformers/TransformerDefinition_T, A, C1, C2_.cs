using Metalogix.Actions;
using Metalogix.DataStructures.Generic;
using Metalogix.Transformers.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Metalogix.Transformers
{
    public sealed class TransformerDefinition<T, A, C1, C2> : ITransformerDefinition
        where T : class
        where A : Metalogix.Actions.Action
        where C1 : IEnumerable
        where C2 : IEnumerable
    {
        private string m_sDefinitionName;

        private readonly bool m_bHidden;

        public Type ActionType
        {
            get { return typeof(A); }
        }

        public Type DataObjectType
        {
            get { return typeof(T); }
        }

        public bool Hidden
        {
            get { return this.m_bHidden; }
        }

        public string Name
        {
            get { return JustDecompileGenerated_get_Name(); }
            set { JustDecompileGenerated_set_Name(value); }
        }

        public string JustDecompileGenerated_get_Name()
        {
            return this.m_sDefinitionName;
        }

        public void JustDecompileGenerated_set_Name(string value)
        {
            this.m_sDefinitionName = value;
        }

        public Type SourceCollectionType
        {
            get { return typeof(C1); }
        }

        public Type TargetCollectionType
        {
            get { return typeof(C2); }
        }

        public TransformerDefinition(string sName, bool bHidden = false)
        {
            this.m_sDefinitionName = sName;
            this.m_bHidden = bHidden;
        }

        public void BeginTransformation(A action, C1 sources, C2 targets, TransformerCollection transformers)
        {
            foreach (ITransformer matchingTransformer in this.GetMatchingTransformers(transformers))
            {
                if (action != null)
                {
                    action.ConnectTransformer(matchingTransformer);
                }

                try
                {
                    MethodInfo method = matchingTransformer.GetType().GetMethod("BeginTransformation",
                        BindingFlags.Instance | BindingFlags.Public);
                    object[] objArray = new object[] { action, sources, targets };
                    method.Invoke(matchingTransformer, objArray);
                }
                finally
                {
                    if (action != null)
                    {
                        action.DisconnectTransformer(matchingTransformer);
                    }
                }
            }
        }

        private bool CheckType(Type transformerType, Type definitionType)
        {
            if (!transformerType.IsGenericType || !definitionType.IsGenericType)
            {
                return this.GetObjectHierarchy(definitionType).Contains(transformerType);
            }

            Type genericTypeDefinition = transformerType.GetGenericTypeDefinition();
            if (!this.GetObjectHierarchy(definitionType.GetGenericTypeDefinition()).Contains(genericTypeDefinition))
            {
                return false;
            }

            bool flag = true;
            Type[] genericArguments = definitionType.GetGenericArguments();
            Type[] typeArray = transformerType.GetGenericArguments();
            if ((int)genericArguments.Length == (int)typeArray.Length)
            {
                for (int i = 0; i < (int)genericArguments.Length; i++)
                {
                    flag &= typeArray[i].IsAssignableFrom(genericArguments[i]);
                }
            }

            return flag;
        }

        private bool CheckType(Type[] transformerConstraints, Type definitionType)
        {
            bool flag = true;
            Type[] typeArray = transformerConstraints;
            for (int i = 0; i < (int)typeArray.Length; i++)
            {
                Type type = typeArray[i];
                flag = (!flag ? false : this.CheckType(type, definitionType));
            }

            return flag;
        }

        public void EndTransformation(A action, C1 sources, C2 targets, TransformerCollection transformers)
        {
            foreach (ITransformer matchingTransformer in this.GetMatchingTransformers(transformers))
            {
                if (action != null)
                {
                    action.ConnectTransformer(matchingTransformer);
                }

                try
                {
                    MethodInfo method = matchingTransformer.GetType()
                        .GetMethod("EndTransformation", BindingFlags.Instance | BindingFlags.Public);
                    object[] objArray = new object[] { action, sources, targets };
                    method.Invoke(matchingTransformer, objArray);
                }
                finally
                {
                    if (action != null)
                    {
                        action.DisconnectTransformer(matchingTransformer);
                    }
                }
            }
        }

        public override bool Equals(object object_0)
        {
            ITransformerDefinition object0 = object_0 as ITransformerDefinition;
            if (object0 != null && this.Name == object0.Name && this.DataObjectType == object0.DataObjectType &&
                this.ActionType == object0.ActionType && this.SourceCollectionType == object0.SourceCollectionType &&
                this.TargetCollectionType == object0.TargetCollectionType)
            {
                return true;
            }

            return false;
        }

        public override int GetHashCode()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(this.Name);
            stringBuilder.Append(this.ActionType.FullName);
            stringBuilder.Append(this.DataObjectType.FullName);
            stringBuilder.Append(this.SourceCollectionType.FullName);
            stringBuilder.Append(this.TargetCollectionType.FullName);
            return stringBuilder.ToString().GetHashCode();
        }

        public TransformerCollection GetMatchingAvailableTransformers()
        {
            TransformerCollection matchingTransformers =
                this.GetMatchingTransformers(TransformerCollection.AvailableTransformers);
            Type type = typeof(A);
            Type type1 = typeof(T);
            Type type2 = typeof(C1);
            Type type3 = typeof(C2);
            Type[] typeArray = new Type[] { type1, type, type2, type3 };
            foreach (Type availableGenericTransformer in TransformerCollection.AvailableGenericTransformers)
            {
                Type[] genericArguments = availableGenericTransformer.GetGenericArguments();
                if ((int)genericArguments.Length != 4 ||
                    !this.CheckType(genericArguments[0].GetGenericParameterConstraints(), type1) ||
                    !this.CheckType(genericArguments[1].GetGenericParameterConstraints(), type) ||
                    !this.CheckType(genericArguments[2].GetGenericParameterConstraints(), type2) ||
                    !this.CheckType(genericArguments[3].GetGenericParameterConstraints(), type3))
                {
                    continue;
                }

                Type type4 = availableGenericTransformer.MakeGenericType(typeArray);
                matchingTransformers.Add(Activator.CreateInstance(type4) as ITransformer);
            }

            return matchingTransformers;
        }

        public TransformerCollection GetMatchingTransformers(TransformerCollection transformers)
        {
            TransformerCollection transformerCollection = new TransformerCollection();
            if (transformers != null)
            {
                Type type = typeof(A);
                Type type1 = typeof(T);
                Type type2 = typeof(C1);
                Type type3 = typeof(C2);
                foreach (ITransformer transformer in transformers)
                {
                    if (!TransformerCollection.IsTransformerEnabled(transformer) ||
                        !this.CheckType(transformer.ActionType, type) ||
                        !this.CheckType(transformer.DataObjectType, type1) ||
                        !this.CheckType(transformer.SourceCollectionType, type2) ||
                        !this.CheckType(transformer.TargetCollectionType, type3))
                    {
                        continue;
                    }

                    transformerCollection.Add(transformer);
                }
            }

            return transformerCollection;
        }

        private List<Type> GetObjectHierarchy(Type type)
        {
            List<Type> types = new List<Type>();
            types.AddRange(type.GetInterfaces());
            do
            {
                if (type.IsGenericType)
                {
                    type = type.GetGenericTypeDefinition();
                }

                types.Add(type);
                type = type.BaseType;
            } while (type != null);

            return types;
        }

        public override string ToString()
        {
            return this.Name;
        }

        public T Transform(T dataObject, A action, C1 sources, C2 targets, TransformerCollection transformers)
        {
            foreach (ITransformer matchingTransformer in this.GetMatchingTransformers(transformers))
            {
                if (dataObject == null)
                {
                    break;
                }

                if (action != null)
                {
                    action.ConnectTransformer(matchingTransformer);
                }

                try
                {
                    MethodInfo method = matchingTransformer.GetType()
                        .GetMethod("Transform", BindingFlags.Instance | BindingFlags.Public);
                    object[] objArray = new object[] { dataObject, action, sources, targets };
                    dataObject = (T)method.Invoke(matchingTransformer, objArray);
                }
                finally
                {
                    if (action != null)
                    {
                        action.DisconnectTransformer(matchingTransformer);
                    }
                }
            }

            return dataObject;
        }
    }
}