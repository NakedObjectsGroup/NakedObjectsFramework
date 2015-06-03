// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Linq;
using NakedObjects.Facade;
using NakedObjects.Facade.Translation;
using org.nakedobjects.@object;
using sdm.systems.application.objectstore;

namespace NakedObjects.Surface.Nof2.Utility {
    public class OStoreOidStrategy : IOidStrategy {
        #region IOidStrategy Members

        public object GetDomainObjectByOid(string objectId) {
            // don't know if I can just cast or if I need to get seperately from spring 
            //var objectResolver = SurfaceUtils.GetObjectResolver();

            var objectResolver = (IObjectStore) org.nakedobjects.@object.NakedObjects.getObjectPersistor();

            return objectResolver.resolve(objectId);
        }

        public object GetServiceByServiceName(string serviceName) {
            // don't know if services can be resolved from oid if so this should work 

            return GetDomainObjectByOid(serviceName);

            //NakedObject service = GetServiceByServiceNameInternal(serviceName);

            //if (service == null) {
            //    throw new ServiceResourceNotFoundNOSException(serviceName);
            //}
            //return service.getObject();
        }


        public object GetDomainObjectByOid(IOidTranslation objectId) {
            throw new NotImplementedException();
        }

        public object GetDomainObjectByTypeAndKey(string objectType, string objectKey) {
            throw new NotImplementedException();
        }

        public object GetServiceByServiceName(IOidTranslation serviceName) {
            throw new NotImplementedException();
        }

        public string GetOidAsString(IObjectFacade nakedObject) {
            // don't know if services need to be handled differently 

            //if (nakedObject.Specification.IsService) {
            //    return nakedObject.Specification.FullName; 
            //}
            return nakedObject.Oid.ToString();
        }

        public string[] GetTypeAndKeyAsStrings(IObjectFacade nakedObject) {
            throw new NotImplementedException();
        }

       
        public string GetObjectId(IObjectFacade nakedobject) {
            throw new NotImplementedException();
        }

        public ITypeFacade GetSpecificationByLinkDomainType(string linkDomainType) {
            throw new NotImplementedException();
        }

        public string GetLinkDomainTypeBySpecification(ITypeFacade spec) {
            throw new NotImplementedException();
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

        public IFrameworkFacade Surface { set; 
            get; }

        #endregion

        private NakedObject GetServiceByServiceNameInternal(string serviceName) {
            if (string.IsNullOrEmpty(serviceName.Trim())) {
                throw new BadRequestNOSException();
            }
            return SurfaceUtils.GetServicesInternal().Where(s => s.getSpecification().getFullName() == serviceName).SingleOrDefault();
        }
    }
}