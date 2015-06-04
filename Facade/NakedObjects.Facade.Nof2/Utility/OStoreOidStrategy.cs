// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using NakedObjects.Facade.Translation;
using org.nakedobjects.@object;
using sdm.systems.application.objectstore;

namespace NakedObjects.Facade.Nof2.Utility {
    public class OStoreOidStrategy : IOidStrategy {
        #region IOidStrategy Members

        public object GetDomainObjectByOid(IOidTranslation objectId) {
            throw new NotImplementedException();
        }

        public object GetServiceByServiceName(IOidTranslation serviceName) {
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

        public IFrameworkFacade Surface { set; get; }

        #endregion

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

        public object GetDomainObjectByTypeAndKey(string objectType, string objectKey) {
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

        public IOidFacade RestoreEncodedOid(string encoded) {
            throw new NotImplementedException();
        }

        public IOidFacade RestoreTypeIdOid(string typeName, string instanceId) {
            throw new NotImplementedException();
        }

        private NakedObject GetServiceByServiceNameInternal(string serviceName) {
            if (string.IsNullOrEmpty(serviceName.Trim())) {
                throw new BadRequestNOSException();
            }
            return SurfaceUtils.GetServicesInternal().Where(s => s.getSpecification().getFullName() == serviceName).SingleOrDefault();
        }
    }
}