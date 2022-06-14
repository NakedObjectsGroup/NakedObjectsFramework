// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Core.Configuration;
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.Serialization;
using NOF2.ValueHolder;

namespace NOF2.Reflector.Facet;

[Serializable]
public sealed class PropertySetterFacetViaValueHolder<T, TU> : PropertySetterFacetAbstract, IImperativeFacet where T : class, IValueHolder<TU> {
    private readonly PropertySerializationWrapper propertyWrapper;

    public PropertySetterFacetViaValueHolder(PropertyInfo property, ILogger logger) =>
        propertyWrapper = SerializationFactory.Wrap(property, logger);

    public override string PropertyName {
        get => propertyWrapper.PropertyInfo.Name;
        protected set { }
    }

    public MethodInfo GetMethod() => propertyWrapper.GetMethod();

    public Func<object, object[], object> GetMethodDelegate() => propertyWrapper.GetGetMethodDelegate();

    public override void SetProperty(INakedObjectAdapter nakedObjectAdapter, INakedObjectAdapter value, INakedFramework framework) {
        try {
            var valueHolder = propertyWrapper.GetValue<T>(nakedObjectAdapter.Object);
            if (value.GetDomainObject() is T obj) {
                valueHolder.Value = obj.Value;
            }
            else {
                valueHolder.Value = Activator.CreateInstance<T>().Value;
            }
        }
        catch (NullReferenceException e) {
            InvokeUtils.InvocationException($"Unexpected null valueholder on {propertyWrapper.PropertyInfo}", e);
        }
        catch (InvalidCastException e) {
            InvokeUtils.InvocationException($"Unexpected type valueholder on {propertyWrapper.PropertyInfo}", e);
        }
        catch (TargetInvocationException e) {
            InvokeUtils.InvocationException($"Exception executing {propertyWrapper.PropertyInfo}", e);
        }
    }
}