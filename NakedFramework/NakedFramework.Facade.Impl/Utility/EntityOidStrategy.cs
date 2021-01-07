// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Util;
using NakedObjects.Facade.Translation;
using NakedObjects.Services;

namespace NakedObjects.Facade.Impl.Utility {
    public class EntityOidStrategy : IOidStrategy {
        private readonly INakedObjectsFramework framework;
        private readonly ILogger<EntityOidStrategy> logger;

        public EntityOidStrategy(INakedObjectsFramework framework, ILogger<EntityOidStrategy> logger) {
            this.framework = framework;
            this.logger = logger;
        }

        #region IOidStrategy Members

        public IFrameworkFacade FrameworkFacade { get; set; }

        public IOidTranslator OidTranslator => FrameworkFacade.OidTranslator;

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
            return GetAdapterByOid(oid?.Value as IOid).GetDomainObject();
        }

        private ITypeSpec GetServiceTypeSpecByServiceName(IOidTranslation id) {
            var type = ValidateServiceId(id);
            ITypeSpec spec;

            try {
                spec = framework.MetamodelManager.GetSpecification(type);
            }
            catch (Exception e) {
                throw new ServiceResourceNotFoundNOSException(type.ToString(), e);
            }

            if (spec == null) {
                throw new ServiceResourceNotFoundNOSException(type.ToString());
            }

            return spec;
        }

        public ITypeFacade GetServiceTypeByServiceName(IOidTranslation id) => new TypeFacade(GetServiceTypeSpecByServiceName(id), FrameworkFacade, framework);

        public ITypeFacade GetSpecificationByLinkDomainType(string linkDomainType) {
            var type = GetType(linkDomainType);
            var spec = framework.MetamodelManager.GetSpecification(type);
            return new TypeFacade(spec, FrameworkFacade, framework);
        }

        public string GetLinkDomainTypeBySpecification(ITypeFacade spec) => GetCode(spec);

        public IOidFacade RestoreOid(OidTranslationSemiColonSeparatedList id) {
            var oid = framework.LifecycleManager.RestoreOid(id.Tokenize());
            return new OidFacade(oid);
        }

        public IOidFacade RestoreSid(OidTranslationSemiColonSeparatedList id) {
            var oid = framework.LifecycleManager.RestoreOid(id.Tokenize());
            return new OidFacade(oid);
        }

        public IOidFacade RestoreOid(OidTranslationSlashSeparatedTypeAndIds id) {
            var type = ValidateObjectId(id);
            var keys = GetKeys(id.InstanceId, type);
            var adapter = GetObject(keys, type);

            if (adapter == null) {
                throw new ObjectResourceNotFoundNOSException($"{id}: null adapter");
            }

            return new OidFacade(adapter.Oid);
        }

        public IOidFacade RestoreSid(OidTranslationSlashSeparatedTypeAndIds id) {
        
            ITypeSpec spec = GetServiceTypeSpecByServiceName(id);
            
            if (spec is IServiceSpec) {

                var service = framework.ServicesManager.GetServicesWithVisibleActions(framework.LifecycleManager).SingleOrDefault(no => no.Spec.IsOfType(spec));

                if (service == null) {
                    throw new ServiceResourceNotFoundNOSException(spec.FullName);
                }

                return new OidFacade(service.Oid);
            }

            if (!spec.IsStatic) {
                // we were looking for a static class masquerading as a service 
                throw new ServiceResourceNotFoundNOSException(spec.FullName);
            }

            return null;
        }

        #endregion

        private INakedObjectAdapter GetAdapterByOidTranslation(IOidTranslation objectId) {
            if (objectId == null) {
                return null;
            }

            var oid = objectId.GetOid(this);
            return GetAdapterByOid(oid.Value as IOid);
        }

        private static INakedObjectAdapter RestoreCollection(ICollectionMemento memento) => memento.RecoverCollection();

        private static INakedObjectAdapter RestoreInline(INakedObjectsFramework framework, IAggregateOid aggregateOid) {
            var parentOid = aggregateOid.ParentOid;
            var parent = RestoreObject(framework, parentOid);
            var assoc = parent.GetObjectSpec().Properties.Single(p => p.Id == aggregateOid.FieldName);

            return assoc.GetNakedObject(parent);
        }

        private static INakedObjectAdapter RestoreViewModel(INakedObjectsFramework framework, IViewModelOid viewModelOid) => framework.NakedObjectManager.GetAdapterFor(viewModelOid) ?? framework.LifecycleManager.GetViewModel(viewModelOid);

        public static INakedObjectAdapter RestoreObject(INakedObjectsFramework framework, IOid oid) => oid.IsTransient ? framework.LifecycleManager.RecreateInstance(oid, oid.Spec) : framework.LifecycleManager.LoadObject(oid, oid.Spec);

        private INakedObjectAdapter GetAdapterByOid(IOid oid) =>
            oid switch {
                null => null,
                ICollectionMemento memento => RestoreCollection(memento),
                IAggregateOid aggregateOid => RestoreInline(framework, aggregateOid),
                IViewModelOid modelOid => RestoreViewModel(framework, modelOid),
                _ => RestoreObject(framework, oid)
            };

        private string GetCode(ITypeFacade spec) => GetCode(TypeUtils.GetType(spec.FullName));

        private static object CoerceType(Type type, string value) =>
            type switch {
                _ when type == typeof(DateTime) => new DateTime(long.Parse(value)),
                _ when type == typeof(Guid) => new Guid(value),
                _ => Convert.ChangeType(value, type)
            };

        private IDictionary<string, object> CreateKeyDictionary(string[] keys, Type type) {
            var keyProperties = framework.Persistor.GetKeys(type);
            var index = 0;
            return keyProperties.ToDictionary(kp => kp.Name, kp => CoerceType(kp.PropertyType, keys[index++]));
        }

        private INakedObjectAdapter GetObject(string[] keys, Type type) {
            var spec = framework.MetamodelManager.GetSpecification(type);
            return spec.IsViewModel ? GetViewModel(keys, (IObjectSpec) spec) : GetDomainObject(keys, type);
        }

        private INakedObjectAdapter GetDomainObject(string[] keys, Type type) {
            try {
                var keyDict = CreateKeyDictionary(keys, type);
                return framework.Persistor.FindByKeys(type, keyDict.Values.ToArray());
            }
            catch (Exception e) {
                logger.LogWarning(e, "Domain Object not found with exception");
                var keysvalue = keys == null ? "null" : keys.Aggregate("", (s, t) => $"{s} {t}");
                var typevalue = type == null ? "null" : type.ToString();
                logger.LogWarning($"Domain Object not found keys: {keysvalue} type: {typevalue}");
                return null;
            }
        }

        private INakedObjectAdapter GetViewModel(string[] keys, IObjectSpec spec) {
            try {
                var viewModel = framework.LifecycleManager.CreateViewModel(spec);
                spec.GetFacet<IViewModelFacet>().Populate(keys, viewModel, framework.NakedObjectManager, framework.DomainObjectInjector, framework.Session, framework.Persistor);
                return viewModel;
            }
            catch (Exception e) {
                logger.LogWarning(e, "View Model not found with exception");
                var aggregate = keys == null ? "null" : keys.Aggregate("", (s, t) => $"{s} {t}");
                var specFullName = spec == null ? "null" : spec.FullName;
                logger.LogWarning($"View Model not found keys: {aggregate} type: {specFullName}");
                return null;
            }
        }

        private Type GetType(string typeName) => GetTypeCodeMapper().TypeFromCode(typeName);

        private ITypeCodeMapper GetTypeCodeMapper() =>
            (ITypeCodeMapper) framework.ServicesManager.GetServices().Where(s => s.Object is ITypeCodeMapper).Select(s => s.Object).FirstOrDefault()
            ?? new DefaultTypeCodeMapper();

        private IKeyCodeMapper GetKeyCodeMapper() =>
            (IKeyCodeMapper) framework.ServicesManager.GetServices().Where(s => s.Object is IKeyCodeMapper).Select(s => s.Object).FirstOrDefault()
            ?? new DefaultKeyCodeMapper();

        private string[] GetKeys(string instanceId, Type type) => GetKeyCodeMapper().KeyFromCode(instanceId, type);

        private string GetCode(Type type) => GetTypeCodeMapper().CodeFromType(type);

        private Type ValidateServiceId(IOidTranslation objectId) => ValidateId(objectId, () => throw new ServiceResourceNotFoundNOSException($"{objectId}: Type not found"));

        private Type ValidateObjectId(IOidTranslation objectId) => ValidateId(objectId, () => throw new ObjectResourceNotFoundNOSException($"{objectId}: Type not found"));

        private Type ValidateId(IOidTranslation objectId, Action onError) {
            if (string.IsNullOrEmpty(objectId.DomainType.Trim())) {
                throw new BadRequestNOSException();
            }

            var type = GetType(objectId.DomainType);

            if (type == null) {
                onError();
            }

            return type;
        }
    }
}