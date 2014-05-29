// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Interactions;

namespace NakedObjects.Architecture.Facets.Actions.Validate {
    public class ActionArgumentsInvalidException : InvalidException {
        public ActionArgumentsInvalidException(InteractionContext ic)
            : this(ic, Resources.NakedObjects.InvalidArguments) {}

        public ActionArgumentsInvalidException(InteractionContext ic, string message)
            : base(ic, message) {}
    }

    // Copyright (c) Naked Objects Group Ltd.
}