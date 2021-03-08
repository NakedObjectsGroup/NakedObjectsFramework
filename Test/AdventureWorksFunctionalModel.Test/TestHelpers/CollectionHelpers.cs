using System.Collections.Generic;


namespace AdventureWorksFunctionalModel.Test
{
    public static class CollectionHelpers
    {
       public static ICollection<T> WithAdded<T>(this ICollection<T> coll, T obj)
        {
            var coll2 = new List<T>(coll);
            coll2.Add(obj);
            return coll2;
        }

        public static ICollection<T> WithRemoved<T>(this ICollection<T> coll, T obj)
        {
            var coll2 = new List<T>(coll);
            coll2.Remove(obj);
            return coll2;
        }
    }
}
