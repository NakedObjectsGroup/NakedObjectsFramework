// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
namespace AdventureWorksModel {
    //This class provides a convenient mechanism for an external assembly
    //to ensure that the assembly containing this class is loaded into memory.
    public class AssemblyHook {
        //This method contains no functionality.  It is just a convenient way
        //to ensure that the assembly containing this code is loaded into memory.
        public static void EnsureAssemblyLoaded() {}
    }
}