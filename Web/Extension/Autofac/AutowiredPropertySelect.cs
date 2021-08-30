using Autofac.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Web.Extension.Autofac
{
    public class AutowiredPropertySelect:IPropertySelector
    {
        public bool InjectProperty(PropertyInfo propertyInfo, object instance)
        {
            return propertyInfo.CustomAttributes.Any(i
                => i.AttributeType == typeof(AutowiredAttribute));
        } 
    }
}
