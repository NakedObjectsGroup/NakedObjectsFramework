// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Linq;
using NakedObjects.Surface;
using NakedObjects.Surface.Nof2.Implementation;
using NakedObjects.Surface.Utility;
using org.nakedobjects.@object;
using org.nakedobjects.@object.persistence;

namespace NakedObjects.Surface.Nof2.Utility {
    public class SerialOidStrategy : IOidStrategy {
        #region IOidStrategy Members

        public object GetDomainObjectByOid(string oid) {
            long idAsLong;
            if (!long.TryParse(oid, out idAsLong)) {
                // check if it's a service 
                NakedObject service = GetServiceByServiceNameInternal(oid);

                if (service == null) {
                    throw new BadRequestNOSException();
                }
                return service.getObject();
            }
            var serialOid = new SerialOid(idAsLong);

            NakedObject adapter = org.nakedobjects.@object.NakedObjects.getObjectLoader().getAdapterFor(serialOid);

            return adapter.getObject();
        }

        public object GetServiceByServiceName(string serviceName) {
            NakedObject service = GetServiceByServiceNameInternal(serviceName);

            if (service == null) {
                throw new ServiceResourceNotFoundNOSException(serviceName);
            }
            return service.getObject();
        }


        public object GetDomainObjectByOid(IOidTranslation objectId) {
            throw new System.NotImplementedException();
        }

        public object GetDomainObjectByTypeAndKey(string objectType, string objectKey) {
            throw new System.NotImplementedException();
        }

        public object GetServiceByServiceName(IOidTranslation serviceName) {
            throw new System.NotImplementedException();
        }

        public string GetOidAsString(IObjectFacade nakedObject) {
            if (nakedObject.Specification.IsService) {
                return nakedObject.Specification.FullName;
            }
            return nakedObject.Oid.ToString();
        }

        public string[] GetTypeAndKeyAsStrings(IObjectFacade nakedObject) {
            throw new System.NotImplementedException();
        }

       

        public string GetObjectId(IObjectFacade nakedobject) {
            throw new System.NotImplementedException();
        }

        public ITypeFacade GetSpecificationByLinkDomainType(string linkDomainType) {
            throw new System.NotImplementedException();
        }

        public string GetLinkDomainTypeBySpecification(ITypeFacade spec) {
            throw new System.NotImplementedException();
        }

        public IOidSurface RestoreOid(OidTranslationSemiColonSeparatedList id) {
            throw new System.NotImplementedException();
        }

        public IOidSurface RestoreSid(OidTranslationSemiColonSeparatedList id) {
            throw new System.NotImplementedException();
        }

        public IOidSurface RestoreOid(OidTranslationSlashSeparatedTypeAndIds id) {
            throw new System.NotImplementedException();
        }

        public IOidSurface RestoreSid(OidTranslationSlashSeparatedTypeAndIds id) {
            throw new System.NotImplementedException();
        }

        public IOidSurface RestoreEncodedOid(string encoded) {
            throw new System.NotImplementedException();
        }

        public IOidSurface RestoreTypeIdOid(string typeName, string instanceId) {
            throw new System.NotImplementedException();
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