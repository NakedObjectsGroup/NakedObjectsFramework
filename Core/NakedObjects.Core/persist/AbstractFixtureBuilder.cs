// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using NakedObjects.Architecture.Persist;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Core.Reflect;

namespace NakedObjects.Core.Persist {
    public abstract class AbstractFixtureBuilder : IFixturesInstaller {
        private static readonly ILog Log;
        private readonly List<object> fixtures = new List<object>();


        static AbstractFixtureBuilder() {
            Log = LogManager.GetLogger(typeof (AbstractFixtureBuilder));
        }

        /// <summary>
        ///     Returns as an array all fixtures that have been added via <see cref="AddFixture" />
        /// </summary>
        protected virtual object[] Fixtures {
            get { return fixtures.ToArray(); }
        }

        #region IFixturesInstaller Members

        public abstract string Name { get; }

        /// <summary>
        ///     Installs all <see cref="AddFixture" /> added  fixtures (ie as returned by <see cref="Fixtures" />
        /// </summary>
        /// <para>
        ///     Once done the set of fixtures is cleared and <see cref="Fixtures" /> returns an empty array.
        /// </para>
        public void InstallFixtures(INakedObjectPersistor persistor, IContainerInjector injector) {
            PreInstallFixtures(persistor);
            InstallFixtures(persistor, injector, Fixtures);
            PostInstallFixtures(persistor);
            persistor.Reset();
            fixtures.Clear();
        }

        public void InstallFixture(INakedObjectPersistor persistor, IContainerInjector injector, string fixtureName) {
            InstallFixtures(persistor, injector, Fixtures);
        }

        public string[] FixtureNames {
            get { return Fixtures.Select(f => f.GetType().Name).ToArray(); }
        }

        #endregion

        public virtual void AddFixture(object fixture) {
            fixtures.Add(fixture);
        }

        private void InstallFixtures(INakedObjectPersistor persistor, IContainerInjector injector, object[] newFixtures) {
            foreach (object fixture in newFixtures) {
                InstallFixture(persistor, injector, fixture);
            }
        }

        private void InstallFixture(INakedObjectPersistor persistor, IContainerInjector injector, object fixture) {
            injector.InitDomainObject(fixture);

            // first, install any child fixtures (if this is a composite.
            object[] childFixtures = GetFixtures(fixture);
            InstallFixtures(persistor, injector, childFixtures);

            // now, install the fixture itself
            try {
                Log.Info("installing fixture: " + fixture);
                persistor.StartTransaction();
                InstallFixture(fixture);
                persistor.EndTransaction();
                Log.Info("fixture installed");
            }
            catch (Exception e) {
                Log.Error("installing fixture " + fixture.GetType().FullName + " failed (" + e.Message + "); aborting fixture ", e);
                try {
                    persistor.AbortTransaction();
                }
                catch (Exception e2) {
                    Log.Error("failure during abort", e2);
                }
                throw;
            }
        }

        /// <summary>
        ///     Calls the fixtures installation method
        /// </summary>
        protected abstract void InstallFixture(object fixture);

        /// <summary>
        ///     Extract any child fixtures of the provided fixture
        /// </summary>
        protected abstract object[] GetFixtures(object fixture);

        protected virtual void PostInstallFixtures(INakedObjectPersistor objectManager) {}

        protected virtual void PreInstallFixtures(INakedObjectPersistor objectManager) {}
    }
}