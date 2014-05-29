// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Text;

namespace NakedObjects {
    /// <summary>
    ///     Helper class for use in method that return strings with
    ///     reasons. If no reasons are specified <see cref="Reason" /> will return <c>null</c>, otherwise
    ///     it will return a <c>string</c> with all the valid reasons concatenated and separated by a semi-colon.
    /// </summary>
    public class ReasonBuilder {
        #region Constructors

        /// <summary>
        ///     Creates a new, empty, TitleBuilder object
        /// </summary>
        public ReasonBuilder() {
            sb = new StringBuilder();
        }

        #endregion

        private readonly StringBuilder sb;

        /// <summary>
        ///     Return the set of appended reasons (separated by semi-colons), or <c>null</c> if there are none
        /// </summary>
        public string Reason {
            get { return sb.Length == 0 ? null : sb.ToString(); }
        }

        /// <summary>
        ///     Append a reason to the list of existing reasons if the condition flag is true
        /// </summary>
        public void AppendOnCondition(bool condition, string reason) {
            if (condition) {
                Append(reason);
            }
        }

        /// <summary>
        ///     Append a reason to the list of existing reasons if the condition flag is true.
        ///     Multiple Appends are separated by semi-colons.
        /// </summary>
        public void Append(string reason) {
            if (sb.Length > 0) {
                sb.Append("; ");
            }
            sb.Append(reason);
        }
    }
}