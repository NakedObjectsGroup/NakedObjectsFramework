

using NakedFramework.Menu;
using System.Collections.Generic;

namespace NakedLegacy.Types {
    public interface IMainMenu {
        IList<IMenu> Menus { get; set; }
    }
}