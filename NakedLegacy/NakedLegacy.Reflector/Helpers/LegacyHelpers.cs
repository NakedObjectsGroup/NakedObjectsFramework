using System;
using System.Linq;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
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
                topLevelMenu.AddAction(menuAction.Name, true);
                break;
        }
    }

    private static string GetName(IMenu legacyMenu, IMetamodelBuilder metamodel, Type declaringType, string name) {
        if (!string.IsNullOrWhiteSpace(name)) {
            return name;
        }

        var spec = metamodel.GetSpecification(declaringType);
        return spec?.GetFacet<INamedFacet>()?.FriendlyName ?? "";
    }

    public static MenuImpl ConvertLegacyToNOFMenu(IMenu legacyMenu, IMetamodelBuilder metamodel, Type declaringType, string name) {
        var menuName = GetName(legacyMenu, metamodel, declaringType, name);
        var mi = new MenuImpl(metamodel, declaringType, false, menuName);
        foreach (var menuComponent in legacyMenu.MenuItems()) {
            AddMenuComponent(mi, menuComponent, declaringType);
        }

        return mi;
    }
}