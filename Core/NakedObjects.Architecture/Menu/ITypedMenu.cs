using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NakedObjects.Architecture.Menu {
    public interface ITypedMenu<TService> : IMenu {

        //Returns this menu (for fluent programming)
        ITypedMenu<TService> AddAction(string actionName, string renamedTo = null);

        //Adds all actions from the service not previously added individually
        //Returns this menu (for fluent programming)
        ITypedMenu<TService> AddAllRemainingActions();

        //Returns the new menu, which will already have been added to the hosting menu
        ITypedMenu<TService> CreateSubMenuOfSameType(string subMenuName);
    }
}
