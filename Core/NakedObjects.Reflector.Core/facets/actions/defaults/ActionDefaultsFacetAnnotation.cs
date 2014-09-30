// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Actions.Defaults;
using NakedObjects.Architecture.Reflect;

namespace NakedObjects.Reflector.DotNet.Facets.Actions.Defaults {
    public class ActionDefaultsFacetAnnotation : ActionDefaultsFacetAbstract {
        private readonly object value;

        public ActionDefaultsFacetAnnotation(object value, IFacetHolder holder)
            : base(holder) {
            this.value = value;
        }

        public override bool CanAlwaysReplace {
            get { return false; }
        }

        public override Tuple<object, TypeOfDefaultValue> GetDefault(INakedObject nakedObject) {
            return new Tuple<object, TypeOfDefaultValue>(value, TypeOfDefaultValue.Explicit);
        }

        protected override string ToStringValues() {
            return "Value=" + value;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}