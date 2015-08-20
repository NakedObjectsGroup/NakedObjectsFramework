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
using RestfulObjects.Snapshot.Constants;
using RestfulObjects.Snapshot.Utility;

namespace RestfulObjects.Snapshot.Representations {
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
            caching = CacheType.NonExpiring;
            //SetEtag(objectContext.Target);
        }

        private void SetLinksAndMembers(HttpRequestMessage req, IMenuFacade menu) {
            var tempLinks = new List<LinkRepresentation>();

            tempLinks.Add(LinkRepresentation.Create(OidStrategy, SelfRelType, Flags));

            if (Flags.FormalDomainModel) {
                //tempLinks.Add(LinkRepresentation.Create(OidStrategy, new DomainTypeRelType(RelValues.DescribedBy, new UriMtHelper(OidStrategy, req, menu.Specification)), Flags));
            }

            // temp disable icons 
            //tempLinks.Add(LinkRepresentation.Create(new IconRelType(objectUri), Flags));
            SetMembers(menu, req, tempLinks);
            Links = tempLinks.ToArray();
        }

        private ActionContextFacade ActionContext(IMenuActionFacade actionFacade, string menuPath) {

            if (!string.IsNullOrEmpty(menuPath)) {
                actionFacade.Action.ExtensionData[IdConstants.MenuPath] = menuPath;
            }

            return new ActionContextFacade {
                Target = OidStrategy.FrameworkFacade.GetServices().List.Single(s => s.Specification.IsOfType(actionFacade.Action.OnType)),
                Action = actionFacade.Action,
                VisibleParameters = actionFacade.Action.Parameters.Select(p => new ParameterContextFacade {Parameter = p, Action = actionFacade.Action}).ToArray()
            };
        }

        private Tuple<string, ActionContextFacade>[] GetMenuItem(IMenuItemFacade item, string parent = "") {

            
            var menuActionFacade= item as IMenuActionFacade;

            if (menuActionFacade != null) {
                return new[] {new Tuple<string, ActionContextFacade>(item.Name, ActionContext(menuActionFacade, parent))};
            }

            var menuFacade = item as IMenuFacade;

            if (menuFacade != null) {
                parent = parent + (string.IsNullOrEmpty(parent) ? "" : "-") + menuFacade.Name;
                return menuFacade.MenuItems.SelectMany(i => GetMenuItem(i, parent)).ToArray();
            }

            return new Tuple<string, ActionContextFacade>[] {};
        }



        private void SetMembers(IMenuFacade menu, HttpRequestMessage req, List<LinkRepresentation> tempLinks) {
            var actionFacades = menu.MenuItems.SelectMany(i => GetMenuItem(i));

            InlineActionRepresentation[] actions = actionFacades.Select(a => InlineActionRepresentation.Create(OidStrategy, req, a.Item2, Flags)).ToArray();

            Members = RestUtils.CreateMap(actions.ToDictionary(m => m.Id, m => (object) m));
        }

        private IDictionary<string, object> GetCustomExtensions(IObjectFacade objectFacade) {
            return objectFacade.ExtensionData == null ? null : objectFacade.ExtensionData.ToDictionary(kvp => kvp.Key, kvp => (object) kvp.Value.ToString().ToLower());
        }

        private void SetExtensions(IMenuFacade menu) {
            if (Flags.SimpleDomainModel) {
                //Extensions = RestUtils.GetExtensions(objectFacade.Specification.SingularName, objectFacade.Specification.Description, objectFacade.Specification.PluralName, objectFacade.Specification.DomainTypeName(OidStrategy), objectFacade.Specification.IsService, null, null, null, null, null, GetCustomExtensions(objectFacade), null, null, OidStrategy);
                Extensions = MapRepresentation.Create();
            }
            else {
                Extensions = MapRepresentation.Create();
            }
        }

        public static MenuRepresentation Create(IOidStrategy oidStrategy, IMenuFacade menu, HttpRequestMessage req, RestControlFlags flags) {
            return new MenuRepresentation(oidStrategy, req, menu, flags);
        }
    }
}