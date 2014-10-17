// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.ServiceModel;
using System.Threading;
using Common.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Security;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Core.Security;
using NakedObjects.Core.Util;
using NakedObjects.Reflector.Transaction;

namespace NakedObjects.Boot {
    // TODO move this to NakedObjects.Core.NakedObjectsSystem in core project once reflector dependency has been removed

    public sealed class NakedObjectsSystem : IConnectionManager {
        private static readonly ILog Log;
        private IAuthenticatorInstaller authenticatorInstaller;
      
        private IServicesInstaller contributedActionsInstaller;
        private IFixturesInstaller fixtureInstaller;
        private ServiceHost host;
        private IServicesInstaller menuServicesInstaller;
        private IObjectPersistorInstaller objectPersistorInstaller;
        private INakedObjectReflector reflector;
        private IServicesInstaller systemServicesInstaller;

        static NakedObjectsSystem() {
            Log = LogManager.GetLogger(typeof (NakedObjectsSystem));
        }

        public IAuthenticationManager AuthenticationManager { get; private set; }

        public IAuthenticatorInstaller AuthenticatorInstaller {
            set { authenticatorInstaller = value; }
        }

       
        public IFixturesInstaller FixtureInstaller {
            set { fixtureInstaller = value; }
            get { return fixtureInstaller; }
        }

        public IObjectPersistorInstaller ObjectPersistorInstaller {
            set { objectPersistorInstaller = value; }
            get { return objectPersistorInstaller; }
        }

        public IReflectorInstaller ReflectorInstaller { get; set; }

        public IServicesInstaller MenuServicesInstaller {
            set { menuServicesInstaller = value; }
        }

        public IServicesInstaller ContributedActionsInstaller {
            set { contributedActionsInstaller = value; }
        }

        public IServicesInstaller SystemServicesInstaller {
            set { systemServicesInstaller = value; }
        }

        public ServiceHost Host {
            set { host = value; }
        }

        #region IConnectionManager Members

        public void Connect(ISession session) {
            InitContext(session);
        }

        public void Disconnect(ISession session) {
            // 1. confirm that session exist (user is connected)
            // 2. remove session from pool singleton, and get singleton reference for this thread to be disposed.
            ResetContext();
        }

        public void Shutdown() {
            Log.Info("shutting down system");

            if (host != null) {
                host.Close();
                host = null;
            }

           // reflector.Shutdown();
        }

        #endregion

        public void AddReflectorEnhancement(IReflectorEnhancementInstaller enhancementInstaller) {
            if (enhancementInstaller != null) {
                ReflectorInstaller.AddEnhancement(enhancementInstaller);
            }
        }


        public static NameValueCollection GetProperties() {
            var properties = new NameValueCollection();
            var keys = new ArrayList(Environment.GetEnvironmentVariables().Keys);
            var values = new ArrayList(Environment.GetEnvironmentVariables().Values);
            for (int count = 0; count < keys.Count; count++)
                properties.Add(keys[count].ToString(), values[count].ToString());
            return properties;
        }

        public void Init() {
            Log.Info("initialising naked objects system");
            Log.Info("working directory: " + new FileInfo(".").FullName);
            try {
                IReflectorEnhancementInstaller enhancement = new TransactionDecoratorInstaller();
                ReflectorInstaller.AddEnhancement(enhancement);

                reflector = ReflectorInstaller.CreateReflector();

                Log.DebugFormat("Culture is {0}", Thread.CurrentThread.CurrentCulture);

                if (authenticatorInstaller != null) {
                    AuthenticationManager = authenticatorInstaller.CreateAuthenticationManager();
                    AuthenticationManager.Init();
                }

                // TODO shutdown the startup context (eg on main thread)
                // NakedObjectsContext.shutdown();        
            }
            catch (Exception e) {
                Log.Error("failed to intialise", e);
                throw;
            }
        }

        private object[] GetServices(IServicesInstaller si) {
            return si == null ? new object[] {} : si.GetServices();
        }

        private void InitContext(ISession session) {
            if (session == null) {
                throw new InvalidStateException("Session not specified on " + Thread.CurrentThread);
            }

          //  context.SetReflector(reflector);
          //  ILifecycleManager objectPersistor = objectPersistorInstaller.CreateObjectPersistor();
          //  NakedObjectsContext.PersistorInstaller = objectPersistorInstaller;
          //  NakedObjectsContext.MenuServicesInstaller = menuServicesInstaller;
          //  NakedObjectsContext.ContributedActionsInstaller = contributedActionsInstaller;
          //  NakedObjectsContext.SystemServicesInstaller = systemServicesInstaller;

          //  NakedObjectsContext.FixturesInstaller = fixtureInstaller;
          //  context.SetObjectPersistor(objectPersistor);
          //  context.SetAuthorizationManager(new NullAuthorizationManager());
          //  context.SetSession(session);
      
          //  //objectPersistor.UpdateNotifier = NakedObjectsContext.UpdateNotifier;
            
          //  objectPersistor.AddServices(menuServicesInstaller, contributedActionsInstaller, systemServicesInstaller);

          //  var ss = GetServices(menuServicesInstaller).Union(GetServices(contributedActionsInstaller)).Union(GetServices(systemServicesInstaller)).ToArray();
          
          ////  objectPersistor.Injector = new DotNetDomainObjectContainerInjector(reflector, ss);      

          //  objectPersistor.Init();

          //  reflector.NonSystemServices = objectPersistor.GetServices(ServiceTypes.Menu | ServiceTypes.Contributor).ToArray();
           
          //  if (fixtureInstaller != null) {
          //      NakedObjectsContext.EnsureReady();
               
          //      context.SetSession(session);
          //      fixtureInstaller.InstallFixtures(NakedObjectsContext.LifecycleManager);
          //  }

          //  // remove all unneeded details in the initialisation context
          //  NakedObjectsContext.MessageBroker.ClearWarnings();
          //  NakedObjectsContext.MessageBroker.ClearMessages();
          //  NakedObjectsContext.UpdateNotifier.AllChangedObjects();
          //  NakedObjectsContext.UpdateNotifier.AllDisposedObjects();
        }

        private void ResetContext() {
            //context.ClearSession();
            //NakedObjectsContext.LifecycleManager.Reset();
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}