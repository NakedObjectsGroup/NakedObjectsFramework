// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Util;
using NakedObjects.Menu;

namespace NakedObjects.Meta.Menu {
    [Serializable]
    public sealed class TypedMenu<TObject> : MenuImpl {
        public TypedMenu(IMetamodel metamodel, bool addAllActions, string name)
            : base(metamodel, name) {
            if (name == null) {
                Name = GetFriendlyNameForObject();
            }
            Id = typeof (TObject).Name;
            if (addAllActions) {
                AddRemainingNativeActions();
                AddContributedActions();
            }
        }

        #region ITypedMenu<TObject> Members

        public IMenu AddAction(string actionName, string renamedTo = null) {
            AddActionFrom<TObject>(actionName, renamedTo);
            return this;
        }

        public IMenu AddRemainingNativeActions() {
            AddAllRemainingActionsFrom<TObject>();
            return this;
        }

        public IMenu AddContributedActions() {
            var spec = GetObjectSpec<TObject>() as IObjectSpecImmutable;
            if (spec != null) {
                spec.ContributedActions.ForEach(ca => AddContributedAction(ca, spec));
            }
            return this;
        }

        public IMenu CreateSubMenuOfSameType(string subMenuName) {
            var sub = new TypedMenu<TObject>(Metamodel, false, subMenuName);
            sub.Id += "-" + subMenuName + ":";
            AddMenuItem(sub);
            return sub;
        }

        #endregion

        private void AddContributedAction(IActionSpecImmutable ca, IObjectSpecImmutable spec) {
            var facet = ca.GetFacet<IContributedActionFacet>();
            string subMenuName = facet.SubMenuWhenContributedTo(spec);
            if (subMenuName != null) {
                string id = facet.IdWhenContributedTo(spec);
                MenuImpl subMenu = GetSubMenuIfExists(subMenuName) ?? CreateMenuImmutableAsSubMenu(subMenuName, id);
                subMenu.AddOrderableElementsToMenu(new List<IActionSpecImmutable> {ca}, subMenu);
            }
            else {
                //i.e. no sub-menu
                AddMenuItem(new MenuAction(ca));
            }
        }

        private string GetFriendlyNameForObject() {
            ITypeSpecImmutable spec = GetObjectSpec<TObject>();
            return spec.GetFacet<INamedFacet>().NaturalName;
        }
    }
}