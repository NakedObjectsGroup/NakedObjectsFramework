// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.Interactions;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Interactions;
using NakedFramework.Core.Resolve;
using NakedFramework.Core.Util;
using static NakedFramework.Core.Util.ToStringHelpers;

namespace NakedFramework.Core.Spec;

public sealed class OneToOneAssociationSpec : AssociationSpecAbstract, IOneToOneAssociationSpec {
    private bool? isFindMenuEnabled;

    public OneToOneAssociationSpec(IOneToOneAssociationSpecImmutable association, INakedFramework framework)
        : base(association, framework) { }

    public override IObjectSpec ElementSpec => null;

    private INakedObjectAdapter GetAssociation(INakedObjectAdapter fromObjectAdapter) {
        var obj = GetFacet<IPropertyAccessorFacet>().GetProperty(fromObjectAdapter, Framework);
        if (obj is null) {
            return null;
        }

        var spec = (IObjectSpec)Framework.MetamodelManager.GetSpecification(obj.GetType());
        return spec.ContainsFacet(typeof(IComplexTypeFacet))
            ? Framework.NakedObjectManager.CreateAggregatedAdapter(fromObjectAdapter, Id, obj)
            : Framework.NakedObjectManager.CreateAdapter(obj, null, null);
    }

    private (INakedObjectAdapter value, TypeOfDefaultValue type) GetDefaultObject(INakedObjectAdapter fromObjectAdapter) {
        var facet = this.GetOpFacet<IPropertyDefaultFacet>() ?? ReturnSpec.GetOpFacet<IDefaultedFacet>();

        var (domainObject, typeOfDefaultValue) = facet switch {
            IPropertyDefaultFacet pdf => (pdf.GetDefault(fromObjectAdapter), TypeOfDefaultValue.Explicit),
            IDefaultedFacet df when !IsNullable => (df.Default, TypeOfDefaultValue.Implicit),
            _ when fromObjectAdapter == null => (null, TypeOfDefaultValue.Implicit),
            _ when fromObjectAdapter.Object.GetType().IsValueType => (0, TypeOfDefaultValue.Implicit),
            _ => (null, TypeOfDefaultValue.Implicit)
        };

        return (Framework.NakedObjectManager.CreateAdapter(domainObject, null, null), typeOfDefaultValue);
    }

    public override string ToString() =>
        $"{NameAndHashCode(this)} [{base.ToString()},persisted={IsPersisted},type={ReturnSpec.ShortName}]";

    #region IOneToOneAssociationSpec Members

    public bool IsChoicesEnabled(INakedObjectAdapter adapter) => ReturnSpec.IsBoundedSet() || ContainsFacet<IPropertyChoicesFacet>() || ContainsFacet<IEnumFacet>();

    public override bool IsMandatory => GetFacet<IMandatoryFacet>().IsMandatory;

    public bool IsFindMenuEnabled {
        get {
            isFindMenuEnabled ??= ContainsFacet<IFindMenuFacet>();
            return isFindMenuEnabled.Value;
        }
    }

    public override INakedObjectAdapter GetNakedObject(INakedObjectAdapter fromObjectAdapter) => GetAssociation(fromObjectAdapter);

    public (string, IObjectSpec)[] GetChoicesParameters() {
        var propertyChoicesFacet = GetFacet<IPropertyChoicesFacet>();
        return propertyChoicesFacet is null
            ? Array.Empty<(string, IObjectSpec)>()
            : propertyChoicesFacet.ParameterNamesAndTypes.Select(t => {
                var (pName, pSpec) = t;
                return (pName, Framework.MetamodelManager.GetSpecification(pSpec));
            }).ToArray();
    }

    public INakedObjectAdapter[] GetChoices(INakedObjectAdapter target, IDictionary<string, INakedObjectAdapter> parameterNameValues) {
        var propertyChoicesFacet = GetFacet<IPropertyChoicesFacet>();
        var enumFacet = GetFacet<IEnumFacet>();

        var objectOptions = propertyChoicesFacet?.GetChoices(target, parameterNameValues);
        if (objectOptions is not null) {
            if (enumFacet is null) {
                return Framework.NakedObjectManager.GetCollectionOfAdaptedObjects(objectOptions).ToArray();
            }

            return Framework.NakedObjectManager.GetCollectionOfAdaptedObjects(enumFacet.GetChoices(target, objectOptions)).ToArray();
        }

        objectOptions = enumFacet?.GetChoices(target);
        if (objectOptions is not null) {
            return Framework.NakedObjectManager.GetCollectionOfAdaptedObjects(objectOptions).ToArray();
        }

        if (ReturnSpec.IsBoundedSet()) {
            return Framework.NakedObjectManager.GetCollectionOfAdaptedObjects(Framework.Persistor.GetBoundedSet(ReturnSpec)).ToArray();
        }

        return null;
    }

    public override INakedObjectAdapter[] GetCompletions(INakedObjectAdapter target, string autoCompleteParm) {
        var propertyAutoCompleteFacet = GetFacet<IAutoCompleteFacet>();
        return propertyAutoCompleteFacet is null ? null : Framework.NakedObjectManager.GetCollectionOfAdaptedObjects(propertyAutoCompleteFacet.GetCompletions(target, autoCompleteParm, Framework)).ToArray();
    }

    public void InitAssociation(INakedObjectAdapter inObjectAdapter, INakedObjectAdapter associate) {
        var initializerFacet = GetFacet<IPropertyInitializationFacet>();
        initializerFacet?.InitProperty(inObjectAdapter, associate);
    }

    public IConsent IsAssociationValid(INakedObjectAdapter inObjectAdapter, INakedObjectAdapter reference) {
        if (reference?.Spec.IsOfType(ReturnSpec) == false) {
            return GetConsent(string.Format(NakedObjects.Resources.NakedObjects.TypeMismatchError, ReturnSpec.SingularName));
        }

        if (!inObjectAdapter.ResolveState.IsNotPersistent()) {
            if (reference?.Spec.IsParseable == false && reference.ResolveState.IsNotPersistent()) {
                return GetConsent(NakedObjects.Resources.NakedObjects.TransientFieldMessage);
            }
        }

        var buf = new InteractionBuffer();
        IInteractionContext ic = InteractionContext.ModifyingPropParam(Framework, false, inObjectAdapter, Identifier, reference);
        InteractionUtils.IsValid(this, ic, buf);
        return InteractionUtils.IsValid(buf);
    }

    public override bool IsEmpty(INakedObjectAdapter inObjectAdapter) => GetAssociation(inObjectAdapter) is null;

    public override bool IsInline => ReturnSpec.ContainsFacet(typeof(IComplexTypeFacet));

    public override INakedObjectAdapter GetDefault(INakedObjectAdapter fromObjectAdapter) => GetDefaultObject(fromObjectAdapter).value;

    public override TypeOfDefaultValue GetDefaultType(INakedObjectAdapter fromObjectAdapter) => GetDefaultObject(fromObjectAdapter).type;

    public override void ToDefault(INakedObjectAdapter inObjectAdapter) {
        var defaultValue = GetDefault(inObjectAdapter);
        if (defaultValue is not null) {
            InitAssociation(inObjectAdapter, defaultValue);
        }
    }

    public void SetAssociation(INakedObjectAdapter inObjectAdapter, INakedObjectAdapter associate) {
        var currentValue = GetAssociation(inObjectAdapter);
        if (currentValue != associate) {
            var setterFacet = GetFacet<IPropertySetterFacet>();
            if (setterFacet is not null) {
                inObjectAdapter.ResolveState.CheckCanAssociate(associate);
                setterFacet.SetProperty(inObjectAdapter, associate, Framework);
            }
        }
    }

    #endregion
}

// Copyright (c) Naked Objects Group Ltd.