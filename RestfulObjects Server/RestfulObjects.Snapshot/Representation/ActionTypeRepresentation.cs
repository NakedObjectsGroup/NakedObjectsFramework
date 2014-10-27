// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using NakedObjects.Surface;
using NakedObjects.Surface.Utility;
using RestfulObjects.Snapshot.Constants;
using RestfulObjects.Snapshot.Utility;

namespace RestfulObjects.Snapshot.Representations {
    [DataContract]
    public class ActionTypeRepresentation : Representation {
        protected ActionTypeRepresentation(HttpRequestMessage req, ActionTypeContextSurface actionTypeContext, RestControlFlags flags) : base(flags) {
            SelfRelType = new TypeMemberRelType(RelValues.Self, new UriMtHelper(req, actionTypeContext));
            SetScalars(actionTypeContext);
            SetLinks(req, actionTypeContext);
            SetParameters(req, actionTypeContext);
            SetExtensions();
            SetHeader();
        }

        [DataMember(Name = JsonPropertyNames.Id)]
        public string Id { get; set; }

        [DataMember(Name = JsonPropertyNames.FriendlyName)]
        public string FriendlyName { get; set; }

        [DataMember(Name = JsonPropertyNames.Description)]
        public string Description { get; set; }

        [DataMember(Name = JsonPropertyNames.HasParams)]
        public bool HasParams { get; set; }

        [DataMember(Name = JsonPropertyNames.MemberOrder)]
        public int MemberOrder { get; set; }

        [DataMember(Name = JsonPropertyNames.Links)]
        public LinkRepresentation[] Links { get; set; }

        [DataMember(Name = JsonPropertyNames.Parameters)]
        public LinkRepresentation[] Parameters { get; set; }

        [DataMember(Name = JsonPropertyNames.Extensions)]
        public MapRepresentation Extensions { get; set; }

        private void SetHeader() {
            caching = CacheType.NonExpiring;
        }

        private void SetExtensions() {
            Extensions = MapRepresentation.Create();
        }

        private void SetScalars(ActionTypeContextSurface actionTypeContext) {
            Id = actionTypeContext.ActionContext.Id;
            FriendlyName = actionTypeContext.ActionContext.Action.Name();
            Description = actionTypeContext.ActionContext.Action.Description();
            HasParams = actionTypeContext.ActionContext.VisibleParameters.Any();
            MemberOrder = actionTypeContext.ActionContext.Action.MemberOrder();
        }

        private void SetParameters(HttpRequestMessage req, ActionTypeContextSurface actionTypeContext) {
            IEnumerable<LinkRepresentation> parms = actionTypeContext.ActionContext.VisibleParameters.
                Select(p => LinkRepresentation.Create(new ParamTypeRelType(new UriMtHelper(req, new ParameterTypeContextSurface {
                    Action = actionTypeContext.ActionContext.Action,
                    OwningSpecification = actionTypeContext.OwningSpecification,
                    Parameter = p.Parameter
                })), Flags));
            Parameters = parms.ToArray();
        }

        private void SetLinks(HttpRequestMessage req, ActionTypeContextSurface actionTypeContext) {
            var domainTypeUri = new UriMtHelper(req, actionTypeContext);
            var tempLinks = new List<LinkRepresentation> {
                LinkRepresentation.Create(SelfRelType, Flags),
                LinkRepresentation.Create(new DomainTypeRelType(RelValues.Up, domainTypeUri), Flags),
                LinkRepresentation.Create(new DomainTypeRelType(RelValues.ReturnType, new UriMtHelper(req, actionTypeContext.ActionContext.Action.ReturnType)), Flags)
            };

            if (actionTypeContext.ActionContext.Action.ReturnType.IsCollection()) {
                tempLinks.Add(LinkRepresentation.Create(new DomainTypeRelType(RelValues.ElementType, new UriMtHelper(req, actionTypeContext.ActionContext.Action.ElementType)), Flags));
            }

            Links = tempLinks.ToArray();
        }


        public static ActionTypeRepresentation Create(HttpRequestMessage req, ActionTypeContextSurface actionTypeContext, RestControlFlags flags) {
            return new ActionTypeRepresentation(req, actionTypeContext, flags);
        }
    }
}