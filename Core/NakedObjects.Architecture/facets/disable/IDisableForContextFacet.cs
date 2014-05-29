// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Interactions;

namespace NakedObjects.Architecture.Facets.Disable {
    /// <summary>
    ///     Disable a property, collection or action based on the state of the target <see cref="INakedObject" /> object.
    /// </summary>
    /// <para>
    ///     In the standard Naked Objects Programming Model, corresponds to
    ///     invoking the <c>DisableXxx</c> support method for the member.
    /// </para>
    public interface IDisableForContextFacet : IFacet, IDisablingInteractionAdvisor {
        /// <summary>
        ///     The reason this object is disabled, or <c>null</c> otherwise
        /// </summary>
        string DisabledReason(INakedObject nakedObject);
    }


    // Copyright (c) Naked Objects Group Ltd.
}