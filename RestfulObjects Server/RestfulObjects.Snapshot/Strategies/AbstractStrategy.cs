using RestfulObjects.Snapshot.Representations;
using RestfulObjects.Snapshot.Utility;

namespace RestfulObjects.Snapshot.Strategies {
    public abstract class AbstractStrategy {
        protected AbstractStrategy(RestControlFlags flags) {
            Flags = flags;
        }

        public RestControlFlags Flags { get; private set; }

        public MapRepresentation GetExtensions() {
            return Flags.SimpleDomainModel ? GetExtensionsForSimple() : MapRepresentation.Create();
        }

        protected abstract MapRepresentation GetExtensionsForSimple();
    }
}