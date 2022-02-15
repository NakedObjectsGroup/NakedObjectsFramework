// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Metamodel.Menu;
using NOF2.Container;
using NOF2.Menu;
using NOF2.ValueHolder;
using IMenu = NakedFramework.Menu.IMenu;

namespace NOF2.Reflector.Helpers;

public static class NOF2Helpers {
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
                if (IsOrImplementsValueHolder(parmType) is not null) {
                    parameters[index] = Activator.CreateInstance(parmType);
                }
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

    private static void AddMenuComponent(IMenu topLevelMenu, IMenuComponent menuComponent) {
        switch (menuComponent) {
            case Menu.IMenu menu:
                var subMenu = topLevelMenu.CreateSubMenu(menu.Name);

                foreach (var component in menu.MenuItems()) {
                    AddMenuComponent(subMenu, component);
                }

                break;
            case IMenuAction menuAction:
                topLevelMenu.AddAction(menuAction.Name, menuAction.DisplayName, true);
                break;
        }
    }

    private static string GetName(IMetamodelBuilder metamodel, Type declaringType, string name) {
        if (!string.IsNullOrWhiteSpace(name)) {
            return name;
        }

        var spec = metamodel.GetSpecification(declaringType);
        return spec?.GetFacet<INamedFacet>()?.FriendlyName ?? "";
    }

    public static MenuImpl ConvertNOF2ToNOFMenu(Menu.IMenu legacyMenu, IMetamodelBuilder metamodel, Type declaringType, string name) {
        var menuName = GetName(metamodel, declaringType, name);
        var mi = new MenuImpl(metamodel, declaringType, false, menuName);
        foreach (var menuComponent in legacyMenu.MenuItems()) {
            AddMenuComponent(mi, menuComponent);
        }

        return mi;
    }

    private static IEnumerable<System.Attribute> GetCustomAttributes(object onObject) =>
        onObject switch {
            MemberInfo mi => mi.GetCustomAttributes(),
            ParameterInfo pi => pi.GetCustomAttributes(),
            _ => Array.Empty<System.Attribute>()
        };

    public static T GetCustomAttribute<T>(this object on) => GetCustomAttributes(on).OfType<T>().FirstOrDefault();

    public static bool IsVoidOrRecognized(Type type, IClassStrategy strategy) => type == typeof(void) || strategy.IsTypeRecognizedByReflector(type);

    public static bool IsIContainerOrRecognized(MethodInfo method, Type type, IClassStrategy strategy) => method.IsStatic && type.IsAssignableTo(typeof(IContainer)) || strategy.IsTypeRecognizedByReflector(type);
}