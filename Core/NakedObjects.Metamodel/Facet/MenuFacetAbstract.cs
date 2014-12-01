using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Menu;
using NakedObjects.Architecture.Spec;
using NakedObjects.Meta.Menu;
using NakedObjects.Meta.SpecImmutable;
using NakedObjects.Util;
using System.Reflection;

namespace NakedObjects.Meta.Facet {
    public abstract class MenuFacetAbstract : FacetAbstract, IMenuFacet {

        protected ObjectSpecImmutable Spec() {
            return (ObjectSpecImmutable)Specification;
        }

        protected static string GetMenuName(ObjectSpecImmutable spec) {
            if (spec.Service) {
                return spec.GetFacet<INamedFacet>().Value ?? NameUtils.NaturalName(spec.ShortName);
            } else {
                return "Actions";
            }
        }

        protected MenuImpl menu;

        public MenuFacetAbstract(ISpecification holder)
            : base(typeof(IMenuFacet), holder) {
                menu = null;
        }

        public IMenuImmutable GetMenu() {
            return menu;
        }

        public abstract void CreateMenu(IMetamodelBuilder metamodel);
    }
}
