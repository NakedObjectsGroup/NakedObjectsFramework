// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Immutable;
using Microsoft.Extensions.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;

namespace NakedObjects.ParallelReflect.FacetFactory {
    /// <summary>
    ///     Note - this factory simply removes the class level attribute from the list of methods.  The action and properties
    ///     look up this attribute directly
    /// </summary>
    public sealed class DisableDefaultMethodFacetFactory : MethodPrefixBasedFacetFactoryAbstract {
        private static readonly string[] FixedPrefixes;
        private readonly ILogger<DisableDefaultMethodFacetFactory> logger;

        static DisableDefaultMethodFacetFactory() =>
            FixedPrefixes = new[] {
                RecognisedMethodsAndPrefixes.DisablePrefix + "Action" + RecognisedMethodsAndPrefixes.DefaultPrefix,
                RecognisedMethodsAndPrefixes.DisablePrefix + "Property" + RecognisedMethodsAndPrefixes.DefaultPrefix
            };

        public DisableDefaultMethodFacetFactory(int numericOrder, ILoggerFactory loggerFactory)
            : base(numericOrder, loggerFactory, FeatureType.ObjectsAndInterfaces) =>
            logger = loggerFactory.CreateLogger<DisableDefaultMethodFacetFactory>();

        public override string[] Prefixes => FixedPrefixes;

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            try {
                foreach (var methodName in FixedPrefixes) {
                    var methodInfo = FindMethod(reflector, type, MethodType.Object, methodName, typeof(string), Type.EmptyTypes);
                    if (methodInfo != null) {
                        methodRemover.RemoveMethod(methodInfo);
                    }
                }
            }
            catch (Exception e) {
                logger.LogError(e, "Unexpected exception");
            }

            return metamodel;
        }
    }
}