using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using FWF.FluidEntity.ComponentModel.Streams;
using FWF.FluidEntity.Logging;

namespace FWF.FluidEntity
{
    public static class ObjectExtensions
    {

        private static readonly BinaryFormatter _binaryFormatter = new BinaryFormatter();

        public static ILog Log
        {
            get;
            set;
        }

        public static byte[] BinarySerialize(this object obj)
        {
            using (var memoryStream = new MemoryPoolStream(1024))
            {
                _binaryFormatter.Serialize(memoryStream, obj);

                memoryStream.Flush();
                memoryStream.Position = 0;

                return memoryStream.ToArray();
            }
        }

        public static object BinaryDeserialize(this byte[] byteData)
        {
            using (var memoryStream = new MemoryStream(byteData))
            {
                memoryStream.Position = 0;

                return _binaryFormatter.Deserialize(memoryStream);
            }
        }

        public static T BinaryClone<T>(this T obj) where T : class
        {
            var binaryObject = obj.BinarySerialize();

            return binaryObject.BinaryDeserialize() as T;
        }

        public static IDictionary<string, object> ParseProperties(this object obj)
        {
            IDictionary<string, object> listProperties = new Dictionary<string, object>();

            var properties = TypeDescriptor.GetProperties(obj);

            foreach (PropertyDescriptor propertyDescriptor in properties)
            {
                var value = propertyDescriptor.GetValue(obj);

                listProperties.Add(propertyDescriptor.Name, value);
            }

            return listProperties;
        }
        

    }
}
