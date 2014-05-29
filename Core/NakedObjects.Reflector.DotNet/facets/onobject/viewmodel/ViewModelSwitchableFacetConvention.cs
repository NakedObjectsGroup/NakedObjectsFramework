// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Objects.ViewModel;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.ViewModel {
    public class ViewModelSwitchableFacetConvention : ViewModelFacetConvention {
        public ViewModelSwitchableFacetConvention(IFacetHolder holder) : base(Type, holder) {}

        private static Type Type {
            get { return typeof (IViewModelFacet); }
        }


        public override bool IsEditView(INakedObject nakedObject) {
            var target = nakedObject.GetDomainObject<IViewModelSwitchable>();

            if (target != null) {
                return target.IsEditView();
            }

            throw new NakedObjectSystemException(nakedObject.Object == null ? "Null domain object" : "Wrong type of domain object: " + nakedObject.Object.GetType().FullName);
        }
    }
}