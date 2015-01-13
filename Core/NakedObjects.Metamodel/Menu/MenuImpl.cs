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
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Menu;

namespace NakedObjects.Meta.Menu {
    public class MenuImpl : IMenu, IMenuImmutable {
        #region Injected Services

        private readonly IMetamodel metamodel;

        #endregion

        public MenuImpl(IMetamodel metamodel, string name) {
            this.metamodel = metamodel;
            Name = name ?? "Undefined";
        }

        #region properties

        private ImmutableList<IMenuItemImmutable> items = ImmutableList<IMenuItemImmutable>.Empty;

        /// <summary>
        /// Will only be set if this menu is a sub-menu of another.
        /// </summary>
        public IMenu SuperMenu { get; private set; }

        /// <summary>
        /// The name of the menu -  will typically be rendered on the UI
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Id is optional.  It is only included to facilitate backwards compatibility with
        /// existing auto-generated menus.
        /// </summary>
        public string Id { get; set; }

        //Includes both actions and sub-menus
        public IList<IMenuItemImmutable> MenuItems {
            get { return items; }
        }

        protected IMetamodel Metamodel {
            get { return metamodel; }
        }

        protected void AddMenuItem(IMenuItemImmutable item) {
            items = items.Add(item); //Only way to add to an immutable collection
        }

        protected bool HasAction(IActionSpecImmutable action) {
            bool nativeAction = MenuItems.OfType<MenuAction>().Any(mi => mi.Action == action);
            if (nativeAction) return true;
            return MenuItems.OfType<MenuImpl>().Any(m => m.HasAction(action));
        }

        #endregion

        #region IMenu Members

        public IMenu WithMenuName(string name) {
            Name = name;
            return this;
        }

        public IMenu WithId(string id) {
            Id = id;
            return this;
        }

        public IMenu AddActionFrom<TObject>(string actionName, string renamedTo = null) {
            Type serviceType = typeof (TObject);

            IActionSpecImmutable actionSpec = GetActionsForObject<TObject>().FirstOrDefault(a => a.Identifier.MemberName == actionName);
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
            return CreateMenuImmutableAsSubMenu(subMenuName, null);
        }

        #endregion

        #region IMenuImmutable Members

        public IMenuActionImmutable GetAction(string actionName) {
            var action = MenuItems.OfType<MenuAction>().FirstOrDefault(a => a.Name == actionName);
            if (action == null) {
                throw new Exception("No action named " + actionName);
            }
            return action;
        }

        public IMenuImmutable GetSubMenu(string menuName) {
            var menu = GetSubMenuIfExists(menuName);
            if (menu == null) {
                throw new Exception("No sub-menu named " + menuName);
            }
            return menu;
        }

        #endregion

        protected MenuImpl GetSubMenuIfExists(string menuName) {
            return MenuItems.OfType<MenuImpl>().FirstOrDefault(a => a.Name == menuName);
        }

        protected IList<IActionSpecImmutable> GetActionsForObject<TObject>() {
            return GetObjectSpec<TObject>().ObjectActions.ToList();
        }

        protected IObjectSpecImmutable GetObjectSpec<TObject>() {
            return Metamodel.GetSpecification(typeof (TObject));
        }

        public void AddOrderableElementsToMenu(IList<IActionSpecImmutable> ordeableElements, MenuImpl toMenu) {
            foreach (var action in ordeableElements) {
                if (action != null) {
                    if (!toMenu.HasAction(action)) {
                        toMenu.AddMenuItem(new MenuAction(action));
                    }
                }
            }
        }

        protected MenuImpl CreateMenuImmutableAsSubMenu(string subMenuName, string id) {
            var subMenu = new MenuImpl(Metamodel, subMenuName);
            subMenu.Id = id;
            AddAsSubMenu(subMenu);
            return subMenu;
        }

        public MenuImpl AddAsSubMenu(MenuImpl subMenu) {
            AddMenuItem(subMenu);
            subMenu.SuperMenu = this;
            return this;
        }
    }
}