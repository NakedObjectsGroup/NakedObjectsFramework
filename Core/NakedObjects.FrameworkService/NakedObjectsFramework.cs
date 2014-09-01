using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Security;
using NakedObjects.Core.Context;
using NakedObjects.Core.Reflect;
using NakedObjects.Objects;

namespace NakedObjects.Service {
    public class NakedObjectsFramework : INakedObjectsFramework {
        private readonly IMessageBroker messageBroker;
        private readonly IUpdateNotifier updateNotifier;
        private readonly ISession session;
        private readonly INakedObjectPersistor objectPersistor;
        private readonly INakedObjectReflector reflector;
        private readonly IAuthorizationManager authorizationManager;

        public NakedObjectsFramework(IMessageBroker messageBroker, IUpdateNotifier updateNotifier, ISession session, INakedObjectPersistor objectPersistor, INakedObjectReflector reflector, IAuthorizationManager authorizationManager, IContainerInjector injector) {
            this.messageBroker = messageBroker;
            this.updateNotifier = updateNotifier;
            this.session = session;
            this.objectPersistor = objectPersistor;
            this.reflector = reflector;
            this.authorizationManager = authorizationManager;
            injector.Framework = this;
        }

        #region INakedObjectsFramework Members

        public IMessageBroker MessageBroker {
            get { return this.messageBroker; }
        }

        public IUpdateNotifier UpdateNotifier {
            get { return this.updateNotifier; }
        }

        public ISession Session {
            get { return this.session; }
        }

        public INakedObjectPersistor ObjectPersistor {
            get { return this.objectPersistor; }
        }

        public INakedObjectReflector Reflector {
            get { return this.reflector; }
        }

        public IAuthorizationManager AuthorizationManager {
            get { return this.authorizationManager; }
        }

        #endregion
    }
}