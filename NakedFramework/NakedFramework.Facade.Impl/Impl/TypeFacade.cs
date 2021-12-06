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
        if (ReferenceEquals(null, other)) {
            return false;
        }

        return ReferenceEquals(this, other) || Equals(other.WrappedValue, WrappedValue);
    }

    public override int GetHashCode() => WrappedValue != null ? WrappedValue.GetHashCode() : 0;

    #region ITypeFacade Members

    public bool IsParseable => WrappedValue.IsParseable;

    public bool IsQueryable => WrappedValue.IsQueryable;

    public bool IsService => WrappedValue is IServiceSpec;

    public bool IsVoid => WrappedValue.IsVoid;

    public bool IsASet => WrappedValue.IsASet;

    public bool IsAggregated => WrappedValue.IsAggregated;

    public bool IsStatic => WrappedValue.IsStatic;

    public bool IsImage {
        get {
            var imageSpec = framework.MetamodelManager.GetSpecification(typeof(Image));
            return WrappedValue.IsOfType(imageSpec);
        }
    }

    public bool IsFileAttachment {
        get {
            var fileSpec = framework.MetamodelManager.GetSpecification(typeof(FileAttachment));
            return WrappedValue.IsOfType(fileSpec);
        }
    }

    public bool IsFile => WrappedValue.IsFile(framework);

    public bool IsDateTime => FullName == "System.DateTime";

    public bool IsDate => WrappedValue.ContainsFacet<IDateValueFacet>();

    public bool IsTime => WrappedValue.ContainsFacet<ITimeValueFacet>();

    public bool IsNumber => WrappedValue.ContainsFacet<IIntegerValueFacet>();

    public string FullName => WrappedValue.FullName;

    public string ShortName => WrappedValue.ShortName;

    public bool IsCollection => WrappedValue.IsCollection && !WrappedValue.IsParseable;

    public bool IsObject => WrappedValue.IsObject;

    public string SingularName => WrappedValue.SingularName;

    public string PluralName => WrappedValue.PluralName;

    public string Description => WrappedValue.Description;

    public bool IsEnum => WrappedValue.ContainsFacet<IEnumValueFacet>();

    public bool IsBoolean => WrappedValue.ContainsFacet<IBooleanValueFacet>();

    public bool IsAlwaysImmutable {
        get {
            var facet = WrappedValue.GetFacet<IImmutableFacet>();
            return facet is { Value: WhenTo.Always };
        }
    }

    public bool IsImmutableOncePersisted {
        get {
            var facet = WrappedValue.GetFacet<IImmutableFacet>();
            return facet is { Value: WhenTo.OncePersisted };
        }
    }

    public bool IsComplexType => WrappedValue.ContainsFacet<IComplexTypeFacet>();

    public IAssociationFacade[] Properties =>
        WrappedValue is IObjectSpec objectSpec
            ? objectSpec.Properties.Select(p => new AssociationFacade(p, FrameworkFacade, framework)).Cast<IAssociationFacade>().ToArray()
            : Array.Empty<IAssociationFacade>();

    public IMenuFacade Menu => new MenuFacade(WrappedValue.Menu, FrameworkFacade, framework);

    public bool IsImmutable(IObjectFacade objectFacade) => WrappedValue.IsAlwaysImmutable() || WrappedValue.IsImmutableOncePersisted() && !objectFacade.IsTransient;

    public IActionFacade[] GetActionLeafNodes() {
        var actionSpecs = FacadeUtils.GetActionsFromSpec(WrappedValue);
        return actionSpecs.Select(a => new ActionFacade(a, FrameworkFacade, framework)).Cast<IActionFacade>().ToArray();
    }

    public ITypeFacade GetElementType(IObjectFacade objectFacade) {
        if (IsCollection) {
            var introspectableSpecification = WrappedValue.GetFacet<ITypeOfFacet>().GetValueSpec(((ObjectFacade)objectFacade).WrappedNakedObject, framework.MetamodelManager.Metamodel);
            var elementSpec = framework.MetamodelManager.GetSpecification(introspectableSpecification);
            return new TypeFacade(elementSpec, FrameworkFacade, framework);
        }

        return null;
    }

    public bool IsOfType(ITypeFacade otherSpec) => WrappedValue.IsOfType(((TypeFacade)otherSpec).WrappedValue);

    public Type GetUnderlyingType() =>  WrappedValue.GetFacet<ITypeFacet>().TypeOrUnderlyingType;

    public IActionFacade[] GetCollectionContributedActions() {
        if (WrappedValue is IObjectSpec objectSpec) {
            return objectSpec.GetCollectionContributedActions().Select(a => new ActionFacade(a, FrameworkFacade, framework)).Cast<IActionFacade>().ToArray();
        }

        return Array.Empty<IActionFacade>();
    }

    public IActionFacade[] GetLocallyContributedActions(ITypeFacade typeFacade, string id) {
        if (WrappedValue is IObjectSpec objectSpec) {
            return objectSpec.GetLocallyContributedActions(((TypeFacade)typeFacade).WrappedValue, id).Select(a => new ActionFacade(a, FrameworkFacade, framework)).Cast<IActionFacade>().ToArray();
        }

        return Array.Empty<IActionFacade>();
    }

    public IFrameworkFacade FrameworkFacade { get; set; }

    public bool Equals(ITypeFacade other) => Equals((object)other);

    public string PresentationHint {
        get {
            var hintFacet = WrappedValue.GetFacet<IPresentationHintFacet>();
            return hintFacet == null ? "" : hintFacet.Value;
        }
    }

    public bool IsStream => WrappedValue.ContainsFacet<IFromStreamFacet>();

    #endregion
}