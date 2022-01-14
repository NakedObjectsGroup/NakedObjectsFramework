using System.Collections.Generic;

namespace NakedLegacy.Rest.Test.Data.AppLib; 

public class Menu : IMenu {
    private IList<IMenuComponent> Items = new List<IMenuComponent>();

    public Menu(string menuName) => Name = menuName;

    public string Name { get; set; }

    public IList<IMenuComponent> MenuItems() => Items;

    public void AddAction(string name) => Items.Add(new MenuAction(name));

    public Menu AddSubMenu(string name) {
        var sub = new Menu(name);
        Items.Add(sub);
        return sub;
    }
}