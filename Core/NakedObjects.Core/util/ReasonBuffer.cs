// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Text;

namespace NakedObjects.Core.Util {
    /// <summary>
    ///     Helper class to create properly concatenated reason string for use in method that return Strings with
    ///     reasons. If no reasons are specified <see cref="Reason" /> will return <c>null</c> , otherwise
    ///     it will return a <c>string</c> with all the valid reasons concatenated with a semi-colon separating
    ///     each one.
    /// </summary>
    public class ReasonBuffer {
        internal StringBuilder reasonBuffer = new StringBuilder();

        /// <summary>
        ///     Return the combined set of reasons, or <c>null</c> if there are none
        /// </summary>
        public virtual string Reason {
            get { return reasonBuffer.Length == 0 ? null : reasonBuffer.ToString(); }
        }

        /// <summary>
        ///     Append a reason to the list of existing reasons
        /// </summary>
        public virtual void Append(string reason) {
            if (reason != null) {
                if (reasonBuffer.Length > 0) {
                    reasonBuffer.Append("; ");
                }
                reasonBuffer.Append(reason);
            }
        }

        /// <summary>
        ///     Append a reason to the list of existing reasons if the condition flag is true
        /// </summary>
        public virtual void AppendOnCondition(bool condition, string reason) {
            if (condition) {
                Append(reason);
            }
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}