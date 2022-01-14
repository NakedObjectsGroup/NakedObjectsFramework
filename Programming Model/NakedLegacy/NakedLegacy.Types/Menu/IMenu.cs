using System.Collections.Generic;

namespace NakedLegacy;

public interface IMenu : IMenuComponent {
    IList<IMenuComponent> MenuItems();
}