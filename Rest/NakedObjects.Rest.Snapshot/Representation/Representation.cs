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

using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using Common.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using NakedObjects.Facade;
using NakedObjects.Rest.Snapshot.Constants;
using NakedObjects.Rest.Snapshot.Utility;

namespace NakedObjects.Rest.Snapshot.Representations {
    [DataContract]
    public class Representation : IRepresentation {
        private static readonly object ModuleBuilderLock = new Object();
        protected CacheType Caching;
        protected string Etag;
        protected List<string> Warnings = new List<string>();
        private static readonly ILog Log = LogManager.GetLogger<Representation>();

        public Representation(IOidStrategy oidStrategy, RestControlFlags flags) {
            OidStrategy = oidStrategy;
            Flags = flags;
        }

        protected IOidStrategy OidStrategy { get; set; }
        protected RestControlFlags Flags { get; }

        private static ModuleBuilder ModuleBuilder { get; set; }

        protected RelType SelfRelType { get; set; }

        #region IRepresentation Members

        public virtual Microsoft.Net.Http.Headers.MediaTypeHeaderValue GetContentType() {
            return SelfRelType != null ? SelfRelType.GetMediaType(Flags) : null;
        }

        public Microsoft.Net.Http.Headers.EntityTagHeaderValue GetEtag() {
            return Etag != null ? new Microsoft.Net.Http.Headers.EntityTagHeaderValue($"\"{Etag}\"") : null;
        }

        public CacheType GetCaching() {
            return Caching;
        }

        public string[] GetWarnings() {
            var allWarnings = new List<string>(Warnings);

            try {
                PropertyInfo[] properties = GetType().GetProperties();

                IEnumerable<IRepresentation> repProperties = properties.Where(p => typeof(IRepresentation).IsAssignableFrom(p.PropertyType)).Select(p => (IRepresentation) p.GetValue(this, null));
                IEnumerable<IRepresentation> repProperties1 = properties.Where(p => typeof(IRepresentation[]).IsAssignableFrom(p.PropertyType)).SelectMany(p => (IRepresentation[]) p.GetValue(this, null));

                allWarnings.AddRange(repProperties.Where(p => p != null).SelectMany(p => p.GetWarnings()));
                allWarnings.AddRange(repProperties1.Where(p => p != null).SelectMany(p => p.GetWarnings()));
            }
            catch (Exception e) {
                Log.ErrorFormat("Error on getting warnings - is a Representation null?", e);
                throw;
            }

            return allWarnings.ToArray();
        }

        public virtual HttpResponseMessage GetAsMessage(MediaTypeFormatter formatter, Tuple<int, int, int> cacheSettings) {
            Microsoft.Net.Http.Headers.MediaTypeHeaderValue ct = GetContentType();

            if (ct != null) {
                //formatter.SupportedMediaTypes.Add(ct);
            }

            var content = new ObjectContent<Representation>(this, formatter);
            var msg = new HttpResponseMessage {Content = content};
            //msg.Content.Headers.ContentType = ct;

            SetCaching(msg, cacheSettings);

            return msg;
        }

        public Uri GetLocation() {
            return SelfRelType != null ? SelfRelType.GetUri() : null;
        }

        #endregion

        protected void SetCaching(HttpResponseMessage m, Tuple<int, int, int> cacheSettings) {
            //int cacheTime = 0;

            //switch (GetCaching()) {
            //    case CacheType.Transactional:
            //        cacheTime = cacheSettings.Item1;
            //        break;
            //    case CacheType.UserInfo:
            //        cacheTime = cacheSettings.Item2;
            //        break;
            //    case CacheType.NonExpiring:
            //        cacheTime = cacheSettings.Item3;
            //        break;
            //}

            //if (cacheTime == 0) {
            //    m.Headers.CacheControl = new CacheControlHeaderValue {NoCache = true};
            //    m.Headers.Pragma.Add(new NameValueHeaderValue("no-cache"));
            //}
            //else {
            //    m.Headers.CacheControl = new CacheControlHeaderValue {MaxAge = new TimeSpan(0, 0, 0, cacheTime)};
            //}

            //DateTime now = DateTime.UtcNow;

            //m.Headers.Date = new DateTimeOffset(now);
            //m.Content.Headers.Expires = new DateTimeOffset(now).Add(new TimeSpan(0, 0, 0, cacheTime));
        }

        protected string GetPropertyValueForEtag(IAssociationFacade property, IObjectFacade target) {
            IObjectFacade valueNakedObject = property.GetValue(target);
           
            if (valueNakedObject == null) {
                return "";
            }

            if (property.Specification.IsParseable) {
                return valueNakedObject.Object.ToString();
            }

            return OidStrategy.FrameworkFacade.OidTranslator.GetOidTranslation(target).Encode();
        }


        protected string GetTransientEtag(IObjectFacade target) {
            var allProperties = target.Specification.Properties.Where(p => !p.IsCollection && !p.IsInline);
            var unusableProperties = allProperties.Where(p => p.IsUsable(target).IsVetoed && !p.IsVisible(target));

            var propertyValues = unusableProperties.ToDictionary(p => p.Id, p => GetPropertyValueForEtag(p, target));

            return propertyValues.Aggregate("", (s, kvp) => s + kvp.Key + ":" + kvp.Value);
        }

        protected void SetEtag(string digest) {
            if (digest != null) {
                Etag = digest;
            }
        }

        private bool IsMutable(IObjectFacade target) {
            return Flags.AllowMutatingActionsOnImmutableObject || !target.Specification.IsImmutable(target);
        }


        protected void SetEtag(IObjectFacade target) {
            if (!target.Specification.IsService && IsMutable(target)) {
                string digest = target.Version.Digest;
                SetEtag(digest);
            }
        }

        private static void EnsureModuleBuilderExists() {
            if (ModuleBuilder == null) {
                const string assemblyName = "NakedObjectsRestProxies";
                AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.Run);
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
                for (int i = 1; i <= parentCtorParms.Length; i++) {
                    ilGenerator.Emit(OpCodes.Ldarg, i);
                }
                ilGenerator.Emit(OpCodes.Call, parentCtor);
                ilGenerator.Emit(OpCodes.Ret);
            }
        }

        protected static void AddStringProperties(ITypeFacade spec, int? maxLength, string pattern, Dictionary<string, object> exts) {
            if (spec.IsParseable) {
                if (maxLength != null) {
                    exts.Add(JsonPropertyNames.MaxLength, maxLength);
                }

                if (!string.IsNullOrEmpty(pattern)) {
                    exts.Add(JsonPropertyNames.Pattern, pattern);
                }

                if (spec.IsDateTime) {
                    exts.Add(JsonPropertyNames.Format, PredefinedFormatType.Date_time.ToRoString());
                }
            }
        }

        public static object GetPropertyValue(IOidStrategy oidStrategy, HttpRequest req, IAssociationFacade property, IObjectFacade target, RestControlFlags flags, bool valueOnly, bool useDateOverDateTime) {
            IObjectFacade valueNakedObject = property.GetValue(target);
            
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

            string title = RestUtils.SafeGetTitle(property, valueNakedObject);
            var helper = new UriMtHelper(oidStrategy, req, property.IsInline ? target : valueNakedObject);
            var optionals = new List<OptionalProperty> {new OptionalProperty(JsonPropertyNames.Title, title)};

            if (property.IsEager(target)) {
                optionals.Add(new OptionalProperty(JsonPropertyNames.Value, ObjectRepresentation.Create(oidStrategy, valueNakedObject, req, flags)));
            }

            return LinkRepresentation.Create(oidStrategy, new ValueRelType(property, helper), flags, optionals.ToArray());
        }
    }
}