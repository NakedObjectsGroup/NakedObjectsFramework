using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Security;
using NakedObjects.Core.Context;
using NakedObjects.Objects;

namespace NakedObjects {
    public interface INakedObjectsFramework {
        IMessageBroker MessageBroker { get; }

        IUpdateNotifier UpdateNotifier { get; }

        ISession Session { get; }

        INakedObjectPersistor ObjectPersistor { get; }

        INakedObjectReflector Reflector { get; }

        IAuthorizationManager AuthorizationManager { get; }
    }
}