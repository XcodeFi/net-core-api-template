using System;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace Common.Common
{
    public static class Mapper
    {
        public static void Map<T>(T source, T destination)
        {
            // Might as well take care of null references early.
            if (source == null)
                throw new ArgumentNullException("source");
            if (destination == null)
                throw new ArgumentNullException("destination");

            var _propertyMap =
                source
                .GetType()
                .GetProperties()
                .ToDictionary(
                    p => p.Name.ToLower(),
                    p => p
                );

            foreach (var item in destination.GetType().GetProperties())
            {
                PropertyInfo p;
                var iName = item.Name.ToLower();
                if (_propertyMap.TryGetValue(iName, out p))
                {
                    if (p != null)
                        item.SetValue(destination, p.GetValue(source, null), null);
                }
            }
        }

        public static void MapDynamic<T>(ExpandoObject source, T destination)
        {
            // Might as well take care of null references early.
            if (source == null)
                throw new ArgumentNullException("source");
            if (destination == null)
                throw new ArgumentNullException("destination");

            var _propertyMap =
                typeof(T)
                .GetProperties()
                .ToDictionary(
                    p => p.Name.ToLower(),
                    p => p
                );

            // By iterating the KeyValuePair<string, object> of
            // source we can avoid manually searching the keys of
            // source as we see in your original code.
            foreach (var kv in source)
            {
                PropertyInfo p;
                if (_propertyMap.TryGetValue(kv.Key.ToLower(), out p))
                {
                    if (p != null)
                        p.SetValue(destination, kv.Value.ToString(), null);
                }
            }
        }
    }
}
