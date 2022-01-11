// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Threading;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Error;
using NakedFramework.Core.Util;

namespace NakedFramework.Metamodel.SemanticsProvider;

[Serializable]
public sealed class CharValueSemanticsProvider : ValueSemanticsProviderAbstract<char>, ICharValueFacet {
    private const char DefaultValueConst = ' ';
    private const bool Immutable = true;

    public CharValueSemanticsProvider(IObjectSpecImmutable spec, ISpecification holder)
        : base(Type, holder, AdaptedType, Immutable, DefaultValueConst, spec) { }

    public static Type Type => typeof(ICharValueFacet);

    public static Type AdaptedType => typeof(char);

    public static KeyValuePair<Type, Func<IObjectSpecImmutable, ISpecification, IValueSemanticsProvider>> Factory => new(AdaptedType, (o, s) => new CharValueSemanticsProvider(o, s));

    

    protected override char DoParse(string entry) {
        try {
            return char.Parse(entry);
        }
        catch (FormatException) {
            throw new InvalidEntryException(FormatMessage(entry));
        }
    }

    protected override string TitleStringWithMask(string mask, char value) => value.ToString(Thread.CurrentThread.CurrentCulture);
}