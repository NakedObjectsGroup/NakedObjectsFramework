// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.IO;
using Common.Logging;
using NakedObjects.Core.Context;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Core.Security;
using NakedObjects.Persistor.Objectstore.Inmemory;
using NakedObjects.Reflector.Audit;
using NakedObjects.Reflector.DotNet.Reflect;
using NakedObjects.Reflector.I18n.Resourcebundle;
using NakedObjects.Reflector.Security;

namespace NakedObjects.Boot {
    public abstract class Run {
        private static readonly ILog Log = LogManager.GetLogger(typeof (Run));

        internal Run() {
            // to prevent subclassing oustside of this project/assembly, ie by an application
            NakedObjectsContext.StartTime = DateTime.Now;
        }

        protected NakedObjectsSystem System { get; private set; }

        protected virtual void Start() {
            SetupSystem();
            System.Init();
            Log.Info("system started");
        }

        protected virtual void SetupSystem() {
            const string dir = ".";
            Environment.CurrentDirectory = dir;
            Log.DebugFormat("Current dir: {0}", new FileInfo(dir).FullName);
            Log.DebugFormat("Real path: {0}", dir);
            EnsureDomainAssembliesAreLoaded();

            System = new NakedObjectsSystem {
                ReflectorInstaller = Reflector,
                Context = Context
            };
        }

        #region overridable properties

        protected virtual IAuthenticatorInstaller Authenticator {
            get { return new WindowsAuthenticatorInstaller(); }
        }

        protected virtual INakedObjectsClient Client {
            get { return null; }
        }

        protected virtual bool Localize {
            get { return false; }
        }

        protected virtual string ModelResourceFile {
            get { return null; }
        }

        protected virtual ResourceBasedI18nDecoratorInstaller I18N {
            get {
                if (Localize) {
                    return new ResourceBasedI18nDecoratorInstaller(ModelResourceFile);
                }
                return null;
            }
        }

        protected virtual string Culture {
            get { return null; }
        }

        protected virtual IFixturesInstaller Fixtures {
            get { return null; }
        }

        protected virtual IServicesInstaller MenuServices {
            get { return null; }
        }

        protected virtual IServicesInstaller ContributedActions {
            get { return null; }
        }

        protected virtual IServicesInstaller SystemServices {
            get { return null; }
        }

        protected virtual IObjectPersistorInstaller Persistor {
            get { return new InMemoryObjectPersistorInstaller(); }
        }

        protected virtual IAuthorizerInstaller Authorizer {
            get { return null; }
        }

        protected virtual IAuditorInstaller Auditor {
            get { return null; }
        }

        protected virtual NakedObjectsContext Context {
            get { return StaticContext.CreateInstance(); }
        }

        protected virtual IReflectorInstaller Reflector {
            get { return new DotNetReflectorInstaller(); }
        }

        /// <summary>
        ///     NakedObjects reflects over domain code during startup to build the view of the domain
        ///     model. For this reason domain assemblies need to be in memory during startup.
        ///     This method provides a convenient mechanism for doing this.  In your implementation
        ///     of the method, we recommend that you call one static (shared) method on a class in
        ///     each of the domain assemblies (the method does not have to do anything)  -
        ///     this is sufficient to ensure that the assembly is brought into memory.
        /// </summary>
        protected abstract void EnsureDomainAssembliesAreLoaded();

        #endregion
    }
}