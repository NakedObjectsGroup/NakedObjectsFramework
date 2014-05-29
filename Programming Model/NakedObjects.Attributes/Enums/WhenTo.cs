// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects {
    /// <summary>
    ///     Enum to indicate duration of behaviour
    /// </summary>
    public enum WhenTo {
        /// <summary>
        ///     Behaviour always happens
        /// </summary>
        Always,

        /// <summary>
        ///     Behaviour never happens
        /// </summary>
        Never,

        /// <summary>
        ///     Behaviour happens once object has been persisted
        /// </summary>
        OncePersisted,

        /// <summary>
        ///     Behaviour happens until object has been persisted
        /// </summary>
        UntilPersisted
    }
}