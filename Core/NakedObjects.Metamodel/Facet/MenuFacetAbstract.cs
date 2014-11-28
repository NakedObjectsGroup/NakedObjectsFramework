using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Menu;
using NakedObjects.Architecture.Spec;
using NakedObjects.Meta.Menus;
using System.Reflection;

namespace NakedObjects.Meta.Facet {
    public abstract class MenuFacetAbstract : FacetAbstract, IMenuFacet {

        protected const string ObjectMenuName = "Actions";

        protected IMenu menu;

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
