// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

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
using NakedObjects.Util;

namespace NakedObjects.Reflect.FacetFactory {
    /// <summary>
    ///     Creates an <see cref="INotContributedActionFacet" /> based on the presence of an
    ///     <see cref="NotContributedActionAttribute" /> annotation
    /// </summary>
    public class ContributedActionAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        public ContributedActionAnnotationFacetFactory()
            : base(FeatureType.Action) {}

        private void Process(IReflector reflector, MemberInfo member, ISpecification holder) {
            var attribute = AttributeUtils.GetCustomAttribute<NotContributedActionAttribute>(member);
            FacetUtils.AddFacet(Create(reflector, attribute, holder));
        }

        public override void Process(IReflector reflector, MethodInfo method, IMethodRemover methodRemover, ISpecificationBuilder specification) {
            Process(reflector, method, specification);
        }

        private INotContributedActionFacet Create(IReflector reflector, NotContributedActionAttribute attribute, ISpecification holder) {
            return attribute == null ? null : new NotContributedActionFacet(holder, attribute.NotContributedToTypes.Select(reflector.LoadSpecification).Cast<IObjectSpecImmutable>().ToArray());
        }
    }
}