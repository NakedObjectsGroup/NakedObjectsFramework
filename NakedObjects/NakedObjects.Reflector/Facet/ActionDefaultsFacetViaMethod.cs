// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.Spec;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.Serialization;

[assembly: InternalsVisibleTo("NakedFramework.ParallelReflector.Test")]
[assembly: InternalsVisibleTo("NakedObjects.Reflector.Test")]

namespace NakedObjects.Reflector.Facet;

[Serializable]
public sealed class ActionDefaultsFacetViaMethod : ActionDefaultsFacetAbstract, IImperativeFacet {
    private readonly MethodSerializationWrapper methodWrapper;

    public ActionDefaultsFacetViaMethod(MethodInfo defaultMethod, ILogger<ActionDefaultsFacetViaMethod> logger) => methodWrapper = SerializationFactory.Wrap(defaultMethod, logger);

    public override (object, TypeOfDefaultValue) GetDefault(INakedObjectAdapter nakedObjectAdapter, INakedFramework framework) {
        // type safety is given by the reflector only identifying methods that match the 
        // parameter type
        var defaultValue = methodWrapper.Invoke<object>(nakedObjectAdapter, Array.Empty<INakedObjectAdapter>());
        return (defaultValue, TypeOfDefaultValue.Explicit);
    }

    #region IImperativeFacet Members

    public MethodInfo GetMethod() => methodWrapper.GetMethod();

    public Func<object, object[], object> GetMethodDelegate() => methodWrapper.GetMethodDelegate();

    #endregion
}

// Copyright (c) Naked Objects Group Ltd.