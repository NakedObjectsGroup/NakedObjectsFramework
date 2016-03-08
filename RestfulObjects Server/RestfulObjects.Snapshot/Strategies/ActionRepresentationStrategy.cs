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
using NakedObjects.Facade.Utility;
using RestfulObjects.Snapshot.Constants;
using RestfulObjects.Snapshot.Representations;
using RestfulObjects.Snapshot.Utility;

namespace RestfulObjects.Snapshot.Strategies {
    public class ActionRepresentationStrategy : AbstractStrategy {
        private IEnumerable<ParameterRepresentation> parameterList;
        private readonly RelType self;

        public ActionRepresentationStrategy(IOidStrategy oidStrategy, HttpRequestMessage req, ActionContextFacade actionContext, RestControlFlags flags)
            : base(oidStrategy, flags) {
            Req = req;
            ActionContext = actionContext;
            self = new MemberRelType(RelValues.Self, new UriMtHelper(oidStrategy, req, actionContext));         
        }

        protected ActionContextFacade ActionContext { get; }

        protected  HttpRequestMessage Req { get; }

        public virtual void CreateParameters() {
            parameterList = GetParameterList();
        }

        public virtual RestControlFlags GetFlags() {
            return Flags;
        }

        public virtual IObjectFacade GetTarget() {
            return ActionContext.Target;
        }

        public virtual RelType GetSelf() {
            return self;
        }

        public virtual string GetId() {
            return ActionContext.Action.Id;
        }

        protected ParameterRepresentation GetParameter(IActionParameterFacade parameter) {
            IObjectFacade objectFacade = ActionContext.Target;
            return ParameterRepresentation.Create(OidStrategy, Req, objectFacade, parameter, Flags);
        }

        protected virtual IEnumerable<ParameterRepresentation> GetParameterList() {
            return ActionContext.VisibleParameters.Select(p => GetParameter(p.Parameter));
        }

        public virtual MapRepresentation GetParameters() {
            return RestUtils.CreateMap(parameterList.ToDictionary(p => p.Name, p => (object) p));
        }

        protected LinkRepresentation CreateDetailsLink() {
            return LinkRepresentation.Create(OidStrategy, new MemberRelType(new UriMtHelper(OidStrategy, Req, ActionContext)), Flags);
        }

        public virtual LinkRepresentation[] GetLinks(bool standalone) {
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

        protected LinkRepresentation CreateUpLink() {
            var helper = new UriMtHelper(OidStrategy, Req, ActionContext.Target);
            ObjectRelType parentRelType = ActionContext.Target.Specification.IsService ? new ServiceRelType(RelValues.Up, helper) : new ObjectRelType(RelValues.Up, helper);
            return LinkRepresentation.Create(OidStrategy, parentRelType, Flags);
        }

        protected LinkRepresentation CreateSelfLink() {
            return LinkRepresentation.Create(OidStrategy, self, Flags);
        }

        protected virtual bool HasParams() {
            return ActionContext.VisibleParameters.Any();
        }

        protected override MapRepresentation GetExtensionsForSimple() {
            return RestUtils.GetExtensions(
                friendlyname: ActionContext.Action.Name,
                description: ActionContext.Action.Description,
                pluralName: null,
                domainType: null,
                isService: null,
                hasParams: HasParams(),
                optional: null,
                maxLength: null,
                pattern: null,
                memberOrder: ActionContext.Action.MemberOrder,
                dataType: null,
                presentationHint: ActionContext.Action.PresentationHint,
                customExtensions: GetCustomPropertyExtensions(),
                returnType: ActionContext.Action.ReturnType,
                elementType: ActionContext.Action.ElementType,
                oidStrategy: OidStrategy,
                useDateOverDateTime: false);
        }

        protected IDictionary<string, object> GetCustomPropertyExtensions() {
            var ext = GetTableViewCustomExtensions(ActionContext.Action.TableViewData);
            if (!string.IsNullOrEmpty(ActionContext.MenuPath)) {
                ext = ext ?? new Dictionary<string, object>();
                ext[IdConstants.MenuPath] = ActionContext.MenuPath;
            }
            return ext;
        }

        protected LinkRepresentation CreateActionLink() {
            List<OptionalProperty> optionalProperties = parameterList.Select(pr => new OptionalProperty(pr.Name, MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.Value, null, typeof (object))))).ToList();

            RelMethod method = GetRelMethod();
            return LinkRepresentation.Create(OidStrategy, new InvokeRelType(new UriMtHelper(OidStrategy, Req, ActionContext)) {Method = method}, Flags,
                new OptionalProperty(JsonPropertyNames.Arguments, MapRepresentation.Create(optionalProperties.ToArray())));
        }

        protected RelMethod GetRelMethod() {
            if (ActionContext.Action.IsQueryOnly) {
                return RelMethod.Get;
            }
            return ActionContext.Action.IsIdempotent ? RelMethod.Put : RelMethod.Post;
        }
    }
}