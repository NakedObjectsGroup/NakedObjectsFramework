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
using NakedObjects.Facade;
using NakedObjects.Facade.Contexts;
using RestfulObjects.Snapshot.Constants;
using RestfulObjects.Snapshot.Representations;
using RestfulObjects.Snapshot.Utility;

namespace RestfulObjects.Snapshot.Strategies {
    public class ActionRepresentationStrategy : AbstractStrategy {
        private readonly ActionContextFacade actionContext;
        private readonly IEnumerable<ParameterRepresentation> parameterList;
        private readonly HttpRequestMessage req;
        private readonly RelType self;

        public ActionRepresentationStrategy(IOidStrategy oidStrategy, HttpRequestMessage req, ActionContextFacade actionContext, RestControlFlags flags)
            : base(oidStrategy, flags) {
            this.req = req;
            this.actionContext = actionContext;
            self = new MemberRelType(RelValues.Self, new UriMtHelper(oidStrategy, req, actionContext));
            parameterList = GetParameterList();
        }

        public RestControlFlags GetFlags() {
            return Flags;
        }

        public IObjectFacade GetTarget() {
            return actionContext.Target;
        }

        public RelType GetSelf() {
            return self;
        }

        public string GetId() {
            return actionContext.Action.Id;
        }

        private ParameterRepresentation GetParameter(IActionParameterFacade parameter) {
            IObjectFacade objectFacade = actionContext.Target;
            return ParameterRepresentation.Create(OidStrategy, req, objectFacade, parameter, Flags);
        }

        private IEnumerable<ParameterRepresentation> GetParameterList() {
            return actionContext.VisibleParameters.Select(p => GetParameter(p.Parameter));
        }

        public MapRepresentation GetParameters() {
            return RestUtils.CreateMap(parameterList.ToDictionary(p => p.Name, p => (object) p));
        }

        private LinkRepresentation CreateDetailsLink() {
            return LinkRepresentation.Create(OidStrategy, new MemberRelType(new UriMtHelper(OidStrategy, req, actionContext)), Flags);
        }

        public LinkRepresentation[] GetLinks(bool standalone) {
            var tempLinks = new List<LinkRepresentation>();

            if (standalone) {
                tempLinks.Add(CreateSelfLink());
                tempLinks.Add(CreateUpLink());
            }
            else {
                tempLinks.Add(CreateDetailsLink());
            }

            tempLinks.Add(CreateActionLink());

            return tempLinks.ToArray();
        }

        private LinkRepresentation CreateUpLink() {
            var helper = new UriMtHelper(OidStrategy, req, actionContext.Target);
            ObjectRelType parentRelType = actionContext.Target.Specification.IsService ? new ServiceRelType(RelValues.Up, helper) : new ObjectRelType(RelValues.Up, helper);
            return LinkRepresentation.Create(OidStrategy, parentRelType, Flags);
        }

        private LinkRepresentation CreateSelfLink() {
            return LinkRepresentation.Create(OidStrategy, self, Flags);
        }

        protected override MapRepresentation GetExtensionsForSimple() {
            return RestUtils.GetExtensions(friendlyname: actionContext.Action.Name,
                description: actionContext.Action.Description,
                pluralName: null,
                domainType: null,
                isService: null,
                hasParams: actionContext.VisibleParameters.Any(),
                optional: null,
                maxLength: null,
                pattern: null,
                memberOrder: actionContext.Action.MemberOrder,
                customExtensions: GetCustomPropertyExtensions(),
                returnType: actionContext.Action.ReturnType,
                elementType: actionContext.Action.ElementType,
                oidStrategy: OidStrategy,
                useDateOverDateTime: false);
        }

        private IDictionary<string, object> GetCustomPropertyExtensions() {
            return GetTableViewCustomExtensions(actionContext.Action.ExtensionData, actionContext.Action.TableViewData);
        }

        private LinkRepresentation CreateActionLink() {
            List<OptionalProperty> optionalProperties = parameterList.Select(pr => new OptionalProperty(pr.Name, MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.Value, null, typeof (object))))).ToList();

            RelMethod method = GetRelMethod();
            return LinkRepresentation.Create(OidStrategy, new InvokeRelType(new UriMtHelper(OidStrategy, req, actionContext)) {Method = method}, Flags,
                new OptionalProperty(JsonPropertyNames.Arguments, MapRepresentation.Create(optionalProperties.ToArray())));
        }

        private RelMethod GetRelMethod() {
            if (actionContext.Action.IsQueryOnly) {
                return RelMethod.Get;
            }
            return actionContext.Action.IsIdempotent ? RelMethod.Put : RelMethod.Post;
        }
    }
}