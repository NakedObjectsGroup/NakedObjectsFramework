using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NakedObjects.Meta.Menus {
    public class TypedMenuImmutable<TService> : MenuImmutable, ITypedMenu<TService> {

        public TypedMenuImmutable(IMetamodelBuilder metamodel, bool addAllActions, string name)
            : base(metamodel, name) {
            if (name == null) {
                this.Name = GetFriendlyNameForService();
            }
            if (addAllActions) {
                AddAllRemainingActions();
            }
        }

        private string GetFriendlyNameForService() {
            var spec = GetObjectSpec<TService>();
            return spec.GetFacet<INamedFacet>().Value ?? spec.ShortName;
        }

        public ITypedMenu<TService> AddAction(string actionName, string renamedTo = null) {
            AddAction<TService>(actionName, renamedTo);
            return this;
        }

        public ITypedMenu<TService> AddAllRemainingActions() {
            AddAllRemainingActionsFrom<TService>();
            return this;
        }

        public ITypedMenu<TService> CreateSubMenuOfSameType(string subMenuName) {
            var sub = new TypedMenuImmutable<TService>(metamodel, false, subMenuName);
            this.MenuItems.Add(sub);
            return sub;
        }
    }
}
