﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Globalization;
using NakedFramework.Architecture.Facet;
using NakedFramework.Core.Error;

namespace NakedFramework.Metamodel.SemanticsProvider;

[Serializable]
public sealed class ULongValueSemanticsProvider : ValueSemanticsProviderAbstract<ulong>, IUnsignedLongValueFacet {
    private const ulong DefaultValueConst = 0;
    private const bool Immutable = true;

    private ULongValueSemanticsProvider() : base(Immutable, DefaultValueConst) { }
    internal static ULongValueSemanticsProvider Instance { get; } = new();

    public static Type AdaptedType => typeof(ulong);

    public static KeyValuePair<Type, IValueSemanticsProvider> Factory => new(AdaptedType, Instance);

    public override Type FacetType => typeof(IUnsignedLongValueFacet);

    protected override ulong DoParse(string entry) {
        try {
            return ulong.Parse(entry, NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands);
        }
        catch (FormatException) {
            throw new InvalidEntryException(FormatMessage(entry));
        }
        catch (OverflowException) {
            throw new InvalidEntryException(OutOfRangeMessage(entry, ulong.MinValue, ulong.MaxValue));
        }
    }

    protected override string TitleStringWithMask(string mask, ulong value) => value.ToString(mask);
}