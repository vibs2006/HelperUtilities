using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelperUtilities.Reflection
{
    public static class ReflectionStaticUtils
    {
        public static object ConvertToObjectWithoutPropertiesWithNullValues<T>(
            this T objectToTransform
            ,bool removeNulls = true)
        {
            var type = objectToTransform.GetType();
            var returnClass = new ExpandoObject() as IDictionary<string, object>;
            foreach (var propertyInfo in type.GetProperties())
            {
                var value = propertyInfo.GetValue(objectToTransform);
                //var valueIsNotAString = !(value is string && !string.IsNullOrWhiteSpace(value.ToString()));
                if (value != null && removeNulls == true)
                {
                    returnClass.Add(propertyInfo.Name, value);
                }
            }
            return returnClass;
        }
    }
}
