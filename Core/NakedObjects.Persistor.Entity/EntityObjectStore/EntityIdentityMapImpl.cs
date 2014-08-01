// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets.Objects.Aggregated;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Resolve;
using NakedObjects.Core.Adapter.Map;

namespace NakedObjects.EntityObjectStore {
    public class EntityIdentityMapImpl : IdentityMapImpl {
        public EntityIdentityMapImpl(INakedObjectPersistor objectPersistor, IIdentityAdapterMap identityAdapterMap, IPocoAdapterMap pocoAdapterMap, EntityObjectStore objectStore)
            : base(objectPersistor, identityAdapterMap, pocoAdapterMap) {
            ObjectStore = objectStore;
        }

        private EntityObjectStore ObjectStore { get; set; }

        public override void AddAdapter(INakedObject nakedObject) {
            base.AddAdapter(nakedObject);
            if (nakedObject.Specification.IsService ||
                nakedObject.Specification.IsViewModel ||
                nakedObject.Specification.ContainsFacet(typeof (IComplexTypeFacet))) {
                return;
            }
            ObjectStore.LoadComplexTypes(nakedObject, nakedObject.ResolveState.IsGhost());
        }
    }
}