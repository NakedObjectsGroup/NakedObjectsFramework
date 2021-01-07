// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Text;

namespace NakedObjects {
    /// <summary>
    ///     Helper class for use in method that return strings with
    ///     reasons. If no reasons are specified <see cref="Reason" /> will return <c>null</c>, otherwise
    ///     it will return a <c>string</c> with all the valid reasons concatenated and separated by a semi-colon.
    /// </summary>
    public class ReasonBuilder {
        private readonly StringBuilder sb;

        #region Constructors

        /// <summary>
        ///     Creates a new, empty, TitleBuilder object
        /// </summary>
        public ReasonBuilder() => sb = new StringBuilder();

        #endregion

        /// <summary>
        ///     Return the set of appended reasons (separated by semi-colons), or <c>null</c> if there are none
        /// </summary>
        public string Reason => sb.Length == 0 ? null : sb.ToString();

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