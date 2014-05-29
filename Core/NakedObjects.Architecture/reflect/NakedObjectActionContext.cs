// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Architecture.Reflect {
    public class NakedObjectActionContext {
        public INakedObject Target { get; set; }
        public INakedObjectAction Action { get; set; }
        public INakedObject[] Parameters { get; set; }
    }
}