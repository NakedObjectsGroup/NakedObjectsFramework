// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Metamodel.NonSerialiZedMenu;

namespace NakedFramework.Metamodel.Facet;

[Serializable]
public sealed class MenuFacetDefault : MenuFacetAbstract {
    //Creates a menu based on the object's actions and their specified ordering
    //For backwards compatibility of UI only, it gives the menu an Id of the type name
    public override void CreateMenu(IMetamodelBuilder metamodel, ITypeSpecImmutable spec) {
        //The Id is specified as follows purely to facilitate backwards compatibility with existing UI
        //It is not needed for menus to function
        var id = spec is IServiceSpecImmutable ? UniqueShortName(spec) : $"{spec.ShortName}-Actions";
        CreateDefaultMenu(metamodel, spec.Type, GetMenuName(spec), id);
    }

    private static string UniqueShortName(ITypeSpecImmutable spec) {
        var usn = spec.ShortName;
        var type = spec.Type;
        if (type.IsGenericType) {
            usn += $"-{type.GetGenericArguments().First().Name}";
        }

        return usn;
    }

    public void CreateDefaultMenu(IMetamodelBuilder metamodel, Type type, string menuName, string id) {
        var menu = new MenuBuilder(metamodel, type, true, menuName, id);
        Menu = menu.ExtractMenu();
    }
}

// Copyright (c) Naked Objects Group Ltd.