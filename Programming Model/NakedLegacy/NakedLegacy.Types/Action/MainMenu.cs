using System.Collections.Generic;

namespace NakedLegacy.Types {
    public class MainMenu : Menu, IMainMenu {
        private IList<IMenu> myMenuItems;

        public MainMenu() : this("") { }

        public MainMenu(string menuName)
            : base(menuName) =>
            myMenuItems = new List<IMenu>();

        public virtual IList<IMenu> Menus {
            get => myMenuItems;
            set => myMenuItems = value;
        }
    }
}