// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Globalization;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Error;
using NakedFramework.Core.Util;

namespace NakedFramework.Metamodel.SemanticsProvider;

[Serializable]
public sealed class LongValueSemanticsProvider : ValueSemanticsProviderAbstract<long>, ILongValueFacet {
    private const bool Immutable = true;
    private const long DefaultValueConst = 0;

    public LongValueSemanticsProvider(IObjectSpecImmutable spec, ISpecification holder)
        : base(Type, holder, AdaptedType, Immutable, DefaultValueConst, spec) { }

    public static Type Type => typeof(ILongValueFacet);

    public static Type AdaptedType => typeof(long);

    public static KeyValuePair<Type, Func<IObjectSpecImmutable, ISpecification, IValueSemanticsProvider>> Factory => new(AdaptedType, (o, s) => new LongValueSemanticsProvider(o, s));

    #region ILongValueFacet Members

    public long LongValue(INakedObjectAdapter nakedObjectAdapter) => nakedObjectAdapter.GetDomainObject<long>();

    #endregion

    protected override long DoParse(string entry) {
        try {
            return long.Parse(entry, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands);
        }
        catch (FormatException) {
            throw new InvalidEntryException(FormatMessage(entry));
        }
        catch (OverflowException) {
            throw new InvalidEntryException(OutOfRangeMessage(entry, long.MinValue, long.MaxValue));
        }
    }

    protected override string TitleStringWithMask(string mask, long value) => value.ToString(mask);
}