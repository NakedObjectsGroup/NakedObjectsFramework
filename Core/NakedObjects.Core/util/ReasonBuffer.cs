// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Text;

namespace NakedObjects.Core.Util {
    /// <summary>
    ///     Helper class to create properly concatenated reason string for use in method that return Strings with
    ///     reasons. If no reasons are specified <see cref="Reason" /> will return <c>null</c> , otherwise
    ///     it will return a <c>string</c> with all the valid reasons concatenated with a semi-colon separating
    ///     each one.
    /// </summary>
    [Obsolete("remove if unused")]
    public class ReasonBuffer {
        private readonly StringBuilder reasonBuffer = new StringBuilder();

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