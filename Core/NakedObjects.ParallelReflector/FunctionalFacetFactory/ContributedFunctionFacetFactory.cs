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
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using NakedFunctions.Meta.Facet;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Meta.Utils;
using NakedObjects.ParallelReflect.FacetFactory;

namespace NakedObjects.ParallelReflect.FunctionalFacetFactory {
    /// <summary>
    ///     Creates an <see cref="IContributedActionFacet" /> based on the presence of an
    ///     <see cref="ContributedActionAttribute" /> annotation
    /// </summary>
    public sealed class ContributedFunctionFacetFactory : FacetFactoryAbstract {
        private readonly ILogger<ContributedFunctionFacetFactory> logger;

        public ContributedFunctionFacetFactory(IFacetFactoryOrder<ContributedFunctionFacetFactory> order, ILoggerFactory loggerFactory)
            : base(order.Order, loggerFactory, FeatureType.Actions, ReflectionType.Functional) =>
            logger = loggerFactory.CreateLogger<ContributedFunctionFacetFactory>();


        private static bool IsContributed(MethodInfo member) {
            var firstParam = member.GetParameters().FirstOrDefault();

            return member.IsDefined(typeof(ExtensionAttribute), false) ||
                   firstParam != null && firstParam.IsDefined(typeof(ContributedActionAttribute), false);
        }

        private static Type GetContributeeType(MethodInfo member) => IsContributed(member) ? member.GetParameters().First().ParameterType : member.DeclaringType;

        private IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, IClassStrategy classStrategy, MethodInfo member, ISpecification holder, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            // all functions are contributed to first parameter or if menu, itself

            var facet = new ContributedFunctionFacet(holder);

            var contributeeType = GetContributeeType(member);
            var result = reflector.LoadSpecification(contributeeType, classStrategy, metamodel);
            metamodel = result.Item2;

            var type = result.Item1 as ITypeSpecImmutable;
            if (type != null) {
                facet.AddContributee(type);
            }

            FacetUtils.AddFacet(facet);
            return metamodel;
        }

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, IClassStrategy classStrategy, MethodInfo method, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) => Process(reflector, classStrategy, method, specification, metamodel);
    }
}