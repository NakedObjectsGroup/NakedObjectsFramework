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
using Common.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Util;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;

namespace NakedObjects.ParallelReflect.FacetFactory {
    /// <summary>
    ///     Creates an <see cref="IContributedActionFacet" /> based on the presence of an
    ///     <see cref="ContributedActionAttribute" /> annotation
    /// </summary>
    public sealed class ContributedActionAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ContributedActionAnnotationFacetFactory));

        public ContributedActionAnnotationFacetFactory(int numericOrder)
            : base(numericOrder, FeatureType.Actions) { }

        private static bool IsParseable(Type type) => type.IsValueType;

        private static bool IsCollection(Type type) =>
            type != null && (
                CollectionUtils.IsGenericEnumerable(type) ||
                type.IsArray ||
                CollectionUtils.IsCollectionButNotArray(type) ||
                IsCollection(type.BaseType) ||
                type.GetInterfaces().Where(i => i.IsPublic).Any(IsCollection));

        private static IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo member, ISpecification holder, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var allParams = member.GetParameters();
            var paramsWithAttribute = allParams.Where(p => p.GetCustomAttribute<ContributedActionAttribute>() != null).ToArray();
            if (!paramsWithAttribute.Any()) {
                return metamodel; //Nothing to do
            }

            var facet = new ContributedActionFacet(holder);
            foreach (var p in paramsWithAttribute) {
                var attribute = p.GetCustomAttribute<ContributedActionAttribute>();
                var parameterType = p.ParameterType;
                IObjectSpecBuilder type;
                (type, metamodel) = reflector.LoadSpecification<IObjectSpecBuilder>(p.ParameterType, metamodel);

                if (type != null) {
                    if (IsParseable(parameterType)) {
                        Log.WarnFormat("ContributedAction attribute added to a value parameter type: {0}", member.Name);
                    }
                    else if (IsCollection(parameterType)) {
                        IObjectSpecImmutable parent;
                        (parent, metamodel) = reflector.LoadSpecification<IObjectSpecImmutable>(member.DeclaringType, metamodel);
                        metamodel = parent is IObjectSpecBuilder
                            ? AddLocalCollectionContributedAction(reflector, p, facet, metamodel)
                            : AddCollectionContributedAction(reflector, member, parameterType, p, facet, attribute, metamodel);
                    }
                    else {
                        facet.AddObjectContributee(type, attribute.SubMenu, attribute.Id);
                    }
                }
            }

            FacetUtils.AddFacet(facet);
            return metamodel;
        }

        private static IImmutableDictionary<string, ITypeSpecBuilder> AddCollectionContributedAction(IReflector reflector, MethodInfo member, Type parameterType, ParameterInfo p, ContributedActionFacet facet, ContributedActionAttribute attribute, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            if (!CollectionUtils.IsGenericQueryable(parameterType)) {
                Log.WarnFormat("ContributedAction attribute added to a collection parameter type other than IQueryable: {0}", member.Name);
            }
            else {
                var returnType = member.ReturnType;
                (_, metamodel) = reflector.LoadSpecification<IObjectSpecImmutable>(returnType, metamodel);
                if (IsCollection(returnType)) {
                    Log.WarnFormat("ContributedAction attribute added to an action that returns a collection: {0}", member.Name);
                }
                else {
                    var elementType = p.ParameterType.GetGenericArguments()[0];
                    IObjectSpecBuilder type;
                    (type, metamodel) = reflector.LoadSpecification<IObjectSpecBuilder>(elementType, metamodel);
                    facet.AddCollectionContributee(type, attribute.SubMenu, attribute.Id);
                }
            }

            return metamodel;
        }

        private static IImmutableDictionary<string, ITypeSpecBuilder> AddLocalCollectionContributedAction(IReflector reflector, ParameterInfo p, ContributedActionFacet facet, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var elementType = p.ParameterType.GetGenericArguments()[0];
            IObjectSpecBuilder type;
            (type, metamodel) = reflector.LoadSpecification<IObjectSpecBuilder>(elementType, metamodel);
            facet.AddLocalCollectionContributee(type, p.Name);
            return metamodel;
        }

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo method, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) => Process(reflector, method, specification, metamodel);
    }
}