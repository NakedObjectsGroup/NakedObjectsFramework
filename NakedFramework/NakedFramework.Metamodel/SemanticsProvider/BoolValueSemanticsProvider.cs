// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NakedFramework.Architecture.Facet;
using NakedFramework.Core.Error;

[assembly: InternalsVisibleTo("NakedFramework.Metamodel.Test")]

namespace NakedFramework.Metamodel.SemanticsProvider;

[Serializable]
public sealed class BooleanValueSemanticsProvider : ValueSemanticsProviderAbstract<bool>, IBooleanValueFacet {
    private const bool DefaultValueConst = false;
    private const bool Immutable = true;

    private BooleanValueSemanticsProvider() : base(Immutable, DefaultValueConst) { }
    internal static BooleanValueSemanticsProvider Instance { get; } = new();

    public static Type AdaptedType => typeof(bool);

    public static KeyValuePair<Type, IValueSemanticsProvider> Factory => new(AdaptedType, Instance);

    public override Type FacetType => typeof(IBooleanValueFacet);

    protected override bool DoParse(string entry) {
        if ("true".StartsWith(entry.ToLower())) {
            return true;
        }

        if ("false".StartsWith(entry.ToLower())) {
            return false;
        }

        throw new InvalidEntryException(string.Format(NakedObjects.Resources.NakedObjects.NotALogical, entry));
    }
}