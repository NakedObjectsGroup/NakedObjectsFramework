// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;

namespace NakedFunctions.Meta.Facet {
    [Serializable]
    public sealed class InjectedQueryableParameterFacet : FacetAbstract, IInjectedParameterFacet {
        private readonly Type typeOfQueryable;

        public InjectedQueryableParameterFacet(ISpecification holder, Type typeOfQueryable) : base(Type, holder) => this.typeOfQueryable = InjectUtils.GetMatchingImpl(typeOfQueryable);

        public static Type Type => typeof(IInjectedParameterFacet);

        #region IInjectedParameterFacet Members

        public object GetInjectedValue(INakedObjectsFramework framework) {
            var f = typeof(InjectUtils).GetMethod("GetInjectedQueryableValue")?.MakeGenericMethod(typeOfQueryable);
            return f?.Invoke(null, new object[] {framework.Persistor});
        }

        #endregion
    }
}