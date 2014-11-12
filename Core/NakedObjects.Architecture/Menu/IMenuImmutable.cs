using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NakedObjects.Architecture.Menu {
    public interface IMenuImmutable : IMenuItemImmutable {
        IList<IMenuItemImmutable> MenuItems { get; }

        IMenuActionImmutable GetAction(string actionName);

        IMenuImmutable GetSubMenu(string menuName);
    }
}
