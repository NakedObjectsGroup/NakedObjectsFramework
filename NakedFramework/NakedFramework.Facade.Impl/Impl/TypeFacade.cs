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
using NakedFramework.Core.Util;
using NakedFramework.Facade.Impl.Utility;
using NakedFramework.Facade.Interface;
using NakedFramework.Value;

namespace NakedFramework.Facade.Impl.Impl;

public class TypeFacade : ITypeFacade {
    private readonly INakedFramework framework;

    public TypeFacade(ITypeSpec spec, IFrameworkFacade frameworkFacade, INakedFramework framework) {
        FrameworkFacade = frameworkFacade ?? throw new NullReferenceException($"{nameof(frameworkFacade)} is null");
        WrappedValue = spec ?? throw new NullReferenceException($"{nameof(spec)} is null");
        this.framework = framework ?? throw new NullReferenceException($"{nameof(framework)} is null");
    }

    public ITypeSpec WrappedValue { get; }

    public override bool Equals(object obj) => obj is TypeFacade tf && Equals(tf);

    public bool Equals(TypeFacade other) {
        if (other is null) {
            return false;
        }

        return ReferenceEquals(this, other) || Equals(other.WrappedValue, WrappedValue);
    }

    public override int GetHashCode() => WrappedValue.GetHashCode();

    #region ITypeFacade Members

    private bool? cachedIsParseable;
    private bool? cachedIsQueryable;
    private bool? cachedIsVoid;
    private bool? cachedIsASet;
    private bool? cachedIsAggregated;
    private bool? cachedIsStatic;
    private bool? cachedIsImage;
    private bool? cachedIsFileAttachment;
    private bool? cachedIsFile;
    private bool? cachedIsDate;
    private bool? cachedIsTime;
    private bool? cachedIsNumber;
    private bool? cachedIsEnum;
    private bool? cachedIsBoolean;
    private bool? cachedIsCollection;
    private bool? cachedIsObject;
    private bool? cachedIsAlwaysImmutable;
    private bool? cachedIsImmutableOncePersisted;
    private bool? cachedIsComplexType;
    private bool? cachedIsStream;
    private string cachedFullName;
    private string cachedShortName;
    private string cachedSingularName;
    private string cachedPluralName;
    private string cachedDescription;
    private IAssociationFacade[] cachedProperties;
    private MenuFacade cachedMenu;
    private IActionFacade[] cachedActionLeafNodes;
    private IActionFacade[] cachedCollectionContributedActions;
    private Type cachedUnderlyingType;
    private string cachedPresentationHint;

    public bool IsParseable => cachedIsParseable ??= WrappedValue.IsParseable;

    public bool IsQueryable => cachedIsQueryable ??= WrappedValue.IsQueryable;

    public bool IsService => WrappedValue is IServiceSpec;

    public bool IsVoid => cachedIsVoid ??= WrappedValue.IsVoid;

    public bool IsASet => cachedIsASet ??= WrappedValue.IsASet;

    public bool IsAggregated => cachedIsAggregated ??= WrappedValue.IsAggregated;

    public bool IsStatic => cachedIsStatic ??= WrappedValue.IsStatic;

    public bool IsImage => cachedIsImage ??= WrappedValue.IsOfType(framework.MetamodelManager.GetSpecification(typeof(Image)));

    public bool IsFileAttachment => cachedIsFileAttachment ??= WrappedValue.IsOfType(framework.MetamodelManager.GetSpecification(typeof(FileAttachment)));

    public bool IsFile => cachedIsFile ??= WrappedValue.IsFile(framework);

    public bool IsDateTime => FullName == "System.DateTime";

    public bool IsDate => cachedIsDate ??= WrappedValue.ContainsFacet<IDateValueFacet>();

    public bool IsTime => cachedIsTime ??= WrappedValue.ContainsFacet<ITimeValueFacet>();

    public bool IsNumber => cachedIsNumber ??= WrappedValue.ContainsFacet<IIntegerValueFacet>();

    public bool IsEnum => cachedIsEnum ??= WrappedValue.ContainsFacet<IEnumValueFacet>();

    public bool IsBoolean => cachedIsBoolean ??= WrappedValue.ContainsFacet<IBooleanValueFacet>();

    public bool IsCollection => cachedIsCollection ??= WrappedValue.IsCollection && !WrappedValue.IsParseable;

    public bool IsObject => cachedIsObject ??= WrappedValue.IsObject;

    public bool IsAlwaysImmutable => cachedIsAlwaysImmutable ??= WrappedValue.GetFacet<IImmutableFacet>() is { Value: WhenTo.Always };

    public bool IsImmutableOncePersisted => cachedIsImmutableOncePersisted ??= WrappedValue.GetFacet<IImmutableFacet>() is { Value: WhenTo.OncePersisted };

    public bool IsComplexType => cachedIsComplexType ??= WrappedValue.ContainsFacet<IComplexTypeFacet>();

    public bool IsStream => cachedIsStream ??= WrappedValue.ContainsFacet<IFromStreamFacet>();

    public string FullName => cachedFullName ??= WrappedValue.FullName;

    public string ShortName => cachedShortName ??= WrappedValue.ShortName;

    public string SingularName => cachedSingularName ??= WrappedValue.SingularName;

    public string PluralName => cachedPluralName ??= WrappedValue.PluralName;

    public string Description(IObjectFacade objectFacade) => cachedDescription ??= WrappedValue.Description(objectFacade.WrappedAdapter());

    public IAssociationFacade[] Properties =>
        cachedProperties ??= WrappedValue is IObjectSpec objectSpec
            ? objectSpec.Properties.Select(p => new AssociationFacade(p, FrameworkFacade, framework)).Cast<IAssociationFacade>().ToArray()
            : Array.Empty<IAssociationFacade>();

    public IMenuFacade Menu => cachedMenu ??= new MenuFacade(WrappedValue.Menu, FrameworkFacade, framework);

    public bool IsImmutable(IObjectFacade objectFacade) => IsAlwaysImmutable || WrappedValue.IsImmutableOncePersisted() && !objectFacade.IsTransient;

    public IActionFacade[] GetActionLeafNodes() =>
        cachedActionLeafNodes ??= FacadeUtils.GetActionsFromSpec(WrappedValue).Select(a => new ActionFacade(a, FrameworkFacade, framework)).Cast<IActionFacade>().ToArray();

    public ITypeFacade GetElementType(IObjectFacade objectFacade) {
        ITypeFacade ElementType() {
            var introspectableSpecification = WrappedValue.GetFacet<ITypeOfFacet>().GetValueSpec(((ObjectFacade)objectFacade).WrappedNakedObject, framework.MetamodelManager.Metamodel);
            var elementSpec = framework.MetamodelManager.GetSpecification(introspectableSpecification);
            return new TypeFacade(elementSpec, FrameworkFacade, framework);
        }

        return IsCollection ? ElementType() : null;
    }

    public bool IsOfType(ITypeFacade otherSpec) => WrappedValue.IsOfType(((TypeFacade)otherSpec).WrappedValue);

    public Type GetUnderlyingType() => cachedUnderlyingType ??= WrappedValue.GetFacet<ITypeFacet>().TypeOrUnderlyingType;

    public IActionFacade[] GetCollectionContributedActions() =>
        cachedCollectionContributedActions ??= WrappedValue is IObjectSpec objectSpec ? objectSpec.GetCollectionContributedActions().Select(a => new ActionFacade(a, FrameworkFacade, framework)).Cast<IActionFacade>().ToArray() : Array.Empty<IActionFacade>();

    public IActionFacade[] GetLocallyContributedActions(ITypeFacade typeFacade, string id) {
        if (WrappedValue is IObjectSpec objectSpec) {
            return objectSpec.GetLocallyContributedActions(((TypeFacade)typeFacade).WrappedValue, id).Select(a => new ActionFacade(a, FrameworkFacade, framework)).Cast<IActionFacade>().ToArray();
        }

        return Array.Empty<IActionFacade>();
    }

    public IFrameworkFacade FrameworkFacade { get; set; }

    public bool Equals(ITypeFacade other) => Equals((object)other);

    public string PresentationHint =>
        cachedPresentationHint ??= WrappedValue.GetFacet<IPresentationHintFacet>()?.Value ?? "";

    #endregion
}