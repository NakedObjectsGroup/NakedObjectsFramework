// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Architecture.Facets.Actions.Invoke {
    public abstract class ActionInvocationFacetAbstract : FacetAbstract, IActionInvocationFacet {
        protected ActionInvocationFacetAbstract(IFacetHolder holder)
            : base(Type, holder) {}

        public static Type Type {
            get { return typeof (IActionInvocationFacet); }
        }

        #region IActionInvocationFacet Members

        public abstract INakedObjectSpecification OnType { get; }
        public abstract INakedObjectSpecification ReturnType { get; }

        public abstract INakedObject Invoke(INakedObject nakedObject, INakedObject[] parameters);

        public abstract INakedObject Invoke(INakedObject nakedObject, INakedObject[] parameters, int resultPage);

        public virtual bool GetIsRemoting(INakedObject target) {
            return false;
        }

        #endregion
    }


    // Copyright (c) Naked Objects Group Ltd.
}