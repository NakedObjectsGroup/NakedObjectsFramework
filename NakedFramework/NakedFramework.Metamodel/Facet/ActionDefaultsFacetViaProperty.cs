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
using NakedFramework.Architecture.Spec;
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.Serialization;

namespace NakedFramework.Metamodel.Facet;

[Serializable]
public sealed class ActionDefaultsFacetViaProperty : ActionDefaultsFacetAbstract, IImperativeFacet {
    private readonly IActionDefaultsFacet actionDefaultsFacet;
    private readonly PropertySerializationWrapper property;

    public ActionDefaultsFacetViaProperty(PropertyInfo property, IActionDefaultsFacet actionDefaultsFacet, ILogger<ActionDefaultsFacetViaProperty> logger) {
        this.property = new PropertySerializationWrapper(property, logger);
        this.actionDefaultsFacet = actionDefaultsFacet;
    }

    public override (object, TypeOfDefaultValue) GetDefault(INakedObjectAdapter nakedObjectAdapter, INakedFramework framework) {
        // type safety is given by the reflector only identifying methods that match the 
        // parameter type
        var defaultValue = property.GetMethodDelegate(nakedObjectAdapter.GetDomainObject(), Array.Empty<object>());
        if (actionDefaultsFacet is not null && (defaultValue is null || string.IsNullOrWhiteSpace(defaultValue.ToString()))) {
            return actionDefaultsFacet.GetDefault(nakedObjectAdapter, framework);
        }

        return (defaultValue, TypeOfDefaultValue.Explicit);
    }

    #region IImperativeFacet Members

    public MethodInfo GetMethod() => property.GetMethod();

    public Func<object, object[], object> GetMethodDelegate() => property.GetMethodDelegate;

    #endregion
}

// Copyright (c) Naked Objects Group Ltd.