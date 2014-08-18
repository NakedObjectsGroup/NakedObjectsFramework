using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Security;
using NakedObjects.Core.Context;
using NakedObjects.Objects;

namespace NakedObjects.Service {
    public class NakedObjectsFramework : INakedObjectsFramework {
        #region INakedObjectsFramework Members

        public IMessageBroker MessageBroker {
            get { return NakedObjectsContext.MessageBroker; }
        }

        public IUpdateNotifier UpdateNotifier {
            get { return NakedObjectsContext.UpdateNotifier; }
        }

        public ISession Session {
            get { return NakedObjectsContext.Session; }
        }

        public INakedObjectPersistor ObjectPersistor {
            get { return NakedObjectsContext.ObjectPersistor; }
        }

        public INakedObjectReflector Reflector {
            get { return NakedObjectsContext.Reflector; }
        }

        public IAuthorizationManager AuthorizationManager {
            get { return NakedObjectsContext.AuthorizationManager; }
        }

        #endregion
    }
}