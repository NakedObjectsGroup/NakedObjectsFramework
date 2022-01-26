using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Metamodel.Menu;
using NakedLegacy.Container;
using NakedLegacy.Menu;
using NakedLegacy.Reflector.Component;
using NakedLegacy.ValueHolder;

namespace NakedLegacy.Reflector.Helpers;

public static class LegacyHelpers {
    public const string AboutPrefix = "about";

    public static Type IsOrImplementsValueHolder(Type type) =>
        type switch {
            null => null,
            _ when type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IValueHolder<>) => type.GetGenericArguments().Single(),
            _ => type.GetInterfaces().Select(IsOrImplementsValueHolder).FirstOrDefault(t => t is not null)
        };

    public static object[] SubstituteNulls(object[] parameters, MethodInfo method) {
        for (var index = 0; index < parameters.Length; index++) {
            if (parameters[index] is null) {
                var parmType = method.GetParameters()[index].ParameterType;
                parameters[index] = Activator.CreateInstance(parmType);
            }
        }

        return parameters;
    }

    public static object[] SubstituteNullsAndContainer(object[] parameters, MethodInfo method, INakedFramework framework) {
        parameters = InjectContainer(parameters, method, framework);
        return SubstituteNulls(parameters, method);
    }

    public static object[] InjectContainer(object[] parameters, MethodInfo method, INakedFramework framework) {
        var lastIndex = parameters.Length - 1;

        if (lastIndex >= 0 && parameters[lastIndex] is null) {
            if (method.GetParameters()[lastIndex].ParameterType.IsAssignableTo(typeof(IContainer))) {
                parameters[lastIndex] = new Component.Container(framework);
            }
        }

        return parameters;
    }

    private static void AddMenuComponent(NakedFramework.Menu.IMenu topLevelMenu, IMenuComponent menuComponent, Type declaringType) {
        switch (menuComponent) {
            case IMenu menu:
                var subMenu = topLevelMenu.CreateSubMenu(menu.Name);

                foreach (var component in menu.MenuItems()) {
                    AddMenuComponent(subMenu, component, declaringType);
                }

                break;
            case IMenuAction menuAction:
                topLevelMenu.AddAction(menuAction.Name, menuAction.DisplayName, true);
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

    public static IEnumerable<System.Attribute> GetCustomAttributes(object onObject) =>
        onObject switch
        {
            MemberInfo mi => mi.GetCustomAttributes(),
            ParameterInfo pi => pi.GetCustomAttributes(),
            _ => new System.Attribute[] { }
        };

    public static T GetCustomAttribute<T>(this object on) => GetCustomAttributes(on).OfType<T>().FirstOrDefault();
}