// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq;
using System.Reflection;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Component;
using NakedFramework.Core.Adapter;
using NakedFramework.Core.Error;
using NakedFramework.Core.Util;
using NakedFramework.Persistor.EF6.Component;

namespace NakedFramework.Persistor.EF6.Util {
    public static class EF6Helpers {
        public static void UpdateVersion(this INakedObjectAdapter nakedObjectAdapter, ISession session, INakedObjectManager manager) {
            var versionObject = nakedObjectAdapter?.GetVersion();
            if (versionObject != null) {
                nakedObjectAdapter.OptimisticLock = new ConcurrencyCheckVersion(session.UserName, DateTime.Now, versionObject);
            }
        }

        public static string GetEF6ProxiedTypeName(object domainObject) => domainObject.GetEF6ProxiedType().FullName;

        public static Type GetEF6ProxiedType(this object domainObject) =>
            domainObject.GetType() switch {
                { } t when FasterTypeUtils.IsEF6Proxy(t) => t.BaseType,
                { } t => t
            };

        internal static T Invoke<T>(this object onObject, string name, params object[] parms) => (T) onObject.GetType().GetMethod(name)?.Invoke(onObject, parms);

        internal static void Invoke(this object onObject, string name, params object[] parms) => onObject.GetType().GetMethod(name)?.Invoke(onObject, parms);

        internal static T GetProperty<T>(this object onObject, string name) => (T) onObject.GetType().GetProperty(name)?.GetValue(onObject);

        private static string GetNamespaceForType(this ObjectContext context, Type type) => context.MetadataWorkspace.GetItems(DataSpace.CSpace).Where(x => x.BuiltInTypeKind is BuiltInTypeKind.EntityType or BuiltInTypeKind.ComplexType).OfType<EdmType>().Where(et => et.Name == type.Name).Select(et => et.NamespaceName).SingleOrDefault();

        internal static StructuralType GetStructuralType(ObjectContext context, Type type) {
            var name = type.Name;
            var ns = context.GetNamespaceForType(type);
            return ns == null ? null : context.MetadataWorkspace.GetType(name, ns, false, DataSpace.CSpace) as StructuralType;
        }

        private static EntityType GetEntityType(this EF6LocalContext context, Type type) => context.GetStructuralType(type) as EntityType;

        private static bool IsTypeInOSpace(this ObjectContext context, Type type) => context.MetadataWorkspace.GetItems(DataSpace.OSpace).Where(x => x.BuiltInTypeKind is BuiltInTypeKind.EntityType or BuiltInTypeKind.ComplexType).OfType<EdmType>().Any(et => et.FullName == type.FullName);

        // problem is that OSpace is not populated until an object set is created. 
        // and there seems to be no way of navigating to the OSpace type from the CSpace. 
        // for the moment then workaround by attempting to create an object set.

        // For complex types this will only work if the parent is queried first
        public static bool ContextKnowsType(this EF6LocalContext context, Type type) =>
            context.WrappedObjectContext.IsTypeInOSpace(type) || context.CanCreateObjectSet(type);

        public static bool IdMembersAreIdentity(this EF6LocalContext context, Type type) {
            var et = GetEntityType(context, type);
            if (et != null) {
                var mp = et.KeyMembers.SelectMany(m => m.MetadataProperties).Where(p => p.Name.Contains("StoreGeneratedPattern")).ToArray();
                return mp.Any() && mp.All(p => p.Value.Equals("Identity"));
            }

            return false;
        }

        private static PropertyInfo[] SafeGetMembers(this EF6LocalContext context, Type type, Func<EntityType, IEnumerable<EdmMember>> getMembers) {
            var et = GetEntityType(context, type);
            return et != null
                ? type.GetProperties().Join(getMembers(et), pi => pi.Name, em => em.Name, (pi, em) => pi).ToArray()
                : Array.Empty<PropertyInfo>();
        }

        public static PropertyInfo[] GetIdMembers(this EF6LocalContext context, Type type) => context.SafeGetMembers(type, et => et.KeyMembers);

        public static PropertyInfo[] GetNavigationMembers(this EF6LocalContext context, Type type) => context.SafeGetMembers(type, et => et.NavigationProperties);

        public static PropertyInfo[] GetMembers(this EF6LocalContext context, Type type) => context.SafeGetMembers(type, et => et.Properties);

        public static PropertyInfo[] GetComplexMembers(this EF6LocalContext context, Type type) {
            var st = context.GetStructuralType(type);
            if (st != null) {
                var cm = st.Members.Where(m => m.TypeUsage.EdmType is ComplexType);
                return type.GetProperties().Join(cm, pi => pi.Name, em => em.Name, (pi, em) => pi).ToArray();
            }

            return Array.Empty<PropertyInfo>();
        }

        public static PropertyInfo[] GetReferenceMembers(this EF6LocalContext context, Type type) => context.GetNavigationMembers(type).Where(x => !CollectionUtils.IsCollection(x.PropertyType)).ToArray();

        public static PropertyInfo[] GetCollectionMembers(this EF6LocalContext context, Type type) => context.GetNavigationMembers(type).Where(x => CollectionUtils.IsCollection(x.PropertyType)).ToArray();

        public static PropertyInfo[] GetNonIdMembers(this EF6LocalContext context, Type type) => context.GetMembers(type).Where(x => !context.GetIdMembers(type).Contains(x)).ToArray();

        public static object CreateQuery(this EF6LocalContext context, Type type, string queryString, params ObjectParameter[] parameters) {
            var mostBaseType = context.GetMostBaseType(type);
            var mi = context.WrappedObjectContext.GetType().GetMethod("CreateQuery").MakeGenericMethod(mostBaseType);
            var parms = new List<object> {queryString, Array.Empty<ObjectParameter>()};

            var os = mi.Invoke(context.WrappedObjectContext, parms.ToArray());

            if (type != mostBaseType) {
                var ot = os.GetType().GetMethod("OfType").MakeGenericMethod(type);
                os = ot.Invoke(os, null);
            }

            return os;
        }

        public static bool CanCreateObjectSet(this EF6LocalContext context, Type type) {
            try {
                var mi = context.WrappedObjectContext.GetType().GetMethod("CreateObjectSet", Type.EmptyTypes).MakeGenericMethod(type);
                mi.Invoke(context.WrappedObjectContext, null);
                return true;
            }
            catch (Exception) {
                // expected (but ugly)
                if (EF6ObjectStore.RequireExplicitAssociationOfTypes) {
                    var msg = $"{type} is not explicitly associated with any DbContext, but 'RequireExplicitAssociationOfTypes' has been set on the PersistorInstaller";
                    throw new InitialisationException(msg);
                }
            }

            return false;
        }

        public static ObjectQuery GetObjectSet(this EF6LocalContext context, Type type) {
            var mostBaseType = context.GetMostBaseType(type);
            var mi = context.WrappedObjectContext.GetType().GetMethod("CreateObjectSet", Type.EmptyTypes).MakeGenericMethod(mostBaseType);
            var os = (ObjectQuery) mi.Invoke(context.WrappedObjectContext, null);
            os.MergeOption = context.DefaultMergeOption;
            return os;
        }

        // used reflectively
        // ReSharper disable once UnusedMember.Global
        public static IQueryable<TDerived> GetObjectSetOfType<TDerived, TBase>(this EF6LocalContext context) where TDerived : TBase {
            var mi = context.WrappedObjectContext.GetType().GetMethod("CreateObjectSet", Type.EmptyTypes).MakeGenericMethod(typeof(TBase));
            var os = (IQueryable<TBase>) InvokeUtils.Invoke(mi, context.WrappedObjectContext, null);
            ((ObjectQuery) os).MergeOption = context.DefaultMergeOption;
            return os.OfType<TDerived>();
        }

        public static object GetQueryableOfDerivedType<T>(this EF6LocalContext context) => context.GetQueryableOfDerivedType(typeof(T));

        public static object GetQueryableOfDerivedType(this EF6LocalContext context, Type type) {
            var mostBaseType = context.GetMostBaseType(type);
            var mi = typeof(EF6Helpers).GetMethod("GetObjectSetOfType").MakeGenericMethod(type, mostBaseType);
            return InvokeUtils.InvokeStatic(mi, new object[] {context});
        }

        public static object CreateObject(this EF6LocalContext context, Type type) {
            object objectSet = context.GetObjectSet(type);
            var methods = objectSet.GetType().GetMethods();
            var mi = methods.Single(m => m.Name == "CreateObject" && m.IsGenericMethod).MakeGenericMethod(type);
            return InvokeUtils.Invoke(mi, objectSet, null);
        }

        public static object[] GetKey(this EF6LocalContext context, object domainObject) => context.GetIdMembers(domainObject.GetEF6ProxiedType()).Select(x => x.GetValue(domainObject, null)).ToArray();

        public static object[] GetKey(this EF6LocalContext context, INakedObjectAdapter nakedObjectAdapter) => context.GetIdMembers(nakedObjectAdapter.GetDomainObject().GetEF6ProxiedType()).Select(x => x.GetValue(nakedObjectAdapter.GetDomainObject(), null)).ToArray();

        public static object First(IEnumerable enumerable) {
            // ReSharper disable once LoopCanBeConvertedToQuery
            // unfortunately this cast doesn't work with entity linq
            // return queryable.Cast<object>().FirstOrDefault();
            foreach (var o in enumerable) {
                return o;
            }

            return null;
        }

        public static IDictionary<string, object> MemberValueMap(ICollection<PropertyInfo> idmembers, ICollection<object> keyValues) {
            if (idmembers.Count != keyValues.Count) {
                throw new NakedObjectSystemException("Member and value counts must match");
            }

            return idmembers.Zip(keyValues, (k, v) => new {Key = k, Value = v})
                            .ToDictionary(x => x.Key.Name, x => x.Value);
        }

        public static IEnumerable<object> GetRelationshipEnds(ObjectContext context, ObjectStateEntry /*RelationshipEntry*/ ose) {
            var key0 = (EntityKey) ose.GetType().GetProperty("Key0", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(ose, null);
            var key1 = (EntityKey) ose.GetType().GetProperty("Key1", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(ose, null);

            var o0 = context.GetObjectByKey(key0);
            var o1 = context.GetObjectByKey(key1);

            return new[] {o0, o1};
        }

        public static IEnumerable<object> GetChangedObjectsInContext(ObjectContext context) {
            var addedOses = context.ObjectStateManager.GetObjectStateEntries(EntityState.Added).ToArray();
            var addedOseRelationships = addedOses.Where(ose => ose.IsRelationship);

            var deletedOses = context.ObjectStateManager.GetObjectStateEntries(EntityState.Deleted).ToArray();
            var deletedOseRelationships = deletedOses.Where(ose => ose.IsRelationship);

            var changedOses = context.ObjectStateManager.GetObjectStateEntries(EntityState.Modified);
            var changedEntities = changedOses.Select(x => x.Entity).ToList();

            addedOseRelationships.ForEach(x => changedEntities.AddRange(GetRelationshipEnds(context, x)));
            deletedOseRelationships.ForEach(x => changedEntities.AddRange(GetRelationshipEnds(context, x)));

            // this is here just to catch a case (adding sales reason to sales order in AdventureWorks) 
            // which doesn't work but which should.
            changedEntities.AddRange(GetRelationshipEndsForEntity(addedOses));
            changedEntities.AddRange(GetRelationshipEndsForEntity(deletedOses));

            // filter added and deleted entries 
            return changedEntities.Where(x => x != null).Distinct().Where(e => {
                context.ObjectStateManager.TryGetObjectStateEntry(e, out var ose);
                return ose != null && ose.State != EntityState.Deleted && ose.State != EntityState.Added;
            });
        }

        public static IEnumerable<object> GetRelationshipEndsForEntity(IEnumerable<ObjectStateEntry> addedOses) {
            var relatedends = addedOses.Where(ose => !ose.IsRelationship).SelectMany(x => x.RelationshipManager.GetAllRelatedEnds());
            var references = relatedends.Where(x => x.GetType().GetGenericTypeDefinition() == typeof(EntityReference<>));
            return references.Select(x => x.GetProperty<object>("Value"));
        }

        public static IEnumerable<object> GetChangedComplexObjectsInContext(EF6LocalContext context) =>
            context.WrappedObjectContext.ObjectStateManager.GetObjectStateEntries(EntityState.Modified).Select(ose => new {Obj = ose.Entity, Prop = context.GetComplexMembers(ose.Entity.GetEF6ProxiedType())}).SelectMany(a => a.Prop.Select(p => p.GetValue(a.Obj, null))).Where(x => x != null).Distinct();
    }
}