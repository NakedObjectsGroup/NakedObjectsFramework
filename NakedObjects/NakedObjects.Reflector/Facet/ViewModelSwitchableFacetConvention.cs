// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedFramework;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.Spec;
using NakedFramework.Core.Error;
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.Facet;

namespace NakedObjects.Reflector.Facet {
    [Serializable]
    public sealed class ViewModelSwitchableFacetConvention : ViewModelFacetAbstract {
        public ViewModelSwitchableFacetConvention(ISpecification holder) : base(Type, holder) { }

        private static Type Type => typeof(IViewModelFacet);

        public override string[] Derive(INakedObjectAdapter nakedObjectAdapter, INakedObjectsFramework framework) => nakedObjectAdapter.GetDomainObject<IViewModel>().DeriveKeys();

        public override void Populate(string[] keys, INakedObjectAdapter nakedObjectAdapter, INakedObjectsFramework framework) => nakedObjectAdapter.GetDomainObject<IViewModel>().PopulateUsingKeys(keys);

        public override bool IsEditView(INakedObjectAdapter nakedObjectAdapter, INakedObjectsFramework framework) {
            var target = nakedObjectAdapter.GetDomainObject<IViewModelSwitchable>();

            if (target == null) {
                throw new NakedObjectSystemException(nakedObjectAdapter.Object == null
                                                         ? "Null domain object"
                                                         : $"Wrong type of domain object: {nakedObjectAdapter.Object.GetType().FullName}");
            }

            return target.IsEditView();
        }
    }
}