// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets.Objects.ViewModel;
using NakedObjects.Architecture.Services;
using NakedObjects.Architecture.Spec;
using NakedObjects.Services;
using NakedObjects.Surface.Nof4.Wrapper;
using NakedObjects.Surface.Utility;
using NakedObjects.Util;

namespace NakedObjects.Surface.Nof4.Utility {
    // to do generalise this 
    public class ExternalOid : IOidStrategy {
        private readonly INakedObjectsFramework framework;

        public ExternalOid(INakedObjectsFramework framework) {
            this.framework = framework;
        }

        private string GetCode(INakedObjectSpecificationSurface spec) {
            return GetCode(TypeUtils.GetType(spec.FullName()));
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
            INakedObjectSpecification spec;
            try {
                spec = framework.Reflector.LoadSpecification(type);
            }
            catch (Exception e) {
                throw new ServiceResourceNotFoundNOSException(type.ToString(), e);
            }
            if (spec == null) {
                throw new ServiceResourceNotFoundNOSException(type.ToString());
            }
            INakedObject service = framework.ObjectPersistor.GetServicesWithVisibleActions(ServiceTypes.Menu | ServiceTypes.Contributor).SingleOrDefault(no => no.Specification.IsOfType(spec));
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
            INakedObjectSpecification spec = framework.Reflector.LoadSpecification(type);
            return new NakedObjectSpecificationWrapper(spec, Surface, framework);
        }

        public string GetLinkDomainTypeBySpecification(INakedObjectSpecificationSurface spec) {
            return GetCode(spec);
        }

        #endregion

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
            INakedObject wrappedNakedObject = ((NakedObjectWrapper) nakedObjectForKey).WrappedNakedObject;

            if (wrappedNakedObject.Specification.IsViewModel) {
                keys = wrappedNakedObject.Specification.GetFacet<IViewModelFacet>().Derive(wrappedNakedObject);
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
            PropertyInfo[] keyProperties = framework.ObjectPersistor.GetKeys(type);
            int index = 0;
            return keyProperties.ToDictionary(kp => kp.Name, kp => CoerceType(kp.PropertyType, keys[index++]));
        }

        protected object GetObject(string[] keys, Type type) {
            INakedObjectSpecification spec = framework.Reflector.LoadSpecification(type);
            return spec.IsViewModel ? GetViewModel(keys, spec) : GetDomainObject(keys, type);
        }


        protected object GetDomainObject(string[] keys, Type type) {
            IDictionary<string, object> keyDict = CreateKeyDictionary(keys, type);
            return framework.ObjectPersistor.FindByKeys(type, keyDict.Values.ToArray()).GetDomainObject();
        }

        protected object GetViewModel(string[] keys, INakedObjectSpecification spec) {
            INakedObject viewModel = framework.ObjectPersistor.CreateViewModel(spec);
            spec.GetFacet<IViewModelFacet>().Populate(keys, viewModel);
            return viewModel.Object;
        }

        private Type GetType(string typeName) {
            return GetTypeCodeMapper().TypeFromCode(typeName);
        }

        private ITypeCodeMapper GetTypeCodeMapper() {
            return (ITypeCodeMapper) framework.ObjectPersistor.GetServices().Where(s => s.Object is ITypeCodeMapper).Select(s => s.Object).FirstOrDefault()
                   ?? new DefaultTypeCodeMapper();
        }

        private IKeyCodeMapper GetKeyCodeMapper() {
            return (IKeyCodeMapper) framework.ObjectPersistor.GetServices().Where(s => s.Object is IKeyCodeMapper).Select(s => s.Object).FirstOrDefault()
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