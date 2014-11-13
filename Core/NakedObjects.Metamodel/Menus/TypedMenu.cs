using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NakedObjects.Meta.Menus {

    public class TypedMenu<TObject> : Menu, ITypedMenu<TObject> {

        public TypedMenu(IMetamodel metamodel, bool addAllActions, string name)
            : base(metamodel, name) {
            if (name == null) {
                this.Name = GetFriendlyNameForObject();
            }
            if (addAllActions) {
                AddAllRemainingActions();
            }
        }

        private string GetFriendlyNameForObject() {
            var spec = GetObjectSpec<TObject>();
            return spec.GetFacet<INamedFacet>().Value ?? spec.ShortName;
        }

        public ITypedMenu<TObject> AddAction(string actionName, string renamedTo = null) {
            AddActionFrom<TObject>(actionName, renamedTo);
            return this;
        }

        public ITypedMenu<TObject> AddAllRemainingActions() {
            AddAllRemainingActionsFrom<TObject>();
            return this;
        }

        public ITypedMenu<TObject> CreateSubMenuOfSameType(string subMenuName) {
            var sub = new TypedMenu<TObject>(metamodel, false, subMenuName);
            this.MenuItems.Add(sub);
            return sub;
        }
    }
}
