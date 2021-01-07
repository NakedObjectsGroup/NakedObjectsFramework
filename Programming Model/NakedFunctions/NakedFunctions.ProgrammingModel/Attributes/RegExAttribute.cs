// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;

namespace NakedFunctions {
    /// <summary>
    ///     Validate and potentially to normalise the format of the input using supplied regular expression.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The RegEx attribute may be applied to any property, or to any parameter within an action method,
    ///         that allows the user to type in text as input. It serves both to validate and potentially to
    ///         normalise the format of the input. RegEx is therefore similar in use to Mask but provides more
    ///         flexibility.
    ///     </para>
    ///     <para>
    ///         For example this annotation should validate UK postcodes and format them with a space
    ///         <code>
    /// [RegEx(Validation = @"([A-Z]{1,2}\d[A-Z\d]?).*(\d[ABD-HJLNP-UW-Z]{2})", Format = @"$1 $2")]
    /// </code>
    ///         <para>
    ///             (Regex from Ben Forta, Regular Expressions, SAMS)
    ///         </para>
    ///     </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Parameter)]
    public class RegExAttribute : Attribute {
        public RegExAttribute() : this("") { }
        public RegExAttribute(string format) : this(format, false) { }

        public RegExAttribute(string format, bool caseSensitive) {
            CaseSensitive = caseSensitive;
            Format = format;
        }

        /// <summary>
        ///     Validation regular expression string a match is considered success.
        /// </summary>
        public string Validation { get; set; }

        /// <summary>
        ///     Message to display if the validation fails
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///     Format regular expression substitution string
        /// </summary>
        /// <para>
        ///     http://msdn.microsoft.com/en-us/library/ewy2t5e0.aspx"/>
        /// </para>
        public string Format { get; set; }

        /// <summary>
        ///     Case sensitivity - defaults to false (non-sensitive)
        /// </summary>
        public bool CaseSensitive { get; set; }
    }
}