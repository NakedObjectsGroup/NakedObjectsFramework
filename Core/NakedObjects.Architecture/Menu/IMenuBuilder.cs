using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NakedObjects.Architecture.Menu {
    /// <summary>
    /// The application programmer needs to provide and register an implementation of this service to define the main menus
    /// </summary>
    public interface IMenuBuilder {

        /// <summary>
        /// Typically this will be implemented by delegating work to an injected IMenuFactory
        /// </summary>
        /// <returns></returns>
        IMenu[] DefineMainMenus();
    }
}
