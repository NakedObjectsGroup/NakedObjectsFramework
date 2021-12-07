// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Globalization;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Error;
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.SemanticsProvider;
using NakedLegacy.Types;

namespace NakedLegacy.Reflector.SemanticsProvider;

[Serializable]
public sealed class WholeNumberValueSemanticsProvider : ValueSemanticsProviderAbstract<WholeNumber>, IIntegerValueFacet {
    
    private const bool Immutable = true;
    private const int TypicalLengthConst = 11;
    private static WholeNumber DefaultValueConst = new(0);

    public WholeNumberValueSemanticsProvider(IObjectSpecImmutable spec, ISpecification holder)
        : base(Type, holder, AdaptedType, Immutable, DefaultValueConst, spec) { }

    public static Type Type => typeof(IIntegerValueFacet);

    public static Type AdaptedType => typeof(WholeNumber);

    #region IIntegerValueFacet Members

    public int IntegerValue(INakedObjectAdapter nakedObjectAdapter) => nakedObjectAdapter.GetDomainObject<WholeNumber>().Number;

    #endregion

    public static bool IsAdaptedType(Type type) => type == AdaptedType;

    protected override WholeNumber DoParse(string entry) {
        try {
            return new WholeNumber(int.Parse(entry, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands));
        }
        catch (FormatException) {
            throw new InvalidEntryException(FormatMessage(entry));
        }
        catch (OverflowException) {
            throw new InvalidEntryException(OutOfRangeMessage(entry, new WholeNumber(int.MinValue), new WholeNumber(int.MaxValue)));
        }
    }

    protected override string TitleStringWithMask(string mask, WholeNumber value) => value.Number.ToString(mask);


    public override string ToString() => "WholeNumberAdapter: ";
}