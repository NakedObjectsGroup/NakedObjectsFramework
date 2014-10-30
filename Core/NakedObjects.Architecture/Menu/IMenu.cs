using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NakedObjects.Architecture.Menu {
    //Implements IMenuItem in order to allow sub-menus to sit alongside simple actions
    public interface IMenu {
        //Adds specified action as the next menu item
        //Returns this menu (for fluent programming)
        IMenu AddAction<TService>(string actionName, string renamedTo = null);

        //Adds all actions from the service not previously added individually,
        //in the order they are specified in the service.
        //Returns this menu (for fluent programming)
        IMenu AddAllRemainingActionsFrom<TService>();

        //Returns the new menu, which will already have been added to the hosting menu
        IMenu CreateSubMenu(string subMenuName);

        //Returns this menu (for fluent programming)
        IMenu AddAsSubMenu(IMenu subMenu);

        string Name { get; }
    }
}
