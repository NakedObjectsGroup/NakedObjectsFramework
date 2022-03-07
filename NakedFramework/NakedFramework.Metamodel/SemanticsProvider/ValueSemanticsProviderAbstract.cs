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
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.Facet;

namespace NakedFramework.Metamodel.SemanticsProvider;

[Serializable]
public abstract class ValueSemanticsProviderAbstract<T> : FacetAbstract, IValueSemanticsProvider<T> {
    private readonly Type adaptedType;

    /// <summary>
    ///     Lazily looked up per <see cref="SpecImmutable" />
    /// </summary>
    protected ValueSemanticsProviderAbstract(Type adapterFacetType,
                                             ISpecification holder,
                                             Type adaptedType,
                                             bool immutable,
                                             T defaultValue,
                                             IObjectSpecImmutable specImmutable)
        : base(adapterFacetType, holder) {
        this.adaptedType = adaptedType;
        IsImmutable = immutable;
        DefaultValue = defaultValue;
        SpecImmutable = specImmutable;
    }

    public IObjectSpecImmutable SpecImmutable { get; }

    /// <summary>
    ///     We don't replace any (non- no-op) facets.
    /// </summary>
    /// <para>
    ///     For example, if there is already a <see cref="IPropertyDefaultFacet" /> then we shouldn't replace it.
    /// </para>
    public override bool CanAlwaysReplace => false;

    public void AddValueFacets(ISpecificationBuilder specification) => ValueTypeHelpers.AddValueFacets(this, specification);

    protected abstract T DoParse(string entry);

    protected virtual string TitleString(T obj) => obj.ToString();

    protected virtual string TitleStringWithMask(string mask, T obj) => obj.ToString();

    protected string OutOfRangeMessage(string entry, T minValue, T maxValue) => string.Format(NakedObjects.Resources.NakedObjects.OutOfRange, entry, minValue, maxValue);

    protected static string FormatMessage(string entry) => string.Format(NakedObjects.Resources.NakedObjects.CannotFormat, entry, typeof(T).Name);

    #region IValueSemanticsProvider<T> Members

    public T DefaultValue { get; }

    public bool IsImmutable { get; }

    public virtual object ParseTextEntry(string entry) {
        if (entry == null) {
            throw new ArgumentException();
        }

        if (entry.Trim().Equals("")) {
            return null;
        }

        return DoParse(entry);
    }

    public string DisplayTitleOf(T obj) => TitleString(obj);

    public string TitleWithMaskOf(string mask, T obj) => TitleStringWithMask(mask, obj);
    public virtual object Value(INakedObjectAdapter adapter, string format = null) => adapter.GetDomainObject<T>();

    #endregion
}

// Copyright (c) Naked Objects Group Ltd.