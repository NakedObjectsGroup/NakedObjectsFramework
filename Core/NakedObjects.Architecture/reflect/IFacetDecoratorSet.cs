using NakedObjects.Architecture.Facets;

namespace NakedObjects.Reflector.Spec {
    public interface IFacetDecoratorSet {
        bool IsEmpty { get; }
        void Add(IFacetDecorator decorator);
        void DecorateAllHoldersFacets(IFacetHolder holder);
    }
}