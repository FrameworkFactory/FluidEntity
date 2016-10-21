using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace FWF.FluidEntity
{
    public static class ObjectExtensions
    {
        
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
