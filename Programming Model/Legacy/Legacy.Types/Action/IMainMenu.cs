

using System.Collections.Generic;

namespace Legacy.Types {
    public interface IMainMenu {
        IList<IMenu> Menus { get; set; }
    }
}