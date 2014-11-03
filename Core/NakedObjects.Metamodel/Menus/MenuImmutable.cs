using NakedObjects.Architecture;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Menu;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.SpecImmutable;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NakedObjects.Meta.Menus {
    //Implements IMenuItem to permit sub-menus
    public class MenuImmutable : IMenu, IMenuItem {
        #region Injected Services
        protected readonly IMetamodelBuilder metamodel;

        #endregion

        public MenuImmutable(IMetamodelBuilder metamodel, string name) {
            this.metamodel = metamodel;
            this.Name = name;
            if (name == null) {
                this.Name = "Undefined";
            }
        }

        #region properties
        public string Name { get; set; }

        private ImmutableList<IMenuItem> items = ImmutableList<IMenuItem>.Empty;
        //Includes both actions and sub-menus
        public IList<IMenuItem> MenuItems { get { return items; } }

        protected void AddMenuItem(IMenuItem item) {
            if (items.Any(i => i.Name == item.Name)) {
                throw new ReflectionException("Menu already contains an item named: "+item.Name);
            }
            items = items.Add(item); //Only way to add to an immutable collection
        }

        #endregion

        public IMenu AddAction<TService>(string actionName, string renamedTo = null) {
            Type serviceType = typeof(TService);

            IActionSpecImmutable actionSpec = GetActionsForService<TService>().Where(a => a.Identifier.MemberName == actionName).FirstOrDefault();
            if (actionSpec == null) {
                throw new ReflectionException("No such action: " + actionName + " on " + serviceType);
            }
            AddMenuItem(new MenuActionImmutable(actionSpec, renamedTo));
            return this;
        }

        protected IList<IActionSpecImmutable> GetActionsForService<TService>() {
            return GetObjectSpec<TService>().ObjectActions.Select(x => x.Spec).ToList();
        }

        protected IObjectSpecImmutable GetObjectSpec<TService>() {
            return metamodel.GetSpecification(typeof(TService));
        }

        public IMenu AddAllRemainingActionsFrom<TService>() {
            var ordeableElements = GetObjectSpec<TService>().ObjectActions;
            AddOrderableElementsToMenu(ordeableElements, this);
            return this;
        }

        protected  void AddOrderableElementsToMenu(IList<IOrderableElement<IActionSpecImmutable>> ordeableElements, MenuImmutable toMenu) {
            foreach (var element in ordeableElements) {
                var action = element.Spec;
                if (action != null && !toMenu.MenuItems.OfType<MenuActionImmutable>().Any(mi => mi.Action == action)) {
                    toMenu.AddMenuItem(new MenuActionImmutable(action, null));
                }
                else if (element.GroupFullName != null) { //i.e. sub-menu
                    var sub = CreateMenuImmutableAsSubMenu(element.GroupFullName);
                    AddOrderableElementsToMenu(element.Set, sub);
                }
            }
        }

        public IMenu CreateSubMenu(string subMenuName) {
            return CreateMenuImmutableAsSubMenu(subMenuName);
        }

        private MenuImmutable CreateMenuImmutableAsSubMenu(string subMenuName) {
            var sub = new MenuImmutable(metamodel, subMenuName);
            this.AddAsSubMenu(sub);
            return sub;
        }

        public IMenu AddAsSubMenu(IMenu subMenu) {
            AddMenuItem(AsMenu(subMenu));
            return this;
        }

        private static MenuImmutable AsMenu(IMenu menu) {
            if (!(menu is MenuImmutable)) {
                throw new ReflectionException("Using an unrecognised implementation of IMenu");
            }
            return menu as MenuImmutable;
        }



    }
}
