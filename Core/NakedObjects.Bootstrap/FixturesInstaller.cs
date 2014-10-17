// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Linq;
using Common.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Util;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Reflector.DotNet.Fixture;

namespace NakedObjects.Boot {
    public class FixturesInstaller : IFixturesInstaller {
        private static readonly ILog Log = LogManager.GetLogger(typeof (FixturesInstaller));
        private readonly object[] fixtures;

        public FixturesInstaller(params object[] fixtures) {
            this.fixtures = fixtures;
        }

        #region IFixturesInstaller Members

        public void InstallFixtures(INakedObjectTransactionManager transactionManager, IContainerInjector injector) {
            //NakedObjectsContext.LifecycleManager.Reset();

            //if (NakedObjectsContext.LifecycleManager.IsInitialized) {
            //    Log.Info("skipping fixtures, as already loaded");
            //    return;
            //}

            var builder = new DotNetFixtureBuilder();
            fixtures.ForEach(builder.AddFixture);
            builder.InstallFixtures(transactionManager, injector);
        }

        public void InstallFixture(INakedObjectTransactionManager transactionManager, IContainerInjector injector, string fixtureName) {
            object fixture = fixtures.FirstOrDefault(f => f.GetType().Name == fixtureName);

            if (fixture != null) {
                var builder = new DotNetFixtureBuilder();
                builder.AddFixture(fixture);
                builder.InstallFixture(transactionManager, injector, fixtureName);
            }
        }

        public string[] FixtureNames {
            get { return fixtures.Select(f => f.GetType().Name).ToArray(); }
        }

        public virtual string Name {
            get { return "FixturesInstaller"; }
        }

        #endregion
    }
}