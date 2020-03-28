// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Data.Common;
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
        private static readonly ILog Log = LogManager.GetLogger(typeof (ObjectContextUtils));
        private static readonly Dictionary<Type, IList<object>> GeneratedKeys = new Dictionary<Type, IList<object>>();

        private static string GetNamespaceForType(this ObjectContext context, Type type) {
            return context.MetadataWorkspace.GetItems(DataSpace.CSpace).
                Where(x => x.BuiltInTypeKind == BuiltInTypeKind.EntityType || x.BuiltInTypeKind == BuiltInTypeKind.ComplexType).
                OfType<EdmType>().Where(et => et.Name == type.Name).Select(et => et.NamespaceName).SingleOrDefault();
        }

        internal static StructuralType GetStructuralType(ObjectContext context, Type type) {
            string name = type.Name;
            string ns = context.GetNamespaceForType(type);
            return ns == null ? null : context.MetadataWorkspace.GetType(name, ns, false, DataSpace.CSpace) as StructuralType;
        }

        private static EntityType GetEntityType(this EntityObjectStore.LocalContext context, Type type) {
            return context.GetStructuralType(type) as EntityType;
        }

        private static bool IsTypeInOSpace(this ObjectContext context, Type type) {
            return context.MetadataWorkspace.GetItems(DataSpace.OSpace).
                Where(x => x.BuiltInTypeKind == BuiltInTypeKind.EntityType || x.BuiltInTypeKind == BuiltInTypeKind.ComplexType).
                OfType<EdmType>().Any(et => et.FullName == type.FullName);
        }

        public static bool ContextKnowsType(this EntityObjectStore.LocalContext context, Type type) {
            // problem is that OSpace is not populated until an object set is created. 
            // and there seems to be no way of navigating to the OSpace type from the CSpace. 
            // for the moment then workaround by attempting to create an object set.

            // For complex types this will only work if the parent is queried first 
            if (context.WrappedObjectContext.IsTypeInOSpace(type)) {
                Log.DebugFormat("Context {0} found type {1} in OSpace", context.Name, type.FullName);
                return true;
            }

            if (context.CanCreateObjectSet(type)) {
                Log.DebugFormat("Context {0} found object set for type  {1}", context.Name, type.FullName);
                return true;
            }
            Log.DebugFormat("Context {0} failed to get object set for type  {1}", context.Name, type.FullName);
            return false;
        }

        private static object GetNextKey(Type type, int key) {
            if (!GeneratedKeys.ContainsKey(type)) {
                GeneratedKeys[type] = new List<object> {key};
                return key;
            }
            while (GeneratedKeys[type].Contains(key)) {
                ++key;
            }
            GeneratedKeys[type].Add(key);

            return key;
        }

        public static object GetNextKey(this EntityObjectStore.LocalContext context, Type type) {
            PropertyInfo idMember = context.GetIdMembers(type).Single();
            string query = string.Format("max(it.{0}) + 1", idMember.Name);

            dynamic os = context.GetObjectSet(type);
            ObjectQuery<DbDataRecord> results = os.Select(query);
            DbDataRecord result = results.Single();
            return GetNextKey(type, result.IsDBNull(0) ? 1 : (int) result[0]);
        }

        public static bool IdMembersAreIdentity(this EntityObjectStore.LocalContext context, Type type) {
            EntityType et = GetEntityType(context, type);
            if (et != null) {
                MetadataProperty[] mp = et.KeyMembers.SelectMany(m => m.MetadataProperties).Where(p => p.Name.Contains("StoreGeneratedPattern")).ToArray();
                return mp.Any() && mp.All(p => p.Value.Equals("Identity"));
            }
            return false;
        }

        private static PropertyInfo[] SafeGetMembers(this EntityObjectStore.LocalContext context, Type type, Func<EntityType, IEnumerable<EdmMember>> getMembers) {
            EntityType et = GetEntityType(context, type);
            if (et != null) {
                return type.GetProperties().Join(getMembers(et), pi => pi.Name, em => em.Name, (pi, em) => pi).ToArray();
            }
            return new PropertyInfo[] {};
        }

        public static PropertyInfo[] GetIdMembers(this EntityObjectStore.LocalContext context, Type type) {
            return context.SafeGetMembers(type, et => et.KeyMembers);
        }

        public static PropertyInfo[] GetNavigationMembers(this EntityObjectStore.LocalContext context, Type type) {
            return context.SafeGetMembers(type, et => et.NavigationProperties);
        }

        public static PropertyInfo[] GetMembers(this EntityObjectStore.LocalContext context, Type type) {
            return context.SafeGetMembers(type, et => et.Properties);
        }

        public static PropertyInfo[] GetComplexMembers(this EntityObjectStore.LocalContext context, Type type) {
            StructuralType st = context.GetStructuralType(type);
            if (st != null) {
                IEnumerable<EdmMember> cm = st.Members.Where(m => m.TypeUsage.EdmType is ComplexType);
                return type.GetProperties().Join(cm, pi => pi.Name, em => em.Name, (pi, em) => pi).ToArray();
            }
            return new PropertyInfo[] {};
        }

        public static PropertyInfo[] GetReferenceMembers(this EntityObjectStore.LocalContext context, Type type) {
            return context.GetNavigationMembers(type).Where(x => !CollectionUtils.IsCollection(x.PropertyType)).ToArray();
        }

        public static PropertyInfo[] GetCollectionMembers(this EntityObjectStore.LocalContext context, Type type) {
            return context.GetNavigationMembers(type).Where(x => CollectionUtils.IsCollection(x.PropertyType)).ToArray();
        }

        public static PropertyInfo[] GetAllMembers(this EntityObjectStore.LocalContext context, Type type) {
            return context.GetMembers(type).Union(context.GetNavigationMembers(type)).ToArray();
        }

        public static PropertyInfo[] GetAllNonIdMembers(this EntityObjectStore.LocalContext context, Type type) {
            return context.GetAllMembers(type).Where(x => !context.GetIdMembers(type).Contains(x)).ToArray();
        }

        public static PropertyInfo[] GetNonIdMembers(this EntityObjectStore.LocalContext context, Type type) {
            return context.GetMembers(type).Where(x => !context.GetIdMembers(type).Contains(x)).ToArray();
        }

        public static dynamic CreateQuery(this EntityObjectStore.LocalContext context, Type type, string queryString, params ObjectParameter[] parameters) {
            Type mostBaseType = context.GetMostBaseType(type);
            MethodInfo mi = context.WrappedObjectContext.GetType().GetMethod("CreateQuery").MakeGenericMethod(mostBaseType);
            var parms = new List<object> {queryString, new ObjectParameter[] {}};

            dynamic os = mi.Invoke(context.WrappedObjectContext, parms.ToArray());

            if (type != mostBaseType) {
                dynamic ot = os.GetType().GetMethod("OfType").MakeGenericMethod(type);
                os = ot.Invoke(os, null);
            }

            return os;
        }

        public static bool CanCreateObjectSet(this EntityObjectStore.LocalContext context, Type type) {
            try {
                MethodInfo mi = context.WrappedObjectContext.GetType().GetMethod("CreateObjectSet", Type.EmptyTypes).MakeGenericMethod(type);
                mi.Invoke(context.WrappedObjectContext, null);
                return true;
            }
            catch (Exception e) {
                // expected (but ugly)
                Log.DebugFormat("Context {0} did not recognise type {1} and threw {2}", context.Name, type.FullName, e.Message);
                if (EntityObjectStore.RequireExplicitAssociationOfTypes) {
                    string msg = string.Format("{0} is not explicitly associated with any DbContext, but 'RequireExplicitAssociationOfTypes' has been set on the PersistorInstaller", type);
                    throw new InitialisationException(msg);
                }
            }
            return false;
        }

        public static dynamic GetObjectSet(this EntityObjectStore.LocalContext context, Type type) {
            Type mostBaseType = context.GetMostBaseType(type);
            MethodInfo mi = context.WrappedObjectContext.GetType().GetMethod("CreateObjectSet", Type.EmptyTypes).MakeGenericMethod(mostBaseType);
            dynamic os = mi.Invoke(context.WrappedObjectContext, null);
            os.MergeOption = context.DefaultMergeOption;
            return os;
        }

        private static string RecordState(Type type, Type msb, MethodInfo mi, MethodInfo gmi, ObjectContext context) {
            try {
                string typeName = type.FullName;
                string msbTypeName = msb.FullName;
                string methodName = mi.Name;
                string methodOwner = mi.DeclaringType.FullName;
                string gMethodName = gmi.Name;
                string gMethodOwner = gmi.DeclaringType.FullName;
                string contextType = context.GetType().FullName;
                return " type: " + typeName +
                       " msb type: " + msbTypeName +
                       " method: " + methodName +
                       " owner: " + methodOwner +
                       " g method: " + gMethodName +
                       " g owner: " + gMethodOwner +
                       " contextType: " + contextType;
            }
            catch (Exception) {
                // catch all errors
                return " state failed ";
            }
        }

        public static ObjectQuery GetObjectSetNonDynamic(this EntityObjectStore.LocalContext context, Type type) {

            string preState = "";

            try {
                Type mostBaseType = context.GetMostBaseType(type);
                ObjectContext objectContext = context.WrappedObjectContext;
                MethodInfo mi = objectContext.GetType().GetMethod("CreateObjectSet", Type.EmptyTypes);
                MethodInfo gmi = mi.MakeGenericMethod(mostBaseType);
                preState = RecordState(type, mostBaseType, mi, gmi, objectContext);
                ObjectQuery os = (ObjectQuery) mi.Invoke(objectContext, null);
                os.MergeOption = context.DefaultMergeOption;
                return os;
            }
            catch (Exception e) {
                Type mostBaseType = context.GetMostBaseType(type);
                ObjectContext objectContext = context.WrappedObjectContext;
                MethodInfo mi = objectContext.GetType().GetMethod("CreateObjectSet", Type.EmptyTypes);
                MethodInfo gmi = mi.MakeGenericMethod(mostBaseType);
                string postState = RecordState(type, mostBaseType, mi, gmi, objectContext);

                string error = "pre - " + preState + " post - " + postState;
                Log.Error(error);
                var ee = new Exception(error, e);
                throw ee;
            }
        }

        public static IQueryable<TDerived> GetObjectSetOfType<TDerived, TBase>(this EntityObjectStore.LocalContext context) where TDerived : TBase {
            MethodInfo mi = context.WrappedObjectContext.GetType().GetMethod("CreateObjectSet", Type.EmptyTypes).MakeGenericMethod(typeof (TBase));
            var os = (IQueryable<TBase>) InvokeUtils.Invoke(mi, context.WrappedObjectContext, null);
            ((ObjectQuery) os).MergeOption = context.DefaultMergeOption;
            return os.OfType<TDerived>();
        }

        public static dynamic GetQueryableOfDerivedType<T>(this EntityObjectStore.LocalContext context) {
            return context.GetQueryableOfDerivedType(typeof (T));
        }

        public static dynamic GetQueryableOfDerivedType(this EntityObjectStore.LocalContext context, Type type) {
            Type mostBaseType = context.GetMostBaseType(type);
            MethodInfo mi = typeof (ObjectContextUtils).GetMethod("GetObjectSetOfType").MakeGenericMethod(type, mostBaseType);
            return InvokeUtils.InvokeStatic(mi, new object[] {context});
        }

        public static dynamic CreateObject(this EntityObjectStore.LocalContext context, Type type) {
            ObjectQuery objectSet = context.GetObjectSetNonDynamic(type);
            MethodInfo[] methods = objectSet.GetType().GetMethods();
            MethodInfo mi = methods.Single(m => m.Name == "CreateObject" && m.IsGenericMethod).MakeGenericMethod(type);
            return InvokeUtils.Invoke(mi, objectSet, null);
        }

        public static object ProxyObject(this EntityObjectStore.LocalContext context, object objectToProxy) {
            if (TypeUtils.IsProxy(objectToProxy.GetType())) {
                return objectToProxy;
            }

            object newObject = context.GetObjectSet(objectToProxy.GetType()).CreateObject();

            PropertyInfo[] idMembers = context.GetIdMembers(objectToProxy.GetType());

            idMembers.ForEach(pi => newObject.GetType().GetProperty(pi.Name).SetValue(newObject, pi.GetValue(objectToProxy, null), null));

            return context.GetObjectSet(objectToProxy.GetType()).ApplyCurrentValues((dynamic) newObject);
        }

        public static object[] GetKey(this EntityObjectStore.LocalContext context, object domainObject) {
            return context.GetIdMembers(domainObject.GetEntityProxiedType()).Select(x => x.GetValue(domainObject, null)).ToArray();
        }

        public static object[] GetKey(this EntityObjectStore.LocalContext context, INakedObjectAdapter nakedObjectAdapter) {
            return context.GetIdMembers(nakedObjectAdapter.GetDomainObject().GetEntityProxiedType()).Select(x => x.GetValue(nakedObjectAdapter.GetDomainObject(), null)).ToArray();
        }
    }
}