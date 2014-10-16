// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Objects.ViewModel;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.ViewModel {
    public class ViewModelEditFacetConvention : ViewModelFacetConvention {
        public ViewModelEditFacetConvention(ISpecification holder) : base(Type, holder) {}

        private static Type Type {
            get { return typeof (IViewModelFacet); }
        }

        public override bool IsEditView(INakedObject nakedObject) {
            return true;
        }
    }
}