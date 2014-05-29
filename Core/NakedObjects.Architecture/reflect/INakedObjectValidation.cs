// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Architecture.Reflect {
    public interface INakedObjectValidation {
        string[] ParameterNames { get; }

        string Execute(INakedObject obj, INakedObject[] parameters);
    }

    // Copyright (c) Naked Objects Group Ltd.
}