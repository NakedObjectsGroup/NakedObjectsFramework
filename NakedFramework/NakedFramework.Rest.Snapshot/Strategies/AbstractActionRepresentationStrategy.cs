// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using NakedFramework.Facade.Contexts;
using NakedFramework.Facade.Interface;
using NakedFramework.Facade.Translation;
using NakedFramework.Rest.Snapshot.Constants;
using NakedFramework.Rest.Snapshot.RelTypes;
using NakedFramework.Rest.Snapshot.Representation;
using NakedFramework.Rest.Snapshot.Utility;

namespace NakedFramework.Rest.Snapshot.Strategies;

public abstract class AbstractActionRepresentationStrategy : AbstractStrategy {
    private readonly UriMtHelper helper;
    private readonly RelType self;
    private IEnumerable<ParameterRepresentation> parameterList;

    protected AbstractActionRepresentationStrategy(IOidStrategy oidStrategy, HttpRequest req, ActionContextFacade actionContext, RestControlFlags flags)
        : base(oidStrategy, flags) {
        Req = req;
        ActionContext = actionContext;
        helper = new UriMtHelper(oidStrategy, req, actionContext);
        self = new MemberRelType(RelValues.Self, helper);
    }

    protected ActionContextFacade ActionContext { get; }

    protected HttpRequest Req { get; }

    public virtual bool ShowParameters() => true;

    public virtual void CreateParameters() => parameterList = GetParameterList();

    public virtual RestControlFlags GetFlags() => Flags;

    public virtual IObjectFacade GetTarget() => ActionContext.Target;

    public virtual RelType GetSelf() => self;

    public virtual string GetId() => ActionContext.Action.Id;

    private ParameterRepresentation GetParameter(ParameterContextFacade parameterContext) {
        var objectFacade = ActionContext.Target;
        return ParameterRepresentation.Create(OidStrategy, Req, objectFacade, parameterContext, Flags);
    }

    protected virtual IEnumerable<ParameterRepresentation> GetParameterList() => ActionContext.VisibleParameters.Select(GetParameter);

    public virtual MapRepresentation GetParameters() => RestUtils.CreateMap(parameterList.ToDictionary(p => p.Name, p => (object)p));

    protected LinkRepresentation CreateDetailsLink() => LinkRepresentation.Create(OidStrategy, new MemberRelType(helper), Flags);

    public abstract LinkRepresentation[] GetLinks();

    protected LinkRepresentation CreateUpLink() {
        RelType parentRelType;

        if (ActionContext.Target is not null) {
            var parentHelper = new UriMtHelper(OidStrategy, Req, ActionContext.Target);
            parentRelType = ActionContext.Target.Specification.IsService ? new ServiceRelType(RelValues.Up, parentHelper) : new ObjectRelType(RelValues.Up, parentHelper);
        }
        else if (ActionContext.MenuId is not null) {
            var parentHelper = new UriMtHelper(OidStrategy, Req, new MenuIdHolder(ActionContext.MenuId));
            parentRelType = new MenuRelType(RelValues.Up, parentHelper);
        }
        else {
            // no parent as such create uplink to home 
            parentRelType = new HomePageRelType(RelValues.Up, new UriMtHelper(OidStrategy, Req));
        }

        return LinkRepresentation.Create(OidStrategy, parentRelType, Flags);
    }

    protected LinkRepresentation CreateSelfLink() => LinkRepresentation.Create(OidStrategy, self, Flags);

    protected virtual bool HasParams() => ActionContext.VisibleParameters.Any();

    protected override MapRepresentation GetExtensionsForSimple(IObjectFacade objectFacade) =>
        RestUtils.GetExtensions(
            ActionContext.Action.Name,
            ActionContext.Action.Description(objectFacade),
            null,
            null,
            null,
            HasParams(),
            null,
            null,
            null,
            ActionContext.Action.MemberOrder,
            null,
            ActionContext.Action.PresentationHint,
            GetCustomActionExtensions(),
            ActionContext.Action.ReturnType,
            ActionContext.Action.ElementType,
            OidStrategy,
            false);

    private IDictionary<string, object> GetCustomActionExtensions() {
        var ext = GetTableViewCustomExtensions(ActionContext.Action.TableViewData);

        if (!string.IsNullOrEmpty(ActionContext.MenuPath)) {
            ext ??= new Dictionary<string, object>();
            ext[JsonPropertyNames.CustomMenuPath] = ActionContext.MenuPath;
        }

        if (ActionContext.Action.RenderEagerly) {
            ext ??= new Dictionary<string, object>();
            ext[JsonPropertyNames.CustomRenderEagerly] = true;
        }

        var multipleLines = ActionContext.Action.NumberOfLines;

        if (multipleLines.HasValue) {
            ext ??= new Dictionary<string, object>();
            ext[JsonPropertyNames.CustomMultipleLines] = multipleLines.Value;
        }

        var createNewProperties = ActionContext.Action.CreateNewProperties;

        if (createNewProperties.Any()) {
            ext ??= new Dictionary<string, object>();
            ext[JsonPropertyNames.CustomCreateNew] = string.Join(',', createNewProperties);
        }

        var editProperties = ActionContext.Action.EditProperties;

        if (editProperties.Any()) {
            ext ??= new Dictionary<string, object>();
            ext[JsonPropertyNames.CustomEditProperties] = string.Join(',', editProperties);
        }

        var finderMethodPrefix = ActionContext.Action.FinderMethodPrefix;

        if (finderMethodPrefix is not null) {
            ext ??= new Dictionary<string, object>();
            ext[JsonPropertyNames.CustomFinderAction] = finderMethodPrefix;
        }

        return ext;
    }

    protected virtual LinkRepresentation CreateActionLink() {
        var optionalProperties = parameterList.Select(pr => new OptionalProperty(pr.Name, MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.Value, null, typeof(object))))).ToList();

        var method = GetRelMethod();
        return LinkRepresentation.Create(OidStrategy, new InvokeRelType(new UriMtHelper(OidStrategy, Req, ActionContext)) { Method = method }, Flags,
                                         new OptionalProperty(JsonPropertyNames.Arguments, MapRepresentation.Create(optionalProperties.ToArray())));
    }

    private RelMethod GetRelMethod() {
        if (ActionContext.Action.IsQueryOnly) {
            return RelMethod.Get;
        }

        return ActionContext.Action.IsIdempotent ? RelMethod.Put : RelMethod.Post;
    }

    private static bool InlineDetails(ActionContextFacade actionContext, RestControlFlags flags) => flags.InlineDetailsInActionMemberRepresentations || actionContext.Action.RenderEagerly;

    public static AbstractActionRepresentationStrategy GetStrategy(bool inline, IOidStrategy oidStrategy, HttpRequest req, ActionContextFacade actionContext, RestControlFlags flags) {
        AbstractActionRepresentationStrategy strategy;
        if (inline) {
            if (actionContext.Target?.IsViewModelEditView == true) {
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
            if (actionContext.Target?.IsViewModelEditView == true) {
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