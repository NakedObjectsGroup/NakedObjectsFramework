// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.Facet;
using NakedFramework.ParallelReflector.Utils;
using NOF2.Menu;
using NOF2.Reflector.Helpers;

namespace NOF2.Reflector.Facet;

[Serializable]
public sealed class MenuFacetViaMethod : MenuFacetAbstract, IImperativeFacet {
    private readonly MethodInfo method;

    public MenuFacetViaMethod(MethodInfo method, ILogger<MenuFacetViaMethod> logger) {
        this.method = method;
        MethodDelegate = LogNull(DelegateUtils.CreateDelegate(method), logger);
    }

    private Func<object, object[], object> MethodDelegate { get; set; }

    public MethodInfo GetMethod() => method;

    public Func<object, object[], object> GetMethodDelegate() => MethodDelegate;

    //Creates a menu based on the definition in the object's Menu method
    public override void CreateMenu(IMetamodelBuilder metamodel, ITypeSpecImmutable spec) {
        var legacyMenu = MethodDelegate.Invoke<IMenu>(method, null, Array.Empty<object>());
        Menu = NOF2Helpers.ConvertNOF2ToNOFMenu(legacyMenu, metamodel, method.DeclaringType, "Actions");
    }
}

// Copyright (c) Naked Objects Group Ltd.