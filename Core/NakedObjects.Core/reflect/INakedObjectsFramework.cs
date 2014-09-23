using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Security;
using NakedObjects.Core.Context;
using NakedObjects.Core.Reflect;
using NakedObjects.Objects;

namespace NakedObjects {
    /// <summary>
    /// Defines a service that provides easy access to the principal components of the framework.
    /// An implementation of this service interface will be injected into any domain
    /// object that needs it.
    /// </summary>
    public interface INakedObjectsFramework {
        IMessageBroker MessageBroker { get; }

        IUpdateNotifier UpdateNotifier { get; }

        ISession Session { get; }

        ILifecycleManager LifecycleManager { get; }

        INakedObjectManager Manager { get; }

        INakedObjectReflector Reflector { get; }

        IAuthorizationManager AuthorizationManager { get; }

        IContainerInjector Injector { get; }
    }
}