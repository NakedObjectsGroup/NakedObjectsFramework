// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace RestfulObjects.Mvc.Model {
    public class SingleValueArgument : ReservedArguments {
        public IValue Value { get; set; }

        public override bool HasValue {
            get { return Value != null; }
        }
    }
}