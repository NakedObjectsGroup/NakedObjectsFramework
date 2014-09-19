// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;

namespace NakedObjects.Surface {
    public abstract class WithContextNOSException : NakedObjectsSurfaceException {
        private IList<ContextSurface> contexts;

        protected WithContextNOSException() {}
        protected WithContextNOSException(string message) : base(message) {}
        protected WithContextNOSException(string message, Exception e) : base(message, e) {}

        protected WithContextNOSException(string message, IList<ContextSurface> contexts) : base(message) {
            Contexts = contexts;
        }

        protected WithContextNOSException(string message, ContextSurface context)
            : base(message) {
            ContextSurface = context;
        }

        public IList<ContextSurface> Contexts {
            get {
                if (contexts == null) {
                    return ContextSurface == null ? new ContextSurface[] {} : new[] {ContextSurface};
                }
                return contexts;
            }
            private set { contexts = value; }
        }

        public ContextSurface ContextSurface { get; private set; }
    }
}