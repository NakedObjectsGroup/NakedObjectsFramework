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
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.Serialization;

namespace NakedFramework.Metamodel.Facet;

[Serializable]
public sealed class PropertyInitializationFacet : FacetAbstract, IPropertyInitializationFacet, IImperativeFacet {
    private readonly PropertySerializationWrapper property;

    public PropertyInitializationFacet(PropertyInfo propertyInfo, ILogger<PropertyInitializationFacet> logger) =>
        property = new PropertySerializationWrapper(propertyInfo, logger);

    public override Type FacetType => typeof(IPropertyInitializationFacet);

    #region IPropertyInitializationFacet Members

    public void InitProperty(INakedObjectAdapter nakedObjectAdapter, INakedObjectAdapter value) {
        try {
            property.SetMethodDelegate(nakedObjectAdapter.GetDomainObject(), new[] { value.GetDomainObject(), null });
        }
        catch (TargetInvocationException e) {
            InvokeUtils.InvocationException($"Exception executing {property}", e);
        }
    }

    #endregion

    #region IImperativeFacet Members

    public MethodInfo GetMethod() => property.SetMethod();

    public Func<object, object[], object> GetMethodDelegate() => property.SetMethodDelegate;

    #endregion
}