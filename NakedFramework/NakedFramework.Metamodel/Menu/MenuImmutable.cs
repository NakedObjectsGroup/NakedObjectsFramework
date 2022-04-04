using System;
using System.Collections.Generic;
using NakedFramework.Architecture.Menu;

namespace NakedFramework.Metamodel.Menu;

[Serializable]
public class MenuImmutable : IMenuImmutable {
    public MenuImmutable(string name, string id, string grouping, IList<IMenuItemImmutable> menuItems) {
        Name = name;
        Id = id;
        Grouping = grouping;
        MenuItems = menuItems;
    }

    public string Name { get; }
    public string Id { get; }
    public string Grouping { get; }
    public IList<IMenuItemImmutable> MenuItems { get; }
}