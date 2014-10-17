// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Core.NakedObjectsSystem;

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
        public void InstallFixtures(INakedObjectTransactionManager transactionManager, IContainerInjector injector) {
            PreInstallFixtures(transactionManager);
            InstallFixtures(transactionManager, injector, Fixtures);
            PostInstallFixtures(transactionManager);
            fixtures.Clear();
        }

        public void InstallFixture(INakedObjectTransactionManager transactionManager, IContainerInjector injector, string fixtureName) {
            InstallFixtures(transactionManager, injector, Fixtures);
        }

        public string[] FixtureNames {
            get { return Fixtures.Select(f => f.GetType().Name).ToArray(); }
        }

        #endregion

        public virtual void AddFixture(object fixture) {
            fixtures.Add(fixture);
        }

        private void InstallFixtures(INakedObjectTransactionManager transactionManager, IContainerInjector injector, object[] newFixtures) {
            foreach (object fixture in newFixtures) {
                InstallFixture(transactionManager, injector, fixture);
            }
        }

        private void InstallFixture(INakedObjectTransactionManager transactionManager, IContainerInjector injector, object fixture) {
            injector.InitDomainObject(fixture);

            // first, install any child fixtures (if this is a composite.
            object[] childFixtures = GetFixtures(fixture);
            InstallFixtures(transactionManager, injector, childFixtures);

            // now, install the fixture itself
            try {
                Log.Info("installing fixture: " + fixture);
                transactionManager.StartTransaction();
                InstallFixture(fixture);
                transactionManager.EndTransaction();
                Log.Info("fixture installed");
            }
            catch (Exception e) {
                Log.Error("installing fixture " + fixture.GetType().FullName + " failed (" + e.Message + "); aborting fixture ", e);
                try {
                    transactionManager.AbortTransaction();
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

        protected virtual void PostInstallFixtures(INakedObjectTransactionManager transactionManager) { }

        protected virtual void PreInstallFixtures(INakedObjectTransactionManager transactionManager) {}
    }
}