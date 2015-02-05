// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NakedObjects.Core.Util {
    public static class CopyUtils {
        // used by reflection 
        // ReSharper disable once UnusedMember.Local
        private static void ShallowCopyCollectionGeneric<T>(ICollection<T> fromCollection, ICollection<T> toCollection) {
            Assert.AssertFalse(toCollection.Any());
            fromCollection.ForEach(toCollection.Add);
        }

        // used by reflection 
        // ReSharper disable once UnusedMember.Local
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