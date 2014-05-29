// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Common.Logging;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Security;
using NakedObjects.Core.Util;
using NakedObjects.Objects;

namespace NakedObjects.Core.Context {
    public class NakedObjectsData {
        private static readonly ILog Log;
        private static int nextId = 1;
        private readonly int id = nextId++;

        internal long accessTime;
        private IMessageBroker messageBroker = new SimpleMessageBroker();
        private INakedObjectPersistor objectPersistor;
        private ISession session;
        private IUpdateNotifier updateNotifier = new SimpleUpdateNotifier();


        static NakedObjectsData() {
            Log = LogManager.GetLogger(typeof (NakedObjectsData));
        }

        internal IMessageBroker MessageBroker {
            get { return messageBroker; }
            set { messageBroker = value; }
        }

        internal INakedObjectPersistor ObjectPersistor {
            get { return objectPersistor; }
            set { objectPersistor = value; }
        }

        internal ISession Session {
            get { return session; }
            set { session = value; }
        }

        internal IUpdateNotifier UpdateNotifier {
            get { return updateNotifier; }
            set { updateNotifier = value; }
        }

        public override string ToString() {
            var toString = new AsString(this);
            toString.Append("context", ExecutionContextId());
            toString.Append("objectPersistor", objectPersistor);
            toString.Append("session", session);
            toString.Append("messageBroker", messageBroker);
            return toString.ToString();
        }

        public virtual string ExecutionContextId() {
            return "#" + id + (session == null ? "" : "/" + session.UserName);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}