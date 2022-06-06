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
using NakedFramework.Core.Util;

namespace NakedFramework.Metamodel.SemanticsProvider;

[Serializable]
public sealed class TimeValueSemanticsProvider : ValueSemanticsProviderAbstract<TimeSpan>, ITimeValueFacet {
    private const bool Immutable = false;
    private static readonly TimeSpan DefaultValueConst = new();

    private TimeValueSemanticsProvider() : base(Immutable, DefaultValueConst) { }
    internal static TimeValueSemanticsProvider Instance { get; } = new TimeValueSemanticsProvider();

    public static Type AdaptedType => typeof(TimeSpan);

    public static KeyValuePair<Type, IValueSemanticsProvider> Factory => new(AdaptedType, Instance);

    public override Type FacetType => typeof(ITimeValueFacet);

    protected override TimeSpan DoParse(string entry) {
        var dateString = entry.Trim();
        try {
            return DateTime.Parse(dateString).TimeOfDay;
        }
        catch (FormatException) {
            throw new InvalidEntryException(FormatMessage(dateString));
        }
    }

    protected override string TitleString(TimeSpan obj) => DateTime.Today.Add(obj).ToShortTimeString();

    protected override string TitleStringWithMask(string mask, TimeSpan obj) => DateTime.Today.Add(obj).ToString(mask);

    public override object Value(INakedObjectAdapter adapter, string format = null) => adapter.GetDomainObject<TimeSpan>().ToString(format);
}

// Copyright (c) Naked Objects Group Ltd.