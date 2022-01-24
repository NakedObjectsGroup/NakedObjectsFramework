// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;

namespace NakedFramework.Metamodel.SemanticsProvider;

[Serializable]
public sealed class StringValueSemanticsProvider : ValueSemanticsProviderAbstract<string>, IStringValueFacet {
    private const string DefaultValueConst = null;
    private const bool Immutable = true;

    public StringValueSemanticsProvider(IObjectSpecImmutable spec, ISpecification holder)
        : base(Type, holder, AdaptedType, Immutable, DefaultValueConst, spec) { }

    public static Type Type => typeof(IStringValueFacet);

    public static Type AdaptedType => typeof(string);

    public static KeyValuePair<Type, Func<IObjectSpecImmutable, ISpecification, IValueSemanticsProvider>> Factory => new(AdaptedType, (o, s) => new StringValueSemanticsProvider(o, s));

    protected override string DoParse(string entry) => entry.Trim().Equals("") ? null : entry;
}