// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.Spec;
using NakedFramework.Facade.Contexts;
using NakedFramework.Facade.Impl.Utility;
using NakedFramework.Facade.Interface;

namespace NakedFramework.Facade.Impl.Impl;

public class AssociationFacade : AbstractCommonFacade, IAssociationFacade {
    private readonly INakedFramework framework;

    public AssociationFacade(IAssociationSpec assoc, IFrameworkFacade frameworkFacade, INakedFramework framework) : base(assoc) {
        WrappedAssocSpec = assoc ?? throw new NullReferenceException($"{nameof(assoc)} is null");
        this.framework = framework ?? throw new NullReferenceException($"{nameof(framework)} is null");
        FrameworkFacade = frameworkFacade ?? throw new NullReferenceException($"{nameof(frameworkFacade)} is null");
    }

    public IAssociationSpec WrappedAssocSpec { get; }

    private (string, ITypeFacade) WrapChoiceParm((string name, IObjectSpec spec) parm) => (parm.name, new TypeFacade(parm.spec, FrameworkFacade, framework));

    public override bool Equals(object obj) => obj is AssociationFacade af && Equals(af);

    public bool Equals(AssociationFacade other) {
        if (ReferenceEquals(null, other)) {
            return false;
        }

        return ReferenceEquals(this, other) || Equals(other.WrappedAssocSpec, WrappedAssocSpec);
    }

    public override int GetHashCode() =>  WrappedAssocSpec.GetHashCode();

    #region IAssociationFacade Members


    public bool IsCollection => WrappedAssocSpec is IOneToManyAssociationSpec;

    public bool IsObject => WrappedAssocSpec is IOneToOneAssociationSpec;

    public bool IsConcurrency => WrappedAssocSpec.ContainsFacet<IConcurrencyCheckFacet>();

    public bool NotNavigable => WrappedAssocSpec.ContainsFacet<INotNavigableFacet>();

    public int MemberOrder => WrappedAssocSpec.GetMemberOrder();

    public bool IsASet => WrappedAssocSpec is IOneToManyAssociationSpec { IsASet: true };

    public bool IsInline => WrappedAssocSpec.IsInline;

    public string Mask => WrappedAssocSpec.GetMask();

    public int AutoCompleteMinLength => WrappedAssocSpec.GetAutoCompleteMinLength();

    public ITypeFacade Specification => new TypeFacade(WrappedAssocSpec.ReturnSpec, FrameworkFacade, framework);

    public ITypeFacade ElementSpecification {
        get {
            var coll = WrappedAssocSpec as IOneToManyAssociationSpec;
            var elementSpec = coll?.ElementSpec;
            return elementSpec == null ? null : new TypeFacade(elementSpec, FrameworkFacade, framework);
        }
    }

    public string Id => WrappedAssocSpec.Id;

    public Choices IsChoicesEnabled(IObjectFacade objectFacade) {
        if (WrappedAssocSpec is IOneToOneFeatureSpec fs) {
            if (fs.IsChoicesEnabled(objectFacade.WrappedAdapter())) {
                return Choices.Single;
            }
        }

        return Choices.NotEnabled;
    }

    public bool IsAutoCompleteEnabled => WrappedAssocSpec is IOneToOneFeatureSpec { IsAutoCompleteEnabled: true };

    public IConsentFacade IsUsable(IObjectFacade target) {
        var consent = WrappedAssocSpec.IsUsable(((ObjectFacade)target).WrappedNakedObject);
        return new ConsentFacade(consent);
    }

    public IObjectFacade GetValue(IObjectFacade target) {
        var result = WrappedAssocSpec.GetNakedObject(((ObjectFacade)target).WrappedNakedObject);
        return ObjectFacade.Wrap(result, FrameworkFacade, framework);
    }

    public bool IsVisible(IObjectFacade objectFacade) => WrappedAssocSpec.IsVisible(((ObjectFacade)objectFacade).WrappedNakedObject);

    public bool IsEager(IObjectFacade objectFacade) =>
        ((TypeFacade)objectFacade.Specification).WrappedValue.ContainsFacet<IEagerlyFacet>() ||
        WrappedAssocSpec.ContainsFacet<IEagerlyFacet>();

    public DataType? DataType => WrappedAssocSpec.GetFacet<IDataTypeFacet>()?.DataType() ?? WrappedAssocSpec.GetFacet<IPasswordFacet>()?.DataType;

    public IObjectFacade[] GetChoices(IObjectFacade target, IDictionary<string, object> parameterNameValues) {
        var oneToOneFeature = WrappedAssocSpec as IOneToOneFeatureSpec;
        var pnv = parameterNameValues?.ToDictionary(kvp => kvp.Key, kvp => framework.GetNakedObject(kvp.Value));
        return oneToOneFeature?.GetChoices(((ObjectFacade)target).WrappedNakedObject, pnv).Select(no => ObjectFacade.Wrap(no, FrameworkFacade, framework)).Cast<IObjectFacade>().ToArray();
    }

    public (string, ITypeFacade)[] GetChoicesParameters() =>
        WrappedAssocSpec is IOneToOneFeatureSpec oneToOneFeature
            ? oneToOneFeature.GetChoicesParameters().Select(WrapChoiceParm).ToArray()
            : Array.Empty<(string, ITypeFacade)>();

    public (IObjectFacade, string)[] GetChoicesAndTitles(IObjectFacade target, IDictionary<string, object> parameterNameValues) =>
        GetChoices(target, parameterNameValues).Select(c => (c, c.TitleString)).ToArray();

    public IObjectFacade[] GetCompletions(IObjectFacade target, string autoCompleteParm) {
        var oneToOneFeature = WrappedAssocSpec as IOneToOneFeatureSpec;
        return oneToOneFeature?.GetCompletions(((ObjectFacade)target).WrappedNakedObject, autoCompleteParm).Select(no => ObjectFacade.Wrap(no, FrameworkFacade, framework)).Cast<IObjectFacade>().ToArray();
    }

    public int Count(IObjectFacade target) => IsCollection ? framework.Persistor.CountField(((ObjectFacade)target).WrappedNakedObject, Id) : 0;

    public bool IsSetToImplicitDefault(IObjectFacade objectFacade) {
        // return true if it's scalar and and still set to its implicit default value (eg 0 for an int)
        if (!DefaultTypeIsExplicit(objectFacade) && WrappedAssocSpec.ReturnSpec.IsParseable) {
            var dflt = WrappedAssocSpec.GetDefault(objectFacade.WrappedAdapter());
            var currentValue = GetValue(objectFacade);

            return dflt?.Object == currentValue?.Object;
        }

        return false;
    }

    public string GetTitle(IObjectFacade objectFacade) {
        var enumFacet = WrappedAssocSpec.GetFacet<IEnumFacet>();

        if (enumFacet != null) {
            return enumFacet.GetTitle(((ObjectFacade)objectFacade).WrappedNakedObject);
        }

        var mask = WrappedAssocSpec.GetFacet<IMaskFacet>();
        if (mask == null) {
            return objectFacade.TitleString;
        }

        var titleFacet = ((TypeFacade)objectFacade.Specification).WrappedValue.GetFacet<ITitleFacet>();
        return titleFacet.GetTitleWithMask(mask.Value, ((ObjectFacade)objectFacade).WrappedNakedObject, framework);
    }

    public IFrameworkFacade FrameworkFacade { get; set; }

    public bool IsFile => WrappedAssocSpec.IsFile(framework);

    public bool IsEnum => WrappedAssocSpec.ContainsFacet<IEnumFacet>();

    public bool IsFindMenuEnabled => WrappedAssocSpec is IOneToOneAssociationSpec { IsFindMenuEnabled: true };

    public bool DoNotCount => WrappedAssocSpec.ContainsFacet<INotCountedFacet>();

    public string GetMaskedValue(IObjectFacade objectFacade) => WrappedAssocSpec.GetMaskedValue(objectFacade, framework);

    public bool DefaultTypeIsExplicit(IObjectFacade objectFacade) {
        var no = ((ObjectFacade)objectFacade).WrappedNakedObject;
        return WrappedAssocSpec.GetDefaultType(no) == TypeOfDefaultValue.Explicit;
    }

    public (bool, string[])? TableViewData => WrappedAssocSpec.GetTableViewData();

    public bool RenderEagerly => WrappedAssocSpec.GetRenderEagerly();

    public string Grouping => WrappedAssocSpec.GetFacet<IMemberOrderFacet>()?.Grouping ?? "";

    #endregion
}