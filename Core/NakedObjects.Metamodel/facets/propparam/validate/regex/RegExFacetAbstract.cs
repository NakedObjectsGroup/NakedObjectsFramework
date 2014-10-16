// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Interactions;

namespace NakedObjects.Architecture.Facets.Propparam.Validate.RegEx {
    public abstract class RegExFacetAbstract : MultipleValueFacetAbstract {
        private readonly string failureMessage;
        private readonly string formatPattern;
        private readonly bool isCaseSensitive;
        private readonly string validationPattern;

        protected RegExFacetAbstract(string validation, string format, bool caseSensitive, string failureMessage, ISpecification holder)
            : base(Type, holder) {
            validationPattern = validation;
            formatPattern = format;
            isCaseSensitive = caseSensitive;
            this.failureMessage = failureMessage;
        }

        public static Type Type {
            get { return typeof (IRegExFacet); }
        }


        public string ValidationPattern {
            get { return validationPattern; }
        }

        public virtual string FailureMessage {
            get { return failureMessage; }
        }

        public string FormatPattern {
            get { return formatPattern; }
        }

        public bool IsCaseSensitive {
            get { return isCaseSensitive; }
        }


        public virtual string Invalidates(InteractionContext ic) {
            INakedObject proposedArgument = ic.ProposedArgument;
            if (proposedArgument == null) {
                return null;
            }
            string titleString = proposedArgument.TitleString();
            if (!DoesNotMatch(titleString)) {
                return null;
            }

            return failureMessage ?? Resources.NakedObjects.InvalidEntry;
        }


        public virtual InvalidException CreateExceptionFor(InteractionContext ic) {
            return new InvalidRegExException(ic, FormatPattern, ValidationPattern, IsCaseSensitive, Invalidates(ic));
        }

        public abstract bool DoesNotMatch(string param1);
        public abstract string Format(string param1);
    }
}