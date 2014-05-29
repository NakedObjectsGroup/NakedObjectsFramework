// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Architecture.Facets.AutoComplete {
    public abstract class AutoCompleteFacetAbstract : FacetAbstract, IAutoCompleteFacet {
        protected const int DefaultPageSize = 50;

        protected AutoCompleteFacetAbstract(IFacetHolder holder)
            : base(Type, holder) {}

        public static Type Type {
            get { return typeof (IAutoCompleteFacet); }
        }

        public int PageSize { get; protected set; }

        public int MinLength { get; protected set; }

        #region IPropertyAutoCompleteFacet Members

        public abstract object[] GetCompletions(INakedObject nakedObject, string autoCompleteParm);

        #endregion
    }
}