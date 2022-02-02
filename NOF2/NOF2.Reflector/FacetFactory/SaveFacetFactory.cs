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
using NakedFramework.Metamodel.Utils;
using NakedFramework.ParallelReflector.FacetFactory;
using NakedFramework.ParallelReflector.Utils;
using NOF2.About;
using NOF2.Reflector.Facet;
using NOF2.Reflector.Helpers;

namespace NOF2.Reflector.FacetFactory;

public sealed class SaveFacetFactory : AbstractNOF2FacetFactoryProcessor, IMethodPrefixBasedFacetFactory {
    private static readonly string[] FixedPrefixes;

    static SaveFacetFactory() {
        FixedPrefixes = new[] { RecognisedMethodsAndPrefixes.MenuMethod };
    }

    public SaveFacetFactory(IFacetFactoryOrder<SaveFacetFactory> order, ILoggerFactory loggerFactory)
        : base(order.Order, loggerFactory, FeatureType.ObjectsAndInterfaces) { }

    public string[] Prefixes => FixedPrefixes;

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        // instance
        const string name = "actionsave";
        var saveMethod = MethodHelpers.FindMethod(reflector, type, MethodType.Object, name, typeof(void), null);
        methodRemover.SafeRemoveMethod(saveMethod);

        IFacet saveFacet;
        if (saveMethod is not null) {
            var aboutSaveMethod = MethodHelpers.FindMethod(reflector, type, MethodType.Object, $"{NOF2Helpers.AboutPrefix}{name}", typeof(void), new[] { typeof(ActionAbout) });

            methodRemover.SafeRemoveMethod(aboutSaveMethod);

            if (aboutSaveMethod is null) {
                saveFacet = new SaveViaActionSaveFacet(saveMethod, specification, Logger<SaveViaActionSaveFacet>());
            }
            else {
                saveFacet = new SaveViaActionSaveWithAboutFacet(saveMethod, aboutSaveMethod, specification, Logger<SaveViaActionSaveWithAboutFacet>());
            }
        }
        else {
            saveFacet = new SaveNullFacet(specification, Logger<SaveNullFacet>());
        }

        FacetUtils.AddFacet(saveFacet);

        return metamodel;
    }
}