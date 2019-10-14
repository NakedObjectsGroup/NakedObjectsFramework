// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using NakedObjects.Facade;
using NakedObjects.Facade.Contexts;
using NakedObjects.Facade.Utility;
using NakedObjects.Rest.Snapshot.Constants;
using NakedObjects.Rest.Snapshot.Utility;

namespace NakedObjects.Rest.Snapshot.Representations {
    [DataContract]
    public class MenuRepresentation : Representation {
        protected MenuRepresentation(IOidStrategy oidStrategy, HttpRequestMessage req, IMenuFacade menu, RestControlFlags flags)
            : base(oidStrategy, flags) {
            var helper = new UriMtHelper(oidStrategy, req, menu);
            SetScalars(menu);
            SelfRelType = new MenuRelType(RelValues.Self, helper);
            SetLinksAndMembers(req, menu);
            SetExtensions(menu);
            SetHeader(menu);
        }

        [DataMember(Name = JsonPropertyNames.Title)]
        public string Title { get; set; }

        [DataMember(Name = JsonPropertyNames.MenuId)]
        public string MenuId { get; set; }

        [DataMember(Name = JsonPropertyNames.Links)]
        public LinkRepresentation[] Links { get; set; }

        [DataMember(Name = JsonPropertyNames.Extensions)]
        public MapRepresentation Extensions { get; set; }

        [DataMember(Name = JsonPropertyNames.Members)]
        public MapRepresentation Members { get; set; }

        private void SetScalars(IMenuFacade menu) {
            Title = menu.Name;
            MenuId = menu.Id;
        }

        private void SetHeader(IMenuFacade menu) {
            Caching = CacheType.NonExpiring;
        }

        private void SetLinksAndMembers(HttpRequestMessage req, IMenuFacade menu) {
            var tempLinks = new List<LinkRepresentation> {LinkRepresentation.Create(OidStrategy, SelfRelType, Flags)};

            SetMembers(menu, req, tempLinks);
            Links = tempLinks.ToArray();
        }

        private ActionContextFacade ActionContext(IMenuActionFacade actionFacade, string menuPath) {
            return new ActionContextFacade {
                MenuPath = menuPath,
                Target = OidStrategy.FrameworkFacade.GetServices().List.Single(s => s.Specification.IsOfType(actionFacade.Action.OnType)),
                Action = actionFacade.Action,
                VisibleParameters = actionFacade.Action.Parameters.Select(p => new ParameterContextFacade {Parameter = p, Action = actionFacade.Action}).ToArray()
            };
        }

        private Tuple<string, ActionContextFacade>[] GetMenuItem(IMenuItemFacade item, string parent = "") {
            var menuActionFacade = item as IMenuActionFacade;

            if (menuActionFacade != null) {
                return new[] {new Tuple<string, ActionContextFacade>(item.Name, ActionContext(menuActionFacade, parent))};
            }

            var menuFacade = item as IMenuFacade;

            if (menuFacade != null) {
                parent = parent + (string.IsNullOrEmpty(parent) ? "" : IdConstants.MenuItemDivider) + menuFacade.Name;
                return menuFacade.MenuItems.SelectMany(i => GetMenuItem(i, parent)).ToArray();
            }

            return new Tuple<string, ActionContextFacade>[] {};
        }

        private bool IsVisibleAndUsable(ActionContextFacade actionContextFacade) {
            return actionContextFacade.Action.IsVisible(actionContextFacade.Target) && 
                   actionContextFacade.Action.IsUsable(actionContextFacade.Target).IsAllowed;
        }

        private void SetMembers(IMenuFacade menu, HttpRequestMessage req, List<LinkRepresentation> tempLinks) {
            var actionFacades = menu.MenuItems.SelectMany(i => GetMenuItem(i)).Where(af => IsVisibleAndUsable(af.Item2));

            InlineActionRepresentation[] actions = actionFacades.Select(a => InlineActionRepresentation.Create(OidStrategy, req, a.Item2, Flags)).ToArray();

            var eq = new Eq();
            // todo fix distinct
            actions = actions.Distinct(eq).ToArray();

            Members = RestUtils.CreateMap(actions.ToDictionary(m => m.Id, m => (object) m));
        }

        private void SetExtensions(IMenuFacade menu) {
            Extensions = MapRepresentation.Create();
        }

        public static MenuRepresentation Create(IOidStrategy oidStrategy, IMenuFacade menu, HttpRequestMessage req, RestControlFlags flags) {
            return new MenuRepresentation(oidStrategy, req, menu, flags);
        }

        #region Nested type: Eq

        private class Eq : IEqualityComparer<InlineActionRepresentation> {
            #region IEqualityComparer<InlineActionRepresentation> Members

            public bool Equals(InlineActionRepresentation x, InlineActionRepresentation y) {
                return x.Id == y.Id;
            }

            public int GetHashCode(InlineActionRepresentation obj) {
                return obj.Id.GetHashCode();
            }

            #endregion
        }

        #endregion
    }
}