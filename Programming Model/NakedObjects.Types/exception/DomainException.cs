// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects {
    /// <summary>
    ///     Indicates that a problem has occurred within the application i.e. domain code, as opposed to within
    ///     a supporting framework or system.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         All exceptions that are generated within the application domain code should inherit from this class or throw
    ///         it directly as this allows the Naked Objects Framework to discriminate between potential framework errors
    ///         and exceptions raised in application code.
    ///     </para>
    /// </remarks>
    public class DomainException : Exception {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DomainException" /> class with the specified
        ///     error message.
        /// </summary>
        public DomainException(string msg)
            : base(msg) {}

        /// <summary>
        ///     Initializes a new instance of the <see cref="DomainException" /> class with a reference to the inner
        ///     exception that is the cause of this exception. The error message will be the same as the cause's
        ///     error message.
        /// </summary>
        public DomainException(Exception cause)
            : this(cause.Message, cause) {}

        /// <summary>
        ///     Initializes a new instance of the <see cref="DomainException" /> class with the specified
        ///     error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        public DomainException(string msg, Exception cause)
            : base(msg, cause) {}
    }
}