// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Globalization;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Error;

namespace NakedFramework.Metamodel.SemanticsProvider;

[Serializable]
public sealed class ShortValueSemanticsProvider : ValueSemanticsProviderAbstract<short>, IShortValueFacet {
    private const short DefaultValueConst = 0;
    private const bool Immutable = true;

    public ShortValueSemanticsProvider(IObjectSpecImmutable spec)
        : base(Type, AdaptedType, Immutable, DefaultValueConst) { }

    public static Type Type => typeof(IShortValueFacet);

    public static Type AdaptedType => typeof(short);

    public static KeyValuePair<Type, Func<IObjectSpecImmutable, IValueSemanticsProvider>> Factory => new(AdaptedType, o => new ShortValueSemanticsProvider(o));

    protected override short DoParse(string entry) {
        try {
            return short.Parse(entry, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands);
        }
        catch (FormatException) {
            throw new InvalidEntryException(FormatMessage(entry));
        }
        catch (OverflowException) {
            throw new InvalidEntryException(OutOfRangeMessage(entry, short.MinValue, short.MaxValue));
        }
    }

    protected override string TitleStringWithMask(string mask, short value) => value.ToString(mask);
}