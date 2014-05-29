// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Threading.Tasks;

namespace NakedObjects.Async {
    /// <summary>
    /// Interface representation of the AsyncService class (in NakedObjects.Asynch.dll) -
    /// for injection into domain code.
    /// </summary>
    public interface IAsyncService {
        /// <summary>
        ///     Domain programmers must take care to ensure thread safety.
        ///     The action passed in must not include any references to stateful objects (e.g. persistent domain objects).
        ///     Typically the action should be on a (stateless) service; it may include primitive references
        ///     such as object Ids that can be used within the called method to retrieve and action on specific
        ///     object instances.
        /// </summary>
        /// <param name="toRun"></param>
        Task RunAsync(Action toRun);
    }
}

