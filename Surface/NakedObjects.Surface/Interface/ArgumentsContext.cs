// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.Generic;

namespace NakedObjects.Surface {
    public class ArgumentsContext {
        public IDictionary<string, object> Values { get; set; }
        public bool ValidateOnly { get; set; }
        public string Digest { get; set; }
        public string SearchTerm { get; set; }
    }
}