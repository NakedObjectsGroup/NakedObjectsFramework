// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects.Services {
    /// <summary>
         //Provides a mechanism for retrieving a domain object given a 'compound key' (a single string that
         //defines both the type and the identity of that object). Also provides a method for determining
         //the compound key for a given object.
         //The intent of making both methods templated, is that it allows for the possibility of different
         //types (or perhaps different namespaces of types) having different ways of putting together the
         //compound key.
    /// </summary>
    public interface IObjectFinder {
        /// <summary>
        /// Given a compound key that defines both the type and the instance
        /// (e.g. "MyNamespace.Foo|10786"), this method will retrieve that object
        /// provided that it also implements the specified 'role interface' (T).
        /// </summary>
        /// <typeparam name="T">The 'role interface' that defines the relationship.</typeparam>
        /// <param name="compoundKey"></param>
        /// <returns></returns>
        T FindObject<T>(string compoundKey);

        /// <summary>
        /// Generates a compound key for the object that defines both its type and
        /// the instance, based on its keys e.g."MyNamespace.Foo|10786" or 
        /// "MyNamespace.Bar|10|56|name". The object must implement the specified
        /// 'role interface', T.
        /// </summary>
        /// <typeparam name="T">The 'role interface' that defines the relationship.</typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        string GetCompoundKey<T>(T obj);

        /// <summary>
        /// Helper method for retring an object that has a single integer key and
        /// where you know the object's type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        object FindBySingleIntegerKey(Type type, int key);

        /// <summary>
        /// Helper method for retring an object that has a single integer key and
        /// where you know the object's type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T FindBySingleIntegerKey<T>(int key) where T : class;
    }
}