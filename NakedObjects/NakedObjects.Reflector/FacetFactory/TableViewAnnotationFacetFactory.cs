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
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.Utils;
using NakedFramework.ParallelReflector.Utils;

namespace NakedObjects.Reflector.FacetFactory;

public sealed class TableViewAnnotationFacetFactory : DomainObjectFacetFactoryProcessor, IAnnotationBasedFacetFactory {
    private readonly ILogger<TableViewAnnotationFacetFactory> logger;

    public TableViewAnnotationFacetFactory(IFacetFactoryOrder<TableViewAnnotationFacetFactory> order, ILoggerFactory loggerFactory)
        : base(order.Order, loggerFactory, FeatureType.CollectionsAndActions) =>
        logger = loggerFactory.CreateLogger<TableViewAnnotationFacetFactory>();

    private void Process(MemberInfo member, Type methodReturnType, ISpecificationBuilder specification) {
        if (CollectionUtils.IsGenericEnumerable(methodReturnType) || CollectionUtils.IsCollection(methodReturnType)) {
            var attribute = member.GetCustomAttribute<TableViewAttribute>();
            FacetUtils.AddFacet(Create(attribute, specification), specification);
        }
    }

    private ITableViewFacet CreateTableViewFacet(TableViewAttribute attribute, ISpecification holder) => TableViewFacet.CreateTableViewFacet(attribute.Title, attribute.Columns, holder, logger);

    private ITableViewFacet Create(TableViewAttribute attribute, ISpecification holder) => attribute is null ? null : CreateTableViewFacet(attribute, holder);

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo method, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        Process(method, method.ReturnType, specification);
        return metamodel;
    }

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        if (property.HasPublicGetter()) {
            Process(property, property.PropertyType, specification);
        }

        return metamodel;
    }
}