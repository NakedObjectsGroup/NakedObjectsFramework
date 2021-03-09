using System.Collections.Generic;

namespace NakedFunctions
{
    public static class CollectionExtensions
    {

        /// <summary>
        /// Helper method for adding to a collection on an immutable domain type.
        /// Enulates an Add method on an immutable collection, but actually using
        /// a mutable collection (which EF currently requires). Returns a NEW
        /// List based on the existing with new element added.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="coll"></param>
        /// <param name="obj"></param>
        /// <returns>A new List as ICollection</returns>
       public static ICollection<T> WithAdded<T>(this ICollection<T> coll, T obj)
        {
            var coll2 = new List<T>(coll);
            coll2.Add(obj);
            return coll2;
        }

        /// <summary>
        /// Helper method for removing from a collection on an immutable domain type.
        /// Enulates a Remove method on an immutable collection, but actually using
        /// a mutable collection (which EF currently requires). Returns a NEW
        /// List based on the existing with specified element not present.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="coll"></param>
        /// <param name="obj"></param>
        /// <returns>A new List as ICollection</returns>
        public static ICollection<T> WithRemoved<T>(this ICollection<T> coll, T obj)
        {
            var coll2 = new List<T>(coll);
            coll2.Remove(obj);
            return coll2;
        }
    }
}
