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
using org.nakedobjects.@object.persistence;

namespace NakedObjects.Facade.Nof2.Utility {
    public class SerialOidStrategy : IOidStrategy {
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

        public IFrameworkFacade FrameworkFacade { set; get; }

        #endregion

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

        public object GetDomainObjectByTypeAndKey(string objectType, string objectKey) {
            throw new NotImplementedException();
        }

        public string GetOidAsString(IObjectFacade nakedObject) {
            if (nakedObject.Specification.IsService) {
                return nakedObject.Specification.FullName;
            }
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
            return FacadeUtils.GetServicesInternal().Where(s => s.getSpecification().getFullName() == serviceName).SingleOrDefault();
        }
    }
}