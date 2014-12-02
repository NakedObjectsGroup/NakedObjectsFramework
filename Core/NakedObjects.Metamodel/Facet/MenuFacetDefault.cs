// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Reflection;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Spec;
using NakedObjects.Meta.Menu;

namespace NakedObjects.Meta.Facet {
    [Serializable]
    public class MenuFacetDefault : MenuFacetAbstract {
        public MenuFacetDefault(ISpecification holder)
            : base(holder) {}

        //Creates a menu based on the object's actions and their specified ordering
        public override void CreateMenu(IMetamodelBuilder metamodel) {
            if (Spec().Type.FullName.StartsWith("System")) return; //Menu not relevant, and could cause error below
            MethodInfo m = GetType().GetMethod("CreateDefaultMenu").MakeGenericMethod(Spec().Type);

            // possible spec type is generic in which case invoke would fail without this check
            if (!m.ContainsGenericParameters) {
                m.Invoke(this, new object[] {metamodel, GetMenuName(Spec())});
            }
        }

        public void CreateDefaultMenu<T>(IMetamodelBuilder metamodel, string menuName) {
            var menu = new TypedMenu<T>(metamodel, false, menuName);
            menu.AddRemainingNativeActions();
            menu.AddContributedActions();
            this.Menu = menu;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}