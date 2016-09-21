// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using NakedObjects.Facade;
using NakedObjects.Facade.Contexts;
using NakedObjects.Rest.Snapshot.Constants;
using NakedObjects.Rest.Snapshot.Representations;
using NakedObjects.Rest.Snapshot.Utility;

namespace NakedObjects.Rest.Snapshot.Strategies {
    public abstract class AbstractActionRepresentationStrategy : AbstractStrategy {
        private readonly RelType self;
        private IEnumerable<ParameterRepresentation> parameterList;

        protected AbstractActionRepresentationStrategy(IOidStrategy oidStrategy, HttpRequestMessage req, ActionContextFacade actionContext, RestControlFlags flags)
            : base(oidStrategy, flags) {
            Req = req;
            ActionContext = actionContext;
            self = new MemberRelType(RelValues.Self, new UriMtHelper(oidStrategy, req, actionContext));
        }

        protected ActionContextFacade ActionContext { get; }

        protected HttpRequestMessage Req { get; }

        public virtual bool ShowParameters() {
            return true;
        }

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

        public abstract LinkRepresentation[] GetLinks();

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
                customExtensions: GetCustomActionExtensions(),
                returnType: ActionContext.Action.ReturnType,
                elementType: ActionContext.Action.ElementType,
                oidStrategy: OidStrategy,
                useDateOverDateTime: false);
        }

        protected IDictionary<string, object> GetCustomActionExtensions() {
            var ext = GetTableViewCustomExtensions(ActionContext.Action.TableViewData);

            if (!string.IsNullOrEmpty(ActionContext.MenuPath)) {
                ext = ext ?? new Dictionary<string, object>();
                ext[JsonPropertyNames.CustomMenuPath] = ActionContext.MenuPath;
            }

            if (ActionContext.Action.RenderEagerly) {
                ext = ext ?? new Dictionary<string, object>();
                ext[JsonPropertyNames.CustomRenderEagerly] = true;
            }

            var multipleLines = ActionContext.Action.NumberOfLines;

            if (multipleLines.HasValue) {
                ext = ext ?? new Dictionary<string, object>();
                ext[JsonPropertyNames.CustomMultipleLines] = multipleLines.Value;
            }

            return ext;
        }

        protected virtual LinkRepresentation CreateActionLink() {
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

        private static bool InlineDetails(ActionContextFacade actionContext, RestControlFlags flags) {
            return flags.InlineDetailsInActionMemberRepresentations || actionContext.Action.RenderEagerly;
        }

        public static AbstractActionRepresentationStrategy GetStrategy(bool inline, IOidStrategy oidStrategy, HttpRequestMessage req, ActionContextFacade actionContext, RestControlFlags flags) {
            AbstractActionRepresentationStrategy strategy;
            if (inline) {
                if (actionContext.Target.IsViewModelEditView) {
                    strategy = new FormActionMemberRepresentationStrategy(oidStrategy, req, actionContext, flags);
                }
                else if (InlineDetails(actionContext, flags)) {
                    strategy = new ActionMemberWithDetailsRepresentationStrategy(oidStrategy, req, actionContext, flags);
                }
                else {
                    strategy = new ActionMemberRepresentationStrategy(oidStrategy, req, actionContext, flags);
                }
            }
            else {
                if (actionContext.Target.IsViewModelEditView) {
                    strategy = new FormActionRepresentationStrategy(oidStrategy, req, actionContext, flags);
                }
                else {
                    strategy = new ActionRepresentationStrategy(oidStrategy, req, actionContext, flags);
                }
            }

            strategy.CreateParameters();

            return strategy;
        }
    }
}