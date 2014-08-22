// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Persist;

namespace NakedObjects.Architecture.Facets.Objects.Ident.Title {
    public abstract class TitleFacetAbstract : FacetAbstract, ITitleFacet {
        protected TitleFacetAbstract(IFacetHolder holder)
            : base(Type, holder) {}

        public static Type Type {
            get { return typeof (ITitleFacet); }
        }

        #region ITitleFacet Members

        public virtual string GetTitleWithMask(string mask, INakedObject nakedObject, INakedObjectManager manager) {
            return GetTitle(nakedObject, manager);
        }

        public abstract string GetTitle(INakedObject nakedObject, INakedObjectManager manager);

        #endregion
    }
}