// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;

namespace NakedObjects.Architecture.Facets {
    public static class FacetFilters {
        static FacetFilters() {
          
        }


        public static IFacetFilter IsA(Type superClass) {
            return new IsAFilter(superClass);
        }

        #region Nested type: AndFilter

        #endregion

        #region Nested type: AnyFilter

        #endregion

        #region Nested type: FalseFilter

        public class FalseFilter : IFacetFilter {
            #region IFacetFilter Members

            public bool Accept(IFacet facet) {
                return false;
            }

            #endregion
        }

        #endregion

        #region Nested type: IsAFilter

        private class IsAFilter : IFacetFilter {
            private readonly Type superClass;

            public IsAFilter(Type superClass) {
                this.superClass = superClass;
            }

            #region IFacetFilter Members

            public bool Accept(IFacet facet) {
                return superClass.IsAssignableFrom(facet.GetType());
            }

            #endregion
        }

        #endregion

        #region Nested type: NoneFilter

        #endregion

        #region Nested type: NotFilter

        #endregion

        #region Nested type: OrFilter

        #endregion

        #region Nested type: TrueFilter

        public class TrueFilter : IFacetFilter {
            #region IFacetFilter Members

            public bool Accept(IFacet facet) {
                return true;
            }

            #endregion
        }

        #endregion
    }
}