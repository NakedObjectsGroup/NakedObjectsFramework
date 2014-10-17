// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Component;

namespace NakedObjects.Core.NakedObjectsSystem {
    public interface IFixturesInstaller : IInstaller {
        string[] FixtureNames { get; }
        void InstallFixtures(ITransactionManager transactionManager, IContainerInjector injector);
        void InstallFixture(ITransactionManager transactionManager, IContainerInjector injector, string fixtureName);
    }

    // Copyright (c) Naked Objects Group Ltd.
}