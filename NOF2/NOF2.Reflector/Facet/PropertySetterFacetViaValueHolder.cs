// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Reflection;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.Spec;
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.Facet;
using NOF2.ValueHolder;

namespace NOF2.Reflector.Facet;

[Serializable]
public sealed class PropertySetterFacetViaValueHolder<T, TU> : PropertySetterFacetAbstract where T : class, IValueHolder<TU> {
    private readonly PropertyInfo property;

    public PropertySetterFacetViaValueHolder(PropertyInfo property, ISpecification holder)
        : base() =>
        this.property = property;

    public override string PropertyName {
        get => property.Name;
        protected set { }
    }

    public override void SetProperty(INakedObjectAdapter nakedObjectAdapter, INakedObjectAdapter value, INakedFramework framework) {
        try {
            var valueHolder = (T)property.GetValue(nakedObjectAdapter.Object);
            if (value.GetDomainObject() is T obj) {
                valueHolder.Value = obj.Value;
            }
            else {
                valueHolder.Value = Activator.CreateInstance<T>().Value;
            }
        }
        catch (NullReferenceException e) {
            InvokeUtils.InvocationException($"Unexpected null valueholder on {property}", e);
        }
        catch (TargetInvocationException e) {
            InvokeUtils.InvocationException($"Exception executing {property}", e);
        }
    }

    protected override string ToStringValues() => $"property={property}";
}