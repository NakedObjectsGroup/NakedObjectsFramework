// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NakedObjects.Architecture;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Menu;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.SpecImmutable;

namespace NakedObjects.Meta.Menus {

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

        private ImmutableList<IMenuItemImmutable> items = ImmutableList<IMenuItemImmutable>.Empty;

        /// <summary>
        /// Will only be set if this menu is a sub-menu of another.
        /// </summary>
        public IMenu SuperMenu { get; private set; }

        public string Name { get; set; }

        //Includes both actions and sub-menus
        public IList<IMenuItemImmutable> MenuItems {
            get { return items; }
        }

        protected void AddMenuItem(IMenuItemImmutable item) {
            items = items.Add(item); //Only way to add to an immutable collection
        }

        protected bool HasAction(IActionSpecImmutable action) {
            bool nativeAction = MenuItems.OfType<MenuAction>().Any(mi => mi.Action == action);
            if (nativeAction) return true;
            return MenuItems.OfType<Menu>().Any(m => m.HasAction(action));
        }

        #endregion

        #region IMenu Members

        public IMenu AddActionFrom<TObject>(string actionName, string renamedTo = null) {
            Type serviceType = typeof (TObject);

            IActionSpecImmutable actionSpec = GetActionsForObject<TObject>().Where(a => a.Identifier.MemberName == actionName).FirstOrDefault();
            if (actionSpec == null) {
                throw new ReflectionException("No such action: " + actionName + " on " + serviceType);
            }
            AddMenuItem(new MenuAction(actionSpec, renamedTo));
            return this;
        }

        public IMenu AddAllRemainingActionsFrom<TObject>() {
            var actions = GetObjectSpec<TObject>().ObjectActions;
            AddOrderableElementsToMenu(actions, this);
            return this;
        }

        public IMenu CreateSubMenu(string subMenuName) {
            return CreateMenuImmutableAsSubMenu(subMenuName);
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

        #endregion

        protected IList<IActionSpecImmutable> GetActionsForObject<TObject>() {
            return GetObjectSpec<TObject>().ObjectActions.Select(x => x.Spec).ToList();
        }

        protected IObjectSpecImmutable GetObjectSpec<TObject>() {
            return metamodel.GetSpecification(typeof (TObject));
        }

        public void AddOrderableElementsToMenu(IList<IOrderableElement<IActionSpecImmutable>> ordeableElements, Menu toMenu) {
            foreach (var element in ordeableElements) {
                var action = element.Spec;
                if (action != null) {
                    if (!toMenu.HasAction(action)) {
                        toMenu.AddMenuItem(new MenuAction(action, null));
                    }
                } else if (!string.IsNullOrEmpty(element.GroupFullName)) { //i.e. sub-menu
                    var sub = CreateMenuImmutableAsSubMenu(element.GroupFullName);
                    AddOrderableElementsToMenu(element.Set, sub);
                }
            }
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
    }
}
