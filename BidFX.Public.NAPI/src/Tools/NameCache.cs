using System;
using System.Collections.Generic;

namespace BidFX.Public.NAPI.Tools
{
    /// <summary>
    /// This class provides a cache of names that can be used to generate unique names for things by appending an
    /// incrementing integer count to a name prefix.
    /// </summary>
    /// <author>Paul Sweeny</author>
    public sealed class NameCache
    {
        private static readonly NameCache DefaultNameCache = new NameCache();
        private readonly IDictionary<string, int> _cache = new Dictionary<string, int>();

        /// <summary>
        /// Gets the default <see cref="NameCache"/>.
        /// </summary>
        public static NameCache Default()
        {
            return DefaultNameCache;
        }

        /// <summary>Gets a unique name by adding an integer to the short name of the given class.</summary>
        /// <param name="type">the class to create a unique name for.</param>
        public string CreateUniqueName(Type type)
        {
            return CreateUniqueName(type.Name);
        }

        /// <summary>Gets a unique name by adding an integer to the given prefix.</summary>
        /// <param name="namePrefix">the name prefix for a set of similar names.</param>
        public string CreateUniqueName(string namePrefix)
        {
            return namePrefix + NextID(namePrefix);
        }

        /// <summary>Gets the next unique ID for a name prefix.</summary>
        /// <remarks>Gets the next unique ID for a name prefix. Each call increments the cached ID for the prefix.</remarks>
        /// <param name="namePrefix">the name prefix for a set of similar names.</param>
        public int NextID(string namePrefix)
        {
            lock (_cache)
            {
                var next = LastID(namePrefix) + 1;
                _cache[namePrefix] = next;
                return next;
            }
        }

        /// <summary>Gets the last unique ID for a name prefix.</summary>
        /// <remarks>Gets the last unique ID for a name prefix. This method does no increment the count for the prefix.</remarks>
        /// <param name="namePrefix">the name prefix for a set of similar names.</param>
        public int LastID(string namePrefix)
        {
            lock (_cache)
            {
                int last;
                _cache.TryGetValue(namePrefix, out last);
                return last;
            }
        }

        /// <summary>Gets the set of all names held in the name cache.</summary>
        /// <returns>a set of names.</returns>
        public ICollection<string> Names()
        {
            var collection = new List<string>(_cache.Keys);
            collection.Sort();
            return collection;
        }
    }
}