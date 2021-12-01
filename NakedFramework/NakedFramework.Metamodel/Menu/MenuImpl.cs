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
using System.Runtime.Serialization;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Menu;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Error;
using NakedFramework.Core.Util;
using NakedFramework.Menu;
using NakedFramework.Metamodel.Utils;

namespace NakedFramework.Metamodel.Menu; 

[Serializable]
public class MenuImpl : IMenu, IMenuImmutable, ISerializable, IDeserializationCallback {
    public MenuImpl(IMetamodel metamodel, Type defaultType, bool addAllActions, string name, string id = null) {
        Metamodel = metamodel;
        Type = defaultType;
        Name = name ?? ObjectSpec.GetFacet<INamedFacet>().NaturalName;
        Id = id ?? defaultType.Name;
        if (addAllActions) {
            AddRemainingNativeActions();
            AddContributedActions();
        }
    }

    public MenuImpl(IMetamodel metamodel, string name, string id = null) {
        Metamodel = metamodel;
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(id)) {
            throw new Exception("Menu Name and Id must not be null, unless a default type is specified");
        }

        Name = name;
        Id = id;
    }

    private ITypeSpecImmutable ObjectSpec => Metamodel.GetSpecification(Type);

    private IList<IActionSpecImmutable> ActionsForObject => ObjectSpec.OrderedObjectActions.ToList();

    private IList<IActionSpecImmutable> ActionsForType(Type type) => Metamodel.GetSpecification(type).OrderedObjectActions.ToList();

    private MenuImpl CreateMenuImmutableAsSubMenu(string subMenuName, string id = null) {
        var subMenu = new MenuImpl(Metamodel, Type, false, subMenuName);
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
        var nativeAction = MenuItems.OfType<MenuAction>().Any(mi => mi.Action == action);
        return nativeAction || MenuItems.OfType<MenuImpl>().Any(m => m.HasAction(action));
    }

    public bool HasActionOrSuperMenuHasAction(IActionSpecImmutable action) =>
        HasAction(action) ||
        SuperMenu is MenuImpl superMenu &&
        superMenu.HasActionOrSuperMenuHasAction(action);

    private MenuImpl GetSubMenuIfExists(string menuName) => MenuItems.OfType<MenuImpl>().FirstOrDefault(a => a.Name == menuName);

    private static void AddOrderableElementsToMenu(IList<IActionSpecImmutable> ordeableElements, MenuImpl toMenu) {
        foreach (var action in ordeableElements) {
            if (action != null) {
                if (!toMenu.HasActionOrSuperMenuHasAction(action)) {
                    toMenu.AddMenuItem(new MenuAction(action));
                }
            }
        }
    }

    private void AddMenuItem(IMenuItemImmutable item) => items = items.Add(item); //Only way to add to an immutable collection

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
        AddMenuItem(new MenuAction(ca));
    }

    private void AddContributedAction(IActionSpecImmutable ca, IObjectSpecImmutable spec) {
        var facet = ca.GetFacet<IContributedActionFacet>();
        var subMenuName = facet?.SubMenuWhenContributedTo(spec);
        if (subMenuName != null) {
            var id = facet.IdWhenContributedTo(spec);
            var subMenu = GetSubMenuIfExists(subMenuName) ?? CreateMenuImmutableAsSubMenu(subMenuName, id);
            AddOrderableElementsToMenu(new List<IActionSpecImmutable> {ca}, subMenu);
        }
        else {
            //i.e. no sub-menu
            AddMenuItem(new MenuAction(ca));
        }
    }

    #region IMenu Members

    public IMenu WithMenuName(string name) {
        Name = name;
        return this;
    }

    private Type Type { get; set; }

    public IMenu WithId(string id) {
        Id = id;
        return this;
    }

    public IMenu AddAction(string actionName) {
        if (Type == null) {
            throw new Exception($"No type has been specified for the action: {actionName}");
        }

        var actionSpec = ActionsForObject.FirstOrDefault(a => a.Identifier.MemberName == actionName);
        if (actionSpec == null) {
            throw new ReflectionException($"No such action: {actionName} on {Type}");
        }

        AddMenuItem(new MenuAction(actionSpec));
        return this;
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

    public IMenu AddAction(Type fromType, string actionName) {
        var actionSpec = ActionsForType(fromType).FirstOrDefault(a => a.Identifier.MemberName == actionName);
        if (actionSpec == null) {
            throw new ReflectionException($"No such action: {actionName} on {fromType}");
        }

        AddMenuItem(new MenuAction(actionSpec));
        return this;
    }

    public IMenu AddRemainingActions(Type fromType) {
        AddOrderableElementsToMenu(ActionsForType(fromType), this);
        return this;
    }

    #endregion

    #region other properties

    private ImmutableList<IMenuItemImmutable> items = ImmutableList<IMenuItemImmutable>.Empty;

    /// <summary>
    ///     Will only be set if this menu is a sub-menu of another.
    /// </summary>
    public IMenu SuperMenu { get; private set; }

    /// <summary>
    ///     The name of the menu -  will typically be rendered on the UI
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     Id is optional.  It is only included to facilitate backwards compatibility with
    ///     existing auto-generated menus.
    /// </summary>
    public string Id { get; set; }

    public string Grouping => "";

    //Includes both actions and sub-menus
    public IList<IMenuItemImmutable> MenuItems => items;

    protected IMetamodel Metamodel { get; }

    #endregion

    #region ISerializable

    private readonly IList<IMenuItemImmutable> tempItems;

    // The special constructor is used to deserialize values. 
    protected MenuImpl(SerializationInfo info, StreamingContext context) {
        tempItems = info.GetValue<IList<IMenuItemImmutable>>("items");
        SuperMenu = info.GetValue<IMenu>("SuperMenu");
        Name = info.GetValue<string>("Name");
        Id = info.GetValue<string>("Id");
        Metamodel = info.GetValue<IMetamodel>("Metamodel");
    }

    public virtual void GetObjectData(SerializationInfo info, StreamingContext context) {
        info.AddValue<IList<IMenuItemImmutable>>("items", items.ToList());
        info.AddValue<IMenu>("SuperMenu", SuperMenu);
        info.AddValue<string>("Name", Name);
        info.AddValue<string>("Id", Id);
        info.AddValue<IMetamodel>("Metamodel", Metamodel);
    }

    public void OnDeserialization(object sender) => items = tempItems.ToImmutableList();

    #endregion
}