// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture;
using NakedObjects.Architecture.Interactions;

namespace NakedObjects.Meta.Except {
    /// <summary>
    ///     The interaction is invalid because the input value does not match the specified RegEx.
    /// </summary>
    public class InvalidRegExException : InvalidException {
        private readonly bool caseSensitive;
        private readonly string format;
        private readonly string validation;

        public InvalidRegExException(InteractionContext ic, string format, string validation, bool caseSensitive)
            : this(ic, format, validation, caseSensitive, Resources.NakedObjects.PatternMessage) {}

        public InvalidRegExException(InteractionContext ic, string format, string validation, bool caseSensitive, string message)
            : base(ic, message) {
            this.format = format;
            this.validation = validation;
            this.caseSensitive = caseSensitive;
        }

        public virtual string FormatPattern {
            get { return format; }
        }

        public virtual string ValidationPattern {
            get { return validation; }
        }

        public virtual bool IsCaseSensitive {
            get { return caseSensitive; }
        }
    }
}