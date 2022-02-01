// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Text.RegularExpressions;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.Spec;
using NakedFramework.Facade.Impl.Impl;
using NakedFramework.Facade.Interface;

namespace NakedFramework.Facade.Impl.Utility;

public static class FacetUtils {
    public static string GetMaskedValue(this ISpecification spec, IObjectFacade objectFacade, INakedFramework framework) {
        if (objectFacade == null) {
            return null;
        }

        var mask = spec.GetFacet<IMaskFacet>();
        var no = ((ObjectFacade)objectFacade).WrappedNakedObject;
        return mask != null ? no.Spec.GetFacet<ITitleFacet>().GetTitleWithMask(mask.Value, no, framework) : no.TitleString();
    }

    public static int GetMemberOrder(this ISpecification spec) {
        var facet = spec.GetFacet<IMemberOrderFacet>();

        if (facet != null && int.TryParse(facet.Sequence, out var result)) {
            return result;
        }

        return 0;
    }

    public static string GetMemberOrderName(this ISpecification spec) {
        var facet = spec.GetFacet<IMemberOrderFacet>();
        return facet?.Name;
    }

    public static string GetMask(this IAssociationSpec spec) {
        var facet = spec.GetFacet<IMaskFacet>() ?? spec.ReturnSpec?.GetFacet<IMaskFacet>();
        return facet?.Value;
    }

    public static string GetMask(this IActionParameterSpec spec) {
        var facet = spec.GetFacet<IMaskFacet>() ?? spec.Spec?.GetFacet<IMaskFacet>();
        return facet?.Value;
    }

    public static int? GetMaxLength(this ISpecification spec) {
        var facet = spec.GetFacet<IMaxLengthFacet>();
        return facet?.Value;
    }

    public static string GetPattern(this ISpecification spec) {
        var facet = spec.GetFacet<IRegExFacet>();
        return facet?.Pattern.ToString();
    }

    public static int GetAutoCompleteMinLength(this ISpecification spec) {
        var facet = spec.GetFacet<IAutoCompleteFacet>();
        return facet?.MinLength ?? 0;
    }

    public static bool GetRenderEagerly(this ISpecification spec) {
        var eagerlyFacet = spec.GetFacet<IEagerlyFacet>();
        return eagerlyFacet?.What == Do.Rendering;
    }

    public static (bool title, string[] columns)? GetTableViewData(this ISpecification spec) {
        var facet = spec.GetFacet<ITableViewFacet>();
        return facet == null ? null : (facet.Title, facet.Columns);
    }

    public static int GetNumberOfLinesWithDefault(this ISpecification spec) {
        var multiline = spec.GetFacet<IMultiLineFacet>();
        return multiline?.NumberOfLines ?? 1;
    }

    public static int? GetNumberOfLines(this ISpecification spec) {
        var multiline = spec.GetFacet<IMultiLineFacet>();
        return multiline?.NumberOfLines;
    }

    public static int GetWidth(this ISpecification spec) {
        var multiline = spec.GetFacet<IMultiLineFacet>();
        return multiline?.Width ?? 0;
    }

    public static string GetPresentationHint(this ISpecification spec) {
        var hintFacet = spec.GetFacet<IPresentationHintFacet>();
        return hintFacet?.Value;
    }

    public static (string, string)? GetRestExtension(this ISpecification spec) {
        var extFacet = spec.GetFacet<IRestExtensionFacet>();
        return extFacet is null ? null : (extFacet.Name, extFacet.Value);
    }

    public static (Regex, string)? GetRegEx(this ISpecification spec) {
        var regEx = spec.GetFacet<IRegExFacet>();
        return regEx == null ? null : (regEx.Pattern, regEx.FailureMessage);
    }

    public static (IConvertible, IConvertible, bool)? GetRange(this ISpecification spec) {
        var rangeFacet = spec.GetFacet<IRangeFacet>();
        return rangeFacet == null ? null : (rangeFacet.Min, rangeFacet.Max, rangeFacet.IsDateRange);
    }

    public static bool IsMandatory(this ISpecification spec) => spec.GetFacet<IMandatoryFacet>().IsMandatory;
}