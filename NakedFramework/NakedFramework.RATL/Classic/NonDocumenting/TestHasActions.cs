using System.Text;
using NakedFramework.RATL.Classic.Interface;
using ROSI.Apis;
using ROSI.Records;

namespace NakedFramework.RATL.Classic.NonDocumenting;

internal abstract class TestHasActions : ITestHasActions {
    protected TestHasActions(DomainObject domainObject) => DomainObject = domainObject;

    protected DomainObject DomainObject { get; }

    public ITestAction[] Actions => DomainObject.GetActions().Select(a => new TestAction(a)).Cast<ITestAction>().ToArray();

    public ITestAction GetAction(string actionName) {
        var actions = Actions.Where(x => x.Name == actionName && string.IsNullOrEmpty(x.SubMenu)).ToArray();
        AssertErrors(actions, actionName);
        return actions.Single();
    }

    public ITestAction GetAction(string actionName, params Type[] parameterTypes) {
        var actions = Actions.Where(x => x.Name == actionName && string.IsNullOrEmpty(x.SubMenu) && x.MatchParameters(parameterTypes)).ToArray();
        AssertErrors(actions, actionName, " (with specified parameters)");
        return actions.Single();
    }

    public ITestAction GetAction(string actionName, string subMenu) {
        var actions = Actions.Where(x => x.Name == actionName && x.SubMenu == subMenu).ToArray();
        AssertErrors(actions, actionName, $" within sub-menu '{subMenu}'");
        return actions.Single();
    }

    public ITestAction GetAction(string actionName, string subMenu, params Type[] parameterTypes) {
        var actions = Actions.Where(x => x.Name == actionName && x.SubMenu == subMenu && x.MatchParameters(parameterTypes)).ToArray();
        AssertErrors(actions, actionName, $" (with specified parameters) within sub-menu '{subMenu}'");
        return actions.Single();
    }

    public virtual string GetObjectActionOrder() {
        var order = new StringBuilder();
        order.Append(AppendMenus(CreateMenuItems(Actions)));
        return order.ToString();
    }

    public ITestHasActions AssertActionOrderIs(string order) {
        Assert.AreEqual(order, GetObjectActionOrder());
        return this;
    }

    public abstract string Title { get; }

    private static void AssertErrors(ITestAction[] actions, string actionName, string condition = "") {
        if (!actions.Any()) {
            Assert.Fail($"No Action named '{actionName}'{condition}");
        }

        if (actions.Count() > 1) {
            Assert.Fail($"{actions.Count()} Actions named '{actionName}' found{condition}");
        }
    }

    private static string AppendMenus(Menu[] menus) {
        var order = new StringBuilder();

        for (var i = 0; i < menus.Length; i++) {
            var menu = menus[i];
            var name = menu.Name;
            order.Append(string.IsNullOrEmpty(name) ? "" : $"({name}:");

            if (menu.Actions.Any()) {
                var names = string.Join(", ", menu.Actions.Select(a => a.Name));
                order.Append(names);
                if (menu.Menus.Any()) {
                    order.Append(", ");
                }
            }

            if (menu.Menus.Any()) {
                order.Append($"{AppendMenus(menu.Menus)}");
            }

            order.Append(string.IsNullOrEmpty(name) ? "" : ")");

            order.Append(i < menus.Length - 1 ? ", " : "");
        }

        return order.ToString();
    }

    private static string GetMenuNameForLevel(string menuPath, int level) {
        var menu = "";

        if (!string.IsNullOrEmpty(menuPath)) {
            var menus = menuPath.Split('_');

            if (menus.Length > level) {
                menu = menus[level];
            }
        }

        return menu;
    }

    private static IEnumerable<(string, ITestAction)> RemoveDuplicateMenuNames(IEnumerable<(string, ITestAction)> menus) => menus.DistinctBy(a => a.Item1);

    private static Menu CreateSubmenuItems(ITestAction[] actions, (string name, ITestAction action) menuSlot, int level) {
        // if not root menu aggregate all actions with same name
        ITestAction[] menuActions = null;
        Menu[] menuItems = null;

        menuActions = actions.Where(a => GetMenuNameForLevel(a.SubMenu, level) == menuSlot.name && string.IsNullOrEmpty(GetMenuNameForLevel(a.SubMenu, level + 1))).ToArray();

        // then collate submenus

        var submenuActions = actions.Where(a => GetMenuNameForLevel(a.SubMenu, level) == menuSlot.name && !string.IsNullOrEmpty(GetMenuNameForLevel(a.SubMenu, level + 1))).ToArray();

        var menuSubSlots = submenuActions.Select(a => (GetMenuNameForLevel(a.SubMenu, level + 1), a)).ToArray();
        menuSubSlots = RemoveDuplicateMenuNames(menuSubSlots).ToArray();

        menuItems = menuSubSlots.Select(slot => CreateSubmenuItems(submenuActions, slot, level + 1)).ToArray();

        return new Menu(menuSlot.name, menuActions, menuItems);
    }

    private static Menu[] CreateMenuItems(ITestAction[] actions) {
        // first create a top level menu for each action
        // note at top level we leave 'un-menued' actions
        // use an anonymous object locally so we can construct objects with readonly properties

        var menuSlots = actions.Select(a => (GetMenuNameForLevel(a.SubMenu, 0), a));

        // remove non unique submenus
        menuSlots = RemoveDuplicateMenuNames(menuSlots);

        // update submenus with all actions under same submenu
        return menuSlots.Select(slot => CreateSubmenuItems(actions, slot, 0)).OrderBy(ms => ms.Name).ToArray();
    }

    private record Menu(string Name, ITestAction[] Actions, Menu[] Menus);
}