using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NakedObjects.Architecture.Menu {
    /// <summary>
    /// The application programmer needs to provide and register an implementation of this service to define the main menus
    /// </summary>
    public interface IMainMenuDefinition {

        /// <summary>
        /// Constructs the IMenus using methods on the factory passed in
        /// </summary>
        /// <returns></returns>
        IMenu[] MainMenus(IMenuFactory factory);
    }
}
