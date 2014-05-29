// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Architecture.Facets.Actions.Choices {
    public abstract class ActionChoicesFacetAbstract : FacetAbstract, IActionChoicesFacet {
        protected ActionChoicesFacetAbstract(IFacetHolder holder)
            : base(Type, holder) {}

        public static Type Type {
            get { return typeof (IActionChoicesFacet); }
        }

        #region IActionChoicesFacet Members

        public abstract object[] GetChoices(INakedObject nakedObject, IDictionary<string, INakedObject> parameterNameValues);
        public abstract Tuple<string, INakedObjectSpecification>[] ParameterNamesAndTypes { get; }
        public abstract bool IsMultiple { get; }

        #endregion
    }


    // Copyright (c) Naked Objects Group Ltd.
}