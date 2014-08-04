// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Security;
using NakedObjects.Core.Context;
using NakedObjects.Objects;
using NakedObjects.Persistor.Objectstore.Inmemory;
using NakedObjects.TestSystem;

namespace NakedObjects.Testing {
    public class ProgrammableContext : NakedObjectsContext {
        private readonly ProgrammableTestSystem programmableTestSystem;
        private INakedObjectPersistor persistor;
        private INakedObjectReflector reflector;
        private ISession session = new TestProxySession();

        public ProgrammableContext(ProgrammableTestSystem programmableTestSystem) {
            this.programmableTestSystem = programmableTestSystem;
        }

        protected override void EnsureContextReady() {
            throw new NotImplementedException();
        }

        protected override void TerminateSession() {
            throw new NotImplementedException();
        }

        protected override ISession GetSession() {
            return session;
        }

        protected override IUpdateNotifier GetUpdateNotifier() {
            return new SimpleUpdateNotifier();
        }

        protected override string GetExecutionContextId() {
            throw new NotImplementedException();
        }

        protected override string[] GetAllExecutionContextIds() {
            throw new NotImplementedException();
        }


        protected override IMessageBroker GetMessageBroker() {
            throw new NotImplementedException();
        }

        protected override INakedObjectPersistor GetObjectPersistor() {
            return persistor ?? (persistor = new InMemoryObjectPersistorInstaller().CreateObjectPersistor());
        }

        public override void SetObjectPersistor(INakedObjectPersistor persistor) {
            this.persistor = persistor;
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

        protected override INakedObjectReflector GetReflector() {
            return reflector ?? (reflector = new ProgrammableReflector(programmableTestSystem));
        }

        public override void SetAuthorizationManager(IAuthorizationManager newAuthorizationManager) {
            throw new NotImplementedException();
        }

        protected override IAuthorizationManager GetAuthorizationManager() {
            throw new NotImplementedException();
        }

        public override void ShutdownSession() {
            throw new NotImplementedException();
        }
    }
}