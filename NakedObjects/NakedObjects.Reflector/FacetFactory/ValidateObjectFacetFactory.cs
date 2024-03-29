// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading;
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
using NakedObjects.Reflector.Utils;

namespace NakedObjects.Reflector.FacetFactory;

public sealed class ValidateObjectFacetFactory : DomainObjectFacetFactoryProcessor, IMethodPrefixBasedFacetFactory {
    private static readonly string[] FixedPrefixes = {
        RecognisedMethodsAndPrefixes.ValidatePrefix
    };

    private ILogger<ValidateObjectFacetFactory> logger;

    public ValidateObjectFacetFactory(IFacetFactoryOrder<ValidateObjectFacetFactory> order, ILoggerFactory loggerFactory)
        : base(order.Order, loggerFactory, FeatureType.ObjectsAndInterfaces) =>
        logger = loggerFactory.CreateLogger<ValidateObjectFacetFactory>();

    public string[] Prefixes => FixedPrefixes;

    private static bool ContainsField(string name, Type type, IClassStrategy classStrategy) =>
        type.GetProperties().Any(p => p.Name.Equals(name, StringComparison.Ordinal) &&
                                      p.HasPublicGetter() &&
                                      !classStrategy.IsIgnored(p) &&
                                      !CollectionUtils.IsCollection(p.PropertyType) &&
                                      !CollectionUtils.IsQueryable(p.PropertyType));

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        var validateMethods = new List<MethodInfo>();
        var methods = ObjectMethodHelpers.FindMethods(reflector, type, MethodType.Object, RecognisedMethodsAndPrefixes.ValidatePrefix, typeof(string));

        if (methods.Any()) {
            foreach (var method in methods) {
                var parameters = method.GetParameters();
                if (parameters.Length >= 2) {
                    var parametersMatch = parameters.Select(parameter => parameter.Name).Select(name => $"{name[0].ToString(Thread.CurrentThread.CurrentCulture).ToUpper()}{name[1..]}").All(p => ContainsField(p, type, reflector.ClassStrategy));
                    if (parametersMatch) {
                        validateMethods.Add(method);
                        methodRemover.SafeRemoveMethod(method);
                    }
                }
            }
        }

        IValidateObjectFacet validateFacet = validateMethods.Any() ? new ValidateObjectFacet(validateMethods, Logger<ValidateObjectFacet>()) : ValidateObjectFacetNull.Instance;
        FacetUtils.AddFacet(validateFacet, specification);
        return metamodel;
    }
}