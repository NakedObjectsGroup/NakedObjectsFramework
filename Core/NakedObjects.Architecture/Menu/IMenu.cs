using NakedObjects.Architecture.Spec;
using System.Collections.Generic;
namespace NakedObjects.Architecture.Menu {

    //IMenu is to IMenuImmutable, as IObjectSpec is to IObjectSpecImmutable -  the runtime equivalent with injected services
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
