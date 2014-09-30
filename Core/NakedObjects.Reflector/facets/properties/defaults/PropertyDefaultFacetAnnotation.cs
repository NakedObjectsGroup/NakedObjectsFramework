// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Properties.Defaults;

namespace NakedObjects.Reflector.DotNet.Facets.Properties.Defaults {
    public class PropertyDefaultFacetAnnotation : PropertyDefaultFacetAbstract {
        private readonly object value;

        public PropertyDefaultFacetAnnotation(object value, IFacetHolder holder)
            : base(holder) {
            this.value = value;
        }

        public override bool CanAlwaysReplace {
            get { return false; }
        }

        public override object GetDefault(INakedObject inObject) {
            return value;
        }

        protected override string ToStringValues() {
            return "Value=" + value;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}