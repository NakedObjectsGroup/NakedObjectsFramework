// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Persist;
using NakedObjects.Core.Reflect;

namespace NakedObjects.Core.NakedObjectsSystem {
    public interface IFixturesInstaller : IInstaller {
        string[] FixtureNames { get; }
        void InstallFixtures(INakedObjectTransactionManager transactionManager, IContainerInjector injector);
        void InstallFixture(INakedObjectTransactionManager transactionManager, IContainerInjector injector, string fixtureName);
    }

    // Copyright (c) Naked Objects Group Ltd.
}