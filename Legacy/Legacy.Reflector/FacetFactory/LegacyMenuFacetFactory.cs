// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Legacy.Reflector.Facet;
using Legacy.Types;
using Microsoft.Extensions.Logging;
using NakedFramework;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.FacetFactory;
using NakedFramework.Architecture.Menu;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.Menu;
using NakedFramework.Metamodel.Utils;
using NakedFramework.ParallelReflector.FacetFactory;
using NakedFramework.ParallelReflector.Utils;
using NakedObjects.Reflector.FacetFactory;

namespace Legacy.Reflector.FacetFactory {
    public sealed class LegacyMenuFacetFactory : LegacyFacetFactoryProcessor, IMethodPrefixBasedFacetFactory {
        private static readonly string[] FixedPrefixes;

        static LegacyMenuFacetFactory() {
            FixedPrefixes = new[] { RecognisedMethodsAndPrefixes.MenuMethod };
        }

        public LegacyMenuFacetFactory(IFacetFactoryOrder<MenuFacetFactory> order, ILoggerFactory loggerFactory)
            : base(order.Order, loggerFactory, FeatureType.ObjectsAndInterfaces) { }

        public string[] Prefixes => FixedPrefixes;

        private static string GetMenuName(ITypeSpecImmutable spec) =>  spec.GetFacet<INamedFacet>().NaturalName;


        private static string MatchMethod(string legacyName, Type declaringType) {
            var name = $"action{legacyName}";
            var action = declaringType.GetMethod(name, BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.Public);

            return action?.Name ?? legacyName;
        }


        private static IMenuImmutable ConvertLegacyToNOFMenu(MainMenu legacyMenu, Type type, IMetamodelBuilder metamodel) {
            var spec = metamodel.GetSpecification(type);
            var mi = new MenuImpl(metamodel, type, false, GetMenuName(spec));
            foreach (var menu in legacyMenu.Menus) {
                switch (menu) {
                    case SubMenu sm:
                        var nsm = mi.CreateSubMenu(sm.Name);
                        // temp hack
                        nsm.AddAction(MatchMethod(sm.Menus.Cast<IMenu>().First().Name, type));
                        break;
                    case Menu m:
                        mi.AddAction(MatchMethod(m.Name, type));
                        break;
                }
            }

            return mi;
        }


        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            // instance
            var method = MethodHelpers.FindMethod(reflector, type, MethodType.Class, "menuOrder", null, null);
            methodRemover.SafeRemoveMethod(method);
            var facet = method is not null ? (IFacet)new MenuFacetViaLegacyMethod(method, specification) : new MenuFacetDefault(specification);
            FacetUtils.AddFacet(facet);

            // mainMenu
            var method1 = MethodHelpers.FindMethod(reflector, type, MethodType.Class, "sharedMenuOrder", null, null);
            methodRemover.SafeRemoveMethod(method1);

            if (method1 is not null) {
                void Action(IMetamodelBuilder builder) {
                    var legacyMenu = (MainMenu)InvokeUtils.InvokeStatic(method1, new object[] { });
                    var mainMenu = ConvertLegacyToNOFMenu(legacyMenu, method1.DeclaringType, builder);
                    builder.AddMainMenu(mainMenu);
                }

                FacetUtils.AddFacet(new IntegrationFacet(specification, Action));
            }

            return metamodel;
        }
    }
}