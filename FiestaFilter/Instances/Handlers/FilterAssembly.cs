using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace Filter.Instances.Handlers
{
    internal class FilterAssembly
    {
        public static IEnumerable<Combine<PacketAttribute, MethodInfo>> FindHandlers<PacketAttribute>()
            where PacketAttribute : Attribute
        {
            return (from Method in AppDomain.CurrentDomain.GetAssemblies()
                        .Where(Assembly => !Assembly.GlobalAssemblyCache)
                        .SelectMany(Types => Types.GetTypes())
                        .SelectMany(Methods => Methods.GetMethods())
                    let CustomAttribute = Attribute.GetCustomAttribute(Method, typeof(PacketAttribute), false) as PacketAttribute
                    where CustomAttribute != null
                    select new Combine<PacketAttribute, MethodInfo>(CustomAttribute, Method));
        }
    }
}
