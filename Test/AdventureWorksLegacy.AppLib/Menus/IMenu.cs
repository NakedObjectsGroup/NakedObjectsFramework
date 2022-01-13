namespace AdventureWorksLegacy.AppLib.Menus;

public interface IMenu : IMenuComponent
{
    IList<IMenuComponent> MenuItems();
}

