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
using NakedFramework.Metamodel.Serialization;

namespace NakedFramework.Metamodel.Facet;

[Serializable]
public sealed class PropertySetterFacetViaSetterMethod : PropertySetterFacetAbstract, IImperativeFacet {
    private readonly PropertySerializationWrapper property;

    public PropertySetterFacetViaSetterMethod(PropertyInfo propertyInfo, ILogger<PropertySetterFacetViaSetterMethod> logger) =>
        property = new PropertySerializationWrapper(propertyInfo, logger, ReflectorDefaults.JitSerialization);

    public override string PropertyName {
        get => property.PropertyInfo.Name;
        protected set { }
    }

    public override void SetProperty(INakedObjectAdapter nakedObjectAdapter, INakedObjectAdapter value, INakedFramework framework) {
        try {
            property.SetMethodDelegate(nakedObjectAdapter.GetDomainObject(), new[] { value.GetDomainObject(), null });
        }
        catch (TargetInvocationException e) {
            InvokeUtils.InvocationException($"Exception executing {property}", e);
        }
    }

    #region IImperativeFacet Members

    public MethodInfo GetMethod() => property.SetMethod();

    public Func<object, object[], object> GetMethodDelegate() => property.SetMethodDelegate;

    #endregion
}