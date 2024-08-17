using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Metalogix.Actions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class SubActionTypesAttribute : Attribute
    {
        private readonly List<Type> m_listSubActionTypes;

        public List<Type> SubActionTypes
        {
            get { return this.m_listSubActionTypes; }
        }

        public SubActionTypesAttribute(Type type) : this(new Type[] { type })
        {
        }

        public SubActionTypesAttribute(Type[] types)
        {
            this.m_listSubActionTypes = new List<Type>(2);
            if (types == null || (int)types.Length == 0)
            {
                return;
            }

            Type[] typeArray = types;
            for (int i = 0; i < (int)typeArray.Length; i++)
            {
                Type type = typeArray[i];
                this.m_listSubActionTypes.Add(type);
            }
        }

        public override string ToString()
        {
            return (
                from type_0 in this.SubActionTypes
                select type_0.Name).Aggregate<string>((string string_0, string string_1) =>
                string.Concat(string_0, ", ", string_1));
        }
    }
}