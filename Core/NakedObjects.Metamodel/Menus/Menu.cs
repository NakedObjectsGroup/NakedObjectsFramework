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
    public class Menu : IMenu, IMenuItem {
        #region Injected Services
        protected readonly IMetamodelBuilder metamodel; 

        #endregion

        public Menu(IMetamodelBuilder metamodel, string name) {
            this.metamodel = metamodel;
            this.MenuItems = ImmutableList<IMenuItem>.Empty;
            this.Name = name;
            if (name == null) {
                this.Name = "Undefined";
            }
        }

        #region properties
        public string Name { get; set; }
        //Includes both actions and sub-menus
        public ImmutableList<IMenuItem> MenuItems { get; set; }

        protected void AddMenuItem(IMenuItem item) {
            MenuItems = MenuItems.Add(item); //Only way to add to an immutable collection
        }

        #endregion

        public IMenu AddAction<TService>(string actionName, string renamedTo = null) {
            Type serviceType = typeof(TService);

            IActionSpecImmutable actionSpec = GetActionsForService<TService>().Where(a => a.Identifier.MemberName == actionName).FirstOrDefault();
            if (actionSpec == null) {
                throw new ReflectionException("No such action: " + actionName + " on " + serviceType);
            }
            AddMenuItem(new MenuAction(actionSpec, renamedTo));
            return this;
        }

        protected IList<IActionSpecImmutable> GetActionsForService<TService>() {
            return GetObjectSpec<TService>().ObjectActions.Select(x => x.Spec).ToList();
        }

        protected IObjectSpecImmutable GetObjectSpec<TService>() {
            return metamodel.GetSpecification(typeof(TService));
        }

        public IMenu AddAllRemainingActionsFrom<TService>() {
            var actions = GetActionsForService<TService>();
            //TODO: Will this add them in the correct order?
            foreach (var action in actions) {
                if (!MenuItems.OfType<MenuAction>().Any(mi => mi.ActionSpec == action)) {
                    AddMenuItem(new MenuAction(action, null));
                }
            }
            return this;
        }

        public IMenu CreateSubMenu(string subMenuName) {
            var sub = new Menu(metamodel, subMenuName);
            this.AddAsSubMenu(sub);
            return sub;
        }

        public IMenu AddAsSubMenu(IMenu subMenu) {          
            AddMenuItem(AsMenu(subMenu));
            return this;
        }

        private static Menu AsMenu(IMenu menu) {
            if (!(menu is Menu)) {
                throw new ReflectionException("Using an unrecognised implementation of IMenu");
            }
            return menu as Menu;
        }



    }
}
