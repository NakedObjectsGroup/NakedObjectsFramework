// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects.Architecture.Facets {
    public static class FacetFilters {
        /// <summary>
        ///     <see cref="IFacetFilter.Accept" />s nothing
        /// </summary>
        public static readonly IFacetFilter FALSE;

        /// <summary>
        ///     <see cref="IFacetFilter.Accept" />s everything
        /// </summary>
        public static readonly IFacetFilter TRUE;

        static FacetFilters() {
            TRUE = new TrueFilter();
            FALSE = new FalseFilter();
        }


        public static IFacetFilter IsA(Type superClass) {
            return new IsAFilter(superClass);
        }

        public static IFacetFilter And(IFacetFilter f1, IFacetFilter f2) {
            return new AndFilter(f1, f2);
        }

        public static IFacetFilter Or(IFacetFilter f1, IFacetFilter f2) {
            return new OrFilter(f1, f2);
        }

        public static IFacetFilter Not(IFacetFilter f1) {
            return new NotFilter(f1);
        }

        public static IFacetFilter Any() {
            return new AnyFilter();
        }

        public static IFacetFilter None() {
            return new NoneFilter();
        }

        #region Nested type: AndFilter

        private class AndFilter : IFacetFilter {
            private readonly IFacetFilter f1;
            private readonly IFacetFilter f2;

            public AndFilter(IFacetFilter f1, IFacetFilter f2) {
                this.f1 = f1;
                this.f2 = f2;
            }

            #region IFacetFilter Members

            public bool Accept(IFacet f) {
                return f1.Accept(f) && f2.Accept(f);
            }

            #endregion
        }

        #endregion

        #region Nested type: AnyFilter

        private class AnyFilter : IFacetFilter {
            #region IFacetFilter Members

            public bool Accept(IFacet f) {
                return true;
            }

            #endregion
        }

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

        private class NoneFilter : IFacetFilter {
            #region IFacetFilter Members

            public bool Accept(IFacet f) {
                return false;
            }

            #endregion
        }

        #endregion

        #region Nested type: NotFilter

        private class NotFilter : IFacetFilter {
            private readonly IFacetFilter f1;

            public NotFilter(IFacetFilter f1) {
                this.f1 = f1;
            }

            #region IFacetFilter Members

            public bool Accept(IFacet f) {
                return !f1.Accept(f);
            }

            #endregion
        }

        #endregion

        #region Nested type: OrFilter

        private class OrFilter : IFacetFilter {
            private readonly IFacetFilter f1;
            private readonly IFacetFilter f2;

            public OrFilter(IFacetFilter f1, IFacetFilter f2) {
                this.f1 = f1;
                this.f2 = f2;
            }

            #region IFacetFilter Members

            public bool Accept(IFacet f) {
                return f1.Accept(f) || f2.Accept(f);
            }

            #endregion
        }

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