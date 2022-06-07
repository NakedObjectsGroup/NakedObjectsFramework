// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Menu;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Error;
using NakedFramework.Core.Util;
using NakedFramework.Menu;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.Utils;

namespace NakedFramework.Metamodel.Menu;

public class MenuBuilder : IMenu {
    private ImmutableList<MenuBuilder> items = ImmutableList<MenuBuilder>.Empty;

    public MenuBuilder(IMetamodelBuilder metamodel, Type defaultType, bool addAllActions, string name, string id = null) {
        Metamodel = metamodel;
        Type = defaultType;
        Name = name ?? ObjectSpec.GetFacet<INamedFacet>().FriendlyName;
        Id = id ?? defaultType.Name;
        if (addAllActions) {
            AddRemainingNativeActions();
            AddContributedActions();
        }
    }

    public MenuBuilder(IMetamodelBuilder metamodel, string name, string id = null) {
        Metamodel = metamodel;
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(id)) {
            throw new Exception("Menu Name and Id must not be null, unless a default type is specified");
        }

        Name = name;
        Id = id;
    }

    private MenuBuilder(IMetamodelBuilder metamodel, IActionSpecImmutable action) {
        Metamodel = metamodel;
        Action = action;
    }

    private IActionSpecImmutable Action { get; }

    private ITypeSpecImmutable ObjectSpec => Metamodel.GetSpecification(Type);

    private IList<IActionSpecImmutable> ActionsForObject => ObjectSpec.OrderedObjectActions.ToList();

    private IMenuItemImmutable ExtractMenuItem => Action is not null ? new MenuAction(Action) : ExtractMenu();
    private Type Type { get; }

    /// <summary>
    ///     Will only be set if this menu is a sub-menu of another.
    /// </summary>
    private IMenu SuperMenu { get; set; }

    /// <summary>
    ///     The name of the menu -  will typically be rendered on the UI
    /// </summary>
    private string Name { get; set; }

    /// <summary>
    ///     Id is optional.  It is only included to facilitate backwards compatibility with
    ///     existing auto-generated menus.
    /// </summary>
    public string Id { get; set; }

    private string Grouping => "";

    //Includes both actions and sub-menus
    private IList<MenuBuilder> MenuItems => items;

    private IMetamodelBuilder Metamodel { get; }

    public IMenu WithMenuName(string name) {
        Name = name;
        return this;
    }

    public IMenu WithId(string id) {
        Id = id;
        return this;
    }

    public IMenu AddAction(string actionName, string friendlyName = null, bool ignoreCase = false) {
        if (Type is null) {
            throw new Exception($"No type has been specified for the action: {actionName}");
        }

        var actionSpec = GetAction(actionName, ignoreCase);
        return AddAction(actionName, friendlyName, Type, actionSpec);
    }

    public IMenu AddAction(Type fromType, string actionName, string friendlyName = null, bool ignoreCase = false) {
        var compare = ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture;
        var actionSpec = ActionsForType(fromType).FirstOrDefault(a => string.Equals(a.Identifier.MemberName, actionName, compare));
        return AddAction(actionName, friendlyName, fromType, actionSpec);
    }

    public IMenu CreateSubMenu(string subMenuName) => CreateMenuImmutableAsSubMenu(subMenuName);

    public IMenu GetSubMenu(string menuName) {
        var menu = GetSubMenuIfExists(menuName);
        if (menu == null) {
            throw new Exception($"No sub-menu named {menuName}");
        }

        return menu;
    }

    public IMenu AddRemainingNativeActions() {
        if (Type == null) {
            throw new Exception("No default type has been specified (as the source for the remaining native actions)");
        }

        AddOrderableElementsToMenu(ActionsForObject, this);
        return this;
    }

    public IMenu AddContributedActions() {
        if (Type == null) {
            throw new Exception("No default type has been specified (as the source for the contributed actions)");
        }

        ObjectSpec?.OrderedContributedActions.ForEach(AddContributed);
        return this;
    }

    public IMenu AddRemainingActions(Type fromType) {
        AddOrderableElementsToMenu(ActionsForType(fromType), this);
        return this;
    }

    private IMenu AddAction(string actionName, string friendlyName, Type fromType, IActionSpecImmutable actionSpec) {
        if (actionSpec is not null) {
            AddFriendlyName(friendlyName, actionSpec);
            AddMenuItem(new MenuBuilder(Metamodel, actionSpec));
            return this;
        }

        return Metamodel.CoreConfiguration.UsePlaceholderForUnreflectedType ? this : throw new ReflectionException($"No such action: {actionName} on {fromType}");
    }

    private IActionSpecImmutable GetAction(string actionName, bool ignoreCase) {
        var actions = ObjectSpec.OrderedObjectActions?.Any() is true ? ObjectSpec.OrderedObjectActions.ToArray() : Array.Empty<IActionSpecImmutable>();
        var compare = ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture;

        return actions.FirstOrDefault(a => string.Equals(a.Identifier.MemberName, actionName, compare));
    }

    private IList<IActionSpecImmutable> ActionsForType(Type type) => Metamodel.GetSpecification(type).OrderedObjectActions.ToList();

    private MenuBuilder CreateMenuImmutableAsSubMenu(string subMenuName, string id = null) {
        var subMenu = new MenuBuilder(Metamodel, Type, false, subMenuName);
        if (id == null) {
            subMenu.Id += $"-{subMenuName}:";
        }
        else {
            subMenu.Id = id;
        }

        subMenu.SuperMenu = this;
        AddMenuItem(subMenu);
        return subMenu;
    }

    private bool HasAction(IActionSpecImmutable action) {
        var nativeAction = MenuItems.Any(mi => mi.Action == action);
        return nativeAction || MenuItems.Any(m => m.HasAction(action));
    }

    private bool HasActionOrSuperMenuHasAction(IActionSpecImmutable action) =>
        HasAction(action) ||
        (SuperMenu is MenuBuilder superMenu &&
         superMenu.HasActionOrSuperMenuHasAction(action));

    private MenuBuilder GetSubMenuIfExists(string menuName) => MenuItems.FirstOrDefault(a => a.Name == menuName);

    private void AddOrderableElementsToMenu(IList<IActionSpecImmutable> orderableElements, MenuBuilder toMenu) {
        foreach (var action in orderableElements) {
            if (action != null) {
                if (!toMenu.HasActionOrSuperMenuHasAction(action)) {
                    toMenu.AddMenuItem(new MenuBuilder(Metamodel, action));
                }
            }
        }
    }

    private void AddMenuItem(MenuBuilder item) => items = items.Add(item); //Only way to add to an immutable collection

    private void AddContributed(IActionSpecImmutable ca) {
        if (ObjectSpec is IObjectSpecImmutable objectSpec) {
            AddContributedAction(ca, objectSpec);
        }
        else {
            AddContributedFunction(ca);
        }
    }

    private void AddContributedFunction(IActionSpecImmutable ca) {
        //i.e. no sub-menu
        AddMenuItem(new MenuBuilder(Metamodel, ca));
    }

    private void AddContributedAction(IActionSpecImmutable ca, IObjectSpecImmutable spec) {
        var facet = ca.GetFacet<IContributedActionIntegrationFacet>();
        var subMenuName = facet?.SubMenuWhenContributedTo(spec);
        if (subMenuName != null) {
            var id = facet.IdWhenContributedTo(spec);
            var subMenu = GetSubMenuIfExists(subMenuName) ?? CreateMenuImmutableAsSubMenu(subMenuName, id);
            AddOrderableElementsToMenu(new List<IActionSpecImmutable> { ca }, subMenu);
        }
        else {
            //i.e. no sub-menu
            AddMenuItem(new MenuBuilder(Metamodel, ca));
        }
    }

    public IMenuImmutable ExtractMenu() => new MenuImmutable(Name, Id, Grouping, MenuItems.Select(i => i.ExtractMenuItem).ToList());

    private static void AddFriendlyName(string friendlyName, IActionSpecImmutable actionSpec) {
        if (!string.IsNullOrWhiteSpace(friendlyName)) {
            var facet = new MemberNamedFacetAnnotation(friendlyName);
            FacetUtils.AddFacet(facet, actionSpec);
        }
    }
}