// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Redirect {
    //Implemented by a 'stub' class that acts as proxy to a service implemented on another server
    //Note that, unlike IRedirectedObject, this defines functions, not properties,
    public interface IRedirectedService {
        //This should be a logical server name, translated to/from a physical address elsewhere.
        string ServerName();

        //The name of the service on the other server
        string ServiceName();
    }
}