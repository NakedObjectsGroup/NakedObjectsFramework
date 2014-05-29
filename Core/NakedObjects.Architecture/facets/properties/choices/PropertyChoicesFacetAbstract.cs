// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Architecture.Facets.Properties.Choices {
    public abstract class PropertyChoicesFacetAbstract : FacetAbstract, IPropertyChoicesFacet {
        protected PropertyChoicesFacetAbstract(IFacetHolder holder)
            : base(Type, holder) {}

        public static Type Type {
            get { return typeof (IPropertyChoicesFacet); }
        }

        #region IPropertyChoicesFacet Members

        public abstract object[] GetChoices(INakedObject nakedObject, IDictionary<string, INakedObject> parameterNameValues);
        public abstract Tuple<string, INakedObjectSpecification>[] ParameterNamesAndTypes { get; }
       

        #endregion
    }
}