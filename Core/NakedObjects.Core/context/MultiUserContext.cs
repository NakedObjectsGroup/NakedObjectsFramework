// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Security;
using NakedObjects.Core.Util;
using NakedObjects.Objects;

namespace NakedObjects.Core.Context {
    public abstract class MultiUserContext : NakedObjectsContext {
        private IAuthorizationManager authorizationManager;
        private INakedObjectReflector reflector;
        protected abstract NakedObjectsData Local { get; }


        protected internal override string GetExecutionContextId() {
            return Local.ExecutionContextId();
        }

        protected internal override IMessageBroker GetMessageBroker() {
            return Local.MessageBroker;
        }

        protected internal override INakedObjectPersistor GetObjectPersistor() {
            return Local.ObjectPersistor;
        }

        protected internal override ISession GetSession() {
            return Local.Session;
        }

        protected internal override IUpdateNotifier GetUpdateNotifier() {
            return Local.UpdateNotifier;
        }

        public override void SetObjectPersistor(INakedObjectPersistor objectManager) {
            // ignore will set lazily for each thread
        }

        public override void SetSession(ISession newSession) {
            if (Local.Session != null) {
                throw new InvalidStateException("Session already exists on this thread");
            }
            Local.Session = newSession;
            Local.accessTime = DateTime.Now.Ticks;
        }

        public override void ClearSession() {
            //Local.TakeSnapshot();
            Local.Session = null;
            Local.MessageBroker.ClearMessages();
            Local.MessageBroker.ClearWarnings();
            Local.MessageBroker.EnsureEmpty();
            Local.UpdateNotifier.EnsureEmpty();
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
    }

    // Copyright (c) Naked Objects Group Ltd.
}