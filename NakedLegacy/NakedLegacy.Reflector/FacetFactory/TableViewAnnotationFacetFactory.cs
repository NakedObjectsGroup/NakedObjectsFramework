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
using NakedLegacy.Attribute;
using NakedLegacy.Reflector.Helpers;

namespace NakedLegacy.Reflector.FacetFactory;

public sealed class TableViewAnnotationFacetFactory : LegacyFacetFactoryProcessor, IAnnotationBasedFacetFactory {
    private readonly ILogger<TableViewAnnotationFacetFactory> logger;

    public TableViewAnnotationFacetFactory(IFacetFactoryOrder<TableViewAnnotationFacetFactory> order, ILoggerFactory loggerFactory)
        : base(order.Order, loggerFactory, FeatureType.CollectionsAndActions) =>
        logger = loggerFactory.CreateLogger<TableViewAnnotationFacetFactory>();

    private void Process(MemberInfo member, Type methodReturnType, ISpecification specification) {
        if (CollectionUtils.IsGenericEnumerable(methodReturnType) || CollectionUtils.IsCollection(methodReturnType)) {
            var attribute = member.GetCustomAttribute<ITableViewAttribute>();
            FacetUtils.AddFacet(Create(attribute, specification));
        }
    }

    private ITableViewFacet CreateTableViewFacet(ITableViewAttribute attribute, ISpecification holder) {
        var columns = attribute.TableColumns ?? new string[] { };
        var distinctColumns = columns.Distinct().ToArray();

        if (columns.Length != distinctColumns.Length) {
            // we had duplicates - log
            var duplicates = columns.GroupBy(x => x).Where(g => g.Count() > 1).Select(g => g.Key).Aggregate("", (s, t) => s != "" ? $"{s}, {t}" : t);
            var name = holder.Identifier == null ? "Unknown" : holder.Identifier.ToString();
            logger.LogWarning($"Table View on {name} had duplicate columns {duplicates}");
            columns = distinctColumns;
        }

        return new TableViewFacet(attribute.TableTitle, columns, holder);
    }

    private ITableViewFacet Create(ITableViewAttribute attribute, ISpecification holder) => attribute is null ? null : CreateTableViewFacet(attribute, holder);

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