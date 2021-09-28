using NakedObjects;

// ReSharper disable InconsistentNaming

namespace Legacy.NakedObjects.Application {
    public interface TitledObject {
        [NakedObjectsIgnore]
        Title title();
    }
}
