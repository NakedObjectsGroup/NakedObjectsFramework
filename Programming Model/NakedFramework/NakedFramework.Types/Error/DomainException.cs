// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;

namespace NakedFramework.Error {
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
            : base(msg) { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DomainException" /> class with a reference to the inner
        ///     exception that is the cause of this exception. The error message will be the same as the cause's
        ///     error message.
        /// </summary>
        public DomainException(Exception cause)
            : this(cause.Message, cause) { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DomainException" /> class with the specified
        ///     error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        public DomainException(string msg, Exception cause)
            : base(msg, cause) { }
    }
}