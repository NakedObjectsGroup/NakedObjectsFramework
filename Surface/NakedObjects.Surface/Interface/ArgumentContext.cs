// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Surface {
    public class ArgumentContext {
        public object Value { get; set; }
        public bool ValidateOnly { get; set; }
        public string Digest { get; set; }
    }
}