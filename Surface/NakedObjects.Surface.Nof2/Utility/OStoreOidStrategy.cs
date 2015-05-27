// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Linq;
using NakedObjects.Surface;
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


        public object GetDomainObjectByOid(ILinkObjectId objectId) {
            throw new System.NotImplementedException();
        }

        public object GetDomainObjectByTypeAndKey(string objectType, string objectKey) {
            throw new System.NotImplementedException();
        }

        public object GetServiceByServiceName(ILinkObjectId serviceName) {
            throw new System.NotImplementedException();
        }

        public string GetOidAsString(INakedObjectSurface nakedObject) {
            // don't know if services need to be handled differently 

            //if (nakedObject.Specification.IsService) {
            //    return nakedObject.Specification.FullName; 
            //}
            return nakedObject.Oid.ToString();
        }

        public string[] GetTypeAndKeyAsStrings(INakedObjectSurface nakedObject) {
            throw new System.NotImplementedException();
        }

       
        public string GetObjectId(INakedObjectSurface nakedobject) {
            throw new System.NotImplementedException();
        }

        public INakedObjectSpecificationSurface GetSpecificationByLinkDomainType(string linkDomainType) {
            throw new System.NotImplementedException();
        }

        public string GetLinkDomainTypeBySpecification(INakedObjectSpecificationSurface spec) {
            throw new System.NotImplementedException();
        }

        public INakedObjectsSurface Surface { set; 
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