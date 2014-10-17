// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Component;
using NakedObjects.Core.Adapter.Map;

namespace NakedObjects.Core.NakedObjectsSystem {
    /// <summary>
    ///     Installs a NakedObjectPersistor during system start up
    /// </summary>
    public interface IObjectPersistorInstaller : IInstaller {
        ILifecycleManager CreateObjectPersistor();

        void SetupMaps(IIdentityAdapterMap adapterMap, IPocoAdapterMap pocoMap);
    }

    // Copyright (c) Naked Objects Group Ltd.
}