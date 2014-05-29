// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Boot;

namespace RestfulObjects.Bootstrap {
    public class RunRest : RunStandaloneBase {
        protected override INakedObjectsClient Client {
            get { return new RestUserInterface(); }
        }
    }
}