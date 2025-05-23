// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Core.Error;

namespace NakedFramework.Metamodel.SemanticsProvider;

[Serializable]
public sealed class EnumValueSemanticsProvider<T> : ValueSemanticsProviderAbstract<T>, IEnumValueFacet {
    private const bool Immutable = true;

    public EnumValueSemanticsProvider() : base(Immutable, GetDefault()) { }

    public static Type AdaptedType => typeof(T);

    public override Type FacetType => typeof(IEnumValueFacet);

    #region IEnumValueFacet Members

    public string IntegralValue(INakedObjectAdapter nakedObjectAdapter) {
        if (nakedObjectAdapter.Object is T || TypeUtils.IsIntegralValueForEnum(nakedObjectAdapter.Object)) {
            return Convert.ChangeType(nakedObjectAdapter.Object, Enum.GetUnderlyingType(typeof(T)))?.ToString();
        }

        return null;
    }

    #endregion

    private static bool IsEnumSubtype() => TypeUtils.IsEnum(AdaptedType) && !(AdaptedType == typeof(Enum));

    private static T GetDefault() {
        // default(T) for an enum just returns 0 - but that's 
        // not necessarily a valid value for the enum - and that breaks serialization
        // return the first value. 
        // Value could be an EnumDataType rather than Enum - so check first
        if (IsEnumSubtype()) {
            var values = Enum.GetValues(AdaptedType);
            if (values.Length > 0) {
                return (T)values.GetValue(0);
            }
        }

        return default;
    }

    protected override T DoParse(string entry) {
        try {
            return (T)Enum.Parse(typeof(T), entry);
        }
        catch (ArgumentException) {
            throw new InvalidEntryException(FormatMessage(entry));
        }
        catch (OverflowException oe) {
            throw new InvalidEntryException(oe.Message);
        }
    }

    protected override string TitleString(T obj) => NameUtils.NaturalName(obj.ToString());

    protected override string TitleStringWithMask(string mask, T value) => TitleString(value);
}