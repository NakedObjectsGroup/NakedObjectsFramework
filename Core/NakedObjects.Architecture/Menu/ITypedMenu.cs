using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NakedObjects.Architecture.Menu {
    /// <summary>
    /// An easy way to build a menu based on the actions of a single object (or service).
    /// You can still add actions, or sub-menus, from other types to it.
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    public interface ITypedMenu<TObject> : IMenu {

        //Returns this menu (for fluent programming)
        ITypedMenu<TObject> AddAction(string actionName, string renamedTo = null);

        //Adds all actions from the service not previously added individually
        //Returns this menu (for fluent programming)
        ITypedMenu<TObject> AddAllRemainingActions();

        //Returns the new menu, which will already have been added to the hosting menu
        ITypedMenu<TObject> CreateSubMenuOfSameType(string subMenuName);
    }
}
