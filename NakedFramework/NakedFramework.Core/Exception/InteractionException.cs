// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Interactions;

namespace NakedObjects.Core {
    public abstract class InteractionException : Exception {
        protected InteractionException(IInteractionContext ic, string message)
            : base(message) {
            InteractionType = ic.InteractionType;
            Identifier = ic.Id;
            Target = ic.Target;
        }

        /// <summary>
        ///     The type of interaction that caused this exception to be raised
        /// </summary>
        public virtual InteractionType InteractionType { get; }

        /// <summary>
        ///     The identifier of the feature (object or member) being interacted with
        /// </summary>
        public virtual IIdentifier Identifier { get; }

        /// <summary>
        ///     The object being interacted with
        /// </summary>
        public virtual INakedObjectAdapter Target { get; }
    }
}