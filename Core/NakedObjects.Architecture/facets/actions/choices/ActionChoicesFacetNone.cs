// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Architecture.Facets.Actions.Choices {
    public class ActionChoicesFacetNone : ActionChoicesFacetAbstract {
        public ActionChoicesFacetNone(IFacetHolder holder)
            : base(holder) {}

        public override bool IsNoOp {
            get { return true; }
        }

        public override Tuple<string, INakedObjectSpecification>[] ParameterNamesAndTypes {
            get { return new Tuple<string, INakedObjectSpecification>[]{}; }
        }

        public override bool IsMultiple {
            get { return false; }
        }

        public override object[] GetChoices(INakedObject nakedObject, IDictionary<string, INakedObject> parameterNameValues) {
            return new object[0];
        }
    }


    // Copyright (c) Naked Objects Group Ltd.
}