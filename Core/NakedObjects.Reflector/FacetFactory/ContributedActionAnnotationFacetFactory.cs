// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

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

namespace NakedObjects.Reflect.FacetFactory {
    /// <summary>
    ///     Creates an <see cref="IContributedActionFacet" /> based on the presence of an
    ///     <see cref="ContributedActionAttribute" /> annotation
    /// </summary>
    public sealed class ContributedActionAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        private readonly ILogger<ContributedActionAnnotationFacetFactory> logger;

        public ContributedActionAnnotationFacetFactory(int numericOrder, ILoggerFactory loggerFactory)
            : base(numericOrder, loggerFactory, FeatureType.Actions) =>
            logger = loggerFactory.CreateLogger<ContributedActionAnnotationFacetFactory>();

        private void Process(IReflector reflector, MethodInfo member, ISpecification holder) {
            var allParams = member.GetParameters();
            var paramsWithAttribute = allParams.Where(p => p.GetCustomAttribute<ContributedActionAttribute>() != null).ToArray();
            if (!paramsWithAttribute.Any()) {
                return; //Nothing to do
            }

            var facet = new ContributedActionFacet(holder);
            foreach (var p in paramsWithAttribute) {
                var attribute = p.GetCustomAttribute<ContributedActionAttribute>();
                var type = reflector.LoadSpecification<IObjectSpecImmutable>(p.ParameterType);
                if (type != null) {
                    if (type.IsParseable) {
                      logger.LogWarning($"ContributedAction attribute added to a value parameter type: {member.Name}");
                    }
                    else if (type.IsCollection) {
                        var parent = reflector.LoadSpecification(member.DeclaringType);

                        if (parent is IObjectSpecBuilder) {
                            AddLocalCollectionContributedAction(reflector, p, facet);
                        }
                        else {
                            AddCollectionContributedAction(reflector, member, type, p, facet, attribute);
                        }
                    }
                    else {
                        facet.AddObjectContributee(type, attribute.SubMenu, attribute.Id);
                    }
                }
            }

            FacetUtils.AddFacet(facet);
        }

        private void AddCollectionContributedAction(IReflector reflector, MethodInfo member, IObjectSpecImmutable type, ParameterInfo p, ContributedActionFacet facet, ContributedActionAttribute attribute) {
            if (!type.IsQueryable) {
                logger.LogWarning($"ContributedAction attribute added to a collection parameter type other than IQueryable: {member.Name}");
            }
            else {
                var returnType = reflector.LoadSpecification<IObjectSpecImmutable>(member.ReturnType);
                if (returnType.IsCollection) {
                    logger.LogWarning($"ContributedAction attribute added to an action that returns a collection: {member.Name}");
                }
                else {
                    var elementType = p.ParameterType.GetGenericArguments()[0];
                    type = reflector.LoadSpecification<IObjectSpecImmutable>(elementType);
                    facet.AddCollectionContributee(type, attribute.SubMenu, attribute.Id);
                }
            }
        }

        private static void AddLocalCollectionContributedAction(IReflector reflector, ParameterInfo p, ContributedActionFacet facet) {
            var elementType = p.ParameterType.GetGenericArguments()[0];
            var type = reflector.LoadSpecification<IObjectSpecImmutable>(elementType);
            facet.AddLocalCollectionContributee(type, p.Name);
        }

        public override void Process(IReflector reflector, MethodInfo method, IMethodRemover methodRemover, ISpecificationBuilder specification) {
            Process(reflector, method, specification);
        }
    }
}