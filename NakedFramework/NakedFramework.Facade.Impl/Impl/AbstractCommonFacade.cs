// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Text.RegularExpressions;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Spec;
using NakedFramework.Facade.Impl.Utility;
using NakedFramework.Facade.Interface;

namespace NakedFramework.Facade.Impl.Impl;

public abstract class AbstractCommonFacade {
    private readonly IFeatureSpec wrappedSpecification;
    private bool? cachedIsDateOnly;
    private bool? cachedIsNullable;
    private bool? cachedIsPassword;
    private NullCache<int?> cachedMaxLength;
    private int? cachedNumberOfLines;
    private NullCache<string> cachedPattern;
    private NullCache<string> cachedPresentationHint;
    private NullCache<(IConvertible, IConvertible, bool)?> cachedRange;
    private NullCache<(Regex, string)?> cachedRegex;
    private NullCache<(string, string)?> cachedRestExtension;
    private int? cachedWidth;
    private string cachedName;
    private string cachedDescription;
    private bool? cachedIsMandatory;

    protected AbstractCommonFacade(IFeatureSpec wrappedSpecification) => this.wrappedSpecification = wrappedSpecification;

    public string PresentationHint => (cachedPresentationHint ??= FacadeUtils.NullCache(wrappedSpecification.GetPresentationHint())).Value;

    public (string, string)? RestExtension => (cachedRestExtension ??= FacadeUtils.NullCache(wrappedSpecification.GetRestExtension())).Value;

    public (Regex, string)? RegEx => (cachedRegex ??= FacadeUtils.NullCache(wrappedSpecification.GetRegEx())).Value;

    public (IConvertible, IConvertible, bool)? Range => (cachedRange ??= FacadeUtils.NullCache(wrappedSpecification.GetRange())).Value;

    public int Width => cachedWidth ??= wrappedSpecification.GetWidth();

    public int NumberOfLines => cachedNumberOfLines ??= wrappedSpecification.GetNumberOfLinesWithDefault();

    public bool IsPassword => cachedIsPassword ??= wrappedSpecification.ContainsFacet<IPasswordFacet>();

    public bool IsNullable => cachedIsNullable ??= wrappedSpecification.ContainsFacet<INullableFacet>();

    public bool IsDateOnly => cachedIsDateOnly ??= wrappedSpecification.ContainsFacet<IDateOnlyFacet>();

    public int? MaxLength => (cachedMaxLength ??= FacadeUtils.NullCache(wrappedSpecification.GetMaxLength())).Value;

    public string Pattern => (cachedPattern ??= FacadeUtils.NullCache(wrappedSpecification.GetPattern())).Value;

    public string Name(IObjectFacade objectFacade) => cachedName ??= wrappedSpecification.Name(objectFacade.WrappedAdapter());

    public string Description(IObjectFacade objectFacade) => cachedDescription ??= wrappedSpecification.Description(objectFacade.WrappedAdapter());

    public bool IsMandatory => cachedIsMandatory ??= wrappedSpecification.IsMandatory();
}