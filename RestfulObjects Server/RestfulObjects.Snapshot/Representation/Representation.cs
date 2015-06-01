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
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using NakedObjects.Surface;
using NakedObjects.Surface.Utility;
using RestfulObjects.Snapshot.Constants;
using RestfulObjects.Snapshot.Utility;

namespace RestfulObjects.Snapshot.Representations {
    [DataContract]
    public class Representation : IRepresentation {
        private static readonly object ModuleBuilderLock = new Object();
        protected CacheType caching;
        protected string etag;
        protected List<string> warnings = new List<string>();

        public Representation(IOidStrategy oidStrategy, RestControlFlags flags) {
            OidStrategy = oidStrategy;
            Flags = flags;
        }

        protected IOidStrategy OidStrategy { get; set; }
        protected RestControlFlags Flags { get; private set; }

        private static ModuleBuilder ModuleBuilder { get; set; }

        protected RelType SelfRelType { get; set; }

        #region IRepresentation Members

        public virtual MediaTypeHeaderValue GetContentType() {
            return SelfRelType != null ? SelfRelType.GetMediaType(Flags) : null;
        }

        public EntityTagHeaderValue GetEtag() {
            return etag != null ? new EntityTagHeaderValue(string.Format("\"{0}\"", etag)) : null;
        }

        public CacheType GetCaching() {
            return caching;
        }

        public string[] GetWarnings() {
            var allWarnings = new List<string>(warnings);
            PropertyInfo[] properties = GetType().GetProperties();

            IEnumerable<IRepresentation> repProperties = properties.Where(p => typeof (IRepresentation).IsAssignableFrom(p.PropertyType)).Select(p => (IRepresentation) p.GetValue(this, null));
            IEnumerable<IRepresentation> repProperties1 = properties.Where(p => typeof (IRepresentation[]).IsAssignableFrom(p.PropertyType)).SelectMany(p => (IRepresentation[]) p.GetValue(this, null));

            allWarnings.AddRange(repProperties.SelectMany(p => p.GetWarnings()));
            allWarnings.AddRange(repProperties1.SelectMany(p => p.GetWarnings()));

            return allWarnings.ToArray();
        }

        public virtual HttpResponseMessage GetAsMessage(MediaTypeFormatter formatter, Tuple<int, int, int> cacheSettings) {
            MediaTypeHeaderValue ct = GetContentType();

            if (ct != null) {
                formatter.SupportedMediaTypes.Add(ct);
            }

            var content = new ObjectContent<Representation>(this, formatter);
            var msg = new HttpResponseMessage {Content = content};
            msg.Content.Headers.ContentType = ct;

            SetCaching(msg, cacheSettings);

            return msg;
        }

        public Uri GetLocation() {
            return SelfRelType != null ? SelfRelType.GetUri() : null;
        }

        #endregion

        protected void SetCaching(HttpResponseMessage m, Tuple<int, int, int> cacheSettings) {
            int cacheTime = 0;

            switch (GetCaching()) {
                case CacheType.Transactional:
                    cacheTime = cacheSettings.Item1;
                    break;
                case CacheType.UserInfo:
                    cacheTime = cacheSettings.Item2;
                    break;
                case CacheType.NonExpiring:
                    cacheTime = cacheSettings.Item3;
                    break;
            }

            if (cacheTime == 0) {
                m.Headers.CacheControl = new CacheControlHeaderValue {NoCache = true};
                m.Headers.Pragma.Add(new NameValueHeaderValue("no-cache"));
            }
            else {
                m.Headers.CacheControl = new CacheControlHeaderValue {MaxAge = new TimeSpan(0, 0, 0, cacheTime)};
            }

            DateTime now = DateTime.UtcNow;

            m.Headers.Date = new DateTimeOffset(now);
            m.Content.Headers.Expires = new DateTimeOffset(now).Add(new TimeSpan(0, 0, 0, cacheTime));
        }

        protected void SetEtag(IObjectFacade target) {
            if (!target.Specification.IsService && !target.Specification.IsImmutable(target)) {
                string digest = target.Version.Digest;
                if (digest != null) {
                    etag = digest;
                }
            }
        }

        private static void EnsureModuleBuilderExists() {
            if (ModuleBuilder == null) {
                const string assemblyName = "NakedObjectsRestProxies";
                AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.Run);
                ModuleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName);
            }
        }

        private static void CreateProperty(TypeBuilder typeBuilder, Type propertyType, string name) {
            FieldBuilder fieldBuilder = typeBuilder.DefineField("_" + name, propertyType, FieldAttributes.Private);

            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(name, PropertyAttributes.None, propertyType, Type.EmptyTypes);

            MethodBuilder getMethodBuilder = typeBuilder.DefineMethod("get_" + name, MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Public, propertyType, Type.EmptyTypes);
            ILGenerator iLGenerator = getMethodBuilder.GetILGenerator();
            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
            iLGenerator.Emit(OpCodes.Ret);

            MethodBuilder setMethodBuilder = typeBuilder.DefineMethod("set_" + name, MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Public, null, new[] {propertyType});

            iLGenerator = setMethodBuilder.GetILGenerator();
            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Ldarg_1);
            iLGenerator.Emit(OpCodes.Stfld, fieldBuilder);
            iLGenerator.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getMethodBuilder);
            propertyBuilder.SetSetMethod(setMethodBuilder);

            Type dataMemberAttrType = typeof (DataMemberAttribute);
            PropertyInfo prop = dataMemberAttrType.GetProperty("Name");

            var customAttribute = new CustomAttributeBuilder(dataMemberAttrType.GetConstructor(new Type[] {}),
                new object[] {},
                new[] {prop},
                new object[] {name});

            propertyBuilder.SetCustomAttribute(customAttribute);
        }

        private static string ComputeMD5HashAsString(string s) {
            return (Math.Abs(BitConverter.ToInt64(ComputMD5HashFromString(s), 0)).ToString(CultureInfo.InvariantCulture));
        }

        private static byte[] ComputMD5HashFromString(string s) {
            byte[] idAsBytes = Encoding.UTF8.GetBytes(s);
            return new MD5CryptoServiceProvider().ComputeHash(idAsBytes);
        }

        protected static T CreateWithOptionals<T>(object[] ctorParms, IList<OptionalProperty> properties) {
            string toHash = (properties.Aggregate("", (s, t) => s + t.Name + "." + t.PropertyType.FullName + ".") + typeof (T).Name).Replace("[]", "Array");
            string hash = ComputeMD5HashAsString(toHash);

            string typeName = "NakedObjects.Snapshot.Rest.Representations." + hash;

            object newRep = CreateInstanceOfDynamicType<T>(ctorParms, properties, typeName);

            foreach (OptionalProperty w in properties) {
                newRep.GetType().GetProperty(w.Name).SetValue(newRep, w.Value, null);
            }

            return (T) newRep;
        }

        private static object CreateInstanceOfDynamicType<T>(object[] ctorParms, IList<OptionalProperty> properties, string typeName) {
            Type proxyType;
            lock (ModuleBuilderLock) {
                EnsureModuleBuilderExists();
                proxyType = ModuleBuilder.GetType(typeName);

                if (proxyType == null) {
                    TypeBuilder typeBuilder = ModuleBuilder.DefineType(typeName, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Sealed, typeof (T));

                    foreach (OptionalProperty w in properties) {
                        CreateProperty(typeBuilder, w.PropertyType, w.Name);
                    }

                    CreateConstructor<T>(typeBuilder);
                    proxyType = typeBuilder.CreateType();
                }
            }

            return Activator.CreateInstance(proxyType, ctorParms);
        }

        private static void CreateConstructor<T>(TypeBuilder typeBuilder) {
            ConstructorInfo parentCtor = (typeof (T)).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).SingleOrDefault();

            if (parentCtor != null) {
                ParameterInfo[] parentCtorParms = parentCtor.GetParameters();
                ConstructorBuilder ctor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, parentCtorParms.Select(p => p.ParameterType).ToArray());

                for (int idx = 0; idx < parentCtorParms.Length; idx++) {
                    ctor.DefineParameter(idx + 1, ParameterAttributes.None, parentCtorParms[idx].Name);
                }

                ILGenerator ilGenerator = ctor.GetILGenerator();

                ilGenerator.Emit(OpCodes.Ldarg_0);
                for (int i = 1; i <= parentCtorParms.Count(); i++) {
                    ilGenerator.Emit(OpCodes.Ldarg, i);
                }
                ilGenerator.Emit(OpCodes.Call, parentCtor);
                ilGenerator.Emit(OpCodes.Ret);
            }
        }

        protected static void AddStringProperties(INakedObjectSpecificationSurface spec, int? maxLength, string pattern, Dictionary<string, object> exts) {
            if (spec.IsParseable) {
                if (maxLength != null) {
                    exts.Add(JsonPropertyNames.MaxLength, maxLength);
                }

                if (!string.IsNullOrEmpty(pattern)) {
                    exts.Add(JsonPropertyNames.Pattern, pattern);
                }

                if (spec.IsDateTime) {
                    exts.Add(JsonPropertyNames.Format, PredefinedType.Date_time.ToRoString());
                }
            }
        }

        protected static object GetPropertyValue(IOidStrategy oidStrategy, HttpRequestMessage req, INakedObjectAssociationSurface property, IObjectFacade target, RestControlFlags flags, bool valueOnly = false) {
            IObjectFacade valueNakedObject = property.GetNakedObject(target);
            string title = RestUtils.SafeGetTitle(property, valueNakedObject);

            if (valueNakedObject == null) {
                return null;
            }
            if (property.Specification.IsParseable || property.Specification.IsCollection) {
                return RestUtils.ObjectToPredefinedType(valueNakedObject.Object);
            }

            if (valueOnly) {
                return RefValueRepresentation.Create(oidStrategy, new ValueRelType(property, new UriMtHelper(oidStrategy, req, valueNakedObject)), flags);
            }

            var helper = new UriMtHelper(oidStrategy, req, property.IsInline ? target : valueNakedObject);
            var optionals = new List<OptionalProperty> {new OptionalProperty(JsonPropertyNames.Title, title)};

            if (property.IsEager(target)) {
                optionals.Add(new OptionalProperty(JsonPropertyNames.Value, ObjectRepresentation.Create(oidStrategy ,valueNakedObject, req, flags)));
            }

            return LinkRepresentation.Create(oidStrategy ,new ValueRelType(property, helper), flags, optionals.ToArray());
        }
    }
}