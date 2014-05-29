// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.Generic;
using System.Linq;

namespace RestfulObjects.Mvc.Model {
    public class ArgumentMap : ReservedArguments {
        public IDictionary<string, IValue> Map { get; set; }

        public override bool HasValue {
            get { return Map.Any(); }
        }

    }
}