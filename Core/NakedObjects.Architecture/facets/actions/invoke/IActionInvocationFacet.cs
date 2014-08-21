// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Architecture.Facets.Actions.Invoke {
    /// <summary>
    ///     Represents the mechanism by which the action should be invoked.
    /// </summary>
    /// <para>
    ///     In the standard Naked Objects Programming Model, corresponds to
    ///     invoking the actual action method itself (a <c>public</c> method that
    ///     does not represent a property, a collection or any of the supporting
    ///     methods).
    /// </para>
    public interface IActionInvocationFacet : IFacet {
        INakedObjectSpecification ReturnType { get; }

        INakedObjectSpecification OnType { get; }

        INakedObject Invoke(INakedObject target, INakedObject[] parameters, INakedObjectPersistor persistor);

        INakedObject Invoke(INakedObject target, INakedObject[] parameters, int resultPage, INakedObjectPersistor persistor);

        bool GetIsRemoting(INakedObject target);
    }


    // Copyright (c) Naked Objects Group Ltd.
}