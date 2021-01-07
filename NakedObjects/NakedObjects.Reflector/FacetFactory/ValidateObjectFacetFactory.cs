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
using System.Threading;
using Microsoft.Extensions.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Util;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;
using NakedObjects.ParallelReflector.FacetFactory;
using NakedObjects.ParallelReflector.Utils;

namespace NakedObjects.Reflector.FacetFactory {
    public sealed class ValidateObjectFacetFactory : ObjectFacetFactoryProcessor, IMethodPrefixBasedFacetFactory {
        private static readonly string[] FixedPrefixes = {
            RecognisedMethodsAndPrefixes.ValidatePrefix
        };

        private ILogger<ValidateObjectFacetFactory> logger;

        public ValidateObjectFacetFactory(IFacetFactoryOrder<ValidateObjectFacetFactory> order, ILoggerFactory loggerFactory)
            : base(order.Order, loggerFactory, FeatureType.ObjectsAndInterfaces) =>
            logger = loggerFactory.CreateLogger<ValidateObjectFacetFactory>();

        public  string[] Prefixes => FixedPrefixes;

        private static bool ContainsField(string name, Type type, IClassStrategy classStrategy) =>
            type.GetProperties().Any(p => p.Name.Equals(name, StringComparison.Ordinal) &&
                                          p.HasPublicGetter() &&
                                          !classStrategy.IsIgnored(p) &&
                                          !CollectionUtils.IsCollection(p.PropertyType) &&
                                          !CollectionUtils.IsQueryable(p.PropertyType));

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector,  Type type, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var methodPeers = new List<ValidateObjectFacet.NakedObjectValidationMethod>();
            var methods = MethodHelpers.FindMethods(reflector, type, MethodType.Object, RecognisedMethodsAndPrefixes.ValidatePrefix, typeof(string));

            if (methods.Any()) {
                foreach (var method in methods) {
                    var parameters = method.GetParameters();
                    if (parameters.Length >= 2) {
                        var parametersMatch = parameters.Select(parameter => parameter.Name).Select(name => $"{name[0].ToString(Thread.CurrentThread.CurrentCulture).ToUpper()}{name.Substring(1)}").All(p => ContainsField(p, type, reflector.ClassStrategy));
                        if (parametersMatch) {
                            methodPeers.Add(new ValidateObjectFacet.NakedObjectValidationMethod(method, Logger<ValidateObjectFacet.NakedObjectValidationMethod>()));
                            methodRemover.SafeRemoveMethod(method);
                        }
                    }
                }
            }

            var validateFacet = methodPeers.Any() ? (IValidateObjectFacet) new ValidateObjectFacet(specification, methodPeers, Logger<ValidateObjectFacet>()) : new ValidateObjectFacetNull(specification);
            FacetUtils.AddFacet(validateFacet);
            return metamodel;
        }
    }
}