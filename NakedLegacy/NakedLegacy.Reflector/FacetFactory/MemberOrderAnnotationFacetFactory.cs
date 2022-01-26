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
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.Utils;
using NakedFramework.ParallelReflector.Utils;
using NakedLegacy.Attribute;

namespace NakedLegacy.Reflector.FacetFactory;

public sealed class MemberOrderAnnotationFacetFactory : LegacyFacetFactoryProcessor, IAnnotationBasedFacetFactory, IMethodFilteringFacetFactory {
    private const string FieldOrder = "FieldOrder";
    private readonly ILogger<MemberOrderAnnotationFacetFactory> logger;

    public MemberOrderAnnotationFacetFactory(IFacetFactoryOrder<MemberOrderAnnotationFacetFactory> order, ILoggerFactory loggerFactory)
        : base(order.Order, loggerFactory, FeatureType.PropertiesAndCollections) =>
        logger = Logger<MemberOrderAnnotationFacetFactory>();

    public bool Filters(MethodInfo method, IClassStrategy classStrategy) => method.IsStatic && method.Name is FieldOrder;

    private string[] GetOrderedMemberNames(MethodInfo method) {
        try {
            var orderedMembers = (string)method.Invoke(null, null);
            return orderedMembers.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(s => s.ToLower()).ToArray();
        }
        catch (Exception e) {
            logger.LogWarning($"Failed to get member order string from {method} : {e.Message}");
        }

        return Array.Empty<string>();
    }

    public IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MemberInfo member, string orderMethodName, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        var memberName = member.Name;
        var declaringType = member.DeclaringType;

        var method = MethodHelpers.FindMethod(reflector, declaringType, MethodType.Class, orderMethodName, typeof(string), null);
        IFacet facet = null;

        if (method is not null) {
            var orderedMemberNames = GetOrderedMemberNames(method);
            if (orderedMemberNames.Any()) {
                var index = Array.IndexOf(orderedMemberNames, memberName.ToLower());
                if (index >= 0) {
                    facet = new MemberOrderFacet("", index.ToString(), specification);
                }
                else {
                    logger.LogWarning($"Failed to find member {memberName} in {orderMethodName} string on {declaringType}");
                }
            }
        }

        var attr = member.GetCustomAttributes().FirstOrDefault(a => a is IMemberOrderAttribute);

        if (attr is IMemberOrderAttribute attribute) {
            if (facet is not null) {
                logger.LogWarning($"Member {memberName} on {declaringType} has MemberOrder annotation and is in {orderMethodName} annotation will take priority");
            }

            facet = new MemberOrderFacet("", attribute.Order.ToString(), specification);
        }

        FacetUtils.AddFacet(facet);
        return metamodel;
    }

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) => Process(reflector, property, FieldOrder, specification, metamodel);
}