// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Security;
using NakedObjects.Objects;

namespace NakedObjects.Core.Context {
    /// <summary>
    ///     Single-user implementation of NakedObjects that simply stores the components and returns them on request
    /// </summary>
    public class StaticContext : NakedObjectsContext {
        private readonly IMessageBroker messages = new SimpleMessageBroker();
        private readonly IUpdateNotifier updateNotifier = new SimpleUpdateNotifier();
        private IAuthorizationManager authorizationManager;
        private INakedObjectPersistor objectPersistor;
        private INakedObjectReflector reflector;
        private ISession session;

        protected internal StaticContext() {}

        protected internal override void EnsureContextReady() {
            // do nothing
        }


        public static NakedObjectsContext CreateInstance() {
            return new StaticContext();
        }

        protected internal override void TerminateSession() {
            session = null;
        }


        protected internal override string GetExecutionContextId() {
            return "#1" + (session == null ? "" : "/" + session.UserName);
        }

        protected internal override string[] GetAllExecutionContextIds() {
            return new[] {GetExecutionContextId()};
        }

        protected internal override IMessageBroker GetMessageBroker() {
            return messages;
        }

        protected internal override INakedObjectPersistor GetObjectPersistor() {
            return objectPersistor;
        }

        protected internal override ISession GetSession() {
            return session;
        }

        protected internal override IUpdateNotifier GetUpdateNotifier() {
            return updateNotifier;
        }

        public override void SetObjectPersistor(INakedObjectPersistor objectManager) {
            objectPersistor = objectManager;
        }

        public override void SetSession(ISession newSession) {
            session = newSession;
        }

        public override void ClearSession() {
            session = null;
        }

        public override void SetReflector(INakedObjectReflector newReflector) {
            reflector = newReflector;
        }

        protected internal override INakedObjectReflector GetReflector() {
            return reflector;
        }

        public override void SetAuthorizationManager(IAuthorizationManager newAuthorizationManager) {
            authorizationManager = newAuthorizationManager;
        }

        protected internal override IAuthorizationManager GetAuthorizationManager() {
            return authorizationManager;
        }

        public override void ShutdownSession() {
            if (objectPersistor != null) {
                objectPersistor.Shutdown();
                objectPersistor = null;
            }
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}