// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Objects.ViewModel;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.ViewModel {
    public class ViewModelFacetConvention : ViewModelFacetAbstract {
        public ViewModelFacetConvention(IFacetHolder holder)
            : base(Type, holder) {}

        protected ViewModelFacetConvention(Type type, IFacetHolder holder)
            : base(type, holder) {}

        private static Type Type {
            get { return typeof (IViewModelFacet); }
        }

        public override string[] Derive(INakedObject nakedObject) {
            return nakedObject.GetDomainObject<IViewModel>().DeriveKeys();
        }

        public override void Populate(string[] keys, INakedObject nakedObject) {
            nakedObject.GetDomainObject<IViewModel>().PopulateUsingKeys(keys);
        }
    }
}