// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Spec;
using NakedFramework.Core.Error;
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.NonSerializedSemanticsProvider;
using NOF2.ValueHolder;

namespace NOF2.Reflector.SemanticsProvider;

[Serializable]
public sealed class ValueHolderWrappingValueSemanticsProvider<T, TU> : FacetAbstract, IValueSemanticsProvider<T> where T : class, IValueHolder<TU>, new() {
    [NonSerialized]
    private T valueHolderInstance;

    public ValueHolderWrappingValueSemanticsProvider() => valueHolderInstance = new T();

    public T ValueHolderInstance => valueHolderInstance ??= new T();

    public override Type FacetType => typeof(IValueSemanticsProvider);

    public static Type AdaptedType => typeof(T);

    public override bool CanAlwaysReplace => false;

    public T DefaultValue => default;

    public object ParseTextEntry(string entry) {
        if (entry == null) {
            throw new ArgumentException();
        }

        return entry.Trim().Equals("") ? null : DoParse(entry);
    }

    public string DisplayTitleOf(T obj) => TitleString(obj);

    public string TitleWithMaskOf(string mask, T obj) => TitleStringWithMask(mask, obj);

    public object Value(INakedObjectAdapter adapter, string format = null) => adapter.GetDomainObject<T>().Display(format);
    public bool IsImmutable => true;
    public void AddValueFacets(ISpecificationBuilder specification) => ValueTypeHelpers.AddValueFacets(this, specification);

    private T DoParse(string entry) {
        try {
            return ValueHolderInstance.Parse(entry) as T;
        }
        catch (Exception e) {
            throw new InvalidEntryException(e.Message);
        }
    }

    public override string ToString() => $"ValueHolderWrappingValueSemanticsProvider<{typeof(T)},{typeof(TU)}>";

    private string TitleStringWithMask(string mask, T obj) => obj.ToString();

    private string TitleString(T obj) => obj.ToString();
}