// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Error;
using NakedFramework.Core.Util;

namespace NakedFramework.Metamodel.SemanticsProvider;

[Serializable]
public sealed class FloatValueSemanticsProvider : ValueSemanticsProviderAbstract<float>, IFloatingPointValueFacet {
    private const float DefaultValueConst = 0;
    private const bool Immutable = true;

    public FloatValueSemanticsProvider(IObjectSpecImmutable spec, ISpecification holder)
        : base(Type, holder, AdaptedType, Immutable, DefaultValueConst, spec) { }

    public static Type Type => typeof(IFloatingPointValueFacet);

    public static Type AdaptedType => typeof(float);

    #region IFloatingPointValueFacet Members

    public float FloatValue(INakedObjectAdapter nakedObjectAdapter) => nakedObjectAdapter.GetDomainObject<float>();

    #endregion

    public static bool IsAdaptedType(Type type) => type == AdaptedType;

    protected override float DoParse(string entry) {
        try {
            return float.Parse(entry);
        }
        catch (FormatException) {
            throw new InvalidEntryException(FormatMessage(entry));
        }
        catch (OverflowException) {
            throw new InvalidEntryException(OutOfRangeMessage(entry, float.MinValue, float.MaxValue));
        }
    }

    protected override string TitleStringWithMask(string mask, float value) => value.ToString(mask);

    public override string ToString() => "FloatAdapter: ";
}