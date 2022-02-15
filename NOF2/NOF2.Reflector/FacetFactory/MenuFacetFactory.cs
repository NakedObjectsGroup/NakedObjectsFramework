// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Immutable;
using Microsoft.Extensions.Logging;
using NakedFramework;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.FacetFactory;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.Utils;
using NakedFramework.ParallelReflector.FacetFactory;
using NakedFramework.ParallelReflector.Utils;
using NOF2.Menu;
using NOF2.Reflector.Helpers;
using MenuFacetViaMethod = NOF2.Reflector.Facet.MenuFacetViaMethod;

namespace NOF2.Reflector.FacetFactory;

public sealed class MenuFacetFactory : AbstractNOF2FacetFactoryProcessor, IMethodPrefixBasedFacetFactory {
    private static readonly string[] FixedPrefixes;

    static MenuFacetFactory() {
        FixedPrefixes = new[] { RecognisedMethodsAndPrefixes.MenuMethod };
    }

    public MenuFacetFactory(IFacetFactoryOrder<MenuFacetFactory> order, ILoggerFactory loggerFactory)
        : base(order.Order, loggerFactory, FeatureType.ObjectsAndInterfaces) { }

    public string[] Prefixes => FixedPrefixes;

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        // instance
        var menuOrderMethod = MethodHelpers.FindMethod(reflector, type, MethodType.Class, "menuOrder", typeof(IMenu), null);
        methodRemover.SafeRemoveMethod(menuOrderMethod);
        var facet = menuOrderMethod is not null ? (IFacet)new MenuFacetViaMethod(menuOrderMethod, specification, Logger<MenuFacetViaMethod>()) : new MenuFacetDefault(specification);
        FacetUtils.AddFacet(facet);

        // mainMenu
        var sharedmenuOrderMethod = MethodHelpers.FindMethod(reflector, type, MethodType.Class, "sharedMenuOrder", typeof(IMenu), null);
        methodRemover.SafeRemoveMethod(sharedmenuOrderMethod);

        if (sharedmenuOrderMethod is not null) {
            void Action(IMetamodelBuilder builder) {
                var legacyMenu = (IMenu)InvokeUtils.InvokeStatic(sharedmenuOrderMethod, Array.Empty<object>());
                var mainMenu = NOF2Helpers.ConvertNOF2ToNOFMenu(legacyMenu, builder, sharedmenuOrderMethod.DeclaringType, legacyMenu.Name);
                builder.AddMainMenu(mainMenu);
            }

            metamodel = FacetUtils.AddIntegrationFacet(reflector, type, Action, metamodel);
        }

        return metamodel;
    }
}