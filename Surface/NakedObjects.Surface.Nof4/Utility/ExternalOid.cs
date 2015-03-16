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
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Util;
using NakedObjects.Services;
using NakedObjects.Surface.Nof4.Wrapper;
using NakedObjects.Surface.Utility;
using NakedObjects.Util;

namespace NakedObjects.Surface.Nof4.Utility {
    // to do generalise this 
    public class ExternalOid : IOidStrategy {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ExternalOid));

        private readonly INakedObjectsFramework framework;

        public ExternalOid(INakedObjectsFramework framework) {
            this.framework = framework;
        }

        #region IOidStrategy Members

        public INakedObjectsSurface Surface { protected get; set; }

        public object GetDomainObjectByOid(LinkObjectId objectId) {
            Type type = ValidateObjectId(objectId);
            string[] keys = GetKeys(objectId.InstanceId, type);
            object domainObject = GetObject(keys, type);

            if (domainObject == null) {
                throw new ObjectResourceNotFoundNOSException(objectId.ToString());
            }

            return domainObject;
        }

        public object GetServiceByServiceName(LinkObjectId oid) {
            Type type = ValidateServiceId(oid);
            IServiceSpec spec;
            try {
                spec = (IServiceSpec) framework.MetamodelManager.GetSpecification(type);
            }
            catch (Exception e) {
                throw new ServiceResourceNotFoundNOSException(type.ToString(), e);
            }
            if (spec == null) {
                throw new ServiceResourceNotFoundNOSException(type.ToString());
            }
            INakedObjectAdapter service = framework.ServicesManager.GetServicesWithVisibleActions(framework.LifecycleManager).SingleOrDefault(no => no.Spec.IsOfType(spec));
            if (service == null) {
                throw new ServiceResourceNotFoundNOSException(type.ToString());
            }
            return service.Object;
        }


        public LinkObjectId GetOid(INakedObjectSurface nakedObject) {
            Tuple<string, string> codeAndKey = GetCodeAndKeyAsTuple(nakedObject);
            return new LinkObjectId(codeAndKey.Item1, codeAndKey.Item2);
        }

        public INakedObjectSpecificationSurface GetSpecificationByLinkDomainType(string linkDomainType) {
            Type type = GetType(linkDomainType);
            ITypeSpec spec = framework.MetamodelManager.GetSpecification(type);
            return new NakedObjectSpecificationWrapper(spec, Surface, framework);
        }

        public string GetLinkDomainTypeBySpecification(INakedObjectSpecificationSurface spec) {
            return GetCode(spec);
        }

        #endregion

        private string GetCode(INakedObjectSpecificationSurface spec) {
            return GetCode(TypeUtils.GetType(spec.FullName()));
        }

        protected Tuple<string, string> GetCodeAndKeyAsTuple(INakedObjectSurface nakedObject) {
            string code = GetCode(nakedObject.Specification);
            return new Tuple<string, string>(code, GetKeyValues(nakedObject));
        }

        private string KeyRepresentation(object obj) {
            if (obj is DateTime) {
                obj = ((DateTime) obj).Ticks;
            }
            return (string) Convert.ChangeType(obj, typeof (string)); // better ? 
        }

        protected string GetKeyValues(INakedObjectSurface nakedObjectForKey) {
            string[] keys;
            INakedObjectAdapter wrappedNakedObject = ((NakedObjectWrapper) nakedObjectForKey).WrappedNakedObject;

            if (wrappedNakedObject.Spec.IsViewModel) {
                keys = wrappedNakedObject.Spec.GetFacet<IViewModelFacet>().Derive(wrappedNakedObject, framework.NakedObjectManager, framework.DomainObjectInjector);
            }
            else {
                PropertyInfo[] keyPropertyInfo = nakedObjectForKey.GetKeys();
                keys = keyPropertyInfo.Select(pi => KeyRepresentation(pi.GetValue(nakedObjectForKey.Object, null))).ToArray();
            }

            return GetKeyCodeMapper().CodeFromKey(keys, nakedObjectForKey.Object.GetType());
        }

        private static object CoerceType(Type type, string value) {
            if (type == typeof (DateTime)) {
                long ticks = long.Parse(value);
                return new DateTime(ticks);
            }

            return Convert.ChangeType(value, type);
        }

        private IDictionary<string, object> CreateKeyDictionary(string[] keys, Type type) {
            PropertyInfo[] keyProperties = framework.Persistor.GetKeys(type);
            int index = 0;
            return keyProperties.ToDictionary(kp => kp.Name, kp => CoerceType(kp.PropertyType, keys[index++]));
        }

        protected object GetObject(string[] keys, Type type) {
            ITypeSpec spec = framework.MetamodelManager.GetSpecification(type);
            return spec.IsViewModel ? GetViewModel(keys, (IObjectSpec) spec) : GetDomainObject(keys, type);
        }


        protected object GetDomainObject(string[] keys, Type type) {
            try {
                IDictionary<string, object> keyDict = CreateKeyDictionary(keys, type);
                return framework.Persistor.FindByKeys(type, keyDict.Values.ToArray()).GetDomainObject();
            }
            catch (Exception e) {
                Log.Warn("Domain Object not found with exception", e);
                Log.WarnFormat("Domain Object not found keys: {0} type: {1}", keys == null ? "null" : keys.Aggregate("", (s, t) => s + " " + t), type == null ? "null" : type.ToString());
                return null;
            }
        }

        protected object GetViewModel(string[] keys, IObjectSpec spec) {
            try {
                INakedObjectAdapter viewModel = framework.LifecycleManager.CreateViewModel(spec);
                spec.GetFacet<IViewModelFacet>().Populate(keys, viewModel, framework.NakedObjectManager, framework.DomainObjectInjector);
                return viewModel.Object;
            }
            catch (Exception e) {
                Log.Warn("View Model not found with exception", e);
                Log.WarnFormat("View Model not found keys: {0} type: {1}", keys == null ? "null" : keys.Aggregate("", (s, t) => s + " " + t), spec == null ? "null" : spec.FullName);
                return null;
            }
        }

        private Type GetType(string typeName) {
            return GetTypeCodeMapper().TypeFromCode(typeName);
        }

        private ITypeCodeMapper GetTypeCodeMapper() {
            return (ITypeCodeMapper) framework.ServicesManager.GetServices().Where(s => s.Object is ITypeCodeMapper).Select(s => s.Object).FirstOrDefault()
                   ?? new DefaultTypeCodeMapper();
        }

        private IKeyCodeMapper GetKeyCodeMapper() {
            return (IKeyCodeMapper) framework.ServicesManager.GetServices().Where(s => s.Object is IKeyCodeMapper).Select(s => s.Object).FirstOrDefault()
                   ?? new DefaultKeyCodeMapper();
        }

        private string[] GetKeys(string instanceId, Type type) {
            return GetKeyCodeMapper().KeyFromCode(instanceId, type);
        }

        private string GetCode(Type type) {
            return GetTypeCodeMapper().CodeFromType(type);
        }

        protected Type ValidateServiceId(LinkObjectId objectId) {
            return ValidateId(objectId, () => { throw new ServiceResourceNotFoundNOSException(objectId.ToString()); });
        }

        protected Type ValidateObjectId(LinkObjectId objectId) {
            return ValidateId(objectId, () => { throw new ObjectResourceNotFoundNOSException(objectId.ToString()); });
        }

        private Type ValidateId(LinkObjectId objectId, Action onError) {
            if (string.IsNullOrEmpty(objectId.DomainType.Trim())) {
                throw new BadRequestNOSException();
            }

            Type type = GetType(objectId.DomainType);

            if (type == null) {
                onError();
            }

            return type;
        }
    }
}