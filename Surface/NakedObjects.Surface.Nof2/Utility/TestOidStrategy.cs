// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Linq;
using System.Reflection;
using NakedObjects.Surface.Nof2.Implementation;
using NakedObjects.Surface.Nof2.Wrapper;
using NakedObjects.Surface.Utility;
using org.nakedobjects.@object;
using org.nakedobjects.@object.persistence;
using sdm.systems.application.value;
using Action = System.Action;

namespace NakedObjects.Surface.Nof2.Utility {
    // Strategy based on each object having a key called 'Id' 
    public class TestOidStrategy : IOidStrategy {
        private const string sep = "-";

        #region IOidStrategy Members

        public object GetDomainObjectByOid(IOidTranslation objectId) {
            Type type = ValidateObjectId(objectId);
            string[] keys = GetKeys(objectId.InstanceId, type);

            NakedObjectSpecification spec = org.nakedobjects.@object.NakedObjects.getSpecificationLoader().loadSpecification(type.FullName);

            NakedObject pattern = org.nakedobjects.@object.NakedObjects.getObjectLoader().createTransientInstance(spec);

            PropertyInfo p = pattern.getObject().GetType().GetProperty("Id");
            ((WholeNumber) p.GetValue(pattern.getObject(), null)).setValue(int.Parse(keys.First()));

            var criteria = new TitleCriteria(spec, pattern.titleString(), false);
            TypedNakedCollection results = org.nakedobjects.@object.NakedObjects.getObjectPersistor().findInstances(criteria);

            if (results.size() == 0) {
                throw new ObjectResourceNotFoundNOSException(objectId.ToString());
            }

            return results.elementAt(0).getObject();
        }

        public object GetServiceByServiceName(IOidTranslation serviceName) {
            Type type = ValidateServiceId(serviceName);

            try {
                return SurfaceUtils.GetServicesInternal().Single(s => s.getSpecification().getFullName() == type.FullName).getObject();
            }
            catch (Exception e) {
                throw new ServiceResourceNotFoundNOSException(serviceName.DomainType, e);
            }
        }

        public string GetObjectId(IObjectFacade nakedobject) {
            throw new NotImplementedException();
        }

        public ITypeFacade GetSpecificationByLinkDomainType(string linkDomainType) {
            Type type = GetType(linkDomainType);
            NakedObjectSpecification spec = org.nakedobjects.@object.NakedObjects.getSpecificationLoader().loadSpecification(type.FullName);
            return new TypeFacade(spec, null, Surface);
        }

        public string GetLinkDomainTypeBySpecification(ITypeFacade spec) {
            return GetCode(spec);
        }

        public IOidFacade RestoreOid(OidTranslationSemiColonSeparatedList id) {
            throw new NotImplementedException();
        }

        public IOidFacade RestoreSid(OidTranslationSemiColonSeparatedList id) {
            throw new NotImplementedException();
        }

        public IOidFacade RestoreOid(OidTranslationSlashSeparatedTypeAndIds id) {
            throw new NotImplementedException();
        }

        public IOidFacade RestoreSid(OidTranslationSlashSeparatedTypeAndIds id) {
            throw new NotImplementedException();
        }

        public IOidFacade RestoreEncodedOid(string encoded) {
            throw new NotImplementedException();
        }

        public IOidFacade RestoreTypeIdOid(string typeName, string instanceId) {
            throw new NotImplementedException();
        }

        public IFrameworkFacade Surface { set;  get; }

        #endregion

        protected Tuple<string, string> GetCodeAndKeyAsTuple(IObjectFacade nakedObject) {
            var code = GetCode(nakedObject.Specification);
            return new Tuple<string, string>(code, GetKeyValues(nakedObject));
        }

        protected static string GetKeyValues(IObjectFacade nakedObjectForKey) {
            PropertyInfo keyProperty = nakedObjectForKey.Object.GetType().GetProperty("Id");

            if (keyProperty != null) {
                object key = keyProperty.GetValue(nakedObjectForKey.Object, null);
                var keys = new[] {key.ToString()};
                return GetKeyCodeMapper().CodeFromKey(keys, nakedObjectForKey.Object.GetType());
            }
            return "";
        }

        private static ITypeCodeMapper GetTypeCodeMapper() {
            // if required introduce some form of indirection here 
            return  new DefaultTypeCodeMapper();
        }

        private static IKeyCodeMapper GetKeyCodeMapper() {
            // if required introduce some form of indirection here 
            return new DefaultKeyCodeMapper();
        }

        private static string[] GetKeys(string instanceId, Type type) {
            return GetKeyCodeMapper().KeyFromCode(instanceId, type);
        }

        private static string GetCode(Type type) {
            return GetTypeCodeMapper().CodeFromType(type);
        }

        private static string GetCode(ITypeFacade spec) {
            return GetCode(SurfaceUtils.GetType(spec.FullName));
        }

        private static Type GetType(string typeName) {
            return GetTypeCodeMapper().TypeFromCode(typeName);
        }

        private static string[] ExtractTypeAndKeys(string encodedTypeAndKeys) {
            return encodedTypeAndKeys.Split('-');
        }

        protected static Type ValidateServiceId(IOidTranslation objectId) {
            return ValidateId(objectId, () => { throw new ServiceResourceNotFoundNOSException(objectId.ToString()); });
        }

        protected static Type ValidateObjectId(IOidTranslation objectId) {
            return ValidateId(objectId, () => { throw new ObjectResourceNotFoundNOSException(objectId.ToString()); });
        }


        private static Type ValidateId(IOidTranslation objectId, Action onError) {
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