using System.Collections.Generic;

namespace NakedLegacy;

public interface IMainMenu {
    IList<IMenu> Menus { get; set; }
}