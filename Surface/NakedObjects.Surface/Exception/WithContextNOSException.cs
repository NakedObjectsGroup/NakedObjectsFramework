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
                    return ContextSurface == null ? new ContextSurface[] { } : new[] { ContextSurface };
                }
                return contexts;
            }
            private set { contexts = value; }
        }

        public ContextSurface ContextSurface { get; private set; }
    }
}