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
using System.Text.RegularExpressions;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.Spec;
using NakedFramework.Facade.Contexts;
using NakedFramework.Facade.Impl.Utility;
using NakedFramework.Facade.Interface;

namespace NakedFramework.Facade.Impl.Impl;

public class AssociationFacade : IAssociationFacade {
    private readonly INakedFramework framework;

    public AssociationFacade(IAssociationSpec assoc, IFrameworkFacade frameworkFacade, INakedFramework framework) {
        WrappedSpec = assoc ?? throw new NullReferenceException($"{nameof(assoc)} is null");
        this.framework = framework ?? throw new NullReferenceException($"{nameof(framework)} is null");
        FrameworkFacade = frameworkFacade ?? throw new NullReferenceException($"{nameof(frameworkFacade)} is null");
    }

    public IAssociationSpec WrappedSpec { get; }

    private (string, ITypeFacade) WrapChoiceParm((string name, IObjectSpec spec) parm) => (parm.name, new TypeFacade(parm.spec, FrameworkFacade, framework));

    public override bool Equals(object obj) => obj is AssociationFacade af && Equals(af);

    public bool Equals(AssociationFacade other) {
        if (ReferenceEquals(null, other)) {
            return false;
        }

        return ReferenceEquals(this, other) || Equals(other.WrappedSpec, WrappedSpec);
    }

    public override int GetHashCode() => WrappedSpec != null ? WrappedSpec.GetHashCode() : 0;

    #region IAssociationFacade Members

    public string Name(IObjectFacade objectFacade) => WrappedSpec.Name(objectFacade.WrappedAdapter());

    public string Description(IObjectFacade objectFacade) => WrappedSpec.Description(objectFacade.WrappedAdapter());

    public bool IsMandatory => WrappedSpec.IsMandatory;

    public bool IsCollection => WrappedSpec is IOneToManyAssociationSpec;

    public bool IsObject => WrappedSpec is IOneToOneAssociationSpec;

    public bool IsConcurrency => WrappedSpec.ContainsFacet<IConcurrencyCheckFacet>();

    public bool NotNavigable => WrappedSpec.ContainsFacet<INotNavigableFacet>();

    public int? MaxLength => WrappedSpec.GetMaxLength();

    public string Pattern => WrappedSpec.GetPattern();

    public int MemberOrder => WrappedSpec.GetMemberOrder();

    public bool IsASet => WrappedSpec is IOneToManyAssociationSpec { IsASet: true };

    public bool IsInline => WrappedSpec.IsInline;

    public string Mask => WrappedSpec.GetMask();

    public int AutoCompleteMinLength => WrappedSpec.GetAutoCompleteMinLength();

    public ITypeFacade Specification => new TypeFacade(WrappedSpec.ReturnSpec, FrameworkFacade, framework);

    public ITypeFacade ElementSpecification {
        get {
            var coll = WrappedSpec as IOneToManyAssociationSpec;
            var elementSpec = coll?.ElementSpec;
            return elementSpec == null ? null : new TypeFacade(elementSpec, FrameworkFacade, framework);
        }
    }

    public string Id => WrappedSpec.Id;

    public Choices IsChoicesEnabled(IObjectFacade objectFacade) {
        if (WrappedSpec is IOneToOneFeatureSpec fs) {
            if (fs.IsChoicesEnabled(objectFacade.WrappedAdapter())) {
                return Choices.Single;
            }
        }

        return Choices.NotEnabled;
    }

    public bool IsAutoCompleteEnabled => WrappedSpec is IOneToOneFeatureSpec { IsAutoCompleteEnabled: true };

    public IConsentFacade IsUsable(IObjectFacade target) {
        var consent = WrappedSpec.IsUsable(((ObjectFacade)target).WrappedNakedObject);
        return new ConsentFacade(consent);
    }

    public IObjectFacade GetValue(IObjectFacade target) {
        var result = WrappedSpec.GetNakedObject(((ObjectFacade)target).WrappedNakedObject);
        return ObjectFacade.Wrap(result, FrameworkFacade, framework);
    }

    public bool IsVisible(IObjectFacade objectFacade) => WrappedSpec.IsVisible(((ObjectFacade)objectFacade).WrappedNakedObject);

    public bool IsEager(IObjectFacade objectFacade) =>
        ((TypeFacade)objectFacade.Specification).WrappedValue.ContainsFacet<IEagerlyFacet>() ||
        WrappedSpec.ContainsFacet<IEagerlyFacet>();

    public DataType? DataType => WrappedSpec.GetFacet<IDataTypeFacet>()?.DataType() ?? WrappedSpec.GetFacet<IPasswordFacet>()?.DataType;

    public IObjectFacade[] GetChoices(IObjectFacade target, IDictionary<string, object> parameterNameValues) {
        var oneToOneFeature = WrappedSpec as IOneToOneFeatureSpec;
        var pnv = parameterNameValues?.ToDictionary(kvp => kvp.Key, kvp => framework.GetNakedObject(kvp.Value));
        return oneToOneFeature?.GetChoices(((ObjectFacade)target).WrappedNakedObject, pnv).Select(no => ObjectFacade.Wrap(no, FrameworkFacade, framework)).Cast<IObjectFacade>().ToArray();
    }

    public (string, ITypeFacade)[] GetChoicesParameters() =>
        WrappedSpec is IOneToOneFeatureSpec oneToOneFeature
            ? oneToOneFeature.GetChoicesParameters().Select(WrapChoiceParm).ToArray()
            : Array.Empty<(string, ITypeFacade)>();

    public (IObjectFacade, string)[] GetChoicesAndTitles(IObjectFacade target, IDictionary<string, object> parameterNameValues) =>
        GetChoices(target, parameterNameValues).Select(c => (c, c.TitleString)).ToArray();

    public IObjectFacade[] GetCompletions(IObjectFacade target, string autoCompleteParm) {
        var oneToOneFeature = WrappedSpec as IOneToOneFeatureSpec;
        return oneToOneFeature?.GetCompletions(((ObjectFacade)target).WrappedNakedObject, autoCompleteParm).Select(no => ObjectFacade.Wrap(no, FrameworkFacade, framework)).Cast<IObjectFacade>().ToArray();
    }

    public int Count(IObjectFacade target) => IsCollection ? framework.Persistor.CountField(((ObjectFacade)target).WrappedNakedObject, Id) : 0;

    public bool IsSetToImplicitDefault(IObjectFacade objectFacade) {
        // return true if it's scalar and and still set to its implicit default value (eg 0 for an int)
        if (!DefaultTypeIsExplicit(objectFacade) && WrappedSpec.ReturnSpec.IsParseable) {
            var dflt = WrappedSpec.GetDefault(objectFacade.WrappedAdapter());
            var currentValue = GetValue(objectFacade);

            return dflt?.Object == currentValue?.Object;
        }

        return false;
    }

    public string GetTitle(IObjectFacade objectFacade) {
        var enumFacet = WrappedSpec.GetFacet<IEnumFacet>();

        if (enumFacet != null) {
            return enumFacet.GetTitle(((ObjectFacade)objectFacade).WrappedNakedObject);
        }

        var mask = WrappedSpec.GetFacet<IMaskFacet>();
        if (mask == null) {
            return objectFacade.TitleString;
        }

        var titleFacet = ((TypeFacade)objectFacade.Specification).WrappedValue.GetFacet<ITitleFacet>();
        return titleFacet.GetTitleWithMask(mask.Value, ((ObjectFacade)objectFacade).WrappedNakedObject, framework);
    }

    public IFrameworkFacade FrameworkFacade { get; set; }

    public bool IsFile => WrappedSpec.IsFile(framework);

    public bool IsDateOnly => WrappedSpec.ContainsFacet<IDateOnlyFacet>();

    public bool IsEnum => WrappedSpec.ContainsFacet<IEnumFacet>();

    public bool IsFindMenuEnabled => WrappedSpec is IOneToOneAssociationSpec { IsFindMenuEnabled: true };

    public (Regex, string)? RegEx => WrappedSpec.GetRegEx();

    public (IConvertible, IConvertible, bool)? Range => WrappedSpec.GetRange();

    public bool DoNotCount => WrappedSpec.ContainsFacet<INotCountedFacet>();

    public int Width => WrappedSpec.GetWidth();

    public string PresentationHint => WrappedSpec.GetPresentationHint();

    public string GetMaskedValue(IObjectFacade objectFacade) => WrappedSpec.GetMaskedValue(objectFacade, framework);

    public bool DefaultTypeIsExplicit(IObjectFacade objectFacade) {
        var no = ((ObjectFacade)objectFacade).WrappedNakedObject;
        return WrappedSpec.GetDefaultType(no) == TypeOfDefaultValue.Explicit;
    }

    public int NumberOfLines => WrappedSpec.GetNumberOfLinesWithDefault();

    public (bool, string[])? TableViewData => WrappedSpec.GetTableViewData();

    public bool RenderEagerly => WrappedSpec.GetRenderEagerly();

    public bool IsPassword => WrappedSpec.ContainsFacet<IPasswordFacet>();

    public bool IsNullable => WrappedSpec.ContainsFacet<INullableFacet>();

    public string Grouping => WrappedSpec.GetFacet<IMemberOrderFacet>()?.Grouping ?? "";

    #endregion
}