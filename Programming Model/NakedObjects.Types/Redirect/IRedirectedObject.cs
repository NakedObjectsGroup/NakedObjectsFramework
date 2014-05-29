// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Redirect {
    //Implemented by a 'stub' class that acts as a proxy for an object managed on another server
    public interface IRedirectedObject {
        //This should be a logical server name, translated to/from a physical address elsewhere.
        string ServerName { get; set; }

        //The Oid of the object on the other server
        string Oid { get; set; }
    }
}