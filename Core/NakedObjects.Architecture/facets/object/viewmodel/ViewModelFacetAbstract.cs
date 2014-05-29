// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Architecture.Facets.Objects.ViewModel {
    public abstract class ViewModelFacetAbstract : FacetAbstract, IViewModelFacet {
        protected ViewModelFacetAbstract(Type type, IFacetHolder holder)
            : base(type, holder) {}

        public abstract string[] Derive(INakedObject nakedObject);
        public abstract void Populate(string[] keys, INakedObject nakedObject);

        public virtual bool IsEditView(INakedObject nakedObject) {
            return false;
        }
    }
}