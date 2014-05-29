// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;

namespace NakedObjects.Architecture.Interactions {
    public abstract class InteractionException : Exception {
        private readonly IIdentifier identifier;
        private readonly InteractionType interactionType;
        private readonly INakedObject target;

        protected InteractionException(InteractionContext ic)
            : this(ic, null) {}

        protected InteractionException(InteractionContext ic, string message)
            : base(message) {
            interactionType = ic.InteractionType;
            identifier = ic.Id;
            target = ic.Target;
        }

        /// <summary>
        ///     The type of interaction that caused this exception to be raised
        /// </summary>
        public virtual InteractionType InteractionType {
            get { return interactionType; }
        }

        /// <summary>
        ///     The identifier of the feature (object or member) being interacted with
        /// </summary>
        public virtual IIdentifier Identifier {
            get { return identifier; }
        }

        /// <summary>
        ///     The object being interacted with
        /// </summary>
        public virtual INakedObject Target {
            get { return target; }
        }
    }
}