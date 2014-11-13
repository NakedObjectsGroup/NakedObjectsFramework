using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NakedObjects.Architecture.Menu {
    /// <summary>
    /// Runtime metamodel of a menu item, which might be an action or a sub-menu
    /// </summary>
    public interface IMenuItemImmutable {
        string Name { get; }
    }
}
