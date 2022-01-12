namespace NakedLegacy;

public class Menu : IMenu {
    public Menu(string name) => Name = name;

    public string Name { get; set; }
}