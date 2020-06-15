// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Util;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;

namespace NakedObjects.Reflect.FacetFactory {
    public sealed class TableViewAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        private readonly ILogger<TableViewAnnotationFacetFactory> logger;

        public TableViewAnnotationFacetFactory(int numericOrder, ILoggerFactory loggerFactory)
            : base(numericOrder, loggerFactory, FeatureType.CollectionsAndActions) =>
            logger = loggerFactory.CreateLogger<TableViewAnnotationFacetFactory>();

        private void Process(MemberInfo member, Type methodReturnType, ISpecification specification) {
            if (CollectionUtils.IsGenericEnumerable(methodReturnType) || CollectionUtils.IsCollection(methodReturnType)) {
                var attribute = member.GetCustomAttribute<TableViewAttribute>();
                FacetUtils.AddFacet(Create(attribute, specification));
            }
        }

        private ITableViewFacet CreateTableViewFacet(TableViewAttribute attribute, ISpecification holder) {
            var columns = attribute.Columns == null ? new string[] { } : attribute.Columns;
            var distinctColumns = columns.Distinct().ToArray();

            if (columns.Length != distinctColumns.Length) {
                // we had duplicates - log 
                var duplicates = columns.GroupBy(x => x).Where(g => g.Count() > 1).Select(g => g.Key).Aggregate("", (s, t) => s != "" ? s + ", " + t : t);
                var name = holder.Identifier == null ? "Unknown" : holder.Identifier.ToString();
                logger.LogWarning($"Table View on {name} had duplicate columns {duplicates}");
                columns = distinctColumns;
            }

            return new TableViewFacet(attribute.Title, columns, holder);
        }

        private ITableViewFacet Create(TableViewAttribute attribute, ISpecification holder) => attribute == null ? null : CreateTableViewFacet(attribute, holder);

        public override void Process(IReflector reflector, MethodInfo method, IMethodRemover methodRemover, ISpecificationBuilder specification) => Process(method, method.ReturnType, specification);

        public override void Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification) {
            if (property.GetGetMethod() != null) {
                Process(property, property.PropertyType, specification);
            }
        }
    }
}