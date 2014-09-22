using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Security;
using NakedObjects.Core.Context;
using NakedObjects.Core.Reflect;
using NakedObjects.Objects;

namespace NakedObjects.Service {
    public class NakedObjectsFramework : INakedObjectsFramework {
        private readonly IAuthorizationManager authorizationManager;
        private readonly IContainerInjector injector;
        private readonly IMessageBroker messageBroker;
        private readonly INakedObjectPersistor objectPersistor;
        private readonly INakedObjectReflector reflector;
        private readonly ISession session;
        private readonly IUpdateNotifier updateNotifier;

        public NakedObjectsFramework(IMessageBroker messageBroker, IUpdateNotifier updateNotifier, ISession session, INakedObjectPersistor objectPersistor, INakedObjectReflector reflector, IAuthorizationManager authorizationManager, IContainerInjector injector) {
            this.messageBroker = messageBroker;
            this.updateNotifier = updateNotifier;
            this.session = session;
            this.objectPersistor = objectPersistor;
            this.reflector = reflector;
            this.authorizationManager = authorizationManager;
            this.injector = injector;
            injector.Framework = this;
        }

        #region INakedObjectsFramework Members

        public IContainerInjector Injector {
            get { return injector; }
        }

        public IMessageBroker MessageBroker {
            get { return messageBroker; }
        }

        public IUpdateNotifier UpdateNotifier {
            get { return updateNotifier; }
        }

        public ISession Session {
            get { return session; }
        }

        public INakedObjectPersistor ObjectPersistor {
            get { return objectPersistor; }
        }

        public INakedObjectManager Manager {
            get { return objectPersistor; }
        }
        public INakedObjectReflector Reflector {
            get { return reflector; }
        }

        public IAuthorizationManager AuthorizationManager {
            get { return authorizationManager; }
        }

        #endregion
    }
}