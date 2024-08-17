using Metalogix.Metabase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Metalogix.Metabase.DataTypes
{
    public class DataTypeUtils
    {
        public DataTypeUtils()
        {
        }

        public static ISmartDataType CreateInstance(Type type, RecordPropertyDescriptor parentProperty)
        {
            if (!typeof(ISmartDataType).IsAssignableFrom(type))
            {
                throw new ArgumentException("Specified type does not implement SmartDataType", "type");
            }

            ConstructorInfo[] constructors = type.GetConstructors();
            bool flag = false;
            ConstructorInfo[] constructorInfoArray = constructors;
            int num = 0;
            while (num < (int)constructorInfoArray.Length)
            {
                ParameterInfo[] parameters = constructorInfoArray[num].GetParameters();
                if ((int)parameters.Length != 1 || parameters[0].ParameterType != typeof(PropertyDescriptor))
                {
                    num++;
                }
                else
                {
                    flag = true;
                    break;
                }
            }

            if (!flag)
            {
                throw new Exception(string.Format("Type does not have valid constructor format (Type: {0})",
                    type.FullName));
            }

            object[] objArray = new object[] { parentProperty };
            return Activator.CreateInstance(type, objArray) as ISmartDataType;
        }

        public static ISmartDataType CreateInstance(Type type, RecordPropertyDescriptor parentProperty,
            string sSerializedValue)
        {
            if (!typeof(ISmartDataType).IsAssignableFrom(type))
            {
                throw new ArgumentException("Specified type does not implement SmartDataType", "type");
            }

            ISmartDataType smartDataType = DataTypeUtils.CreateInstance(type, parentProperty);
            smartDataType.Deserialize(sSerializedValue);
            return smartDataType;
        }

        public static Type[] LoadSmartDataTypes()
        {
            bool flag;
            return DataTypeUtils.LoadSmartDataTypes(out flag);
        }

        public static Type[] LoadSmartDataTypes(out bool bEntryAssemblyNotFound)
        {
            List<Type> types = new List<Type>();
            Assembly entryAssembly = Assembly.GetEntryAssembly();
            bEntryAssemblyNotFound = entryAssembly == null;
            AssemblyName[] assemblyNameArray =
                (entryAssembly != null ? entryAssembly.GetReferencedAssemblies() : new AssemblyName[0]);
            for (int i = 0; i < (int)assemblyNameArray.Length; i++)
            {
                AssemblyName assemblyName = assemblyNameArray[i];
                try
                {
                    Type[] exportedTypes = Assembly.Load(assemblyName).GetExportedTypes();
                    for (int j = 0; j < (int)exportedTypes.Length; j++)
                    {
                        Type type = exportedTypes[j];
                        if (type.BaseType != null && typeof(ISmartDataType).IsAssignableFrom(type) &&
                            !type.IsAbstract && !types.Contains(type))
                        {
                            types.Add(type);
                        }
                    }
                }
                catch (Exception exception)
                {
                }
            }

            return types.ToArray();
        }

        public static T ReadAttributeFromSmartDataType<T>(Type smartDataType)
            where T : Attribute
        {
            if (!typeof(ISmartDataType).IsAssignableFrom(smartDataType))
            {
                throw new ArgumentException("Specified type is not subclass of SmartDataType", "smartDataType");
            }

            object[] customAttributes = smartDataType.GetCustomAttributes(typeof(T), false);
            return ((int)customAttributes.Length > 0 ? (T)(customAttributes[0] as T) : default(T));
        }

        public static string ReadDisplayNameAttribute(Type smartDataType)
        {
            if (!typeof(ISmartDataType).IsAssignableFrom(smartDataType))
            {
                throw new ArgumentException("Specified type does not implement SmartDataType", "smartDataType");
            }

            DisplayNameAttribute displayNameAttribute =
                DataTypeUtils.ReadAttributeFromSmartDataType<DisplayNameAttribute>(smartDataType);
            string empty = string.Empty;
            if (displayNameAttribute != null)
            {
                empty = displayNameAttribute.DisplayName;
            }

            return empty;
        }
    }
}