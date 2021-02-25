// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Immutable;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.FacetFactory;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.Utils;
using NakedObjects.ParallelReflector.FacetFactory;
using NakedObjects.ParallelReflector.Utils;

#pragma warning disable 612

namespace NakedObjects.Reflector.FacetFactory {
    public sealed class IconMethodFacetFactory : ObjectFacetFactoryProcessor, IMethodPrefixBasedFacetFactory {
        private static readonly string[] FixedPrefixes = {RecognisedMethodsAndPrefixes.IconNameMethod};

        public IconMethodFacetFactory(IFacetFactoryOrder<IconMethodFacetFactory> order, ILoggerFactory loggerFactory)
            : base(order.Order, loggerFactory, FeatureType.ObjectsAndInterfaces) { }

        public string[] Prefixes => FixedPrefixes;

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var method = MethodHelpers.FindMethod(reflector, type, MethodType.Object, RecognisedMethodsAndPrefixes.IconNameMethod, typeof(string), Type.EmptyTypes);
            var attribute = type.GetCustomAttribute<IconNameAttribute>();
            methodRemover.SafeRemoveMethod(method);
            if (method is not null) {
                FacetUtils.AddFacet(new IconFacetViaMethod(method, specification, attribute?.Value, Logger<IconFacetViaMethod>()));
            }
            else {
                FacetUtils.AddFacet(Create(attribute, specification));
            }

            return metamodel;
        }

        private static IIconFacet Create(IconNameAttribute attribute, ISpecification holder) => attribute != null ? new IconFacetAnnotation(attribute.Value, holder) : null;
    }
}