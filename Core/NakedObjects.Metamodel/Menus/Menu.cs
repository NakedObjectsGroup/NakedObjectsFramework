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
    public class Menu : IMenu {
        #region Injected Services
        protected readonly IMetamodel metamodel;

        #endregion

        public Menu(IMetamodel metamodel, string name) {
            this.metamodel = metamodel;
            this.Name = name;
            if (name == null) {
                this.Name = "Undefined";
            }
        }

        #region properties
        public string Name { get; set; }

        /// <summary>
        /// Will only be set if this menu is a sub-menu of another.
        /// </summary>
        public IMenu SuperMenu { get; private set; }

        private ImmutableList<IMenuItemImmutable> items = ImmutableList<IMenuItemImmutable>.Empty;
        //Includes both actions and sub-menus
        public IList<IMenuItemImmutable> MenuItems { get { return items; } }

        protected void AddMenuItem(IMenuItemImmutable item) {
            items = items.Add(item); //Only way to add to an immutable collection
        }

        #endregion

        public IMenu AddActionFrom<TObject>(string actionName, string renamedTo = null) {
            Type serviceType = typeof(TObject);

            IActionSpecImmutable actionSpec = GetActionsForObject<TObject>().Where(a => a.Identifier.MemberName == actionName).FirstOrDefault();
            if (actionSpec == null) {
                throw new ReflectionException("No such action: " + actionName + " on " + serviceType);
            }
            AddMenuItem(new MenuAction(actionSpec, renamedTo));
            return this;
        }

        protected IList<IActionSpecImmutable> GetActionsForObject<TObject>() {
            return GetObjectSpec<TObject>().ObjectActions.Select(x => x.Spec).ToList();
        }

        protected IObjectSpecImmutable GetObjectSpec<TObject>() {
            return metamodel.GetSpecification(typeof(TObject));
        }

        public IMenu AddAllRemainingActionsFrom<TObject>() {
            var ordeableElements = GetObjectSpec<TObject>().ObjectActions;
            AddOrderableElementsToMenu(ordeableElements, this);
            return this;
        }

        public void AddOrderableElementsToMenu(IList<IOrderableElement<IActionSpecImmutable>> ordeableElements, Menu toMenu) {
            foreach (var element in ordeableElements) {
                var action = element.Spec;
                if (action != null && !toMenu.MenuItems.OfType<MenuAction>().Any(mi => mi.Action == action)) {
                    toMenu.AddMenuItem(new MenuAction(action, null));
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

        private Menu CreateMenuImmutableAsSubMenu(string subMenuName) {
            var subMenu = new Menu(metamodel, subMenuName);
            this.AddAsSubMenu(subMenu);
            return subMenu;
        }

        public Menu AddAsSubMenu(Menu subMenu) {
            AddMenuItem(subMenu);
            subMenu.SuperMenu = this;
            return this;
        }

        public IMenuActionImmutable GetAction(string actionName) {
            var action = MenuItems.OfType<MenuAction>().FirstOrDefault(a => a.Name == actionName);
            if (action == null) {
                throw new Exception("No action named " + actionName);
            }
            return action;
        }

        public IMenuImmutable GetSubMenu(string menuName) {
            var menu = MenuItems.OfType<Menu>().FirstOrDefault(a => a.Name == menuName);
            if (menu == null) {
                throw new Exception("No sub-menu named " + menuName);
            }
            return menu;
        }
    }
}
