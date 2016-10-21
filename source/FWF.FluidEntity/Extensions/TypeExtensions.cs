using System;
using System.Reflection;

namespace FWF.FluidEntity
{
    public static class TypeExtensions
    {

        public static string AssemblyName(this Type type)
        {
            string typeName = string.Format(
                "{0}, {1}",
                type.FullName,
                type.Assembly.GetName().Name
                );

            return typeName;
        }

        public static T CreateInstance<T>(this Type type)
        {
            var typeString = type.FullName;

            var constructorInfo = type.GetConstructor(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.Instance,
                null,
                Type.EmptyTypes,
                null
                );

            if (constructorInfo == null)
            {
                throw new Exception(
                    string.Format("Invalid type {0}, missing default constructor", typeString)
                    );
            }

            if (constructorInfo.IsPublic)
            {
                return (T)Activator.CreateInstance(type, true);
            }

            // Object is possibily a singleton component if the default constructor is not
            // public.  Locate the static singleton property to create the component

            var propertyInfo = type.GetProperty("Current");

            // Singleton property could be either Current or Instance
            if (propertyInfo == null)
            {
                propertyInfo = type.GetProperty("Instance");
            }

            if (propertyInfo == null)
            {
                throw new Exception(
                    string.Format(
                        "Unable to use type {0}, non-public constructor or missing known singleton property.",
                        typeString
                        )
                    );
            }

            // Get the singleton static property value
            T obj;

            try
            {
                obj = (T)propertyInfo.GetValue(null, null);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    string.Format(
                        "Unable to use type {0}, non-public constructor or missing known singleton property: {1}",
                        typeString, ex.Message
                        ),
                    ex
                    );
            }

            return obj;
        }


    }
}
