namespace AdventureWorksLegacy.AppLib.Menus;

public class MenuAction : IMenuAction  {
    public string Name { get; set; }
    public MenuAction(string name) => Name = name;
}