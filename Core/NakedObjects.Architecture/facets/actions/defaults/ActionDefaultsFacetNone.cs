// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Reflect;

namespace NakedObjects.Architecture.Facets.Actions.Defaults {
    public class ActionDefaultsFacetNone : ActionDefaultsFacetAbstract {
        public ActionDefaultsFacetNone(IFacetHolder holder)
            : base(holder) {}

        public override bool IsNoOp {
            get { return true; }
        }

        public override Tuple<object, TypeOfDefaultValue> GetDefault(INakedObject nakedObject) {
            return null;
        }
    }


    // Copyright (c) Naked Objects Group Ltd.
}