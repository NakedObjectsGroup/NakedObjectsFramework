// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Common.Logging;
using NakedObjects.Core.NakedObjectsSystem;

namespace NakedObjects.Boot {
    public abstract class RunStandaloneBase : Run {

        private static readonly ILog Log = LogManager.GetLogger(typeof(RunStandaloneBase));

        protected override sealed void SetupSystem() {
            base.SetupSystem();

            if (Culture != null) {
                Thread.CurrentThread.CurrentCulture = new CultureInfo(Culture);
            }
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

         //   var enhancements = new List<IReflectorEnhancementInstaller>();


            //ResourceBasedI18nDecoratorInstaller i18n = I18N;
            //if (i18n != null) {
            //    enhancements.Add(i18n);
            //}

            //AddReflectorEnhancements(enhancements);
            //foreach (IReflectorEnhancementInstaller enhancement in enhancements) {
            //    System.AddReflectorEnhancement(enhancement);
            //}

            System.ObjectPersistorInstaller = Persistor;
            System.AuthenticatorInstaller = Authenticator;
            System.MenuServicesInstaller = MenuServices;
            System.ContributedActionsInstaller = ContributedActions;
            System.SystemServicesInstaller = SystemServices;
            System.FixtureInstaller = Fixtures;
        }

       // protected virtual void AddReflectorEnhancements(List<IReflectorEnhancementInstaller> enhancements) {}

        protected virtual void ProductSpecificStart(NakedObjectsSystem system) {}

        protected override sealed void Start() {
            try {
                base.Start();
                ProductSpecificStart(System);
                Client.StartClient(System);
            }
            catch (Exception exception) {
                Log.Error("Error on Framework start", exception);
               // NakedObjectsContext.InitialisationFatalError = exception;
            }
        }

        protected override void EnsureDomainAssembliesAreLoaded() {
            //do nothing -  registering of services is sufficient to ensure assemblies are loaded
        }
    }
}