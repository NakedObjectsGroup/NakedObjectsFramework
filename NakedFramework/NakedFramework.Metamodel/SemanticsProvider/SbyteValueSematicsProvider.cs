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
using NakedFramework.Core.Util;

namespace NakedFramework.Metamodel.SemanticsProvider;

[Serializable]
public sealed class SbyteValueSemanticsProvider : ValueSemanticsProviderAbstract<sbyte>, ISbyteValueFacet {
    private const sbyte DefaultValueConst = 0;
    private const bool Immutable = true;

    public SbyteValueSemanticsProvider(IObjectSpecImmutable spec, ISpecification holder)
        : base(Type, holder, AdaptedType, Immutable, DefaultValueConst, spec) { }

    public static Type Type => typeof(ISbyteValueFacet);

    public static Type AdaptedType => typeof(sbyte);

    public static KeyValuePair<Type, Func<IObjectSpecImmutable, ISpecification, IValueSemanticsProvider>> Factory => new(AdaptedType, (o, s) => new SbyteValueSemanticsProvider(o, s));

    #region ISbyteValueFacet Members

    public sbyte SByteValue(INakedObjectAdapter nakedObjectAdapter) => nakedObjectAdapter.GetDomainObject<sbyte>();

    #endregion

    protected override sbyte DoParse(string entry) {
        try {
            return sbyte.Parse(entry);
        }
        catch (FormatException) {
            throw new InvalidEntryException(FormatMessage(entry));
        }
        catch (OverflowException) {
            throw new InvalidEntryException(OutOfRangeMessage(entry, sbyte.MinValue, sbyte.MaxValue));
        }
    }

    protected override string TitleStringWithMask(string mask, sbyte value) => value.ToString(mask);
}