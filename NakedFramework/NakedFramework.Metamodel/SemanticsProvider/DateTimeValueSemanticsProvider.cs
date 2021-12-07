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
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Error;

namespace NakedFramework.Metamodel.SemanticsProvider;

[Serializable]
public sealed class DateTimeValueSemanticsProvider : ValueSemanticsProviderAbstract<DateTime>, IDateValueFacet {
    private const bool Immutable = false;
    private static readonly DateTime DefaultValueConst = new();

    public DateTimeValueSemanticsProvider(IObjectSpecImmutable spec, ISpecification holder)
        : base(Type, holder, AdaptedType, Immutable, DefaultValueConst, spec) { }

    // inject for testing 
    public static DateTime? TestDateTime { get; set; }

    public static Type Type => typeof(IDateValueFacet);

    public static Type AdaptedType => typeof(DateTime);

    public static KeyValuePair<Type, Func<IObjectSpecImmutable, ISpecification, IValueSemanticsProvider>> Factory => new(AdaptedType, (o, s) => new DateTimeValueSemanticsProvider(o, s));

    #region IDateValueFacet Members

    public DateTime DateValue(INakedObjectAdapter nakedObjectAdapter) => (DateTime?)nakedObjectAdapter?.Object ?? Now();

    #endregion

    protected override DateTime DoParse(string entry) {
        var dateString = entry.Trim();
        try {
            return DateTime.Parse(dateString);
        }
        catch (FormatException) {
            throw new InvalidEntryException(FormatMessage(dateString));
        }
    }

    protected override string TitleStringWithMask(string mask, DateTime value) => value.ToString(mask);

    private static DateTime Now() => TestDateTime ?? DateTime.Now;
}

// Copyright (c) Naked Objects Group Ltd.