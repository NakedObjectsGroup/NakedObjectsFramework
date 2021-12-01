// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Reflection;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Spec;
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.Menu;
using NakedLegacy.Types;

namespace NakedLegacy.Reflector.Facet; 

[Serializable]
public sealed class MenuFacetViaLegacyMethod : MenuFacetAbstract {
    private readonly MethodInfo method;

    public MenuFacetViaLegacyMethod(MethodInfo method, ISpecification holder)
        : base(holder) =>
        this.method = method;



    private string MatchMethod(string legacyName, Type declaringType) {
        var name = $"action{legacyName}";
        var action = declaringType.GetMethod(name, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);

        return action?.Name ?? legacyName;
    }

    private MenuImpl ConvertLegacyToNOFMenu(MainMenu legacyMenu, IMetamodelBuilder metamodel) {
        var mi = new MenuImpl(metamodel, method.DeclaringType, false, GetMenuName(Spec));
        foreach (var menu in legacyMenu.Menus) {
            switch (menu) {
                case SubMenu sm:
                    var nsm = mi.CreateSubMenu(sm.Name);
                    // temp hack
                    nsm.AddAction(MatchMethod(sm.Menus.Cast<IMenu>().First().Name, method.DeclaringType));
                    break;
                case Menu m:
                    mi.AddAction(MatchMethod(m.Name, method.DeclaringType));
                    break;
            }
        }

        return mi;
    }

    //Creates a menu based on the definition in the object's Menu method
    public override void CreateMenu(IMetamodelBuilder metamodel) {
        var legacyMenu = (MainMenu) InvokeUtils.InvokeStatic(method, new object[] {});
        Menu = ConvertLegacyToNOFMenu(legacyMenu, metamodel);
    }
}

// Copyright (c) Naked Objects Group Ltd.