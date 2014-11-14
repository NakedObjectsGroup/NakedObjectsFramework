using NakedObjects.Architecture.Spec;
using System.Collections.Generic;
namespace NakedObjects.Architecture.Menu {

    /// <summary>
    /// Extension of IMenuImmutable that provides methods for building the menu
    /// during reflection time.
    /// </summary>
    public interface IMenu : IMenuImmutable {

        //Adds specified action as the next menu item
        //Returns this menu (for fluent programming)
        IMenu AddActionFrom<TObject>(string actionName, string renamedTo = null);

        //Adds all actions from the service not previously added individually,
        //in the order they are specified in the service.
        //Returns this menu (for fluent programming)
        IMenu AddAllRemainingActionsFrom<TService>();

        //Returns the new menu, which will already have been added to the hosting menu
        IMenu CreateSubMenu(string subMenuName);
    }
}
