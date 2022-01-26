// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.Spec;
using NakedFramework.Facade.Impl.Utility;
using NakedFramework.Facade.Interface;

namespace NakedFramework.Facade.Impl.Impl;

public class ActionFacade : IActionFacade {
    private readonly INakedFramework framework;

    public ActionFacade(IActionSpec action, IFrameworkFacade frameworkFacade, INakedFramework framework) {
        WrappedSpec = action ?? throw new NullReferenceException($"{nameof(action)} is null");
        this.framework = framework ?? throw new NullReferenceException($"{nameof(framework)} is null");
        FrameworkFacade = frameworkFacade ?? throw new NullReferenceException($"{nameof(frameworkFacade)} is null");
    }

    public IActionSpec WrappedSpec { get; }

    public ITypeFacade Specification => new TypeFacade(WrappedSpec.ReturnSpec, FrameworkFacade, framework);

    public override bool Equals(object obj) => obj is ActionFacade af && Equals(af);

    private bool Equals(ActionFacade other) => other is not null && (ReferenceEquals(this, other) || Equals(other.WrappedSpec, WrappedSpec));

    public override int GetHashCode() => WrappedSpec != null ? WrappedSpec.GetHashCode() : 0;

    #region IActionFacade Members

    public bool IsContributed => WrappedSpec.IsContributedMethod;

    public string Name(IObjectFacade objectFacade) => WrappedSpec.Name(objectFacade.WrappedAdapter());

    public string Description(IObjectFacade objectFacade) => WrappedSpec.Description(objectFacade.WrappedAdapter());

    public bool IsQueryOnly => WrappedSpec.ReturnSpec.IsQueryable || WrappedSpec.ContainsFacet<IQueryOnlyFacet>();

    public bool IsStatic => WrappedSpec.IsStaticFunction;

    public bool IsStaticObjectMenu => WrappedSpec.IsStaticFunction && WrappedSpec.ContainsFacet<IStaticMenuFunctionFacet>();

    public bool IsIdempotent => WrappedSpec.ContainsFacet<IIdempotentFacet>();

    public string[] CreateNewProperties(IObjectFacade objectFacade) => WrappedSpec.GetFacet<ICreateNewFacet>()?.OrderedProperties(objectFacade.WrappedAdapter(), framework) ?? Array.Empty<string>();

    public bool IsQueryContributedAction => WrappedSpec.GetFacet<IContributedFunctionFacet>()?.IsContributedToCollection == true;

    public string[] EditProperties => WrappedSpec.GetFacet<IEditPropertiesFacet>()?.Properties ?? Array.Empty<string>();

    public int MemberOrder => WrappedSpec.GetMemberOrder();

    public string MemberOrderName => WrappedSpec.GetMemberOrderName();

    public string Id => WrappedSpec.Id;

    public ITypeFacade ReturnType => new TypeFacade(WrappedSpec.ReturnSpec, FrameworkFacade, framework);

    public ITypeFacade ElementType {
        get {
            var elementSpec = WrappedSpec.ElementSpec;
            return elementSpec is null ? null : new TypeFacade(elementSpec, FrameworkFacade, framework);
        }
    }

    public int ParameterCount => WrappedSpec.ParameterCount;

    public IActionParameterFacade[] Parameters => WrappedSpec.Parameters.Select(p => new ActionParameterFacade(p, FrameworkFacade, framework)).Cast<IActionParameterFacade>().ToArray();

    public bool IsVisible(IObjectFacade objectFacade) => WrappedSpec.IsVisible(((ObjectFacade)objectFacade)?.WrappedNakedObject);

    public IConsentFacade IsUsable(IObjectFacade objectFacade) => new ConsentFacade(WrappedSpec.IsUsable(((ObjectFacade)objectFacade)?.WrappedNakedObject));

    public string FinderMethodPrefix => WrappedSpec.GetFinderMethodPrefix();

    public ITypeFacade OnType => new TypeFacade(WrappedSpec.OnSpec, FrameworkFacade, framework);

    public IFrameworkFacade FrameworkFacade { get; set; }

    public bool RenderEagerly => WrappedSpec.GetRenderEagerly();

    public (bool, string[])? TableViewData => WrappedSpec.GetTableViewData();

    public int PageSize => WrappedSpec.GetFacet<IPageSizeFacet>().Value;

    public string PresentationHint => WrappedSpec.GetPresentationHint();

    public (string, string)? RestExtension => WrappedSpec.GetRestExtension();

    public int? NumberOfLines => WrappedSpec.GetNumberOfLines();

    #endregion
}