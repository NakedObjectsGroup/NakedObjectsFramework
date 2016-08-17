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
using NakedObjects.Facade.Translation;
using NakedObjects.Services;
using NakedObjects.Util;

namespace NakedObjects.Facade.Impl.Utility {
    public class EntityOidStrategy : IOidStrategy {
        private static readonly ILog Log = LogManager.GetLogger(typeof(EntityOidStrategy));
        private readonly INakedObjectsFramework framework;

        public EntityOidStrategy(INakedObjectsFramework framework) {
            this.framework = framework;
        }

        #region IOidStrategy Members

        public IFrameworkFacade FrameworkFacade { get; set; }

        public object GetDomainObjectByOid(IOidTranslation objectId) {
            var adapter = GetAdapterByOidTranslation(objectId);
            return adapter == null ? null : GetAdapterByOidTranslation(objectId).GetDomainObject();
        }

        public IObjectFacade GetObjectFacadeByOid(IOidTranslation objectId) {
            var adapter = GetAdapterByOidTranslation(objectId);
            return adapter == null ? null : ObjectFacade.Wrap(adapter, FrameworkFacade, framework);
        }

        public object GetServiceByServiceName(IOidTranslation serviceName) {
            var oid = serviceName.GetSid(this);
            return GetAdapterByOid(oid.Value as IOid).GetDomainObject();
        }

        public ITypeFacade GetSpecificationByLinkDomainType(string linkDomainType) {
            Type type = GetType(linkDomainType);
            ITypeSpec spec = framework.MetamodelManager.GetSpecification(type);
            return new TypeFacade(spec, FrameworkFacade, framework);
        }

        public string GetLinkDomainTypeBySpecification(ITypeFacade spec) {
            return GetCode(spec);
        }

        public IOidFacade RestoreOid(OidTranslationSemiColonSeparatedList id) {
            var oid = framework.LifecycleManager.RestoreOid(id.Tokenize());
            return new OidFacade(oid);
        }

        public IOidFacade RestoreSid(OidTranslationSemiColonSeparatedList id) {
            var oid = framework.LifecycleManager.RestoreOid(id.Tokenize());
            return new OidFacade(oid);
        }

        public IOidFacade RestoreOid(OidTranslationSlashSeparatedTypeAndIds id) {
            Type type = ValidateObjectId(id);
            string[] keys = GetKeys(id.InstanceId, type);
            var adapter = GetObject(keys, type);

            if (adapter == null) {
                throw new ObjectResourceNotFoundNOSException(id + ": null adapter");
            }

            return new OidFacade(adapter.Oid);
        }

        public IOidFacade RestoreSid(OidTranslationSlashSeparatedTypeAndIds id) {
            Type type = ValidateServiceId(id);
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

            return new OidFacade(service.Oid);
        }

        #endregion

        private INakedObjectAdapter GetAdapterByOidTranslation(IOidTranslation objectId) {
            if (objectId == null) {
                return null;
            }

            var oid = objectId.GetOid(this);
            return GetAdapterByOid(oid.Value as IOid);
        }

        private static INakedObjectAdapter RestoreCollection(ICollectionMemento memento) {
            return memento.RecoverCollection();
        }

        private static INakedObjectAdapter RestoreInline(INakedObjectsFramework framework, IAggregateOid aggregateOid) {
            IOid parentOid = aggregateOid.ParentOid;
            INakedObjectAdapter parent = RestoreObject(framework, parentOid);
            IAssociationSpec assoc = parent.GetObjectSpec().Properties.Where((p => p.Id == aggregateOid.FieldName)).Single();

            return assoc.GetNakedObject(parent);
        }

        private static INakedObjectAdapter RestoreViewModel(INakedObjectsFramework framework, IViewModelOid viewModelOid) {
            return framework.NakedObjectManager.GetAdapterFor(viewModelOid) ?? framework.LifecycleManager.GetViewModel(viewModelOid);
        }

        public static INakedObjectAdapter RestoreObject(INakedObjectsFramework framework, IOid oid) {
            return oid.IsTransient ? framework.LifecycleManager.RecreateInstance(oid, oid.Spec) : framework.LifecycleManager.LoadObject(oid, oid.Spec);
        }

        private INakedObjectAdapter GetAdapterByOid(IOid oid) {
            if (oid == null) {
                return null;
            }

            if (oid is ICollectionMemento) {
                return RestoreCollection(oid as ICollectionMemento);
            }

            if (oid is IAggregateOid) {
                return RestoreInline(framework, oid as IAggregateOid);
            }

            if (oid is IViewModelOid) {
                return RestoreViewModel(framework, oid as IViewModelOid);
            }

            return RestoreObject(framework, oid);
        }

        private string GetCode(ITypeFacade spec) {
            return GetCode(TypeUtils.GetType(spec.FullName));
        }

        private static object CoerceType(Type type, string value) {
            if (type == typeof(DateTime)) {
                long ticks = long.Parse(value);
                return new DateTime(ticks);
            }
            if (type == typeof(Guid))
            {
                return new Guid(value);
            }
            return Convert.ChangeType(value, type);
        }

        private IDictionary<string, object> CreateKeyDictionary(string[] keys, Type type) {
            PropertyInfo[] keyProperties = framework.Persistor.GetKeys(type);
            int index = 0;
            return keyProperties.ToDictionary(kp => kp.Name, kp => CoerceType(kp.PropertyType, keys[index++]));
        }

        private INakedObjectAdapter GetObject(string[] keys, Type type) {
            ITypeSpec spec = framework.MetamodelManager.GetSpecification(type);
            return spec.IsViewModel ? GetViewModel(keys, (IObjectSpec) spec) : GetDomainObject(keys, type);
        }

        private INakedObjectAdapter GetDomainObject(string[] keys, Type type) {
            try {
                IDictionary<string, object> keyDict = CreateKeyDictionary(keys, type);
                return framework.Persistor.FindByKeys(type, keyDict.Values.ToArray());
            }
            catch (Exception e) {
                Log.Warn("Domain Object not found with exception", e);
                Log.WarnFormat("Domain Object not found keys: {0} type: {1}", keys == null ? "null" : keys.Aggregate("", (s, t) => s + " " + t), type == null ? "null" : type.ToString());
                return null;
            }
        }

        private INakedObjectAdapter GetViewModel(string[] keys, IObjectSpec spec) {
            try {
                INakedObjectAdapter viewModel = framework.LifecycleManager.CreateViewModel(spec);
                spec.GetFacet<IViewModelFacet>().Populate(keys, viewModel, framework.NakedObjectManager, framework.DomainObjectInjector);
                return viewModel;
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

        private Type ValidateServiceId(IOidTranslation objectId) {
            return ValidateId(objectId, () => { throw new ServiceResourceNotFoundNOSException(objectId + ": Type not found"); });
        }

        private Type ValidateObjectId(IOidTranslation objectId) {
            return ValidateId(objectId, () => { throw new ObjectResourceNotFoundNOSException(objectId + ": Type not found"); });
        }

        private Type ValidateId(IOidTranslation objectId, Action onError) {
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