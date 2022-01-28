namespace AdventureWorks.NOF2.AppLib;

public class Menu : IMenu
{
    public Menu(string name) : this()
    {
        Name = name;
        
    }

    public Menu()
    {
        Name = "";
        Components = new List<IMenuComponent>();
    }

    public string Name { get; init; }

    internal List<IMenuComponent> Components { get; init; }

    public IList<IMenuComponent> MenuItems() => Components;

    public Menu AddAction(string name)
    {
        Components.Add(new MenuAction(name));
        return this;
    }

    public Menu AddSubMenu(string name)
    {
        var sub = new Menu(name);
        Components.Add(sub);
        return sub;
    }

}

