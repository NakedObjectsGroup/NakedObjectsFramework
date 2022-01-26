// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Error;
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.SemanticsProvider;

namespace NakedLegacy.Reflector.SemanticsProvider;

[Serializable]
public sealed class ValueHolderWrappingValueSemanticsProvider<T, TU> : ValueSemanticsProviderAbstract<T> where T : class, IValueHolder<TU>, new() {
    private const bool Immutable = true;
    private const int TypicalLengthConst = 11;
    private static T DefaultValueConst = default;

    public ValueHolderWrappingValueSemanticsProvider(IObjectSpecImmutable spec, ISpecification holder)
        : base(Type, holder, AdaptedType, Immutable, DefaultValueConst, spec) { }

    public static Type Type => typeof(IValueSemanticsProvider);

    public static Type AdaptedType => typeof(T);

    public static bool IsAdaptedType(Type type) => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IValueHolder<>);

    protected override T DoParse(string entry) {
        try {
            return new T().Parse(entry) as T;
        }
        catch (Exception e) {
            throw new InvalidEntryException(e.Message);
        }
    }

    public override object Value(INakedObjectAdapter adapter, string format = null) => adapter.GetDomainObject<T>().Display(format);

    protected override string TitleStringWithMask(string mask, T value) => ""; //value.Number.ToString(mask);
    public override string ToString() => "WholeNumberAdapter: ";
}