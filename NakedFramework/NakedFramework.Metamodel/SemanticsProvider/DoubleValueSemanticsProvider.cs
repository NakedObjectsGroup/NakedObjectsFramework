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
using NakedFramework.Core.Error;

namespace NakedFramework.Metamodel.SemanticsProvider;

[Serializable]
public sealed class DoubleValueSemanticsProvider : ValueSemanticsProviderAbstract<double>, IDoubleFloatingPointValueFacet {
    private const double DefaultValueConst = 0;
    private const bool Immutable = true;

    public DoubleValueSemanticsProvider(IObjectSpecImmutable spec)
        : base(Type, AdaptedType, Immutable, DefaultValueConst, spec) { }

    private static Type Type => typeof(IDoubleFloatingPointValueFacet);

    public static Type AdaptedType => typeof(double);

    public static KeyValuePair<Type, Func<IObjectSpecImmutable, ISpecification, IValueSemanticsProvider>> Factory => new(AdaptedType, (o, s) => new DoubleValueSemanticsProvider(o));

    protected override double DoParse(string entry) {
        try {
            return double.Parse(entry);
        }
        catch (FormatException) {
            throw new InvalidEntryException(NakedObjects.Resources.NakedObjects.NotANumber);
        }
        catch (OverflowException) {
            throw new InvalidEntryException(OutOfRangeMessage(entry, double.MinValue, double.MaxValue));
        }
    }

    protected override string TitleStringWithMask(string mask, double value) => value.ToString(mask);
}