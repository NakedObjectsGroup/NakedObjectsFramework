// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Menu;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.SpecImmutable;
using System;
using System.Collections.Generic;

namespace NakedObjects.Meta.Menus {
    public class TypedMenu<TObject> : Menu, ITypedMenu<TObject> {
        public TypedMenu(IMetamodel metamodel, bool addAllActions, string name)
            : base(metamodel, name) {
            if (name == null) {
                this.Name = GetFriendlyNameForObject();
            }
            if (addAllActions) {
                AddRemainingNativeActions();
                AddContributedActions();
            }
        }

        #region ITypedMenu<TObject> Members

        public ITypedMenu<TObject> AddAction(string actionName, string renamedTo = null) {
            AddActionFrom<TObject>(actionName, renamedTo);
            return this;
        }

        public ITypedMenu<TObject> AddRemainingNativeActions() {
            AddAllRemainingActionsFrom<TObject>();
            return this;
        }

        public ITypedMenu<TObject> AddContributedActions() {
            foreach (var ca in GetObjectSpec<TObject>().ContributedActions) {
                //TODO: Check if sub menu already exists
                Menu sub = new Menu(metamodel, ca.Item2); //Item 2 should be friendly name of the contributing service
                //Item1 is contributing service class name, not used.
                sub.AddOrderableElementsToMenu(ca.Item3, sub); //Item 3 should be the actions
                this.AddAsSubMenu(sub);
            }
            return this;
        }

        public ITypedMenu<TObject> CreateSubMenuOfSameType(string subMenuName) {
            var sub = new TypedMenu<TObject>(metamodel, false, subMenuName);
            AddMenuItem(sub);
            return sub;
        }

        #endregion

        private string GetFriendlyNameForObject() {
            var spec = GetObjectSpec<TObject>();
            return spec.GetFacet<INamedFacet>().Value ?? spec.ShortName;
        }
    }
}
