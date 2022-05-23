// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Core.Error;
using NakedFramework.Metamodel.NonSerializedSemanticsProvider;

namespace NakedFramework.Metamodel.SemanticsProvider;

[Serializable]
public sealed class DateTimeValueSemanticsProvider : ValueSemanticsProviderAbstract<DateTime>, IDateValueFacet {
    private const bool Immutable = false;
    private static DateTimeValueSemanticsProvider instance;
    private static readonly DateTime DefaultValueConst = new();

    private DateTimeValueSemanticsProvider() : base(Immutable, DefaultValueConst) { }
    internal static DateTimeValueSemanticsProvider Instance => instance ??= new DateTimeValueSemanticsProvider();

    // inject for testing 
    public static DateTime? TestDateTime { get; set; }

    public static Type AdaptedType => typeof(DateTime);

    public static KeyValuePair<Type, IValueSemanticsProvider> Factory => new(AdaptedType, Instance);

    public override Type FacetType => typeof(IDateValueFacet);

    private DateTime DateValue(INakedObjectAdapter nakedObjectAdapter) => (DateTime?)nakedObjectAdapter?.Object ?? Now();

    private DateTime GetUtcDate(string dateString) {
        var year = int.Parse(dateString[..4]);
        var month = int.Parse(dateString[5..7]);
        var day = int.Parse(dateString[8..]);

        return new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc).Date;
    }

    protected override DateTime DoParse(string entry) {
        var dateString = entry.Trim();
        try {
            var date = dateString.Length is 10 && dateString.Contains('-')
                ? GetUtcDate(dateString)
                : DateTime.Parse(dateString);
            return date;
        }
        catch (FormatException) {
            throw new InvalidEntryException(FormatMessage(dateString));
        }
    }

    protected override string TitleStringWithMask(string mask, DateTime value) => value.ToString(mask);

    private static DateTime Now() => TestDateTime ?? DateTime.Now;

    public override object Value(INakedObjectAdapter adapter, string format = null) =>
        format switch {
            null => ToUniversalTime(DateValue(adapter)),
            "s" => ToUniversalTime(DateValue(adapter)).ToString(format),
            _ => DateValue(adapter).Date.ToString(format)
        };

    private static DateTime ToUniversalTime(DateTime dt) =>
        dt.Kind == DateTimeKind.Unspecified
            ? new DateTime(dt.Ticks, DateTimeKind.Utc).ToUniversalTime()
            : dt.ToUniversalTime();
}

// Copyright (c) Naked Objects Group Ltd.