// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    public class ContributedActionAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        public ContributedActionAnnotationFacetFactory(int numericOrder)
            : base(numericOrder, FeatureType.Action) {}

        private void Process(IReflector reflector, MethodInfo member, ISpecification holder) {
            var allParams = member.GetParameters();
            IEnumerable<ParameterInfo> paramsWithAttribute = allParams.Where(p => p.GetCustomAttribute<ContributedActionAttribute>() != null);
            if (!paramsWithAttribute.Any()) return; //Nothing to do
            var facet = new ContributedActionFacet(holder);
            foreach (ParameterInfo p in paramsWithAttribute) {
                var attribute = p.GetCustomAttribute<ContributedActionAttribute>();
                var type = reflector.LoadSpecification<IObjectSpecImmutable> (p.ParameterType);
                if (type != null && !type.IsParseable) {
                    //TODO: This guard is really only there for a unit test -  SMELL! Should be mocked out
                    if (type.IsCollection) {
                        if (type.IsQueryable) {
                            var returnType = reflector.LoadSpecification<IObjectSpecImmutable>(member.ReturnType);
                            if (!returnType.IsQueryable) {
                                //Don't allow collection-contributed actions that return collections
                                Type elementType = p.ParameterType.GetGenericArguments()[0];
                                type = reflector.LoadSpecification<IObjectSpecImmutable>(elementType);
                                facet.AddCollectionContributee(type, attribute.SubMenu, attribute.Id);
                            }
                        }
                    } else {
                        facet.AddObjectContributee(type, attribute.SubMenu, attribute.Id);
                    }
                }
            }
            FacetUtils.AddFacet(facet);
        }

        public override void Process(IReflector reflector, MethodInfo method, IMethodRemover methodRemover, ISpecificationBuilder specification) {
            Process(reflector, method, specification);
        }
    }
}