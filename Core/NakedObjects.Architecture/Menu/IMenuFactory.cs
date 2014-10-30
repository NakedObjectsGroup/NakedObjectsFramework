using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NakedObjects.Architecture.Menu {
    public interface IMenuFactory {
        //Creates an empty, un-typed menu, (for which a name must be specified).
        IMenu NewMenu(string name);

        //Creates a new menu based on a service of type T. If the optional name
        //parameter is not specified, then the menu takes its name from the service.
        ITypedMenu<T> NewMenu<T>(bool addAllActions, string name = null);
    }
}
