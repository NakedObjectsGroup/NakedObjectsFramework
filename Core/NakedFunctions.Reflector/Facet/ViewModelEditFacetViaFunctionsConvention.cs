// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Reflection;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;

namespace NakedFunctions.Meta.Facet {
    [Serializable]
    public sealed class ViewModelEditFacetViaFunctionsConvention : ViewModelFacetAbstract {
        private readonly MethodInfo deriveFunction;
        private readonly ISpecification holder;
        private readonly MethodInfo populateFunction;

        public ViewModelEditFacetViaFunctionsConvention(ISpecification holder, MethodInfo deriveFunction,
                                                        MethodInfo populateFunction) : base(Type, holder) {
            this.holder = holder;
            this.deriveFunction = deriveFunction;
            this.populateFunction = populateFunction;
        }

        private static Type Type => typeof(IViewModelFacet);

        public override string[] Derive(INakedObjectAdapter nakedObjectAdapter,
                                        INakedObjectManager nakedObjectManager,
                                        IDomainObjectInjector injector,
                                        ISession session,
                                        IObjectPersistor persistor) =>
            deriveFunction.Invoke(null, deriveFunction.GetParameterValues(nakedObjectAdapter, session, persistor)) as string[];

        public override void Populate(string[] keys,
                                      INakedObjectAdapter nakedObjectAdapter,
                                      INakedObjectManager nakedObjectManager,
                                      IDomainObjectInjector injector,
                                      ISession session,
                                      IObjectPersistor persistor) {
            var newVm = populateFunction.Invoke(null, populateFunction.GetParameterValues(nakedObjectAdapter, keys, session, persistor));

            nakedObjectAdapter.ReplacePoco(newVm);
        }

        public override bool IsEditView(INakedObjectAdapter nakedObjectAdapter, ISession session, IObjectPersistor persistor) => true;
    }
}