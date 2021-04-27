// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Http;
using NakedFramework.Facade.Contexts;
using NakedFramework.Facade.Interface;
using NakedFramework.Facade.Translation;
using NakedFramework.Rest.Snapshot.Constants;
using NakedFramework.Rest.Snapshot.RelTypes;
using NakedFramework.Rest.Snapshot.Utility;

namespace NakedFramework.Rest.Snapshot.Representation {
    [DataContract]
    public class MenuRepresentation : Representation {
        protected MenuRepresentation(IFrameworkFacade frameworkFacade, HttpRequest req, IMenuFacade menu, RestControlFlags flags)
            : base(frameworkFacade.OidStrategy, flags) {
            var helper = new UriMtHelper(frameworkFacade.OidStrategy, req, menu);
            SetScalars(menu);
            SelfRelType = new MenuRelType(RelValues.Self, helper);
            SetLinksAndMembers(req, frameworkFacade, menu);
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

        private void SetHeader(IMenuFacade menu) => Caching = CacheType.NonExpiring;

        private void SetLinksAndMembers(HttpRequest req, IFrameworkFacade frameworkFacade, IMenuFacade menu) {
            var tempLinks = new List<LinkRepresentation> {LinkRepresentation.Create(OidStrategy, SelfRelType, Flags)};

            SetMembers(frameworkFacade, menu, req);
            Links = tempLinks.ToArray();
        }

        private (string name, ActionContextFacade action)[] GetMenuItem(IFrameworkFacade frameworkFacade, IMenuItemFacade item, string parent = "") => frameworkFacade.GetMenuItem(item, parent);

        private static bool IsVisibleAndUsable(ActionContextFacade actionContextFacade) =>
            actionContextFacade.Action.IsVisible(actionContextFacade.Target) &&
            actionContextFacade.Action.IsUsable(actionContextFacade.Target).IsAllowed;

        private void SetMembers(IFrameworkFacade frameworkFacade, IMenuFacade menu, HttpRequest req) {
            ActionContextFacade SetMenuId(ActionContextFacade action) {
                action.MenuId = menu.Id;

                foreach (var p in action.VisibleParameters) {
                    p.MenuId = menu.Id;
                }

                return action;
            }

            var actionFacades = menu.MenuItems.SelectMany(i => GetMenuItem(frameworkFacade, i, i.Grouping)).Where(af => IsVisibleAndUsable(af.action)).ToArray();

            var actions = actionFacades.Select(a => InlineActionRepresentation.Create(OidStrategy, req, SetMenuId(a.action), Flags)).ToArray();

            var actionComparer = new ActionComparer();
            actions = actions.Distinct(actionComparer).ToArray();

            Members = RestUtils.CreateMap(actions.ToDictionary(m => m.Id, m => (object) m));
        }

        private void SetExtensions(IMenuFacade menu) => Extensions = MapRepresentation.Create();

        public static MenuRepresentation Create(IFrameworkFacade frameworkFacade, IMenuFacade menu, HttpRequest req, RestControlFlags flags) => new(frameworkFacade, req, menu, flags);

        #region Nested type: ActionComparer

        private class ActionComparer : IEqualityComparer<InlineActionRepresentation> {
            #region IEqualityComparer<InlineActionRepresentation> Members

            public bool Equals(InlineActionRepresentation x, InlineActionRepresentation y) => x.Id == y.Id;

            public int GetHashCode(InlineActionRepresentation obj) => obj.Id.GetHashCode();

            #endregion
        }

        #endregion
    }
}