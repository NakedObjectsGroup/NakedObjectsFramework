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

    public override bool Equals(object obj) => obj is ActionFacade af && Equals(af);

    private bool Equals(ActionFacade other) => other is not null && (ReferenceEquals(this, other) || Equals(other.WrappedSpec, WrappedSpec));

    public override int GetHashCode() => WrappedSpec.GetHashCode();

    #region IActionFacade Members

    private bool? cachedIsContributed;
    private string cachedName;
    private string cachedDescription;
    private bool? cachedIsQueryOnly;
    private bool? cachedIsStatic;
    private bool? cachedIsStaticObjectMenu;
    private bool? cachedIsIdempotent;
    private bool? cachedIsQueryContributedAction;
    private string[] cachedCreateNewProperties;
    private string[] cachedEditProperties;
    private int? cachedMemberOrder;
    private string cachedMemberOrderName;
    private TypeFacade cachedReturnType;

    private NullCache<TypeFacade> cachedElementType;
    private int? cachedParameterCount;
    private IActionParameterFacade[] cachedParameters;
    private bool? cachedIsVisible;
    private IConsentFacade cachedIsUsable;
    private string cachedFinderMethodPrefix;
    private TypeFacade cachedOnType;
    private bool? cachedRenderEagerly;
    private int? cachedPageSize;

    private NullCache<int?> cachedNumberOfLines;
    private NullCache<(bool title, string[] columns)?> cachedTableViewData;
    private NullCache<string> cachedPresentationHint;
    private NullCache<(string, string)?> cachedRestExtension;

    public bool IsContributed => cachedIsContributed ??= WrappedSpec.IsContributedMethod;

    public string Name(IObjectFacade objectFacade) => cachedName ??= WrappedSpec.Name(objectFacade.WrappedAdapter());

    public string Description(IObjectFacade objectFacade) => cachedDescription ??= WrappedSpec.Description(objectFacade.WrappedAdapter());

    public bool IsQueryOnly => cachedIsQueryOnly ??= WrappedSpec.ReturnSpec.IsQueryable || WrappedSpec.ContainsFacet<IQueryOnlyFacet>();

    public bool IsStatic => cachedIsStatic ??= WrappedSpec.IsStaticFunction;

    public bool IsStaticObjectMenu => cachedIsStaticObjectMenu ??= WrappedSpec.IsStaticFunction && WrappedSpec.ContainsFacet<IStaticMenuFunctionFacet>();

    public bool IsIdempotent => cachedIsIdempotent ??= WrappedSpec.ContainsFacet<IIdempotentFacet>();

    public bool IsQueryContributedAction => cachedIsQueryContributedAction ??= WrappedSpec.ContainsFacet<IContributedToCollectionFacet>();

    public string[] CreateNewProperties(IObjectFacade objectFacade) => cachedCreateNewProperties ??= WrappedSpec.GetFacet<ICreateNewFacet>()?.OrderedProperties(objectFacade.WrappedAdapter(), framework) ?? Array.Empty<string>();

    public string[] EditProperties => cachedEditProperties ??= WrappedSpec.GetFacet<IEditPropertiesFacet>()?.Properties ?? Array.Empty<string>();

    public int MemberOrder => cachedMemberOrder ??= WrappedSpec.GetMemberOrder();

    public string MemberOrderName => cachedMemberOrderName ??= WrappedSpec.GetMemberOrderName();

    public string Id => WrappedSpec.Id;

    public ITypeFacade ReturnType => cachedReturnType ??= new TypeFacade(WrappedSpec.ReturnSpec, FrameworkFacade, framework);

    public ITypeFacade ElementType => (cachedElementType ??= FacadeUtils.NullCache(WrappedSpec.ElementSpec is null ? null : new TypeFacade(WrappedSpec.ElementSpec, FrameworkFacade, framework))).Value;

    public int ParameterCount => cachedParameterCount ??= WrappedSpec.ParameterCount;

    public IActionParameterFacade[] Parameters => cachedParameters ??= WrappedSpec.Parameters.Select(p => new ActionParameterFacade(p, FrameworkFacade, framework)).Cast<IActionParameterFacade>().ToArray();

    public bool IsVisible(IObjectFacade objectFacade) => cachedIsVisible ??= WrappedSpec.IsVisible(((ObjectFacade)objectFacade)?.WrappedNakedObject);

    public IConsentFacade IsUsable(IObjectFacade objectFacade) => cachedIsUsable ??= new ConsentFacade(WrappedSpec.IsUsable(((ObjectFacade)objectFacade)?.WrappedNakedObject));

    public string FinderMethodPrefix => cachedFinderMethodPrefix ??= WrappedSpec.GetFinderMethodPrefix();

    public ITypeFacade OnType => cachedOnType ??= new TypeFacade(WrappedSpec.OnSpec, FrameworkFacade, framework);

    public IFrameworkFacade FrameworkFacade { get; set; }

    public bool RenderEagerly => cachedRenderEagerly ??= WrappedSpec.GetRenderEagerly();

    public (bool, string[])? TableViewData => (cachedTableViewData ??= FacadeUtils.NullCache(WrappedSpec.GetTableViewData())).Value;

    public int PageSize => cachedPageSize ??= WrappedSpec.GetFacet<IPageSizeFacet>().Value;

    public string PresentationHint => (cachedPresentationHint ??= FacadeUtils.NullCache(WrappedSpec.GetPresentationHint())).Value;

    public (string, string)? RestExtension => (cachedRestExtension ??= FacadeUtils.NullCache(WrappedSpec.GetRestExtension())).Value;

    public int? NumberOfLines => (cachedNumberOfLines ??= FacadeUtils.NullCache(WrappedSpec.GetNumberOfLines())).Value;

    #endregion
}