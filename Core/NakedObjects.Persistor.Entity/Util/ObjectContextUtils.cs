// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Reflection;
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Core;
using NakedObjects.Core.Util;
using NakedObjects.Persistor.Entity.Component;
using NakedObjects.Util;

namespace NakedObjects.Persistor.Entity.Util {
    public static class ObjectContextUtils {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ObjectContextUtils));
        private static readonly Dictionary<Type, IList<object>> GeneratedKeys = new Dictionary<Type, IList<object>>();

        internal static T Invoke<T>(this object onObject, string name, params object[] parms) => (T) onObject.GetType().GetMethod(name)?.Invoke(onObject, parms);

        internal static void Invoke(this object onObject, string name, params object[] parms) => onObject.GetType().GetMethod(name)?.Invoke(onObject, parms);

        internal static T GetProperty<T>(this object onObject, string name) => (T) onObject.GetType().GetProperty(name)?.GetValue(onObject);

        private static string GetNamespaceForType(this ObjectContext context, Type type) => context.MetadataWorkspace.GetItems(DataSpace.CSpace).Where(x => x.BuiltInTypeKind == BuiltInTypeKind.EntityType || x.BuiltInTypeKind == BuiltInTypeKind.ComplexType).OfType<EdmType>().Where(et => et.Name == type.Name).Select(et => et.NamespaceName).SingleOrDefault();

        internal static StructuralType GetStructuralType(ObjectContext context, Type type) {
            var name = type.Name;
            var ns = context.GetNamespaceForType(type);
            return ns == null ? null : context.MetadataWorkspace.GetType(name, ns, false, DataSpace.CSpace) as StructuralType;
        }

        private static EntityType GetEntityType(this EntityObjectStore.LocalContext context, Type type) => context.GetStructuralType(type) as EntityType;

        private static bool IsTypeInOSpace(this ObjectContext context, Type type) => context.MetadataWorkspace.GetItems(DataSpace.OSpace).Where(x => x.BuiltInTypeKind == BuiltInTypeKind.EntityType || x.BuiltInTypeKind == BuiltInTypeKind.ComplexType).OfType<EdmType>().Any(et => et.FullName == type.FullName);

        // problem is that OSpace is not populated until an object set is created. 
        // and there seems to be no way of navigating to the OSpace type from the CSpace. 
        // for the moment then workaround by attempting to create an object set.

        // For complex types this will only work if the parent is queried first
        public static bool ContextKnowsType(this EntityObjectStore.LocalContext context, Type type) =>
            context.WrappedObjectContext.IsTypeInOSpace(type) || context.CanCreateObjectSet(type);

        public static bool IdMembersAreIdentity(this EntityObjectStore.LocalContext context, Type type) {
            var et = GetEntityType(context, type);
            if (et != null) {
                var mp = et.KeyMembers.SelectMany(m => m.MetadataProperties).Where(p => p.Name.Contains("StoreGeneratedPattern")).ToArray();
                return mp.Any() && mp.All(p => p.Value.Equals("Identity"));
            }

            return false;
        }

        private static PropertyInfo[] SafeGetMembers(this EntityObjectStore.LocalContext context, Type type, Func<EntityType, IEnumerable<EdmMember>> getMembers) {
            var et = GetEntityType(context, type);
            return et != null
                ? type.GetProperties().Join(getMembers(et), pi => pi.Name, em => em.Name, (pi, em) => pi).ToArray()
                : new PropertyInfo[] { };
        }

        public static PropertyInfo[] GetIdMembers(this EntityObjectStore.LocalContext context, Type type) => context.SafeGetMembers(type, et => et.KeyMembers);

        public static PropertyInfo[] GetNavigationMembers(this EntityObjectStore.LocalContext context, Type type) => context.SafeGetMembers(type, et => et.NavigationProperties);

        public static PropertyInfo[] GetMembers(this EntityObjectStore.LocalContext context, Type type) => context.SafeGetMembers(type, et => et.Properties);

        public static PropertyInfo[] GetComplexMembers(this EntityObjectStore.LocalContext context, Type type) {
            var st = context.GetStructuralType(type);
            if (st != null) {
                var cm = st.Members.Where(m => m.TypeUsage.EdmType is ComplexType);
                return type.GetProperties().Join(cm, pi => pi.Name, em => em.Name, (pi, em) => pi).ToArray();
            }

            return new PropertyInfo[] { };
        }

        public static PropertyInfo[] GetReferenceMembers(this EntityObjectStore.LocalContext context, Type type) => context.GetNavigationMembers(type).Where(x => !CollectionUtils.IsCollection(x.PropertyType)).ToArray();

        public static PropertyInfo[] GetCollectionMembers(this EntityObjectStore.LocalContext context, Type type) => context.GetNavigationMembers(type).Where(x => CollectionUtils.IsCollection(x.PropertyType)).ToArray();

        public static PropertyInfo[] GetNonIdMembers(this EntityObjectStore.LocalContext context, Type type) => context.GetMembers(type).Where(x => !context.GetIdMembers(type).Contains(x)).ToArray();

        public static object CreateQuery(this EntityObjectStore.LocalContext context, Type type, string queryString, params ObjectParameter[] parameters) {
            var mostBaseType = context.GetMostBaseType(type);
            var mi = context.WrappedObjectContext.GetType().GetMethod("CreateQuery").MakeGenericMethod(mostBaseType);
            var parms = new List<object> {queryString, new ObjectParameter[] { }};

            var os = mi.Invoke(context.WrappedObjectContext, parms.ToArray());

            if (type != mostBaseType) {
                var ot = os.GetType().GetMethod("OfType").MakeGenericMethod(type);
                os = ot.Invoke(os, null);
            }

            return os;
        }

        public static bool CanCreateObjectSet(this EntityObjectStore.LocalContext context, Type type) {
            try {
                var mi = context.WrappedObjectContext.GetType().GetMethod("CreateObjectSet", Type.EmptyTypes).MakeGenericMethod(type);
                mi.Invoke(context.WrappedObjectContext, null);
                return true;
            }
            catch (Exception) {
                // expected (but ugly)
                if (EntityObjectStore.RequireExplicitAssociationOfTypes) {
                    var msg = $"{type} is not explicitly associated with any DbContext, but 'RequireExplicitAssociationOfTypes' has been set on the PersistorInstaller";
                    throw new InitialisationException(Log.LogAndReturn(msg));
                }
            }

            return false;
        }

        public static ObjectQuery GetObjectSet(this EntityObjectStore.LocalContext context, Type type) {
            var mostBaseType = context.GetMostBaseType(type);
            var mi = context.WrappedObjectContext.GetType().GetMethod("CreateObjectSet", Type.EmptyTypes).MakeGenericMethod(mostBaseType);
            var os = (ObjectQuery) mi.Invoke(context.WrappedObjectContext, null);
            os.MergeOption = context.DefaultMergeOption;
            return os;
        }

        // used reflectively
        // ReSharper disable once UnusedMember.Global
        public static IQueryable<TDerived> GetObjectSetOfType<TDerived, TBase>(this EntityObjectStore.LocalContext context) where TDerived : TBase {
            var mi = context.WrappedObjectContext.GetType().GetMethod("CreateObjectSet", Type.EmptyTypes).MakeGenericMethod(typeof(TBase));
            var os = (IQueryable<TBase>) InvokeUtils.Invoke(mi, context.WrappedObjectContext, null);
            ((ObjectQuery) os).MergeOption = context.DefaultMergeOption;
            return os.OfType<TDerived>();
        }

        public static object GetQueryableOfDerivedType<T>(this EntityObjectStore.LocalContext context) => context.GetQueryableOfDerivedType(typeof(T));

        public static object GetQueryableOfDerivedType(this EntityObjectStore.LocalContext context, Type type) {
            var mostBaseType = context.GetMostBaseType(type);
            var mi = typeof(ObjectContextUtils).GetMethod("GetObjectSetOfType").MakeGenericMethod(type, mostBaseType);
            return InvokeUtils.InvokeStatic(mi, new object[] {context});
        }

        public static object CreateObject(this EntityObjectStore.LocalContext context, Type type) {
            object objectSet = context.GetObjectSet(type);
            var methods = objectSet.GetType().GetMethods();
            var mi = methods.Single(m => m.Name == "CreateObject" && m.IsGenericMethod).MakeGenericMethod(type);
            return InvokeUtils.Invoke(mi, objectSet, null);
        }

        public static object[] GetKey(this EntityObjectStore.LocalContext context, object domainObject) => context.GetIdMembers(domainObject.GetEntityProxiedType()).Select(x => x.GetValue(domainObject, null)).ToArray();

        public static object[] GetKey(this EntityObjectStore.LocalContext context, INakedObjectAdapter nakedObjectAdapter) => context.GetIdMembers(nakedObjectAdapter.GetDomainObject().GetEntityProxiedType()).Select(x => x.GetValue(nakedObjectAdapter.GetDomainObject(), null)).ToArray();
    }
}