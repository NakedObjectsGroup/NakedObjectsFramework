// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using NakedObjects.Facade;
using NakedObjects.Rest.Snapshot.Constants;
using NakedObjects.Rest.Snapshot.Utility;

namespace NakedObjects.Rest.Snapshot.Representations {
    [DataContract]
    public class Representation : IRepresentation {
        private static readonly object ModuleBuilderLock = new object();
        protected CacheType Caching;
        protected string Etag;
        protected List<string> Warnings = new List<string>();

        public Representation(IOidStrategy oidStrategy, RestControlFlags flags) {
            OidStrategy = oidStrategy;
            Flags = flags;
        }

        protected IOidStrategy OidStrategy { get; set; }
        protected RestControlFlags Flags { get; }

        private static ModuleBuilder ModuleBuilder { get; set; }

        protected RelType SelfRelType { get; set; }

        #region IRepresentation Members

        public virtual MediaTypeHeaderValue GetContentType() => SelfRelType?.GetMediaType(Flags);

        public EntityTagHeaderValue GetEtag() => Etag != null ? new EntityTagHeaderValue($"\"{Etag}\"") : null;

        public CacheType GetCaching() => Caching;

        public string[] GetWarnings() {
            var allWarnings = new List<string>(Warnings);

            var properties = GetType().GetProperties();

            var repProperties = properties.Where(p => typeof(IRepresentation).IsAssignableFrom(p.PropertyType)).Select(p => (IRepresentation) p.GetValue(this, null));
            var repProperties1 = properties.Where(p => typeof(IRepresentation[]).IsAssignableFrom(p.PropertyType)).SelectMany(p => (IRepresentation[]) p.GetValue(this, null));

            allWarnings.AddRange(repProperties.Where(p => p != null).SelectMany(p => p.GetWarnings()));
            allWarnings.AddRange(repProperties1.Where(p => p != null).SelectMany(p => p.GetWarnings()));

            return allWarnings.ToArray();
        }

        public Uri GetLocation() => SelfRelType?.GetUri();

        #endregion

        protected void SetEtag(string digest) {
            if (digest != null) {
                Etag = digest;
            }
        }

        private bool IsMutable(IObjectFacade target) => Flags.AllowMutatingActionsOnImmutableObject || !target.Specification.IsImmutable(target);

        protected void SetEtag(IObjectFacade target) {
            if (!target.Specification.IsService && IsMutable(target)) {
                var digest = target.Version.Digest;
                SetEtag(digest);
            }
        }

        private static void EnsureModuleBuilderExists() {
            if (ModuleBuilder == null) {
                const string assemblyName = "NakedObjectsRestProxies";
                var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.Run);
                ModuleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName);
            }
        }

        private static void CreateProperty(TypeBuilder typeBuilder, Type propertyType, string name) {
            var fieldBuilder = typeBuilder.DefineField("_" + name, propertyType, FieldAttributes.Private);

            var propertyBuilder = typeBuilder.DefineProperty(name, PropertyAttributes.None, propertyType, Type.EmptyTypes);

            var getMethodBuilder = typeBuilder.DefineMethod("get_" + name, MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Public, propertyType, Type.EmptyTypes);
            var iLGenerator = getMethodBuilder.GetILGenerator();
            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
            iLGenerator.Emit(OpCodes.Ret);

            var setMethodBuilder = typeBuilder.DefineMethod("set_" + name, MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Public, null, new[] {propertyType});

            iLGenerator = setMethodBuilder.GetILGenerator();
            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Ldarg_1);
            iLGenerator.Emit(OpCodes.Stfld, fieldBuilder);
            iLGenerator.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getMethodBuilder);
            propertyBuilder.SetSetMethod(setMethodBuilder);

            var dataMemberAttrType = typeof(DataMemberAttribute);
            var prop = dataMemberAttrType.GetProperty("Name");

            var customAttribute = new CustomAttributeBuilder(dataMemberAttrType.GetConstructor(new Type[] { }),
                new object[] { },
                new[] {prop},
                new object[] {name});

            propertyBuilder.SetCustomAttribute(customAttribute);
        }

        private static string ComputeMD5HashAsString(string s) => Math.Abs(BitConverter.ToInt64(ComputeMD5HashFromString(s), 0)).ToString(CultureInfo.InvariantCulture);

        private static byte[] ComputeMD5HashFromString(string s) {
            var idAsBytes = Encoding.UTF8.GetBytes(s);
            return new MD5CryptoServiceProvider().ComputeHash(idAsBytes);
        }

        protected static T CreateWithOptionals<T>(object[] ctorParms, IList<OptionalProperty> properties) {
            var toHash = (properties.Aggregate("", (s, t) => s + t.Name + "." + t.PropertyType.FullName + ".") + typeof(T).Name).Replace("[]", "Array");
            var hash = ComputeMD5HashAsString(toHash);

            var typeName = "NakedObjects.Snapshot.Rest.Representations." + hash;

            var newRep = CreateInstanceOfDynamicType<T>(ctorParms, properties, typeName);

            foreach (var p in properties) {
                newRep.GetType().GetProperty(p.Name)?.SetValue(newRep, p.Value, null);
            }

            return (T) newRep;
        }

        private static object CreateInstanceOfDynamicType<T>(object[] ctorParms, IList<OptionalProperty> properties, string typeName) {
            Type proxyType;
            lock (ModuleBuilderLock) {
                EnsureModuleBuilderExists();
                proxyType = ModuleBuilder.GetType(typeName);

                if (proxyType == null) {
                    var typeBuilder = ModuleBuilder.DefineType(typeName, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Sealed, typeof(T));

                    foreach (var w in properties) {
                        CreateProperty(typeBuilder, w.PropertyType, w.Name);
                    }

                    CreateConstructor<T>(typeBuilder);
                    proxyType = typeBuilder.CreateType();
                }
            }

            return Activator.CreateInstance(proxyType, ctorParms);
        }

        private static void CreateConstructor<T>(TypeBuilder typeBuilder) {
            var parentCtor = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).SingleOrDefault();

            if (parentCtor != null) {
                var parentCtorParms = parentCtor.GetParameters();
                var ctor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, parentCtorParms.Select(p => p.ParameterType).ToArray());

                for (var idx = 0; idx < parentCtorParms.Length; idx++) {
                    ctor.DefineParameter(idx + 1, ParameterAttributes.None, parentCtorParms[idx].Name);
                }

                var ilGenerator = ctor.GetILGenerator();

                ilGenerator.Emit(OpCodes.Ldarg_0);
                for (var i = 1; i <= parentCtorParms.Length; i++) {
                    ilGenerator.Emit(OpCodes.Ldarg, i);
                }

                ilGenerator.Emit(OpCodes.Call, parentCtor);
                ilGenerator.Emit(OpCodes.Ret);
            }
        }

        public static object GetPropertyValue(IOidStrategy oidStrategy, HttpRequest req, IAssociationFacade property, IObjectFacade target, RestControlFlags flags, bool valueOnly, bool useDateOverDateTime) {
            var valueNakedObject = property.GetValue(target);

            if (valueNakedObject == null) {
                return null;
            }

            if (target.IsTransient && property.IsUsable(target).IsAllowed && property.IsVisible(target) && property.IsSetToImplicitDefault(target)) {
                return null;
            }

            if (property.Specification.IsParseable || property.Specification.IsCollection) {
                return RestUtils.ObjectToPredefinedType(valueNakedObject.Object, useDateOverDateTime);
            }

            if (valueOnly) {
                return RefValueRepresentation.Create(oidStrategy, new ValueRelType(property, new UriMtHelper(oidStrategy, req, valueNakedObject)), flags);
            }

            var title = RestUtils.SafeGetTitle(property, valueNakedObject);
            var helper = new UriMtHelper(oidStrategy, req, property.IsInline ? target : valueNakedObject);
            var optionals = new List<OptionalProperty> {new OptionalProperty(JsonPropertyNames.Title, title)};

            if (property.IsEager(target)) {
                optionals.Add(new OptionalProperty(JsonPropertyNames.Value, ObjectRepresentation.Create(oidStrategy, valueNakedObject, req, flags)));
            }

            return LinkRepresentation.Create(oidStrategy, new ValueRelType(property, helper), flags, optionals.ToArray());
        }
    }
}