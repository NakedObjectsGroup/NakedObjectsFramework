// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Util;
using NakedObjects.Surface.Nof4.Implementation;

namespace NakedObjects.Surface.Nof4.Utility {
    public class MVCOid : IOidStrategy {
        private readonly INakedObjectsFramework framework;

        public MVCOid(INakedObjectsFramework framework) {
            this.framework = framework;
        }

        #region IOidStrategy Members

        public INakedObjectsSurface Surface { protected get; set; }

        private static INakedObjectAdapter RestoreCollection(ICollectionMemento memento) {
            return memento.RecoverCollection();
        }

        private static INakedObjectAdapter RestoreInline( INakedObjectsFramework framework, IAggregateOid aggregateOid) {
            IOid parentOid = aggregateOid.ParentOid;
            INakedObjectAdapter parent = RestoreObject(framework, parentOid);
            IAssociationSpec assoc = parent.GetObjectSpec().Properties.Where((p => p.Id == aggregateOid.FieldName)).Single();

            return assoc.GetNakedObject(parent);
        }

        private static INakedObjectAdapter RestoreViewModel( INakedObjectsFramework framework, IViewModelOid viewModelOid) {
            return framework.NakedObjectManager.GetAdapterFor(viewModelOid) ?? framework.LifecycleManager.GetViewModel(viewModelOid);
        }

        public static INakedObjectAdapter RestoreObject( INakedObjectsFramework framework, IOid oid) {
            return oid.IsTransient ? framework.LifecycleManager.RecreateInstance(oid, oid.Spec) : framework.LifecycleManager.LoadObject(oid, oid.Spec);
        }


        private INakedObjectAdapter GetAdapterByOid(ILinkObjectId objectId) {
            var encodedId = objectId.ToString();

            if (string.IsNullOrEmpty(encodedId)) {
                return null;
            }

            IOid oid = framework.LifecycleManager.RestoreOid(encodedId.Split(';'));

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


        public object GetDomainObjectByOid(ILinkObjectId objectId) {
          return GetAdapterByOid(objectId).GetDomainObject();
        }

        public object GetServiceByServiceName(ILinkObjectId serviceName) {
            return framework.GetNakedObjectFromId(serviceName.ToString()).GetDomainObject();
        }

        public ILinkObjectId GetOid(INakedObjectSurface nakedObject) {
            INakedObjectAdapter no = ((dynamic) nakedObject).WrappedNakedObject;
            var id = framework.GetObjectId(no);
            return new MVCObjectId(id);
        }

        public ILinkObjectId GetOid(object domainObject) {
            var no = framework.GetNakedObject(domainObject);
            var id = framework.GetObjectId(no);
            return new MVCObjectId(id);
        }

        public ILinkObjectId GetOid(string servicename) {
            var service = framework.ServicesManager.GetService(servicename);
            var id = framework.GetObjectId(service);
            return new MVCObjectId(id);
        }

        public ILinkObjectId GetOid(string typeName, string instanceId) {
           return new MVCObjectId(typeName);
        }

        public INakedObjectSpecificationSurface GetSpecificationByLinkDomainType(string linkDomainType) {
            throw new NotImplementedException();
        }

        public string GetLinkDomainTypeBySpecification(INakedObjectSpecificationSurface spec) {
            throw new NotImplementedException();
        }

        #endregion
    }
}