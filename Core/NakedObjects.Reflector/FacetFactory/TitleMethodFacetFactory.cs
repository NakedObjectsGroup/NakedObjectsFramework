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
using Microsoft.Extensions.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;
using NakedObjects.ParallelReflector.FacetFactory;
using NakedObjects.ParallelReflector.Utils;

namespace NakedObjects.Reflector.FacetFactory {
    public sealed class TitleMethodFacetFactory : ObjectFacetFactoryProcessor, IMethodPrefixBasedFacetFactory, IAnnotationBasedFacetFactory {
        private static readonly string[] FixedPrefixes = {
            RecognisedMethodsAndPrefixes.ToStringMethod,
            RecognisedMethodsAndPrefixes.TitleMethod
        };

        private readonly ILogger<TitleMethodFacetFactory> logger;

        public TitleMethodFacetFactory(IFacetFactoryOrder<TitleMethodFacetFactory> order, ILoggerFactory loggerFactory)
            : base(order.Order, loggerFactory, FeatureType.ObjectsAndInterfaces) =>
            logger = loggerFactory.CreateLogger<TitleMethodFacetFactory>();

        public  string[] Prefixes => FixedPrefixes;

        /// <summary>
        ///     If no title or ToString can be used then will use Facets provided by
        ///     <see cref="FallbackFacetFactory" /> instead.
        /// </summary>
        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector,  Type type, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            IList<MethodInfo> attributedMethods = new List<MethodInfo>();
            foreach (var propertyInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Instance)) {
                if (propertyInfo.GetCustomAttribute<TitleAttribute>() is not null) {
                    if (attributedMethods.Count > 0) {
                        logger.LogWarning($"Title annotation is used more than once in {type.Name}, this time on property {propertyInfo.Name}; this will be ignored");
                    }

                    attributedMethods.Add(propertyInfo.GetGetMethod());
                }
            }

            if (attributedMethods.Count > 0) {
                // attributes takes priority
                FacetUtils.AddFacet(new TitleFacetViaProperty(attributedMethods.First(), specification, Logger<TitleFacetViaProperty>()));
                return metamodel;
            }

            try {
                var titleMethod = MethodHelpers.FindMethod(reflector, type, MethodType.Object, RecognisedMethodsAndPrefixes.TitleMethod, typeof(string), Type.EmptyTypes);
                IFacet titleFacet = null;

                methodRemover.SafeRemoveMethod(titleMethod);
                if (titleMethod is not null) {
                    titleFacet = new TitleFacetViaTitleMethod(titleMethod, specification, Logger<TitleFacetViaTitleMethod>());
                }

                var toStringMethod = MethodHelpers.FindMethod(reflector, type, MethodType.Object, RecognisedMethodsAndPrefixes.ToStringMethod, typeof(string), Type.EmptyTypes);
                if (toStringMethod is not null && toStringMethod.DeclaringType != typeof(object)) {
                    methodRemover.SafeRemoveMethod(toStringMethod);
                }
                else {
                    // on object do not use
                    toStringMethod = null;
                }

                var maskMethod = MethodHelpers.FindMethod(reflector, type, MethodType.Object, RecognisedMethodsAndPrefixes.ToStringMethod, typeof(string), new[] {typeof(string)});

                
                methodRemover.SafeRemoveMethod(maskMethod);
                

                // todo does this make sense ? 
                if (titleFacet is null && toStringMethod is not null) {
                    titleFacet = new TitleFacetViaToStringMethod(maskMethod, specification, Logger<TitleFacetViaToStringMethod>());
                }

                FacetUtils.AddFacet(titleFacet);
            }
            catch (Exception e) {
                logger.LogError(e, "Unexpected Exception");
            }

            return metamodel;
        }
    }
}