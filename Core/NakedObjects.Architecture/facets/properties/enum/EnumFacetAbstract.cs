// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Architecture.Facets.Properties.Enums {
    public abstract class EnumFacetAbstract : MarkerFacetAbstract, IEnumFacet {
        protected EnumFacetAbstract(IFacetHolder holder)
            : base(Type, holder) {}

        public static Type Type {
            get { return typeof (IEnumFacet); }
        }

        #region IEnumFacet Members

        public abstract object[] GetChoices(INakedObject inObject);
        public abstract object[] GetChoices(INakedObject inObject, object[] choiceValues);
        public abstract string GetTitle(INakedObject inObject);

        #endregion
    }
}