// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Immutable;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.FacetFactory;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.Utils;
using NakedLegacy.Attribute;
using NakedLegacy.Reflector.Helpers;

namespace NakedLegacy.Reflector.FacetFactory;

public sealed class LengthAnnotationFacetFactory : LegacyFacetFactoryProcessor, IAnnotationBasedFacetFactory {
    private const string FieldOrder = "FieldOrder";
    private readonly ILogger<LengthAnnotationFacetFactory> logger;

    public LengthAnnotationFacetFactory(IFacetFactoryOrder<LengthAnnotationFacetFactory> order, ILoggerFactory loggerFactory)
        : base(order.Order, loggerFactory, FeatureType.PropertiesAndCollections) =>
        logger = Logger<LengthAnnotationFacetFactory>();

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        var attr = property.GetCustomAttribute<IMaxLengthAttribute>();

        // expect more to be added 

        switch (attr) {
            case { } max:
                FacetUtils.AddFacet(new MaxLengthFacetAnnotation(max.MaxLength, specification));
                break;
        }

        return metamodel;
    }
}