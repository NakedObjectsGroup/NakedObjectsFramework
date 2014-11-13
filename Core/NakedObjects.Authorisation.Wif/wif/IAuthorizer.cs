using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;

namespace NakedObjects.Reflect.Security.Wif {
    public interface IAuthorizer {
        bool IsVisible(ISession session, INakedObject target, IIdentifier member);
        bool IsUsable(ISession session, INakedObject target, IIdentifier member);
    }
}