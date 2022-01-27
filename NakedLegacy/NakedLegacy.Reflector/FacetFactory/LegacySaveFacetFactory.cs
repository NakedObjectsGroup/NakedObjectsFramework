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
using NakedLegacy.About;
using NakedLegacy.Reflector.Facet;
using NakedLegacy.Reflector.Helpers;

namespace NakedLegacy.Reflector.FacetFactory;

public sealed class LegacySaveFacetFactory : LegacyFacetFactoryProcessor, IMethodPrefixBasedFacetFactory {
    private static readonly string[] FixedPrefixes;

    static LegacySaveFacetFactory() {
        FixedPrefixes = new[] { RecognisedMethodsAndPrefixes.MenuMethod };
    }

    public LegacySaveFacetFactory(IFacetFactoryOrder<LegacySaveFacetFactory> order, ILoggerFactory loggerFactory)
        : base(order.Order, loggerFactory, FeatureType.ObjectsAndInterfaces) { }

    public string[] Prefixes => FixedPrefixes;

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        // instance
        const string name = "actionsave";
        var saveMethod = MethodHelpers.FindMethod(reflector, type, MethodType.Object, name, typeof(void), null);
        methodRemover.SafeRemoveMethod(saveMethod);
        var aboutSaveMethod = MethodHelpers.FindMethod(reflector, type, MethodType.Object, $"{LegacyHelpers.AboutPrefix}{name}", typeof(void), new[] { typeof(ActionAbout) });

        methodRemover.SafeRemoveMethod(aboutSaveMethod);

        var facet = saveMethod is not null ? (IFacet)new SaveViaActionSaveFacet(saveMethod, aboutSaveMethod, specification, Logger<SaveViaActionSaveFacet>()) : new SaveNullFacet(specification, Logger<SaveNullFacet>());
        FacetUtils.AddFacet(facet);

        return metamodel;
    }
}