// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Reflection;
using Common.Logging;
using NakedObjects.Architecture.Persist;
using NakedObjects.Core.Persist;
using NakedObjects.Reflector.DotNet.Reflect.Util;

namespace NakedObjects.Reflector.DotNet.Fixture {
    public class DotNetFixtureBuilder : AbstractFixtureBuilder {
        private static readonly ILog Log = LogManager.GetLogger(typeof (DotNetFixtureBuilder));
        private DotNetFixtureServices fixtureServices;

        public override string Name {
            get { return "Dotnet fixtures builder"; }
        }

        protected override void PreInstallFixtures(INakedObjectTransactionManager transactionManager) {
            fixtureServices = new DotNetFixtureServices();
        }

        private static void SetValue(PropertyInfo property, object injectee, object value) {
            if (property != null) {
                Log.DebugFormat("Injecting {0} into instance of {1}", value, injectee.GetType().FullName);
                try {
                    property.SetValue(injectee, value, null);
                }
                catch (TargetInvocationException e) {
                    InvokeUtils.InvocationException("Exception executing " + property, e);
                }
            }
            else {
                Log.DebugFormat("Failed to inject {0} into instance of {1}", value, injectee.GetType().FullName);
            }
        }

        private static MethodInfo GetInstallMethod(object fixture) {
            return fixture.GetType().GetMethod("Install", new Type[0]) ??
                   fixture.GetType().GetMethod("install", new Type[0]);
        }


        protected override void InstallFixture(object fixture) {
            PropertyInfo property = fixture.GetType().GetProperty("Service");
            SetValue(property, fixture, fixtureServices);

            MethodInfo installMethod = GetInstallMethod(fixture);
            Log.Debug("Invoking install method");
            try {
                installMethod.Invoke(fixture, new object[0]);
            }
            catch (TargetInvocationException e) {
                InvokeUtils.InvocationException("Exception executing " + installMethod, e);
            }
        }


        /// <summary>
        ///     Obtain any child fixtures for this fixture
        /// </summary>
        protected override object[] GetFixtures(object fixture) {
            MethodInfo getFixturesMethod = fixture.GetType().GetMethod("GetFixtures", new Type[] {});
            Log.Debug("Obtaining (any) child fixtures");
            return getFixturesMethod == null ? new object[] {} : (object[]) getFixturesMethod.Invoke(fixture, new object[] {});
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}