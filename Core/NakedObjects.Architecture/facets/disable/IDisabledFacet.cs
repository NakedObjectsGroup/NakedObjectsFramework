// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Interactions;

namespace NakedObjects.Architecture.Facets.Disable {
    /// <summary>
    ///     Disable a property, collection or action
    /// </summary>
    /// <para>
    ///     In the standard Naked Objects Programming Model, corresponds to annotating the member
    ///     with <see cref="DisabledAttribute" />
    /// </para>
    public interface IDisabledFacet : ISingleWhenValueFacet, IDisablingInteractionAdvisor {
        /// <summary>
        ///     The reason why the (feature of the) target object is currently disabled, or <c>null</c> if enabled
        /// </summary>
        string DisabledReason(INakedObject target);
    }
}