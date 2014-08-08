// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Util;
using NakedObjects.Core.Context;
using NakedObjects.Core.Persist;

namespace NakedObjects.Core.Util {
    public static class CopyUtils {
        private static void ShallowCopyCollectionGeneric<T>(ICollection<T> fromCollection, ICollection<T> toCollection) {
            Assert.AssertFalse(toCollection.Any());
            fromCollection.ForEach(toCollection.Add);
        }

        private static void ShallowUpdateCollectionGeneric<T>(ICollection<T> fromCollection, ICollection<T> toCollection) {
            var toRemove = new List<T>();
            toCollection.Where(i => !fromCollection.Contains(i)).ForEach(toRemove.Add);
            toRemove.ForEach(i => toCollection.Remove(i));
            var toAdd = new List<T>();
            fromCollection.Where(i => !toCollection.Contains(i)).ForEach(toAdd.Add);
            toAdd.ForEach(toCollection.Add);
        }

        private static void ShallowCopyCollection(object fromCollection, object toCollection) {
            MethodInfo cm = typeof (CopyUtils).GetMethod("ShallowCopyCollectionGeneric", BindingFlags.Static | BindingFlags.NonPublic);
            MethodInfo gcm = cm.MakeGenericMethod(toCollection.GetType().GetGenericArguments());
            gcm.Invoke(null, new[] {fromCollection, toCollection});
        }

        private static void ShallowUpdateCollection(object fromCollection, object toCollection) {
            MethodInfo cm = typeof (CopyUtils).GetMethod("ShallowUpdateCollectionGeneric", BindingFlags.Static | BindingFlags.NonPublic);
            MethodInfo gcm = cm.MakeGenericMethod(toCollection.GetType().GetGenericArguments());
            gcm.Invoke(null, new[] {fromCollection, toCollection});
        }


        public static object CloneObjectTest(object domainObject) {
            object clone = Activator.CreateInstance(domainObject.GetType());
            return CopyProperties(domainObject, clone);
        }

        private static object CopyProperties(object domainObject, object clone) {
            CopyScalarProperties(domainObject, clone);
            CopyCollectionProperties(domainObject, clone);
            return clone;
        }

        //public static INakedObject CloneInlineObject(object domainObject, INakedObject parent, INakedObjectAssociation field) {
        //    object clone = NakedObjectsContext.ObjectPersistor.CreateObject(reflector.LoadSpecification(domainObject.GetType()));
        //    INakedObject nakedObject = PersistorUtils.CreateAggregatedAdapterClone(parent, field, clone);
        //    NakedObjectsContext.ObjectPersistor.InitInlineObject(parent, clone);
        //    CopyProperties(domainObject, clone);
        //    return nakedObject;
        //}

        private static void CopyCollectionProperties(object fromObject, object toObject) {
            fromObject.GetType().GetProperties().
                       Where(p => CollectionUtils.IsCollection(p.PropertyType)).
                       ForEach(p => ShallowCopyCollection(p.GetValue(fromObject, null), p.GetValue(toObject, null)));
        }

        private static void UpdateCollectionProperties(object fromObject, object toObject) {
            fromObject.GetType().GetProperties().
                       Where(p => CollectionUtils.IsCollection(p.PropertyType)).
                       ForEach(p => ShallowUpdateCollection(p.GetValue(fromObject, null), p.GetValue(toObject, null)));
        }

        private static void CopyScalarProperties(object fromObject, object toObject) {
            fromObject.GetType().GetProperties().
                       Where(p => !CollectionUtils.IsCollection(p.PropertyType)).
                       Where(p => p.CanRead && p.CanWrite).
                       ForEach(p => p.SetValue(toObject, p.GetValue(fromObject, null), null));
        }

        public static void UpdateFromClone(object originalObject, object clonedObject) {
            CopyScalarProperties(clonedObject, originalObject);
            UpdateCollectionProperties(clonedObject, originalObject);
        }
    }
}