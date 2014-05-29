// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using Common.Logging;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Security;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Core.Util;
using NakedObjects.Objects;

namespace NakedObjects.Core.Context {
    /// <summary>
    ///     A repository of all the NOF components that are shared by a running system
    /// </summary>
    public abstract class NakedObjectsContext {
        private static readonly ILog Log;
        private static NakedObjectsContext singleton;
        private IObjectPersistorInstaller persistorInstaller;

        static NakedObjectsContext() {
            Log = LogManager.GetLogger(typeof (NakedObjectsContext));
        }

        /// <summary>
        ///     Creates a new instance of the execution context holder. Will throw a StartupException if an instance
        ///     has already been created.
        /// </summary>
        protected NakedObjectsContext() {
            // TODO remove next line once XAT test framework is updated
            Reset();
            if (singleton != null) {
                throw new InvalidStateException("Naked Objects Singleton already set up");
            }
            singleton = this;
        }

        public static DateTime StartTime { get; set; }

        /// <summary>
        ///     Returns an descriptive identifier for the execution context
        /// </summary>
        public static string ExecutionContextId {
            get { return Instance.GetExecutionContextId(); }
        }

        public static string[] AllExecutionContextIds {
            get { return Instance.GetAllExecutionContextIds(); }
        }

        /// <summary>
        ///     Returns the singleton providing access to the set of execution contexts
        /// </summary>
        public static NakedObjectsContext Instance {
            get { return singleton; }
        }

        /// <summary>
        ///     Returns the message broker for this execution context
        /// </summary>
        public static IMessageBroker MessageBroker {
            get { return Instance.GetMessageBroker(); }
        }

        /// <summary>
        ///     Returns the update notifier, which collects the changes made to objects
        /// </summary>
        public static IUpdateNotifier UpdateNotifier {
            get {
                CheckValidState();
                return Instance.GetUpdateNotifier();
            }
        }

        /// <summary>
        ///     Determine if the execution context is set up and ready to be used. Returns false if any of the
        ///     following are not set: reflector; persistor; or message broker.
        /// </summary>
        public static bool IsReady {
            get {
                return Instance.GetReflector() != null &&
                       Instance.GetObjectPersistor() != null &&
                       Instance.GetMessageBroker() != null;
            }
        }

        /// <summary>
        ///     Returns the session representing this user for this execution context
        /// </summary>
        public static ISession Session {
            get { return Instance.GetSession(); }
        }

        /// <summary>
        ///     Returns the object persistor for this execution context
        /// </summary>
        public static INakedObjectPersistor ObjectPersistor {
            get {
                CheckValidState();
                return Instance.GetObjectPersistor();
            }
        }

        /// <summary>
        ///     Return the specification loader for this execution context
        /// </summary>
        public static INakedObjectReflector Reflector {
            get {
                //  CheckValidState();
                return Instance.GetReflector();
            }
        }

        public static IAuthorizationManager AuthorizationManager {
            get {
                CheckValidState();
                return Instance.GetAuthorizationManager();
            }
        }

        public static IObjectPersistorInstaller PersistorInstaller {
            get { return Instance.GetPersistorInstaller(); }
            set { Instance.SetPersistorInstaller(value); }
        }

        public static IServicesInstaller MenuServicesInstaller { get; set; }
        public static IServicesInstaller ContributedActionsInstaller { get; set; }
        public static IServicesInstaller SystemServicesInstaller { get; set; }

        public static IFixturesInstaller FixturesInstaller { get; set; }
        public static Exception InitialisationFatalError { get; set; }

        public static string[] InitialisationWarnings {
            get {
                //ILoggerRepository[] repositories = LoggerManager.GetAllRepositories();
                //IEnumerable<WarningAppender> appenders = repositories[0].GetAppenders().OfType<WarningAppender>().Where(a => a.HasMessages);
                //return appenders.SelectMany(a => a.Events.ToEnumerable()).Select(le => le.MessageObject.ToString()).Distinct().ToArray();

                return new string[] {};
            }
        }

        internal virtual void SetPersistorInstaller(IObjectPersistorInstaller value) {
            persistorInstaller = value;
        }

        internal virtual IObjectPersistorInstaller GetPersistorInstaller() {
            return persistorInstaller;
        }

        public static void EnsureReady() {
            Instance.EnsureContextReady();
        }

        protected internal abstract void EnsureContextReady();

        /// <summary>
        ///     Terminate the session for the user in the current context
        /// </summary>
        public static void CloseSession() {
            Instance.ClearSession();
        }


        private static void CheckValidState() {
            Assert.AssertNotNull("no specification loader set up", Instance.GetReflector());
        }

        /// <summary>
        ///     Resets the singleton, so another can created.
        /// </summary>
        public static void Reset() {
            singleton = null;
        }

        /// <summary>
        ///     Terminate the session for the user in the current execution context
        /// </summary>
        protected internal abstract void TerminateSession();

        /// <summary>
        ///     Find the session for the current execution context
        /// </summary>
        protected internal abstract ISession GetSession();

        /// <summary>
        ///     Find the update notifier for the current execution context
        /// </summary>
        protected internal abstract IUpdateNotifier GetUpdateNotifier();

        /// <summary>
        ///     Generate the id for the current execution context
        /// </summary>
        protected internal abstract string GetExecutionContextId();

        protected internal abstract string[] GetAllExecutionContextIds();

        /// <summary>
        ///     Find the message broker for the current execution context
        /// </summary>
        protected internal abstract IMessageBroker GetMessageBroker();

        /// <summary>
        ///     Find the object persistor for the current execution context
        /// </summary>
        protected internal abstract INakedObjectPersistor GetObjectPersistor();

        /// <summary>
        ///     Initialise the object persistor for the current execution context
        /// </summary>
        public abstract void SetObjectPersistor(INakedObjectPersistor persistor);

        /// <summary>
        ///     Initialise the session for the current execution context
        /// </summary>
        public abstract void SetSession(ISession newSession);

        /// <summary>
        ///     Terminate the session for the current execution context
        /// </summary>
        public abstract void ClearSession();

        /// <summary>
        ///     Initialise the specification loader for the current execution context
        /// </summary>
        public abstract void SetReflector(INakedObjectReflector newReflector);

        /// <summary>
        ///     Find the specification loader for the current execution context
        /// </summary>
        protected internal abstract INakedObjectReflector GetReflector();

        public abstract void SetAuthorizationManager(IAuthorizationManager newAuthorizationManager);

        protected internal abstract IAuthorizationManager GetAuthorizationManager();

        public static void Shutdown() {
            Log.Info("shutting down");
            Instance.ShutdownSession();
        }

        public abstract void ShutdownSession();
    }

    // Copyright (c) Naked Objects Group Ltd.
}