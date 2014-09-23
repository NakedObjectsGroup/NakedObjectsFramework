// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Security;
using NakedObjects.Core.Adapter.Map;
using NakedObjects.Core.NakedObjectsSystem;

namespace NakedObjects.Persistor {
    public abstract class AbstractObjectPersistorInstaller : IObjectPersistorInstaller {
        protected IIdentityAdapterMap identityAdapterMap;
        protected IPocoAdapterMap pocoAdapterMap;

        #region IObjectPersistorInstaller Members

        public abstract ILifecycleManager CreateObjectPersistor();

        public abstract string Name { get; }

        public void SetupMaps(IIdentityAdapterMap adapterMap, IPocoAdapterMap pocoMap) {
            identityAdapterMap = adapterMap;
            pocoAdapterMap = pocoMap;
        }

        #endregion
    }


    // Copyright (c) Naked Objects Group Ltd.
}