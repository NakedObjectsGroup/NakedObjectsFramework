using System;
using System.Linq;
using System.Reflection;
using NakedFramework.Architecture.Component;
using NakedFramework.Metamodel.Menu;

namespace NakedLegacy.Reflector.Helpers;

public static class LegacyHelpers {
    public static Type IsOrImplementsValueHolder(Type type) =>
        type switch {
            null => null,
            _ when type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IValueHolder<>) => type.GetGenericArguments().Single(),
            _ => type.GetInterfaces().Select(IsOrImplementsValueHolder).FirstOrDefault(t => t is not null)
        };

    private static void AddMenuComponent(NakedFramework.Menu.IMenu topLevelMenu, IMenuComponent menuComponent, Type declaringType) {
        switch (menuComponent) {
            case IMenu menu:
                var subMenu = topLevelMenu.CreateSubMenu(menu.Name);

                foreach (var component in menu.MenuItems()) {
                    AddMenuComponent(subMenu, component, declaringType);
                }

                break;
            case IMenuAction menuAction:
                topLevelMenu.AddAction(menuAction.Name);
                break;
        }
    }

    public static MenuImpl ConvertLegacyToNOFMenu(IMenu legacyMenu, IMetamodelBuilder metamodel, Type declaringType) {
        var mi = new MenuImpl(metamodel, declaringType, false, legacyMenu.Name);
        foreach (var menuComponent in legacyMenu.MenuItems()) {
            AddMenuComponent(mi, menuComponent, declaringType);
        }

        return mi;
    }
}